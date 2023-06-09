import dotenv from "dotenv";
import {Mqtt} from "./protocols/mqtt/mqtt.js";

dotenv.config();

const istance = new Mqtt();
istance.cb_message("/qzer2021pn/chat");

//todo fix 
/*
const { SerialPort } = require("serialport"); //pacchetto globale
const { ReadlineParser } = require("@serialport/parser-readline"); //sotto pacchetto
//const { CustomError } = require("./CustomError"); // file locale per gestile le eccezioni TODO
const s_serial = require("./settings/serial_set"); // file di configurazione locale 
const setting = require("./settings/generic_set");
const s_mqtt = require("./settings/mqtt_set");
const readline = require("readline");

//config serial connection
const serial_port = new SerialPort({
    path: s_serial.path,
    baudRate: s_serial.baudRate,
    autoOpen: s_serial.autoOpen 
  });

//parser serialport (used to parse data from serial)
const parser = serial_port.pipe(new ReadlineParser({ delimiter: s_serial.delimiter }));

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

*/
