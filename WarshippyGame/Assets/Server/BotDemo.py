#!/usr/bin/env python
# -*- coding: utf-8 -*-
import json
import logging
import telegram
from telegram.error import NetworkError, Unauthorized
from telegram.ext import Updater, CommandHandler, MessageHandler, Filters
from time import sleep
import time
from chuck import ChuckNorris
import subprocess
import os
from gtts import gTTS
import zmq
import base64
jokes = ChuckNorris()
import threading
import GameHelper
import ssl
import sys
 
import paho.mqtt.client
class chat_bot_user:
    user_id = " "
    user_photo_id = " "
    user_name = " "

client = paho.mqtt.client.Client()


positions = [["0:0","0:1","0:2","0:3","0:4"],
            ["1:0","1:1","1:2","1:3","1:4"],
            ["2:0","2:1","2:2","2:3","2:4"],
            ["3:0","3:1","3:2","3:3","3:4"],
            ["4:0","4:1","4:2","4:3","4:4"]]

update_id = None
chat_id = 415919768
botToken = "729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk"
ReceivedMessage = ""

def startBot():
    # w = threading.Thread(target=startEcho)
    # w.start()
    # startEcho()
    while getNewUserNameAndProfile() == False:
        print("[Bot] We don't have any user photo")
        return False
    return True
def error(update, context):
    """Log Errors caused by Updates."""
    print('Update "%s" caused error "%s"', update, context.error)

def getid(update, context):
    update.message.reply_text(update.message.chat_id)
def on_connect(client, userdata, flags, rc):
    print('connected (%s)' % client._client_id)
    client.subscribe(topic='BOT', qos=2)
def on_message(client, userdata, message):
    print('------------------------------')
    print('topic: %s' % message.topic)
    print('payload: %s' % message.payload)
    print('qos: %d' % message.qos)

def main():
    """Start the bot."""
    print("[Bot] Starting the bot...")
    # Create the Updater and pass it your bot's token.
    # Make sure to set use_context=True to use the new context based callbacks
    # Post version 12 this will no longer be necessary
    updater = Updater("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk", use_context=True,workers =10)

    # Get the dispatcher to register handlers
    dp = updater.dispatcher

    # # on different commands - answer in Telegram
    dp.add_handler(CommandHandler("getid", getid))
    # dp.add_handler(CommandHandler("help", help))

    # on noncommand i.e message - echo the message on Telegram
    dp.add_handler(MessageHandler(Filters.text, echo))

    # log all errors
    dp.add_error_handler(error)

    # Start the Bot
    updater.start_polling()

    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    #startBot()
    global client
    client.connect(host='127.0.0.1', port=1883)
    client.on_disconnect = on_disconnect
    print("[Bot] Bot started...")
    updater.idle()


def readUserInput():
    global update_id
    data = " "
    while True:
        try:
            echo()
        except NetworkError:
            sleep(1)
        except Unauthorized:
            # The user has removed or blocked the bot.
            update_id += 1

##BOT UTILS
def setBotToken(token):
    global botToken
    botToken = token

##BOT INTERACTION FUNCTIONS
def send_image_url(imageUrl):
    global botToken
    print("[Bot] Sending photo: " + imageUrl)
    bot = telegram.Bot(botToken)
    #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id

    bot.send_photo(chat_id = chat_id,photo=imageUrl)
    return
def send_audio(imageUrl):
    global botToken
    print("[Bot] Sending audio: " + imageUrl)
    bot = telegram.Bot(botToken)
    #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
    tts = gTTS(imageUrl,'en')
    tts.save('hello.mp3')
    bot.send_voice(chat_id=chat_id, voice=open('hello.mp3', 'rb'))
    return
def send_image(imageFile):
            #command = 'curl -s -X POST https://api.telegram.org/bot' + botToken + '/sendPhoto -F chat_id=' + chat_id + " -F photo=@" + imageFile
            #subprocess.call(command.split(' '))
            global botToken
            print("[Bot] Sending photo: " + imageFile)
            bot = telegram.Bot(botToken)
            #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
            try:
                bot.send_photo(chat_id = chat_id,photo=open(imageFile,'rb'))
            except:
                print("[Bot] Cannot send photo")
                pass
            
            return
def send_attack_query():
    global botToken
    print("[Bot] Sending attack query")
    bot = telegram.Bot(botToken)
    custom_keyboard = [["0:0","0:1","0:2","0:3","0:4"],
            ["1:0","1:1","1:2","1:3","1:4"],
            ["2:0","2:1","2:2","2:3","2:4"],
            ["3:0","3:1","3:2","3:3","3:4"],
            ["4:0","4:1","4:2","4:3","4:4"]]
    reply_markup = telegram.ReplyKeyboardMarkup(custom_keyboard)
    bot.send_message(chat_id=chat_id,remove_keyboard = True,text="Custom Keyboard Test",reply_markup=reply_markup)
