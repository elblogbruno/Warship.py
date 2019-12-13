import base64
import time
import zmq
import BotDemo as bot
import sys
import threading
import GameHelper 
import os
import sys
import threading
port = "5555"
hasStarted = False
isMessageExistance = False
oldCoordenates = " "
context = zmq.Context()
socket = context.socket(zmq.REP)
socket.bind("tcp://*:"+str(port))


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
    bot.send_text("Please tell me the coordinates where to attack! [x:x]:")
    bot.send_attack_query()
def onNewText(message):
        if(isBase64(message)):
            print("[Server] It's base64 string, maybe an image")
            print(message)
            onNewImage(message)
        else:
            print("[Server] It's a normal string, maybe a message")
            onNewMessage(message)


def onReadMessage():
    message = socket.recv()
    onNewText(message)
    print ("[Server] Received request: " + str(message))
    
def getBotAttackCoordenates():
    #print("[Server] Attacking PC Client with this coordenates: " + GameHelper.getCoordenates())
    global oldCoordenates
    newCoordenates = GameHelper.getCoordenates()
    if(newCoordenates != oldCoordenates):
            print("[Server] New attack coordenates: " + newCoordenates)
            oldCoordenates = newCoordenates
            print ("[Server] Sending this coordenates: " + newCoordenates)
            socket.send_string(newCoordenates)
    else:
            pass
    return oldCoordenates


def startServer():
    while True:
        getBotAttackCoordenates()
        #onNewMessageFromHelper()

def onNewMessageFromHelper():
    
    global oldCoordenates
    newCoordenates = GameHelper.getMessages()
    if(newCoordenates != oldCoordenates):
            print("[Server] Bot message: "+ str(newCoordenates))
            oldCoordenates = newCoordenates
    else:
            pass
    return oldCoordenates

if __name__ == "__main__": 
    z = threading.Thread(target=getBotAttackCoordenates)
    w = threading.Thread(target=onReadMessage)
    bot.setBotToken("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk")
    z.start()
    w.start()
    bot.main()
    
        
