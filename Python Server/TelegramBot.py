import logging

from gtts import gTTS

from elements.boat import Boat
from elements.ChatBotUser import ChatBotUser

from utils.secrets import TELEGRAM_TOKEN
from telegram import ReplyKeyboardMarkup
from telegram.ext import (Updater, CommandHandler, MessageHandler, Filters,
                          ConversationHandler)
from telegram.error import NetworkError, Unauthorized
from mqtt.mqtt_handler import MQTT_Handler

from enum import Enum

from utils.game_utils import *
from utils.utils import *
from utils.game_helper import *

import os
import base64

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)

IMAGE_FOLDER = "temp_image_folder"

class GameState(Enum):
    Playing = 1
    PlacingBoats = 2
    AskingForPicture = 3
    WaitingForPlayer1 = 4
    WaitingForUs = 5
    idle = 5
    blocked = 6
    unblocked = 7


ASKING_PICTURE, ASKING_WAR, ASKING_BOATS, ASKING_ORIENTATION, ATTACKING = range(5)


class TELEGRAM_BOT(object):
    def send_text(self, msg, context=None, update=None):
        print("[Bot] Sending message: " + msg)
        if context == None or update == None:
            self.context.bot.send_message(chat_id=self.chat_id, text=msg)
        else:
            context.bot.send_message(chat_id=update.message.chat_id, text=msg)
        return

    def send_image(self, msg, context=None, update=None):
        print("[Bot] Sending image: " + msg)
        if context == None or update == None:
            self.context.bot.send_photo(chat_id=self.chat_id, photo=open(msg, 'rb'))
        else:
            context.bot.send_photo(chat_id=update.message.chat_id, photo=open(msg, 'rb'))
        return

    def send_audio(self, imageUrl):
        global botToken
        print("[Bot] Sending audio: " + imageUrl)

        tts = gTTS(imageUrl, 'en')
        tts.save('hello.mp3')
        self.context.bot.send_voice(chat_id=self.chat_id, voice=open('hello.mp3', 'rb'))
        return

    def filter_text(self, text):
        if text == "waitingforplayer2":
            self.game_state = GameState.WaitingForUs
        elif text == "waitingforplayer1":
            self.game_state = GameState.WaitingForPlayer1
        elif text == "block":
            self.game_state = GameState.blocked
        elif text == "unblock":
            self.game_state = GameState.unblocked
        else:
            self.send_text(text)

    def __init__(self, token, debug=False):
        self.debug = debug
        self.token = token
        self.host = '127.0.0.1'
        self.topic = 'BOT'
        self.imageTopic = 'IMAGE'
        self.game_state = GameState.AskingForPicture
        self.mqtt_handler = MQTT_Handler(self.host, self.topic, self.imageTopic, self)
        self.isMqttConnected = self.mqtt_handler.isConnected()
        self.shouldGetImage = True
        self.asking_boats = False
        self.boats_available = []

        createFolder(IMAGE_FOLDER)

        self.main()
        if debug:
            print("[BOT] Bot has started: " + str(self.isMqttConnected))

    def facts_to_str(user_data):
        facts = list()

        for key, value in user_data.items():
            facts.append('{} - {}'.format(key, value))

        return "\n".join(facts).join(['\n', '\n'])

    def start(self, update, context):
        reply_keyboard = [['Yes', 'No']]
        self.chat_id = update.message.chat_id
        self.context = context
        self.shouldGetImage = True
        markup = ReplyKeyboardMarkup(reply_keyboard, one_time_keyboard=True)

        update.message.reply_text(
            "Hi! My name is Doctor Botter. I will hold a more complex conversation with you. "
            "Would you like to play Warshippy with your oponent?",
            reply_markup=markup)

        return ASKING_PICTURE

    def ask_picture(self, update, context):
        print("[BOT] Asking for picture")
        self.send_text("Please send me your profile picture!", context, update)
        self.new_chat_user = ChatBotUser()

        if self.shouldGetImage and self.isMqttConnected:
            photo = update.message.photo
            user = update.message.from_user

            if user:
                self.new_chat_user.user_name = user.username
                self.new_chat_user.user_id = user['id']
                if self.debug:
                    print(self.new_chat_user.user_name)
            else:
                print("[Bot] There's no user information yet...")
            if photo:
                self.new_chat_user.user_photo_id = photo[-1]["file_id"]
                self.send_text("Getting profile picture....", context, update)
                self.on_new_user_found(self.new_chat_user, update)
                return ASKING_WAR
            else:
                print("[Bot] There is no user photo yet.")
                return ASKING_PICTURE
        else:
            self.send_text("There was an error with MQTT and Image retrying....")
            self.isMqttConnected = self.mqtt_handler.reconnect()
            return ASKING_PICTURE

    def on_new_user_found(self, chat_user, update):
        print('[Bot] Bot is talking with user {} and his user ID is: {} and user photo ID is: {}'.format(
            chat_user.user_name, chat_user.user_id, chat_user.user_photo_id))
        try:
            filename = "./{0}/{1}.jpg".format(IMAGE_FOLDER, chat_user.user_photo_id)

            if os.path.exists(filename):
                print("[Bot] Photo Exists...")
                self.send_text("This picture does already exist in my database. I'll use it as you seem to like it!")
                self.send_text("Photo received succesfully!")
                self.send_text("Welcome to warshippy {}! Let's get the war started! ".format(chat_user.user_name))
                self.send_text("war")
            else:
                print("[Bot] Photo hasn't been downloaded yet. Should start downloading now.")
                image_file = self.context.bot.get_file(chat_user.user_photo_id)
                image_file.download(custom_path=filename)
                print("[Bot] Downloading user picture..." + filename)
                self.send_text("Photo received succesfully!")
                with open(filename, "rb") as image_file:
                    encoded_string = base64.b64encode(image_file.read())
                    self.mqtt_handler.publish_on_mqtt(encoded_string, True)
                    self.mqtt_handler.publish_on_mqtt(chat_user.user_name, True)
                self.chat_user = chat_user
                self.send_text("Welcome to warshippy {}! Let's get the war started! ".format(self.chat_user.user_name))
                custom_keyboard_table = [["War", "Nooo I am afraid."]]
                reply_markup_coordenates = ReplyKeyboardMarkup(custom_keyboard_table)

                update.message.reply_text('Are you sure you are prepare to go to war?',
                                          reply_markup=reply_markup_coordenates)
        except NetworkError as e:
            print("[Bot] ERROR " + str(e))

    def ask_for_war(self, update, context):
        print("[ASK_FOR_WAR] " + update.message.text)
        if update.message.text == "War":
            reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
            update.message.reply_text('Please send me your first boat position:', reply_markup=reply_markup_coordenates)
            return ASKING_BOATS
        else:
            reply_markup_coordenates = ReplyKeyboardMarkup(yes_table)
            update.message.reply_text('Really like to end it all?', reply_markup=reply_markup_coordenates)
            return ASKING_BOATS

    def update_table_to_bot(self, img_data):
        with open("game_table.png", "wb") as fh:
            fh.write(base64.decodebytes(img_data))
        self.send_image('game_table.png')

    def set_boat_orientation(self, update, context):
        print("Setting boat orientation: " + update.message.text)
        self.aux_boat.orientation = update.message.text

        if someBoxOccupied(self.boats_available, self.aux_boat):
            self.send_text(
                "This position is ocuppied by another boat you placed. Re-run /boat to add a better boat you stupid")
            reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
            update.message.reply_text("Let's place it again noob :", reply_markup=reply_markup_coordenates)
            return ASKING_BOATS
        elif len(self.boats_available) == 2:
            self.boats_available.append(self.aux_boat)
            # self.send_text()
            reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
            update.message.reply_text("You can only place 3 boats, what means?. You guess exactly,WAR!!",
                                      reply_markup=reply_markup_coordenates)
            self.mqtt_handler.publish_on_mqtt("bot-player-ready", True)
            return ATTACKING
        else:
            self.boats_available.append(self.aux_boat)
            reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
            update.message.reply_text("Let's place your next boat :", reply_markup=reply_markup_coordenates)

            return ASKING_BOATS

    def show_boats(self):
        self.send_text("These are your boats")
        for boat in self.boats_available:
            self.send_text("Coordinates: " + str(boat.x) + " / " + str(boat.y))
            self.send_text("Orientation: " + boat.orientation)

    def add_boat_to_table(self, update, context):
        self.m_gameState = GameState.PlacingBoats

        # custom_keyboard_table = [["0:0","0:1","0:2","0:3","0:4"],
        #         ["1:0","1:1","1:2","1:3","1:4"],
        #         ["2:0","2:1","2:2","2:3","2:4"],
        #         ["3:0","3:1","3:2","3:3","3:4"],
        #         ["4:0","4:1","4:2","4:3","4:4"]]

        reply_markup_coordinates = ReplyKeyboardMarkup(coordinates_table)

        # i = len(self.boats_available)
        # if i < 2:
        self.aux_boat = Boat()

        # update.message.reply_text('Alright, Where you would like to place your {} boat?, '
        #                 'for example "at 3:1 coordenates"'.format(i),reply_markup = reply_markup_coordenates)

        if isCorrectCoordenate(update.message.text):
            self.aux_boat.coordinates = update.message.text

            s = self.aux_boat.coordinates.split(":")

            self.aux_boat.x = s[0]
            self.aux_boat.y = s[1]

            # self.boats_available[i].orientation = self.ask_for_orientation(update,context)
            print("Will ask for orientation")
            self.send_text("Orientation time!")
            reply_markup_coordinates = ReplyKeyboardMarkup(orientation_table)
            update.message.reply_text('Send me the boat orientation:', reply_markup=reply_markup_coordinates)
            return ASKING_ORIENTATION
        else:
            self.send_text("The boat coordinate was incorrect!")
            reply_markup_coordinates = ReplyKeyboardMarkup(coordinates_table)
            update.message.reply_text("Choose again noob :", reply_markup=reply_markup_coordinates)

            return ASKING_BOATS
        # else:
        #     self.send_text("You can only place 3 boats.")
        #     return ATTACKING

    def update_callback_response(self, update, context):
        if self.game_state == GameState.blocked:
            self.send_text("Too fast ma boy, wait for player 1 to shoot!", context, update)
        elif self.game_state == GameState.WaitingForUs:
            self.send_text("Player 1 is waiting for you, camm'ooon you are so slow!")
        elif self.game_state == GameState.WaitingForPlayer1:
            self.send_text("Too fast ma boy, wait for player 1 to place the boats!", context, update)

    def can_attack(self):
        return self.game_state != GameState.blocked and self.game_state != GameState.WaitingForUs and self.game_state != GameState.WaitingForPlayer1

    def attack_boat(self, update, context):
        if self.can_attack():
            text = update.message.text
            self.send_text("Attacking user at this position: ".format(text), context, update)
            self.mqtt_handler.publish_on_mqtt(text)
        else:
            self.update_callback_response(update, context)
        return ATTACKING

    def done(self, update, context):
        self.send_text("Goodbye you looser.")
        return ConversationHandler.END

    def main(self):
        print("Bot Main Called: " + get_coordinates_as_string() + get_orientation_as_string())
        # Create the Updater and pass it your bot's token.
        # Make sure to set use_context=True to use the new context based callbacks
        # Post version 12 this will no longer be necessary
        updater = Updater(self.token, use_context=True)

        # Get the dispatcher to register handlers
        dp = updater.dispatcher

        # Add conversation handler with the states CHOOSING, TYPING_CHOICE and TYPING_REPLY
        conv_handler = ConversationHandler(
            entry_points=[CommandHandler('start', self.start)],

            states={
                ASKING_PICTURE: [MessageHandler(Filters.photo,
                                                self.ask_picture),
                                 MessageHandler(Filters.regex('^Yes$'), self.ask_picture)
                                 ],
                ASKING_WAR: [MessageHandler(Filters.regex('^(War|Nooo I am afraid.)$'), self.ask_for_war)],
                ASKING_BOATS: [MessageHandler(Filters.regex('^({})$'.format(get_coordinates_as_string())),
                                              self.add_boat_to_table)],
                ASKING_ORIENTATION: [
                    MessageHandler(Filters.regex('^(Vertical|Horizontal)$'), self.set_boat_orientation)],
                ATTACKING: [MessageHandler(Filters.regex('^({})$'.format(get_coordinates_as_string())),
                                           self.attack_boat),
                            ],

            },

            fallbacks=[MessageHandler(Filters.regex('^Goodbye My Lover, Goodbye my friend$'), self.done),
                       MessageHandler(Filters.regex('^War'), self.ask_for_war)],

            allow_reentry=True
        )

        dp.add_handler(conv_handler)

        # Start the Bot
        updater.start_polling()

        # Run the bot until you press Ctrl-C or the process receives SIGINT,
        # SIGTERM or SIGABRT. This should be used most of the time, since
        # start_polling() is non-blocking and will stop the bot gracefully.
        updater.idle()


if __name__ == "__main__":
    bot = TELEGRAM_BOT(TELEGRAM_TOKEN, True)
