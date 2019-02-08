# -*- coding: utf-8 -*-

#pip install asciimatics
#pip install termcolor
#pip install pyfiglet
#pip install colorama
#pip install chuck-norris-python
#pip install keyboard


import os
import sys
import keyboard
import json
import emoji
from colorama import init
init(strip=not sys.stdout.isatty()) # strip colors if stdout is redirected
from termcolor import cprint
from pyfiglet import figlet_format
from chuck import ChuckNorris
from termcolor import colored
from time import sleep
import string

jokes = ChuckNorris()
water = "W"
hit = "O"
try:    # file exists
    with open('data.txt') as json_file:
        data = json.load(json_file)
        print ("File does exist")
except:
    print ("File does not exist")
    data = {}
    data['playerA'] = []
    data['playerB'] = []
    data['playerA'].append({
        'name': "",
        'wonA': 0,
        'lostA' : 0
    })
    data['playerB'].append({
        'name': "",
        'wonB': 0,
        'lostB': 0
    })
    with open('data.txt', 'w') as outfile:
        json.dump(data, outfile)
for p in data['playerA']:
    if p['wonA'] == 0  and  p['lostA'] == 0:
        wonA = 0
        lostA = 0
    else:
        wonA = p['wonA']
        lostA = p['lostA']
for p in data['playerB']:
    if p['wonB'] == 0  and  p['lostB'] == 0:
        wonB = 0
        lostB = 0
    else:
        wonB = p['wonB']
        lostB = p['lostB']
def startBoard():
        r = []
        for i in range(5):
                r = r + [ ["W"] * 5 ]
        return r
def final(b1):
        for panels in b1:
                if "S" in  panels:
                        return False
        return True
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
def getOrientation():
        s = raw_input("Would you like to place the boat vertically or horizontally? (v / h)")
        while s != "v" and s != "h":
                print ("Sorry, this is not a valid option")
                s = raw_input("Would you like to place the boat vertically or horizontally? (v / h)")
        return s
def getPosition():
        wordS = " "
        s = raw_input("Initial box [row:column from 0 to 4]:")
        while wrongPosition(s) == True:
                print ("Sorry, this is not a valid option")
                s = raw_input("Initial box [row:column from 0 to 4]:")
        wordS = s.split(":")
        return wordS
def applyPlay(taulell,shoot):
    s = shoot
    word = taulell[int(s[0])][int(s[1])]
    if word == "X" or word == "O":
        print ("This box has already been played didn't you noticed stupid piece of shit! You've missed a shot! and some bolts and nuts in your brain!")
        return taulell
    elif word == "S":
        taulell[int(s[0])][int(s[1])] = "O"
        print ("IMPACT! GOOD JOB MY GENERAL!")
        return taulell
    elif word == "W":
        taulell[int(s[0])][int(s[1])] = "S"
        print ("YOU'VE HIT WATER! R U DUM OR YOU COMB BOLDS WTF? are you training to be silly I don know plis kill yourself")
        return taulell
def someBoxOccupied(b,x,y,o):
        if int(x)+ 2 > 5 or int(y) + 2 > 5:
                return False
        else:
                x = int(x)
                y  = int(y)
                print (b)
                if o == "v":
                        print (b[x][y])
                        print (b[x+1][y])
                        print (b[x+2][y])

                        if b[x][y] == 'W' and b[x+1][y] == 'W' and b[x+2][y] == 'W' :
                                return True
                        else:
                                return False
                elif o == "h":
                        print (b[x][y])
                        print (b[x][y+1])
                        print (b[x][y+2])
                        if b[x][y] == 'W' and b[x][y+1] == 'W' and b[x][y+2] == 'W' :
                                return True
                        else:
                                return False
def showBoard(b):
        for panels in b:
                print (''.join(panels))
def thereIsNoNumber(s):
    sp = s.split(":")
    if sp[0] in string.digits:
                if sp[1] in string.digits:
                        return False
    else:
        return True
def placeShip3(tb,i):
    print ("Reading the 3 positions ship number" + str(i))
    position = getPosition()
    orientation = getOrientation()
    boxStatus = someBoxOccupied(tb,position[0],position[1],orientation)
    if boxStatus == True:
        return applyPlay(tb,position)
def setColor(color,player):
    if(color == 1):
        color = "blue"
    elif(color == 2):
        color = "red"
    return colored(player, color)
