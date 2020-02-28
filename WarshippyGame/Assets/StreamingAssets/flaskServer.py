import logging
import threading
import os
from time import sleep
import telegram
from telegram.error import NetworkError, Unauthorized
from telegram.ext import Updater, CommandHandler, MessageHandler, Filters
import atexit
import re
from concurrent.futures import ProcessPoolExecutor, ThreadPoolExecutor
from flask import Flask, request, redirect, url_for
from werkzeug.utils import secure_filename
import GameHelper
import threading
import BotDemo as bot
import json
ALLOWED_EXTENSIONS = set(['txt', 'pdf', 'png', 'jpg', 'jpeg', 'gif'])
oldCoordenates = " "
chat_id = 415919768

# lock to control access to variable
dataLock = threading.Lock()
# thread handler
yourThread = threading.Thread()
TOKEN = "729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk"
app = Flask(__name__)
@app.route('/sendimage', methods = ['GET', 'POST'])
def upload_file():
    file = request.files['image']
    # if user does not select file, browser also
    # submit a empty part without filename
    if file.filename == '':
        print('No selected file')
        return redirect(request.url)
    elif file.filename != '':
        filename = secure_filename(file.filename)+".jpg"
        print(filename)
        file.save(filename)
        bot.send_image(filename)
    return filename
@app.route("/sendmessage")
def sendmessage():
    q = request.args.get('text')
    bot.send_text(q)
    return q
@app.route("/askForPhoto")
def askForPhoto():
    bot.getNewUserNameAndProfile()
    q = "photo"
    return q
@app.route("/sendaudio")
def sendaudio():
    q = request.args.get('text')
    bot.send_audio(q)
    return q
@app.route("/getmessage")
def getmessage():
    # file1 = open("message.txt","r") 
    # q = file1.read() 
    with open('message.txt') as json_file:
        q = json.load(json_file)
    print("Getting message: "+ str(q))
    return q
@app.route('/', methods=['GET', 'POST'])
def handle_request():
    return "Flask Server & Android are Working Successfully"


def startServer():
    app.run(port=5001,debug = True)

# if __name__ == "__main__":
#     startServer()