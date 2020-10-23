import paho.mqtt.client
import asyncio
import os
from hbmqtt.broker import Broker
import socket    
import time
class MQTT_Handler(object):

    def on_disconnect(self,client, userdata, rc):
        if rc != 0:
            print("[MQTT] Unexpected MQTT disconnection. Will auto-reconnect")
            print("[MQTT] isMqttConnected: " + self.isMqttConnected)

            self.isMqttConnected = False
            self.mqtt_handler.reconnect()
            self.bot.send_text("There was an error. Please try again!")

    def on_connect(self,client, userdata, flags, rc):
        self.isMqttConnected = rc == 0

        client.subscribe(topic=self.topic, qos=2)
        client.subscribe(topic=self.imageTopic, qos=2)
        
        self.publish_on_mqtt("Connected topic: {0}".format(self.topic),False)

        print(self.host + " " + str(self.isMqttConnected) + " " + str(client.is_connected()))

    def on_message(self,client, userdata, msg):
        print("[MQTT_HANDLER] TOPIC: " + msg.topic+ " MSG: "+str(msg.payload))
        if msg.topic == "BOT":
            print("MSG Topic is BOT")
            self.bot.send_text(str(msg.payload.decode("utf-8")))
        else:
            print("MSG Topic is IMAGE")
            self.bot.update_table_to_bot(msg.payload)

    def connect(self,client):
        client.on_disconnect = self.on_disconnect
        client.on_connect = self.on_connect
        client.on_message = self.on_message
        client.connect(host=self.host,keepalive=60, port=1883)
        client.loop_start()  #Start loop 
        time.sleep(4) # Wait for connection setup to complete
        #client.loop_stop()    #Stop loop 

    def __init__(self, host,topic,imageTopic,bot):
        self.client = paho.mqtt.client.Client()
        self.isMqttConnected = False
        self.topic = topic
        self.imageTopic = imageTopic
        hostname = socket.gethostname()    
        IPAddr = socket.gethostbyname(hostname)    
        print(IPAddr)
        self.host = IPAddr
        self.bot = bot
        self.connect(self.client)

    def isConnected(self):
        return self.isMqttConnected

    def reconnect(self):
        self.client.reconnect()
        return self.client.is_connected()

    def publish_on_mqtt(self,text,isImage):
        topic = self.topic
        if isImage:
            topic = self.imageTopic     
        print("[MQTT_HANDLER] Publishing {0} on this topic: {1}".format(text,topic))
        self.client.publish(topic, text)