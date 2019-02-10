#!/usr/bin/env python
# -*- coding: utf-8 -*-
#
# Simple Bot to reply to Telegram messages
# This program is dedicated to the public domain under the CC0 license.

"""
This Bot uses the Updater class to handle the bot.

First, a few callback functions are defined. Then, those functions are passed to
the Dispatcher and registered at their respective places.
Then, the bot is started and runs until we press Ctrl-C on the command line.

Usage:
Example of a bot-user conversation using ConversationHandler.
Send /start to initiate the conversation.
Press Ctrl-C on the command line or send a signal to the process to stop the
bot.
"""
from PIL import Image, ImageDraw, ImageFont
from telegram import ReplyKeyboardMarkup
from telegram.ext import (Updater, CommandHandler, MessageHandler, Filters, RegexHandler,
                          ConversationHandler)

import logging

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)

CHOOSING, TYPING_REPLY, TYPING_CHOICE = range(3)

reply_keyboard = [['Age', 'Favourite colour'],
                  ['Number of siblings', 'Something else...'],['Start Game']]
markup = ReplyKeyboardMarkup(reply_keyboard, one_time_keyboard=True)
positions = {"0:0":(20,20),"0:1":(140,20),"0:2":(260,20),"0:3":(375,20),"0:4":(490,20),
            "1:0":(20,135),"1:1":(140,135),"1:2":(260,135),"1:3":(375,135),"1:4":(495,135),
            "2:0":(20,255),"2:1":(140,255),"2:2":(260,255),"2:3":(375,255),"2:4":(495,255),
            "3:0":(20,375),"3:1":(140,375),"3:2":(260,375),"3:3":(375,375),"3:4":(495,375),
            "4:0":(20,495),"4:1":(140,495),"4:2":(260,495),"4:3":(375,495),"4:4":(495,495)}
size = 85, 85
grid = Image.open('grid.png', 'r').convert('RGBA')
wave_icon = Image.open('wave_icon.png', 'r').convert('RGBA')
wave_icon.load()
ship_icon = Image.open('ship_icon_better1.png', 'r').convert('RGBA')
ship_icon.load()
white_paint = Image.open('white_paint.png', 'r').convert('RGBA')
white_paint.load()
sunk_icon = Image.open('sunk_ship.png', 'r').convert('RGBA')
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
def facts_to_str(user_data):
    facts = list()

    for key, value in user_data.items():
        facts.append('{} - {}'.format(key, value))

    return "\n".join(facts).join(['\n', '\n'])


def start(bot, update):
    chat_id = update.message.chat_id
    update.message.reply_text(
        "Hi! I'm the General Kenobi of ships. I will hold a more complex conversation with you before we start."
        "Why don't you tell me something about yourself before we start playing?",
        reply_markup=markup)
    bot.send_photo(chat_id=chat_id, photo='https://vignette.wikia.nocookie.net/central/images/4/40/Hello_there.jpg/revision/latest?cb=20171117031251')
    return CHOOSING


def regular_choice(bot, update, user_data):
    text = update.message.text
    user_data['choice'] = text
    update.message.reply_text(
        'Your {}? Yes, I would love to hear about that!'.format(text.lower()))
    text = update.message.text
    category = user_data['choice']
    user_data[category] = text
    print(user_data)
    return TYPING_REPLY


def custom_choice(bot, update):
    update.message.reply_text('Alright, please send me the category first, '
                              'for example "Most impressive skill"')

    return TYPING_CHOICE

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
            chat_id = update.message.chat_id
            pos = positions[update.message.text]
            print pos
            pasteIcon(ship_icon,pos,ship_mask)
            bot.send_photo(chat_id=chat_id, photo=open('out.png', 'rb'))
def received_information(bot, update, user_data):
    text = update.message.text
    category = user_data['choice']
    user_data[category] = text
    del user_data['choice']
    chat_id = update.message.chat_id
    update.message.reply_text("Neat! Just so you know, this is what you already told me:"
                              "{}"
                              "You can tell me more, or change your opinion on something.".format(
                                  facts_to_str(user_data)), reply_markup=markup)
    pos = positions[update.message.text]
    print pos
    pasteIcon(ship_icon,pos,ship_mask)
    bot.send_photo(chat_id=chat_id, photo=open('out.png', 'rb'))
    return CHOOSING

def done(bot, update, user_data):
    if 'choice' in user_data:
        del user_data['choice']

    update.message.reply_text("I learned these facts about you:"
                              "{}"
                              "Starting Game!".format(facts_to_str(user_data)))

    user_data.clear()
    return ConversationHandler.END


def error(bot, update, error):
    """Log Errors caused by Updates."""
    logger.warning('Update "%s" caused error "%s"', update, error)


def main():
    # Create the Updater and pass it your bot's token.
    updater = Updater("729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk")

    # Get the dispatcher to register handlers
    dp = updater.dispatcher
    def stop_and_restart():
        """Gracefully stop the Updater and replace the current process with a new one"""
        updater.stop()
        os.execl(sys.executable, sys.executable, *sys.argv)

    def restart(bot, update):
        update.message.reply_text('Bot is restarting...')
        Thread(target=stop_and_restart).start()

    # ...or here...

    dp.add_handler(CommandHandler('r', restart))
    # Add conversation handler with the states CHOOSING, TYPING_CHOICE and TYPING_REPLY
    conv_handler = ConversationHandler(
        entry_points=[CommandHandler('start', start)],

        states={
            CHOOSING: [RegexHandler('^(Age|Favourite colour|Number of siblings)$',
                                    regular_choice,
                                    pass_user_data=True),
                       RegexHandler('^Something else...$',
                                    custom_choice),
                       ],
            TYPING_CHOICE: [MessageHandler(Filters.text,
                                           regular_choice,
                                           pass_user_data=True),
                            ],

            TYPING_REPLY: [MessageHandler(Filters.text,
                                          received_information,
                                          pass_user_data=True),
                           ],
        },

        fallbacks=[RegexHandler('^Start Game$', done, pass_user_data=True)]
    )

    dp.add_handler(conv_handler)

    # log all errors
    dp.add_error_handler(error)

    # Start the Bot
    updater.start_polling()

    # Run the bot until you press Ctrl-C or the process receives SIGINT,
    # SIGTERM or SIGABRT. This should be used most of the time, since
    # start_polling() is non-blocking and will stop the bot gracefully.
    updater.idle()
    while True:
        try:
            echo(bot)
        except NetworkError:
            sleep(1)
        except Unauthorized:
            # The user has removed or blocked the bot.
if __name__ == '__main__':
    main()
