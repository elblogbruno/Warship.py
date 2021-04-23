import subprocess

from FlaskServer import * 
from TelegramBot import *

from threading import Thread

from utils.secrets import TELEGRAM_TOKEN

def flask_server(): 
   #server = BOT_SERVER(True)
   subprocess.call('python FlaskServer.py', creationflags=subprocess.CREATE_NEW_CONSOLE)

def telegram_bot():
   #bot = TELEGRAM_BOT(TELEGRAM_TOKEN,True)
   subprocess.call('python TelegramBot.py', creationflags=subprocess.CREATE_NEW_CONSOLE)

if __name__ == "__main__":
   #Thread(target = flask_server,daemon=True).start() 
   #telegram_bot()
   Thread(target = telegram_bot,daemon=True).start()
   Thread(target = flask_server,daemon=True).start() 

   
   
   