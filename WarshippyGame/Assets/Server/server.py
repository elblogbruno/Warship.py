#
#   Hello World server in Python
#   Binds REP socket to tcp://*:5555
#   Expects b"Hello" from client, replies with b"World"
#
import base64
import time
import zmq
import BotDemo as bot
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:5555")
print("SERVER started")
bot.setBotToken("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk")

def isBase64(s):
    try:
        return base64.b64encode(base64.b64decode(s)) == s
    except Exception:
        return False
def onNewMessage(message):
    print("Received request: %s" % message)
    socket.send(message)
def onNewImage(message):
    image_64_decode = base64.b64decode(message + b'===')
    fh = open("Image_position.png","wb")
    fh.write(image_64_decode)
    fh.close()
    bot.send_image("Image.png")
try:
    while True:
        #  Wait for next request from client

        message = socket.recv()
        print(message)
        if message == "Quit":
            break
        if(len(message) > 1):
            if(isBase64(message)):
                print("It's base64 string, maybe an image")
                print(message)
                onNewImage(message)
            else:
                print("It's a normal string, maybe a message")
                onNewMessage(message)
        #bot.sendText(message)

        time.sleep(0.1)
        #socket.send(b"World")
except KeyboardInterrupt:
        print("shutdown")
        socket.close()
