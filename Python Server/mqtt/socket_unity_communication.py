#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#

import time
from threading import Thread

import zmq
import json
import queue

context = zmq.Context()


class SocketUnity(object):
    def __init__(self, host, topic, imageTopic, bot):
        self.socket_initialized = False
        self.topic = topic
        self.imageTopic = imageTopic
        self.bot = bot
        self.socket = context.socket(zmq.REP)
        self.socket.bind("tcp://*:5555")
        self.socket_initialized = True
        self.q = queue.Queue(5)
        print("[SocketUnity]  Socket initialized.")

    def on_message(self, client, userdata, msg):
        print("[MQTT_HANDLER] Received text from Unity TOPIC: " + msg.topic + " MSG: " + str(msg.payload))
        if msg.topic == "BOT":
            print("MSG Topic is BOT")
            self.bot.filter_text(str(msg.payload.decode("utf-8")))
        else:
            print("MSG Topic is IMAGE")
            self.bot.update_table_to_bot(msg.payload)

    def is_connected(self):
        return self.socket_initialized

    def publish_on_mqtt(self, message, is_image=False):
        topic = self.topic
        if is_image:
            topic = self.imageTopic
        print("[Socket Unity] Adding {0} to the queue: {1}".format(message, topic))
        # message1 = self.socket.recv()
        message = {
            "points": 0,
            "attack_pos": str(message),
            "status": is_image,
        }
        message = json.dumps(message)
        print(message)

        self.q.put(message)

    def start_listening(self):
        thread = Thread(target=self.main_loop)
        thread.daemon = True
        thread.start()

    def main_loop(self):
        while True:
            #  Wait for next request from client
            print("Will recv")
            message = self.socket.recv()
            print("Received request: %s" % message)

            #  Do some 'work'.
            #  Try reducing sleep time to 0.01 to see how blazingly fast it communicates
            #  In the real world usage, you just need to replace time.sleep() with
            #  whatever work you want python to do, maybe a machine learning task?
            time.sleep(1)

            #  Send reply back to client
            #  In the real world usage, after you finish your work, send your output here
            # self.socket.send(b"World")
            if not self.q.empty():
                message = self.q.get()
                print("Sending {0}".format(message))
                self.socket.send_json(message)
