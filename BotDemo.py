#!/usr/bin/env python
# -*- coding: utf-8 -*-
"""Simple Bot to reply to Telegram messages.

This is built on the API wrapper, see echobot2.py to see the same example built
on the telegram.ext bot framework.
This program is dedicated to the public domain under the CC0 license.
"""
import logging
import telegram
from telegram.error import NetworkError, Unauthorized
from time import sleep
from chuck import ChuckNorris
jokes = ChuckNorris()
import socket
# next create a socket object
s = socket.socket()
print ("Socket successfully created")
# reserve a port on your computer in our
# case it is 12345 but it can be anything
port = 8000
s.bind(('', port))
print ("socket binded to %s" %(port))
# put the socket into listening mode
s.listen(5)
print ("socket is listening")
update_id = None
c, addr = s.accept()
print ('Got connection from', addr)
word = 'Thank you for connecting'

def main():
    # a forever loop until we interrupt it or
    # an error occurs

    """Run the bot."""
    global update_id
    # Telegram Bot Authorization Token
    bot = telegram.Bot('729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk')

    # get the first pending update_id, this is so we can skip over it in case
    # we get an "Unauthorized" exception.
    try:
        update_id = bot.get_updates()[0].update_id
    except IndexError:
        update_id = None

    logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s')

    while True:
        try:
            echo(bot)
        except NetworkError:
            sleep(1)
        except Unauthorized:
            # The user has removed or blocked the bot.
            update_id += 1
    print ('server started and listening')

def sendData(text):

    my_str_as_bytes = str.encode(text)
    s.send(my_str_as_bytes)
           # Close the connection with the client


def echo(bot):
    """Echo the message the user sent."""
    global update_id
    # Request updates after the last update_id
    for update in bot.get_updates(offset=update_id, timeout=10):
        update_id = update.update_id + 1

        if update.message:  # your bot can receive updates without messages
            # Reply to the message
            #print(update.message.text)
            joke = jokes.random(categories=['nerdy'])
            #Printing initial joke
            #print (joke.joke)
            if update.message.text == "Chuck":
                update.message.reply_text(joke.joke)
                c.send(update.message.text)
                c.send(joke.joke)
            else:
                msg = update.message.text
                update.message.reply_text(update.message.text)
                #sendData("hello")
                c.send(update.message.text)
if __name__ == '__main__':
    main()
