#!/usr/bin/env python
# -*- coding: utf-8 -*-
#TODO CONVERSATION HANDLER
import json
import logging
import telegram

from secrets import TELEGRAM_TOKEN
from telegram.ext import (Updater, CommandHandler, MessageHandler, Filters,
                          ConversationHandler)

from telegram.error import NetworkError, Unauthorized
from time import sleep

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

from enum import Enum

class GameState(Enum):
    Playing = 1
    PlacingBoats = 2
    AskingForPicture = 3
    idle = 4

class chat_bot_user:
    user_id = " "
    user_photo_id = " "
    user_name = " "

client = paho.mqtt.client.Client()

logging.basicConfig(level=logging.DEBUG,
                    format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')
positions = [["0:0","0:1","0:2","0:3","0:4"],
            ["1:0","1:1","1:2","1:3","1:4"],
            ["2:0","2:1","2:2","2:3","2:4"],
            ["3:0","3:1","3:2","3:3","3:4"],
            ["4:0","4:1","4:2","4:3","4:4"]]


update_id = None
chat_id = 415919768
#botToken = "729316731:AAHXSFIvFXLtXuTFe34vPDkLzGxHgGEhPm8"
bot = telegram.Bot(TELEGRAM_TOKEN)
m_gameState = GameState.idle
ReceivedMessage = ""
shouldGetImage = True
isMqttConnected = True
def publishOnMqtt(text,isImage):
    topic = "BOT"
    if isImage:
        topic = "IMAGE"     
    client.publish(topic, text)
def error(update, context):
    """Log Errors caused by Updates."""
    print('Update "{update}" caused error "{context}"'.format(update=update,context=context.error))
def gethelp(update, context):
    update.message.reply_text("Use /getid to get chat id.")
def getid(update, context):
    update.message.reply_text(update.message.chat_id)
def on_connect(client, userdata, flags, rc):
    if rc == 0:
        global isMqttConnected
        print("[MQTT] Connected ok: " + isMqttConnected)
        isMqttConnected = True
    print('connected (%s)' % client._client_id)
    client.subscribe(topic='BOT', qos=2)
def on_message(client, userdata, message):
    print('------------------------------')
    print('topic: %s' % message.topic)
    print('payload: %s' % message.payload)
    print('qos: %d' % message.qos)

def on_disconnect(client, userdata, rc):
    if rc != 0:
        global isMqttConnected
        print("[MQTT] Unexpected MQTT disconnection. Will auto-reconnect")
        print("[MQTT] isMqttConnected: " + isMqttConnected)

        isMqttConnected = False
        client.connect(host='127.0.0.1',keepalive=60, port=1883)
        send_text("There was an error. Please try again!")

def start(update, context):
    update.message.reply_text(
        "Hi! My name is Doctor Botter. I will hold a more complex conversation with you. "
        "Why don't you tell me something about yourself?",
        reply_markup=markup)

    return CHOOSING
def main():
    """Start the bot."""
    print("[Bot] Starting the bot...")
    # Create the Updater and pass it your bot's token.
    # Make sure to set use_context=True to use the new context based callbacks
    # Post version 12 this will no longer be necessary
    updater = Updater(TELEGRAM_TOKEN, use_context=True)

    # Get the dispatcher to register handlers
    dp = updater.dispatcher

    # # on different commands - answer in Telegram
    dp.add_handler(CommandHandler("getid", getid))
    dp.add_handler(CommandHandler("gethelp", gethelp))

    # on noncommand i.e message - echo the message on Telegram
    dp.add_handler(MessageHandler(Filters.text, echo))
    dp.add_handler(MessageHandler(Filters.photo, getImage))
  
    # Add conversation handler with the states CHOOSING, TYPING_CHOICE and TYPING_REPLY
    conv_handler = ConversationHandler(
        entry_points=[CommandHandler('start', start)],

        states={
            CHOOSING: [MessageHandler(Filters.regex('^(Age|Favourite colour|Number of siblings)$'),
                                      regular_choice),
                       MessageHandler(Filters.regex('^Something else...$'),
                                      custom_choice)
                       ],

            TYPING_CHOICE: [MessageHandler(Filters.text,
                                           regular_choice)
                            ],

            TYPING_REPLY: [MessageHandler(Filters.text,
                                          received_information),
                           ],
        },

        fallbacks=[MessageHandler(Filters.regex('^Done$'), done)]
    )

    dp.add_handler(conv_handler)

    # Start the Bot
    updater.start_polling()
   
    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    #startBot()
    
    global client
    client.connect(host='127.0.0.1',keepalive=60, port=1883)
    client.on_disconnect = on_disconnect
    client.on_connect = on_connect
    
    print("[Bot] Bot started...")
    updater.idle()

    




##BOT INTERACTION FUNCTIONS
def send_image_url(imageUrl):
    global botToken
    print("[Bot] Sending photo: " + imageUrl)
    
    #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id

    bot.send_photo(chat_id = chat_id,photo=imageUrl)
    return
def send_audio(imageUrl):
    global botToken
    print("[Bot] Sending audio: " + imageUrl)
    
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
    
    custom_keyboard = [["0:0","0:1","0:2","0:3","0:4"],
            ["1:0","1:1","1:2","1:3","1:4"],
            ["2:0","2:1","2:2","2:3","2:4"],
            ["3:0","3:1","3:2","3:3","3:4"],
            ["4:0","4:1","4:2","4:3","4:4"]]
    reply_markup = telegram.ReplyKeyboardMarkup(custom_keyboard)
    bot.send_message(chat_id=chat_id,remove_keyboard = True,text="Custom Keyboard Test",reply_markup=reply_markup)
def send_text(msg,context,update):
    print("[Bot] Sending message: " + msg)
    
    context.bot.sendMessage(chat_id,msg,timeout = 10)
    return

##SERVER INTERACTION
# def get_user_text():
#     global botToken
#     global ReceivedMessage
    
#     #chat_id = bot.get_updates(timeout = 10)[-1].message.chat_id
#     try:
#         with open('message.txt') as json_file:
#             data = json.load(json_file)
#         #message = GameHelper.getMessages()
#         print("[Bot] Giving user text: " + data)
#         return data
#     except:
#         print("[Bot] Cannot send photo")
#         return "[Bot] Cannot send photo"
#         pass
    
#     return    


def getImage(update, context):
    new_chat_user = chat_bot_user()    
    photo = update.message.photo
    user = update.message.from_user
    global shouldGetImage
    global isMqttConnected
    if shouldGetImage and isMqttConnected:
        if user:
            new_chat_user.user_name = user.username
            new_chat_user.user_id = user['id'] 
        else:
            print("[Bot] There's no user information yet...")
        if photo:
            new_chat_user.user_photo_id = photo[-1]["file_id"]
            send_text("Getting profile picture....")
            OnNewUserFound(new_chat_user)
        else:
            print("[Bot] There is no user photo yet.")
    else:
        send_text("There was an error with MQTT and Image retrying....")
        client.connect(host='127.0.0.1',keepalive=60, port=1883)
        isMqttConnected = True
        askForStart()
def askForStart():
    print("[Bot] Asking for user image")
    send_text("Please send me your profile picture!")
    #getNewUserNameAndProfile()
def getNewUserNameAndProfile():
    print("[Bot] Getting user name and profile picture")
    global shouldGetImage 
    shouldGetImage = True
    askForStart()


def OnNewUserFound(chat_user):
    print('[Bot] Bot is talking with user {} and his user ID is: {} and user photo ID is: {}'.format(chat_user.user_name, chat_user.user_id,chat_user.user_photo_id))
    try:      
        global shouldGetImage
        shouldGetImage = False
        if(os.path.exists('{}.jpg'.format(chat_user.user_photo_id))):
            print("[Bot] Photo Exists...")
            send_text("This picture does already exist in my database. I'll use it as you seem to like it!")
            send_text("Photo received succesfully!")
            send_text("Welcome to warshippy {}! Let's get the war started! ".format(chat_user.user_name))
        else:
            print("[Bot] Photo hasn't been downloaded yet. Should start downloading now.")
            newFile = bot.get_file(chat_user.user_photo_id)
            newFile.download(custom_path='./'  + chat_user.user_photo_id +'.jpg')
            print("[Bot] Downloading user picture..." + chat_user.user_photo_id)
            send_text("Photo received succesfully!")
            publishOnMqtt(chat_user.user_photo_id+'.jpg',True)
            publishOnMqtt(chat_user.user_name,True)
            send_text("Welcome to warshippy {}! Let's get the war started! ".format(chat_user.user_name))
            send_ask_boat_distribution()
            m_gameState = GameState.PlacingBoats
    except telegram.TelegramError as e:
        print ("[Bot] ERROR " + str(e))
        # if (str(e) == "Invalid file id") :
        #     print("[Bot] Invalid file id. Asking again:")
        #     sendText("Invalid file. Send a correct profile picture:")
        #     getNewUserNameAndProfile()
        # else:
        #     print("[Bot] Other unknown error.")
        # pass


#########
def echo(update, context):
    print("[Bot] Echoing what user is typing.")
    chat_id = update.message.chat_id
    message = update.message.text
    print("[Bot] Current Chat ID: " + str(chat_id))
    print("[Bot] Current Received Message: " + str(message))
    update.message.reply_text(message)
    setCurrentMessage(message)


def setCurrentMessage(text):
    if m_gameState == GameState.Playing:
        isCorrect = GameHelper.update_position(text)
        GameHelper.setCurrentMessage(text)
        if isCorrect:
            publishOnMqtt(text,False)
            print("[BotDemo] Correct position.")
        else:
            print("[BotDemo] Not a correct position. Passing")
    else:
        print("Placing boats currently and getting BOATS")

def send_ask_boat_distribution():
    global botToken
    print("[Bot] Sending attack query")
    send_text("Choose where to place your boats. You can only place 3!")
    custom_keyboard_table = [["0:0","0:1","0:2","0:3","0:4"],
            ["1:0","1:1","1:2","1:3","1:4"],
            ["2:0","2:1","2:2","2:3","2:4"],
            ["3:0","3:1","3:2","3:3","3:4"],
            ["4:0","4:1","4:2","4:3","4:4"]]
    reply_markup_coordenates = telegram.ReplyKeyboardMarkup(custom_keyboard_table)

    custom_keyboard_orientation = [["Vertically","Horizontally"]]
    reply_markup_orientation = telegram.ReplyKeyboardMarkup(custom_keyboard_orientation)
    i = 0
    while i < 3: 
        bot.send_message(chat_id=chat_id,remove_keyboard = True,text="Would you like to place your boat vertically or horizontally? ",reply_markup=reply_markup_orientation)   
        sleep(5)
        bot.send_message(chat_id=chat_id,remove_keyboard = True,text="Choose were to place your boat number {0}".format(i),reply_markup=reply_markup_coordenates)   
        
        if GameHelper.someBoxOccupied():
            send_text("There's a boat already here or you are trying to set the boat out of the fucking playing table you retard!")
            bot.send_message(chat_id=chat_id,remove_keyboard = True,text="Choose were to place your boat number {0}".format(i),reply_markup=reply_markup)   

        #send_text("Would you like to place the boat vertically or horizontally? (v / h)")
        i = i + 1
# if __name__ == "__main__":
#     main()