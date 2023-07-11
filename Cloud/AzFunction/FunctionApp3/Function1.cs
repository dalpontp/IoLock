using System;
using System.Text;
using System.Text.Json;
using FunctionApp3.Classes;
using FunctionApp3.DTO;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Data.SqlClient;
using System.Data;

namespace FunctionApp3
{
    public class Function1
    {
        private readonly IConfiguration config;
        private readonly ILogger _logger;

        public Function1(IConfiguration config, ILogger<Function1> _logger)
        {
            this.config = config;
            this._logger = _logger;
        }

        [Function("Function1")]
        public void Run([ServiceBusTrigger("messagesfromgateway", Connection = "servicebuscs")] string myQueueItem)
        {
            _logger.LogInformation($"C# ServiceBus queue trigger function processed message: {myQueueItem}");

            //istance pswGenerator
            PswGenerator pswGenerator = new PswGenerator();

            //deserialize string from servicebus message
            DTO.TransitMessage msgFromGateway = JsonSerializer.Deserialize<DTO.TransitMessage>(myQueueItem);
            
            string gateway_id = msgFromGateway.id_gateway;
            string pic_code = msgFromGateway.payload;
            string pic_id = msgFromGateway.id_pic;
            string psw = pswGenerator.GenerateRandomNumber().ToString();

            _logger.LogInformation($"PSW: {psw}");

            //create record
            DTO.Record record = new Record()
            {
                PicCode = pic_code,
                PicID = Int32.Parse(pic_id),
                GatewayID = gateway_id,
                Psw = psw
            };
            
            //connect to db
            try
            {
                _logger.LogInformation("Connection to DB...\n");
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = "server";
                builder.UserID = "user";
                builder.Password = "psw";
                builder.InitialCatalog = "database";

                using (SqlConnection connection = new SqlConnection(builder.ConnectionString))
                {
                    _logger.LogInformation("=========================================\n");
                    String update = $"UPDATE DoorCredentials SET psw = @recordPsw, picCode = @recordPicCode WHERE picId = @recordPicID";
                    String select = "SELECT picId, picCode, psw FROM DoorCredentials";
                    //update in db
                    using (SqlCommand update_command = new SqlCommand(update, connection)) {
                        _logger.LogInformation("Connected to DB");
                        update_command.Parameters.Add("@recordPsw", SqlDbType.Int).Value = record.Psw;
                        update_command.Parameters.Add("@recordPicCode", SqlDbType.NChar).Value = record.PicCode;
                        update_command.Parameters.Add("@recordPicID", SqlDbType.NVarChar).Value = record.PicID;
                        connection.Open();
                        update_command.ExecuteNonQuery();
                        update_command.Dispose();
                    }
                    using (SqlCommand command = new SqlCommand(select, connection))
                    {
                        //connection.Open();
                        _logger.LogInformation("Connected to DB");
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Console.WriteLine("{0} {1} {2}", reader.GetInt32(0), reader.GetString(1), reader.GetString(2));
                                Console.WriteLine(reader);
                            }
                        }
                        command.Dispose();
                    }
                    //disconnect from server
                    connection.Close();
                }
            }
            catch (SqlException e)
            {
                _logger.LogError(e.ToString());
            }
            //create message to send 
            DTO.TransitMessage msgToGateway = new TransitMessage()
            {
                id_pic = pic_id,
                id_gateway = gateway_id,
                payload = psw.ToString(),
            };
            //connect to iothub device
            ServiceClient serviceClient = ServiceClient.CreateFromConnectionString("iotdevicecs");
            _logger.LogInformation($"Connected to IoT Hub");
            //create message
            Message message = new Message(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(msgToGateway)));     
            //send message to device
            serviceClient.SendAsync(gateway_id, message).GetAwaiter().GetResult();
            //disconnect
            serviceClient.Dispose();
        }
    }
}
