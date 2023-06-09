//import
import {mqtt_setting} from "../../settings/mqtt_set.js";
import * as d_set from "../../settings/date_set.js";
import mqtt from "mqtt";

export class Mqtt {
  constructor() { 
    this.client = mqtt.connect(mqtt_setting.servers.mosquitto);
    this.cb_connect();
  } 

  cb_connect() {
    this.client.on("connect", () => {
      if (this.client.connected) {
        console.log("Connected to MQTT broker:", mqtt_setting.servers.mosquitto);
        this.subscribes(mqtt_setting.subscribes);
      }
    });
  }

  cb_message(topic) {
    this.client.on("message", (space, message) => {
      if ((topic == space)) {
        let date = new Date();
        const formattedDate = date.toLocaleString(undefined, d_set.options);
        let jsonObject;
        try {
          jsonObject = JSON.parse(message);
        } catch (error) {
          let payload = message.toString();
          console.log(formattedDate,  topic ,"ERROR: not json string ->", payload);
        }
        if (jsonObject) {
          console.log(this.extractkey(jsonObject));
        }
      }
    });
  }

  publish(topic, jsonstring) {
    this.publish(topic, jsonstring);
  }

  subscribe(topic) {
    this.client.subscribe(topic);
  }

  subscribes(topics) {
    for (let key in topics) {
      if (topics.hasOwnProperty(key)) {
        this.subscribe(topics[key]);
        
      }
    }
  }

  extractkey(jsonObject, keysToExtract) {
    if (keysToExtract) {
      //const keysToExtract = ["from", "msg"];
      for (const key of keysToExtract)
        if (jsonObject.hasOwnProperty(key))
          extractedFields[key] = jsonObject[key];
      return extractedFields;
    } else {
      return JSON.stringify(jsonObject);
    }
  }
}