def placeInitialShips(tb,players):
        i = 0
        while i < 3:
                print ("Reading the 3 positions ship number: " + str(i))
                position = getPosition()
                orientation = getOrientation()
                boxStatus = someBoxOccupied(tb,position[0],position[1],orientation)
                if boxStatus == True:
                        print (applyPlay(tb,position))
                        if orientation == "v":
                                position[0] = int(position[0]) + int(1)
                                print (applyPlay(tb,position))
                                position[0] = int(position[0]) + int(1)
                                print (applyPlay(tb,position))
                                showBoard(tb)
                        elif orientation == "h":
                                position[1] = int(position[1]) + int(1)
                                print (applyPlay(tb,position))
                                position[1] = int(position[1]) + int(1)
                                print (applyPlay(tb,position))
                                showBoard(tb)
                        print ("|" * 10)
                        print ("Boat was placed succesfully")
                        print ("|" * 10)
                        i = i + 1
                else:
                        print ("|"*10)
                        print ("This boat can't be placed here" + str(boxStatus))
                        print ("|"*10)
                        print ("Reading the 3 positions ship number: " + str(i))
                        position = getPosition()
                        orientation = getOrientation()
                        boxStatus = someBoxOccupied(tb,position[0],position[1],orientation)
        print ("|"*10)
        print ("Initial boats were placed succesfully")
        print ("|"*10)
        Play(tb,players)
def placeShips(tb):
        i = 0
        while i < 3:
                print ("Reading the 3 positions ship number: " + str(i))
                position = getPosition()
                orientation = getOrientation()
                boxStatus = someBoxOccupied(tb,position[0],position[1],orientation)
                if boxStatus == True:
                        print (applyPlay(tb,position))
                        if orientation == "v":
                                position[0] = int(position[0]) + int(1)
                                print (applyPlay(tb,position))
                                position[0] = int(position[0]) + int(1)
                                print (applyPlay(tb,position))
                                showBoard(tb)
                        elif orientation == "h":
                                position[1] = int(position[1]) + int(1)
                                print (applyPlay(tb,position))
                                position[1] = int(position[1]) + int(1)
                                print (applyPlay(tb,position))
                                showBoard(tb)
                        print ("|" * 10)
                        print ("Boat was placed succesfully")
                        print ("|" * 10)
                        i = i + 1
                else:
                        print ("|"*10)
                        print ("This boat can't be placed here" + str(boxStatus))
                        print ("|"*10)
                        print ("Reading the 3 positions ship number: " + str(i))
                        position = getPosition()
                        orientation = getOrientation()
                        boxStatus = someBoxOccupied(tb,position[0],position[1],orientation)
                        showBoard(tb)
def cleanScreen(i):
    print ("\n"*i)
def Play(table,players):
        showBoard(table)
        print ("Let's start with this game.")
        print ("But first a Chuck Norris joke to warm up")
        joke = jokes.random(categories=['nerdy'])
        print (joke.joke)
        print (final(table))
        PlayerA = True
        PlayerB = False
        while final(table) == False:
            if PlayerA == True:
                print ("It's your turn " + players[0])
                positionA = getPosition()
                applyPlay(table,positionA)
                showBoard(table)
                if final(table) == False:
                        PlayerB = True
                        PlayerA = False
                else:
                        PlayerB = False
                        PlayerA = True
            elif PlayerB == True:
                print ("It's your turn " + players[1])
                positionB = getPosition()
                applyPlay(table,positionB)
                showBoard(table)
                if final(table) == False:
                        PlayerB = False
                        PlayerA = True
                else:
                        PlayerB = True
                        PlayerA = False
        ExitGame(PlayerA,PlayerB,players)
def ExitGame(statusA,statusB,players):
        global wonA
        global wonB
        global lostA
        global lostB
        if(statusA == True):
                print ("You've won " + players[0])
                wonA = wonA +1
                lostB = lostB +1
                data['playerA'].append({
                        'wonA': wonA,
                })
                data['playerB'].append({
                        'lostB': lostB,
                })
        elif(statusB == True):
                print ("You've won " + players[1])
                wonB = wonB +1
                lostA = lostA +1
                data['playerB'].append({
                        'wonB': wonB,
                }),
                data['playerA'].append({
                        'lostA': lostA,
                })
        cleanScreen(5)
        print (wonA)
        print (lostA)
        print (wonB)
        print (lostB)
        print ("//---------------------------\\")
        print ("|| 1)PLAY AGAIN              ||")
        print ("|| 2)MAIN MENU               ||")
        print ("|| 3)EXIT                    ||")
        print ("\\---------------------------//")
        s = input("Choose An Option: ")



        if s == 1:
                Game()
        elif s == 2:
                MainMenu()
        elif s == 3:
                sys.exit(0)
        elif s == 1940:
                ChuckNorris()
