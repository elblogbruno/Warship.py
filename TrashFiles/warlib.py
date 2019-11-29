
from tkinter import *
import BotDemo as bot
from PIL import Image, ImageTk
from tkinter import messagebox
import random
import os
import time
import threading
import string
import tkintermultiplewindows as tk
gameTitle = "Welcome to Warship.py"
positions = {"0:0":[(20,20)," "],"0:1":[(140,20)," "],"0:2":[(260,20)," "],"0:3":[(375,20)," "],"0:4":[(490,20)," "],
            "1:0":[(20,135)," "],"1:1":[(140,135)," "],"1:2":[(260,135)," "],"1:3":[(375,135)," "],"1:4":[(495,135)," "],
            "2:0":[(20,255)," "],"2:1":[(140,255)," "],"2:2":[(260,255)," "],"2:3":[(375,255)," "],"2:4":[(495,255)," "],
            "3:0":[(20,375)," "],"3:1":[(140,375)," "],"3:2":[(260,375)," "],"3:3":[(375,375)," "],"3:4":[(495,375)," "],
            "4:0":[(20,495)," "],"4:1":[(140,495)," "],"4:2":[(260,495)," "],"4:3":[(375,495)," "],"4:4":[(495,495)," "]}
BOT_TOKEN  ="729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk"
size = 85, 85
path = 'sprites/'
grid = Image.open(path + 'grid.png', 'r').convert('RGBA')
wave_icon = Image.open(path + 'wave_icon.png', 'r').convert('RGBA')
wave_icon.load()
ship_icon = Image.open(path + 'ship_icon_better1.png', 'r').convert('RGBA')
ship_icon.load()
white_paint = Image.open(path + 'white_paint.png', 'r').convert('RGBA')
white_paint.load()
sunk_icon = Image.open(path + 'sunk_ship.png', 'r').convert('RGBA')
sunk_icon.load()

ship_icon.thumbnail(size, Image.ANTIALIAS)
ship_mask=ship_icon.split()[3]
white_paint.thumbnail(size, Image.ANTIALIAS)
white_mask=white_paint.split()[3]
wave_icon.thumbnail(size, Image.ANTIALIAS)
wave_mask=wave_icon.split()[3]
sunk_icon.thumbnail(size, Image.ANTIALIAS)
sunk_mask=sunk_icon.split()[3]

def pasteIcon(icon,pos,icon_mask):
    grid.paste(icon,pos,icon_mask)
    grid.save("out.png")
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
def startThread():
    hilo1 = threading.Thread(target=echoo())
    hilo1.start()
def echoo():
    while True:
        time.sleep(2)
        bot.echo()
def update_position(pos):
    if ":" in pos :
        if thereIsNoNumber(pos) == False:
            if wrongPosition(pos) == False:
                if positions.has_key(pos):
                    position = positions[pos][0]
                    print ("This is our position to attack: " + str(pos))
                    print ("This is our position to paste the icon: " + str(position))
                    pasteIcon(ship_icon,position,ship_mask)
                    bot.send_image(BOT_TOKEN,"out.png")
                    msg = "Where do you want to attack?"
                    bot.sendText(BOT_TOKEN,msg)
    else:
        msg = "This position is not available, Please write a coordenate like this: 2:2"
        bot.sendText(BOT_TOKEN,msg)
def ClearGrid(panel):
    for position in positions:
        print(position)
        pasteIcon(white_paint,positions[position][0],white_mask)
    img2 = ImageTk.PhotoImage(Image.open("out.png"))
    #panel = tk.Label(self, image = img2)
    panel.configure(image=img2)
    panel.image = img2
def update_position_panel(pos,panel):
    if ":" in pos :
        if thereIsNoNumber(pos) == False:
            if wrongPosition(pos) == False:
                if positions.has_key(pos):
                    position = positions[pos][0]
                    print ("This is our position to attack: " + str(pos))
                    print ("This is our position to paste the icon: " + str(position))
                    pasteIcon(ship_icon,position,ship_mask)
                    img2 = ImageTk.PhotoImage(Image.open("out.png"))
                    panel.configure(image=img2)
                    panel.image = img2
                    bot.send_image(BOT_TOKEN,"out.png")
                    msg = "Where do you want to attack?"
                    bot.sendText(BOT_TOKEN,msg)
                    bot.send_audio(bot.warlib.BOT_TOKEN,"Atacando barco en las coordenadas " + pos)
    else:
        messagebox.showinfo(gameTitle, "This position is not available")
        msg = "This position is not available, Please write a coordenate like this: 2:2"
        bot.sendText(BOT_TOKEN,msg)
