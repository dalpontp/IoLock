const { SerialPort } = require("serialport"); //pacchetto globale
const { ReadlineParser } = require("@serialport/parser-readline"); //sotto pacchetto
//const { CustomError } = require("./CustomError"); // file locale per gestile le eccezioni TODO
const s = require("./config/serial"); // file di configurazione locale 

//config serial connection
const serial_port = new SerialPort({
    path: "COM2",
    baudRate: 9600,
    autoOpen: false,
  });

//parser serialport (used to parse data from serial)
const parser = serial_port.pipe(new ReadlineParser({ delimiter: "\r\n" }));

//connect to serial port
serial_port.open((res) => {
  if (res == null) {
    console.log("Connected to serial port:", serial_port.path);
  } else {
    console.log(res);
  }
});

//forward from serial to ???
parser.on("data", (stream) => {
  console.log("Data from serial", stream);
  let jsonString = JSON.stringify(stream_to_obj(stream));
  //TODO
});

//write on serial
function write_on_serial(mex) {
  //TODO 
  s.serial_port.write(mex);
}

//stream to obj TODO chose standard
function stream_to_obj(data) {
  let timestamp = Date.now();
  let [codice, pic] = data.split(",");
  let obj = { code: codice, pic: pic, timestamp: timestamp };
  return obj;
}