def DetectExit():
        while True:#making a loop
            try: #used try so that if user pressed other than the given key error will not be shown
                if keyboard.is_pressed('q'):#if key 'q' is pressed
                    print('Returning to main Matrix!')
                    MainMenu()
                else:
                    pass
            except:
                break #if user pressed a key other than the given key the loop will break
def Game():
    print("Starting")
        #Declaring initial variables
    playerA = True
    playerB = False
    once = False
    turn = 0
    joke = jokes.random(categories=['nerdy'])
    #Printing initial joke
    print (joke.joke)
    print ("Which color  do you want to be Player 1?")
    print ("1) Blue, 2) Red")
    color_choose = input("Write 1 or 2 = ")
    print ("Which color  do you want to be Player 2?")
    print ("1) Blue, 2) Red")
    color_choose1 = input("Write 1 or 2 = ")
    print ("Entering the matrix....")


    player_name1 = raw_input("Player 1 tell me your name ")

    cprint(figlet_format(player_name1, font='larry3d' ),'red', attrs=['bold'])#Prints the name of the player 2

    player_name2 = raw_input("Player 2 tell me your name ")

    cprint(figlet_format(player_name2, font='larry3d' ),'blue', attrs=['bold'])#Prints the name of the player 2

    players = [player_name1, player_name2]
    colored = setColor(color_choose,players[0])
    colored1 = setColor(color_choose1,players[1])
    #players = [colored,colored1]
    #print colored + " " + "You'll be the player 1"
    #print colored1 + " " + "You'll be the player 2"
    color = [colored,colored1]
    data['playerA'].append({
        'name': players[0],
    })
    data['playerB'].append({
        'name': players[1],
    })

    print ("Name: " + data['playerA'][0]['name'])
    print ("Name: " + data['playerB'][0]['name'])
    print ("WELCOME TO WARSHIP.PY")
    print ("|"*10)
    print ("Let's place initial (hided what did you thought? you are not gonna win always) ships")
    print ("|"*10)
    tb = placeInitialShips([["W", "W", "W", "W", "W"], ["W", "W", "W", "W", "W"], ["W", "W", "W", "W", "W"], ["W", "W", "W", "W", "W"], ["W", "W", "W", "W", "W"]],color)
def ChuckNorris():
        print ("Welcome to Chuck Norris Mode")
        sleep(5)
        while True:#making a loop
            try: #used try so that if user pressed other than the given key error will not be shown
                joke = jokes.random(categories=['nerdy'])
                sleep(1)
                print (joke.joke)
                if keyboard.is_pressed('q'):#if key 'q' is pressed
                    print('Returning to main Matrix!')
                    MainMenu()
                else:
                    pass
            except:
                break #if user pressed a key other than the given key the loop will break
def Statistics():
         print ("Statistics")
         print ("Click q to return to MainMenu")
         with open('data.txt') as json_file:
             data = json.load(json_file)
             print ("File does exist")
         print (data['playerA'][0]['wonA'])
         print (data['playerA'][0]['lostA'])
         print (data['playerB'][0]['wonB'])
         print (data['playerB'][0]['lostB'])
         DetectExit()

def MainMenu():
                #Bot.main()
                #os.system("py BotDemo.py")
                cprint(figlet_format('Warship.py', font='big'),
                       'yellow', attrs=['bold'])
                wordS = " "
                #title =  u"\uD83D\uDEA2"
                #print(data)
                #print (data['playerA'][0]['wonA'])
                #print (data['playerA'][0]['lostA'])
                #print (data['playerB'][0]['wonB'])
                #print (data['playerB'][0]['lostB'])
                print ("~"*67)
                print ("Welcome to Warship.py, a game about sinking ships and winning wars.")
                print ("Created by Bruno Moya at Codelearn")
                print ("www.elblogdebruno.com - github.com/warship")
                print ("~"*67)
                print ("//---------------------------\\")
                print ("|| 1)PLAY THE GAME           ||")
                print ("|| 2)STATISTICS              ||")
                print ("|| 3)EXIT                    ||")
                print ("|| 4)TELEGRAM BOT MODE       ||")
                print ("\\---------------------------//")
                s = input("Choose An Option: ")
                if s == 1:
                       print("Starting game")
                       Game()
                       print("Starting game")
                elif s == 2:
                       Statistics()
                elif s == 3:
                       sys.exit(0)
                elif s == 4:
                       TelegramBot()
                elif s == 1940:
                       ChuckNorris()

MainMenu()
