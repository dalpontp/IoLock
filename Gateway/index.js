import iotDevice from "azure-iot-device";
import { Mqtt } from "azure-iot-device-mqtt";
import { SerialPort } from "serialport";
import { ReadlineParser } from "@serialport/parser-readline"; //sotto pacchetto
import dotenv from "dotenv"; //env
dotenv.config(); //env

//config serial connection
const serial_port = new SerialPort({
  path: process.env.SERIAL_PATH,
  baudRate: parseInt(process.env.SERIAL_BAUDRATE),
  autoOpen: process.env.SERIAL_AUTOOPEN === "true", //check if the variable is 'true', return a boolean
});

//connect to serial port
serial_port.open((err) => {
  if (err) {
    console.error("SERIAL - Connection failed, port: ",serial_port.path," ",err); //err.message exist?
  } else {
    console.log("Connected to serial port: ", serial_port.path);
  }
});
 
//Connect to IotHub device
const client = iotDevice.Client.fromConnectionString(process.env.DEVICE_CONNECTIONSTRING,Mqtt); //Mqtt
client.open(function (err) {
  if (err) {
    console.error("IOT Connection error:", err.message);
  } else {
    console.log("Connected to IoT Hub", client.name);
  }
});

//Listening serialport parser (used to parse data from serial)
const parser = serial_port.pipe( 
  new ReadlineParser({ delimiter: process.env.SERIAL_DELIMITER })
);
parser.on("data", (stream) => {
  console.log("Data from serial: \n", stream);
  //split the incoming data
  const streamarray = stream.split('/');
  //create a obj
  const jsonMessage = { 
    "id_pic": streamarray[0],
    "id_gateway":streamarray[1], 
    "type":streamarray[2], // 0 se da pic a gateway - 1 se da gateway a pic
    "payload":streamarray[3]
  }
  if (jsonMessage.id_gateway == process.env.DEVICE_NAME && jsonMessage.type == '0') {
    console.log("SERIAL - Recived valid Message: ", jsonMessage);
    const jsonstring = JSON.stringify(jsonMessage)
    const iotmessage = new iotDevice.Message(jsonstring);
    client.sendEvent(iotmessage) // TOO ADD CALLBACK // ,printResultFor('send')
    console.log("send message?")
    } 
    else {
    console.error("SERIAL - Recived invalid message: ", jsonMessage);
    }
});

//Listening from IoThub device
client.on("message", function (msg) {
  console.log("Message from IoThub Device: \n",msg.getData().toString("utf-8")); //TODO log time
  write_on_serial(msg.getData().toString("utf-8"));
  //delete message from device queue
  client.complete(msg, function (err) {
    if (err) {
      console.error("Error message not completed:", err.message);
    } else {
      console.log("Message completed, deleted from Device queue.");
    }
  });
});

//write on serial
function write_on_serial(msg) {
  const outcomeJson = JSON.parse(msg);
  const outcomearray = ""+outcomeJson.id_pic+"/"+outcomeJson.id_gateway+"/1/"+outcomeJson.payload+"\r\n";
  
  //debug
  console.log("ARRAY OUTCOME = ", outcomearray)

  serial_port.write(outcomearray, function (err) {
    if (err) {
      return console.error("SERIAL - Error on write: ", err.message);
    }
    console.log("Message written on serial");
  });
} 


//Publish to EvensHub, endpoint itegrated in iothub
//function publish_on_eventshub(jsonmsg) {

  /*
  const producerClient = new EventHubProducerClient(process.env.IOTHUB_EVENTS_CONNECTIONSTRING
    //process.env.EVENTHUBS_SEND_CONNECTIONSTRING,
    ,process.env.IOTHUB_NAME   
  );
  let info = producerClient.getEventHubProperties()
  console.log("info: ", info)
  //write on eventshub
  let msg2 = [jsonmsg];
  console.log("msg = ", msg2)
  const eventDataBatch = await producerClient.createBatch();
  console.log("batch created?")
  eventDataBatch.tryAdd({ body: msg2 }); //body need to be an arraay
  console.log("added msg on bach?")
  await producerClient.sendBatch(eventDataBatch);
  console.log("sended? "); //debug 
  await producerClient.close();*/
//}

//
// 
//  99/200/1/17254
//  99/200/0/17254
//
