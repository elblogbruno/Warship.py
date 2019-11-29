import base64
import time
import zmq
import BotDemo as bot
import sys
import threading
import GameHelper 
port = "5555"

oldCoordenates = " "
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:"+str(port))


def startServer():
    print("[Server] Starting server.")
    z = threading.Thread(target=worker)
    z.start()
def isBase64(s):
    try:
        return base64.b64encode(base64.b64decode(s)) == s
    except Exception:
        return False
def onNewMessage(message):
    print("[Server] Received request: %s" % message)
    socket.send(message)
def onNewImage(message):
    image_64_decode = base64.b64decode(message + b'===')
    fh = open("Image_position.png","wb")
    fh.write(image_64_decode)
    fh.close()
    bot.send_image("Image_position.png")
    bot.sendText("Please tell me the coordinates where to attack! [x:x]:")
    bot.sendAttackQuery()
def onNewText(message):
        if(isBase64(message)):
            print("[Server] It's base64 string, maybe an image")
            print(message)
            onNewImage(message)
        else:
            print("[Server] It's a normal string, maybe a message")
            onNewMessage(message)


def getBotAttackCoordenates():
    print("[Server] Attacking PC Client with this coordenates: " + GameHelper.getCoordenates())
    global oldCoordenates
    newCoordenates = GameHelper.getCoordenates()
    if(newCoordenates != oldCoordenates):
            print("[Server] New attack coordenates: " + newCoordenates)
            oldCoordenates = newCoordenates
    else:
            print("[Server] Same attack coordenates: " + oldCoordenates)
    return oldCoordenates
def worker():
    i = 0
    while True:
        try:
            print("[Server] Server is working.")
            message = socket.recv()
            print ("[Server] Received request: " + str(message))
            #time.sleep (1)
            coordenates_to_attack = getBotAttackCoordenates()
            print ("[Server] Sending this coordenates: " + coordenates_to_attack)
            socket.send_string(coordenates_to_attack)
            onNewText(message)
        except zmq.ZMQError as e:
            i = i +1
            print("[Server] Valor de i: " + str(i))
            print("[Server] ERROR: " + str(e))
     


if __name__ == "__main__":
    startServer()
    bot.setBotToken("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk")
    bot.startBot()
