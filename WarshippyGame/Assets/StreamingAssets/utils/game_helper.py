# -*- coding: utf-8 -*-

import string
import os
coordenates = " "
message = " "
def setCurrentMessage(text):
        global message
        message  = text
        print("[GameHelper] Setting current message to: " + message)
def getMessages():
        global message
        #print("[GameHelper] Returning current message to server: " + message)
        return message
def wrongPosition(position):
        l = position.split(":")
        if len(l) == 2:
                if str(l[0]) in string.digits and str(l[1]) in string.digits:
                        if(len(l[0]) == 1 and len(l[1]) == 1):
                                return False
                        else:
                                return True
                else:
                        return True
        else:
                return True
def thereIsNoNumber(s):
    sp = s.split(":")
    if sp[0] in string.digits:
                if sp[1] in string.digits:
                        return False
    else:
        return True
def isAValidPosition(s):
    sp = s.split(":")
    for pos in sp:
        if pos >= 5 or pos < 0:
                return False
    return True

def isCorrectCoordenate(pos):
    print("CHECKING IF {} IS CORRECT".format(pos))
    if pos != None:
        if ":" in pos:
                if thereIsNoNumber(pos) == False:
                        if wrongPosition(pos) == False:
                                return True
                else:
                        return False   
        else:
                return False
    else:
            return False
def getCoordenates():
        global coordenates
        return coordenates
def update_position(pos):
    if pos != None:
        if ":" in pos:
                if thereIsNoNumber(pos) == False:
                        if wrongPosition(pos) == False:
                                msg = "Attacking user at this coordenates : " + pos
                                #bot.send_text(msg)
                                global coordenates
                                coordenates = pos
                                return True
                else:
                        msg = "This position is not available, Please write a coordenate like this: 2:2 [x,y]"
                        #bot.send_text(msg)
                        return False
                        
        else:
                msg = "This position is not available, Please write a coordenate like this: 2:2 [x,y]"
                #bot.send_text(msg)
                return False
    else:
            return False
def someBoxOccupied(b,boat):
        o = boat.orientation
        x = int(boat.x)
        y = int(boat.y)
        if int(x)+ 2 > 5 or int(y) + 2 > 5:
                return False
        elif len(b) < 0:
                return False
        else:
                for boats in b:
                        return boats.x == x and boats.y == y and boats.orientation == o