def send_text(msg):
    global update_id
    global botToken
    print("[Bot] Sending message: " + msg)
    bot = telegram.Bot(botToken)
    bot.sendMessage(chat_id,msg,timeout = 10)
    return

##SERVER INTERACTION
def get_user_text():
    global botToken
    global ReceivedMessage
    bot = telegram.Bot(botToken)
    #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
    try:
        with open('message.txt') as json_file:
            data = json.load(json_file)
        #message = GameHelper.getMessages()
        print("[Bot] Giving user text: " + data)
        return data
    except:
        print("[Bot] Cannot send photo")
        return "[Bot] Cannot send photo"
        pass
    
    return    
def askForStart():
    print("[Bot] Asking for user image")
    send_text("Please send me your profile picture!")
def getNewUserNameAndProfile():
    print("[Bot] Getting user name and profile picture")
    global update_id
    logging.warning(update_id)
    global botToken
    bot = telegram.Bot(botToken)
    new_chat_user = chat_bot_user()
    askForStart()
    while new_chat_user.user_name == " " and new_chat_user.user_id == " " and new_chat_user.user_photo_id == " ":
        
        for update in bot.get_updates(offset=update_id, timeout=10):
            update_id = update.update_id + 1
            print ("[Bot] Looking for user photo input...")
            if update.message:
                photo = update.message.photo
                user = update.message.from_user
                if user:
                    new_chat_user.user_name = user.username
                    new_chat_user.user_id = user['id'] 
                else:
                    print("[Bot] There's no user information yet...")
                if photo:
                    new_chat_user.user_photo_id = photo[-1]["file_id"]
                    send_text("Getting profile picture....")
                    break
                else:
                    print("[Bot] There is no user photo yet.")
            else:
                print("[Bot] User does not want to play.")
                return False

        OnNewUserFound(new_chat_user)
    return True
def OnNewUserFound(chat_user):
    print('[Bot] Bot is talking with user {} and his user ID is: {} and user photo ID is: {}'.format(chat_user.user_name, chat_user.user_id,chat_user.user_photo_id))
    try:
        global botToken
        bot = telegram.Bot(botToken)

        if(os.path.exists('{}.jpg'.format(chat_user.user_photo_id))):
            print("[Bot] Photo Exists...")
            send_text("This picture does already exist in my database. I'll use it as you seem to like it!")
            send_text("Photo received succesfully!")
            send_text("Welcome to warshippy {}! Let's get the war started! ".format(chat_user.user_name))
        else:
            print("[Bot] Photo hasn't been downloaded")
            newFile = bot.get_file(chat_user.user_photo_id)
            newFile.download(custom_path='./'  + chat_user.user_photo_id +'.jpg')
            print("[Bot] Downloading user picture...")
            send_text("Photo received succesfully!")
            send_text("Welcome to warshippy {}! Let's get the war started! ".format(chat_user.user_name))
    except telegram.TelegramError as e:
        print ("[Bot] ERROR " + str(e))
        # if (str(e) == "Invalid file id") :
        #     print("[Bot] Invalid file id. Asking again:")
        #     sendText("Invalid file. Send a correct profile picture:")
        #     getNewUserNameAndProfile()
        # else:
        #     print("[Bot] Other unknown error.")
        # pass

def echo(update, context):
    print("[Bot] Echoing what user is typing.")
    update.message.chat_id
    message = update.message.text
    print("[Bot] Current Chat ID: " + str(chat_id))
    print("[Bot] Current Received Message: " + str(message))
    update.message.reply_text(message)
    setCurrentMessage(message)

def setCurrentMessage(text):
    isCorrect = GameHelper.update_position(text)
    GameHelper.setCurrentMessage(text)
    if isCorrect:
        client.publish("BOT", text)
        print("[BotDemo] Correct position.")
    else:
        print("[BotDemo] Not a correct position. Passing")
    # data = {'message': text, 'has_attacked': 'true'}
    # with open('message.txt', 'w') as outfile:
    #     json.dump(data, outfile)

def on_disconnect(client, userdata, rc):
    if rc != 0:
        print("Unexpected MQTT disconnection. Will auto-reconnect")
        client.connect(host='127.0.0.1', port=1883)
        send_text("There was an error. Please write that again!")
if __name__ == "__main__":
    main()