#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""Simple Bot to reply to Telegram messages.

This is built on the API wrapper, see echobot2.py to see the same example built
on the telegram.ext bot framework.
This program is dedicated to the public domain under the CC0 license.
"""
#from PIL import Image, ImageDraw, ImageFont
import logging
import telegram
from telegram.error import NetworkError, Unauthorized
from time import sleep
from chuck import ChuckNorris
import subprocess
import os
from gtts import gTTS
jokes = ChuckNorris()

import warlib

# import socket
# # next create a socket object
# s = socket.socket()
# print ("Socket successfully created")
# # reserve a port on your computer in our
# # case it is 12345 but it can be anything
# port = 8000
# s.bind(('', port))
# print ("socket binded to %s" %(port))
# # put the socket into listening mode
# s.listen(5)
# print ("socket is listening")
# update_id = None
# c, addr = s.accept()
# print ('Got connection from', addr)
# word = 'Thank you for connecting'


# positions = {"0:0":[(20,20)," "],"0:1":[(140,20)," "],"0:2":[(260,20)," "],"0:3":[(375,20)," "],"0:4":[(490,20)," "],
#             "1:0":[(20,135)," "],"1:1":[(140,135)," "],"1:2":[(260,135)," "],"1:3":[(375,135)," "],"1:4":[(495,135)," "],
#             "2:0":[(20,255)," "],"2:1":[(140,255)," "],"2:2":[(260,255)," "],"2:3":[(375,255)," "],"2:4":[(495,255)," "],
#             "3:0":[(20,375)," "],"3:1":[(140,375)," "],"3:2":[(260,375)," "],"3:3":[(375,375)," "],"3:4":[(495,375)," "],
#             "4:0":[(20,495)," "],"4:1":[(140,495)," "],"4:2":[(260,495)," "],"4:3":[(375,495)," "],"4:4":[(495,495)," "]}
# size = 85, 85
# grid = Image.open('grid.png', 'r').convert('RGBA')
# wave_icon = Image.open('wave_icon.png', 'r').convert('RGBA')
# wave_icon.load()
# ship_icon = Image.open('ship_icon_better1.png', 'r').convert('RGBA')
# ship_icon.load()
# white_paint = Image.open('white_paint.png', 'r').convert('RGBA')
# white_paint.load()
# sunk_icon = Image.open('sunk_ship.png', 'r').convert('RGBA')
# sunk_icon.load()
# custom_keyboard = [['top-left', 'top-right'],['bottom-left', 'bottom-right']]
# reply_markup = telegram.ReplyKeyboardMarkup(custom_keyboard)
# ship_icon.thumbnail(size, Image.ANTIALIAS)
# ship_mask=ship_icon.split()[3]
# white_paint.thumbnail(size, Image.ANTIALIAS)
# white_mask=white_paint.split()[3]
# wave_icon.thumbnail(size, Image.ANTIALIAS)
# wave_mask=wave_icon.split()[3]
# sunk_icon.thumbnail(size, Image.ANTIALIAS)
# sunk_mask=sunk_icon.split()[3]
update_id = None
once = True
chat_id = 415919768
# def pasteIcon(icon,pos,icon_mask):
#     grid.paste(icon,pos,icon_mask)
#     grid.save("out.png")
def main():
    """Run the bot."""
    global update_id
    # Telegram Bot Authorization Token
    bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
    # hilo1 = threading.Thread(target=echoo())
    # hilo1.start()
    # get the first pending update_id, this is so we can skip over it in case
    # we get an "Unauthorized" exception.
    try:
        update_id =  bot.get_updates()[0].update_id
    except IndexError:
        update_id = None

    logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
    global once
    while once == False:
        try:
            echo()
        except NetworkError:
            sleep(1)
        except Unauthorized:
            # The user has removed or blocked the bot.
            update_id += 1
# def sendData(text):
#
#     my_str_as_bytes = str.encode(text)
#     s.send(my_str_as_bytes)
#            # Close the connection with the client

def send_image_url(botToken,imageUrl):
            print("Sending photo: " + imageUrl)
            bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
            #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id

            bot.send_photo(chat_id = chat_id,photo=imageUrl)
            return
def send_audio(botToken,imageUrl):
            print("Sending audio: " + imageUrl)
            bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
            #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
            tts = gTTS(imageUrl,'es')
            tts.save('hello.mp3')
            bot.send_voice(chat_id=chat_id, voice=open('hello.mp3', 'rb'))
            return
def send_image(botToken, imageFile):
            #command = 'curl -s -X POST https://api.telegram.org/bot' + botToken + '/sendPhoto -F chat_id=' + chat_id + " -F photo=@" + imageFile
            #subprocess.call(command.split(' '))
            print("Sending photo: " + imageFile)
            bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
            #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
            bot.send_photo(chat_id = chat_id,photo=open(imageFile,'rb'))
            return
def sendText(botToken,msg):
            # text = '%(language)s' % \
            #         {'language': msg}
            global update_id
            #print("this is update id " + str(update_id))
            print("Sending message: " + msg)
            #os.system("curl -s -X POST https://api.telegram.org/bot"+botToken+"/sendMessage -F chat_id=" + chat_id + '-F text="%s"' % msg)
            bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
            #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
            #print("this is chat id " + str(chat_id))
            bot.sendMessage(chat_id,msg,timeout = 10)
            #bot.sendMessage(chat_id=chat_id,text="Custom Keyboard Test",reply_markup=reply_markup)
            return
def sendPhoto():
            global bot
            chat_id = "415919768"
            bot.send_photo(chat_id=chat_id, photo=open('out.png', 'rb'))
# def echo():
#         print("Getting what user said")
#         global update_id
#         logging.warning(update_id)
#         #Request updates after the last update_id
#         bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
#         once = False
#         chat_id =  bot.get_updates(timeout = 10)[-1].message.chat_id
#         old_message = bot.get_updates()[0].message.text
#         new_message = " "
#         print ("Old message " + old_message)
#         if new_message == old_message:
#             print("nothing new")
#         else:
#             try:
#                 update_id = bot.get_updates()[0].update_id + 1
#                 bot.get_updates(offset = update_id)[0].message.reply_text(bot.get_updates()[0].message.text)
#                 new_message = bot.get_updates()[0].message.text
#                 print ("Chat id " + str(chat_id))
#                 print("New Message " + new_message)
#                 #tts = gTTS((bot.get_updates()[0].message.text),'en')
#                 #tts.save('hello.mp3')
#                 #bot.send_voice(chat_id=chat_id, voice=open('hello.mp3', 'rb'))
#             except IndexError:
#                 update_id = None
def echo():
    print("Getting what user said")
    global update_id
    logging.warning(update_id)
    # Request updates after the last update_id
    bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')
    for update in bot.get_updates(offset=update_id, timeout=10):
        update_id = update.update_id + 1
        once = False
        if update.message:
            chat_id =  bot.get_updates(timeout = 10)[-1].message.chat_id
            print (chat_id)
            print(update.message.text)
            warlib.update_position(update.message.text)
            # if update.message.text in warlib.positions:
            #     if warlib.thereIsNoNumber(update.message.text) == False:
            #         if  warlib.wrongPosition(update.message.text) == False:
            #             if warlib.positions.has_key(update.message.text):
            #                 pos = warlib.positions[update.message.text][0]
            #                 warlib.positions[update.message.text][1] = "W"
            #                 print(warlib.positions[update.message.text][1])
            #                 print("This is position from warlib.py " + str(pos))
            #                 #pasteIcon(wave_icon,pos,wave_mask)
            #                 update.message.reply_text("Attacking oponent at this coordenates: " + update.message.text)
            #                 warlib.update_position(update.message.text)
            #                 #file = open("coordenates.txt","w")
            #                 #file.write(update.message.text)
            #             else:
            #                 update.message.reply_text("This position " + update.message.text + " is not available")
            # else:
            #     update.message.reply_text("Please write a coordenate like this: 2:2")

            #tts = gTTS((update.message.text),'en')
            #tts.save('hello.mp3')
            #bot.send_voice(chat_id=chat_id, voice=open('hello.mp3', 'rb'))

if __name__ == '__main__':
     main()
