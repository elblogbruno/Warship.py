import logging
import queue
from time import sleep
import telegram
from gtts import gTTS

from FlaskServer import GameServer
from elements.MessagePacket import MessagePacket
from elements.TableManager import TableManager
from elements.boat import Boat
from elements.ChatBotUser import ChatBotUser
from elements.table_element import ElementType, TableElement
# from mqtt.socket_unity_communication import SocketUnity

from utils.secrets import TELEGRAM_TOKEN
from telegram import ReplyKeyboardMarkup
from telegram.ext import (Updater, CommandHandler, MessageHandler, Filters,
                          ConversationHandler)
from telegram.error import NetworkError, Unauthorized
# from mqtt.mqtt_handler import MQTT_Handler

from enum import Enum

from utils.game_utils import *
from utils.utils import *
from utils.game_helper import *

from config import *
import os
import base64

# Enable logging
logging.basicConfig(format='%(asctime)s - %(name)s - %(levelname)s - %(message)s',
                    level=logging.INFO)

logger = logging.getLogger(__name__)


class GameState(Enum):
    Playing = 1
    PlacingBoats = 2
    AskingForPicture = 3
    WaitingForPlayer1 = 4
    WaitingForUs = 5
    idle = 5
    blocked = 6
    unblocked = 7


ASKING_PICTURE, ASKING_WAR, ASKING_BOATS, ASKING_ORIENTATION, ASKING_REPLACE, ATTACKING = range(6)


class TELEGRAM_BOT(object):
    def send_text(self, msg, context=None, update=None):
        print("[Bot] Sending message to chat: " + msg)
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
        print("[Bot] Sending audio: " + imageUrl)

        tts = gTTS(imageUrl, 'en')
        tts.save('hello.mp3')
        self.context.bot.send_voice(chat_id=self.chat_id, voice=open('hello.mp3', 'rb'))
        return

    def filter_text(self, text):
        # print(text)
        if len(text) > 0:
            response = MessagePacket("ack", "tcp", 0)
            response.to_json(text)
            print("Received this response from Unity {}".format(text))
            self.last_message = "other"
            if response.type_message == "control":
                if response.message == "waiting_for_player_2":
                    self.game_state = GameState.WaitingForUs
                    self.send_text("CAMMON you loser so late")
                elif response.message == "waiting_for_player_1":
                    self.game_state = GameState.WaitingForPlayer1
                elif response.message == "block":
                    self.game_state = GameState.blocked
                elif response.message == "unblock":
                    self.game_state = GameState.unblocked
            elif response.type_message == "text":
                self.send_text(response.message)
            elif response.type_message == "attack_response":
                self.process_attack_response(response)
            elif response.type_message == "attack_position":
                self.receive_attack(response.message)
            elif response.type_message == "tcp":
                self.queue.put(response)
                # print("ACK RECEIVED FROM UNITY")
                # print(0.000001)
                self.queue.get(0)




        else:
            print("Received unknown response type")

    def __init__(self, token, debug=False):
        self.debug = debug
        self.token = token
        self.host = '127.0.0.1'

        self.game_state = GameState.AskingForPicture
        self.socket_handler = GameServer(self)

        self.table_manager = TableManager()
        self.opponent_table_manager = TableManager()

        self.isMqttConnected = self.socket_handler.is_connected()
        self.shouldGetImage = True
        self.asking_boats = False

        createFolder(IMAGE_FOLDER)

        self.queue = queue.Queue(5)
        self.socket_handler.start_listening()
        self.main()

        if debug:
            print("[BOT] Bot has started: " + str(self.isMqttConnected))

    def publish_on_server(self, message, message_type, points):
        m = MessagePacket(message, message_type, points)
        self.queue.put(m)

    def get_status(self):
        if self.queue.empty():
            a = MessagePacket(message="No messages", type_message="no_messages", points=0)
            return a.to_string()

        print("GETTING FROM QUEUE")
        a = self.queue.get()

        if a.dic['type_message'] == "tcp":
            print("We are blocked")
            return MessagePacket(message="TCP ACK", type_message="no_messages", points=0).to_string()

        return a.to_string()

    def facts_to_str(user_data):
        facts = list()

        for key, value in user_data.items():
            facts.append('{} - {}'.format(key, value))

        return "\n".join(facts).join(['\n', '\n'])

    def start(self, update, context):
        if len(self.table_manager.boats_position) > 0:
            return self.on_boats_placed(update, context)

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
        self.new_chat_user = ChatBotUser()
        print("[BOT] Asking for picture")
        self.send_text("Please send me your profile picture!", context, update)

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
            self.isMqttConnected = self.socket_handler.reconnect()
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
            else:
                print("[Bot] Photo hasn't been downloaded yet. Should start downloading now.")
                image_file = self.context.bot.get_file(chat_user.user_photo_id)
                image_file.download(custom_path=filename)
                print("[Bot] Downloading user picture..." + filename)
                self.send_text("Photo received succesfully!")
                with open(filename, "rb") as image_file:
                    encoded_string = base64.b64encode(image_file.read()).decode("utf-8")
                    self.publish_on_server(encoded_string, "image", 0)
                    self.publish_on_server(chat_user.user_name, "image", 0)
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

    def on_boats_placed(self, update, context):
        self.send_text("You have added all your boats. Now you can start the war!")
        self.show_table()

        custom_keyboard_table = [["Yes I need to replace them", "Nooo I don't mind I want war ."]]
        reply_markup_coordenates = ReplyKeyboardMarkup(custom_keyboard_table)

        update.message.reply_text("But first, would you like to replace your boats on the board?",
                                  reply_markup=reply_markup_coordenates)

        return ASKING_REPLACE

    def on_replace_boats(self, update, context):
        reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
        update.message.reply_text('Please send me your first boat position again noob :', reply_markup=reply_markup_coordenates)

        self.table_manager.reset()

        return ASKING_BOATS

    def on_yes_pressed(self, update, context):
        reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
        update.message.reply_text(
            "You can only place {0} boats, what means?. You guess exactly,WAR!!".format(MAXIMUM_BOATS),
            reply_markup=reply_markup_coordenates)

        if self.game_state != GameState.WaitingForUs:  # if they are not waitnig for us we are waiting for them
            self.game_state = GameState.WaitingForPlayer1

        self.publish_on_server("bot-player-ready", "control", 0)
        return ATTACKING

    def set_boat_orientation(self, update, context):
        print("Setting boat orientation: " + update.message.text)
        self.aux_boat.orientation = update.message.text

        # if self.table_manager.some_box_occupied(self.aux_boat):
        #     self.send_text("This position is ocuppied by another boat you placed you stupid")
        #     reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
        #     update.message.reply_text("Let's place it again noob:", reply_markup=reply_markup_coordenates)
        #     return ASKING_BOATS
        if self.table_manager.get_number_of_element_type(ElementType.Boat) == (MAXIMUM_BOATS * BOAT_SIZE - 3):
            # self.table_manager.add_element_to_table(self.aux_boat, self.aux_boat.orientation)
            added = self.table_manager.add_element_to_table(self.aux_boat, self.aux_boat.orientation)

            if added:
                return self.on_boats_placed(update, context)
            else:
                self.send_text("You last boat and you place it where you can't. You are a noob.")
                reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
                update.message.reply_text("Let's place it again noob:", reply_markup=reply_markup_coordenates)
                return ASKING_BOATS
        else:
            # self.table_manager.add_element_to_table(self.aux_boat, self.aux_boat.orientation)
            added = self.table_manager.add_element_to_table(self.aux_boat, self.aux_boat.orientation)
            if added:
                self.send_text("You have added your boat. Now you can place another one")
                self.show_table()
                reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
                update.message.reply_text("Let's place your next boat :", reply_markup=reply_markup_coordenates)
            else:
                self.send_text("Boat can't be added. Please try again.")
                reply_markup_coordenates = ReplyKeyboardMarkup(coordinates_table)
                update.message.reply_text("Let's place it again noob:", reply_markup=reply_markup_coordenates)

            return ASKING_BOATS

    def send_text_to_unity(self, update, context):
        print("[SEND_TEXT_TO_UNITY] " + update.message.text)
        self.publish_on_server(update.message.text, "text", 0)

    def add_boat_to_table(self, update, context):
        self.game_state = GameState.PlacingBoats
        self.aux_boat = Boat()

        if is_correct_coordenate(update.message.text):
            self.aux_boat.coordinates = update.message.text
            s = self.aux_boat.coordinates.split(":")

            self.aux_boat.x = s[0]
            self.aux_boat.y = s[1]
            self.aux_boat.is_parent = True

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

    def update_callback_response(self, update, context):
        if self.game_state == GameState.blocked:
            self.send_text("Too fast ma boy, wait for player 1 to shoot!", context, update)
        elif self.game_state == GameState.WaitingForUs:
            self.send_text("Player 1 is waiting for you, camm'ooon you are so slow!")
        elif self.game_state == GameState.WaitingForPlayer1:
            self.send_text("Too fast ma boy, wait for player 1 to place the boats!", context, update)

    def can_attack(self):
        return self.game_state == GameState.unblocked and self.game_state != GameState.blocked and self.game_state != GameState.WaitingForUs and self.game_state != GameState.WaitingForPlayer1

    def process_attack_response(self, attack_result):
        if attack_result.message == "missed":
            self.send_text("You missed!")
        else:
            self.send_text("You hit!")

            element = TableElement(ElementType.Boat)

            """
            Unity sends the coordinates in the format: x:y:orientation
            """

            s = attack_result.message.split(":")
            element.x = s[0]
            element.y = s[1]
            element.coordinates = ":".join([element.x, element.y])

            self.opponent_table_manager.add_element_to_table(element, s[2])

    def receive_attack(self, position_to_receive_attack):
        print(position_to_receive_attack)

        if is_correct_coordenate(position_to_receive_attack):
            self.send_text("Unity user is attacking you at {0}".format(position_to_receive_attack))
            element = self.table_manager.get_element_at_coordenates(coordenates=position_to_receive_attack)

            if element and element.type == ElementType.Boat:
                if self.table_manager.get_number_of_element_type(type=ElementType.Boat) > 0:

                    boat_size, changed = self.table_manager.recalculate_table_state(element)

                    if changed:

                        self.send_text("Oh no, oh no, oh no no no no no, a boat at {0} coordenates was turn down".format(
                                position_to_receive_attack))
                        self.send_text(msg="Number of boats alive: " + str(boat_size))
                    else:
                        self.send_text("Player 1 Hit a part of your boat. Be aware of him/her!")

                    self.show_table(True)

                    self.publish_on_server(position_to_receive_attack, "attack_result",
                                           boat_size)
                else:
                    self.send_text("You have lost madafaca!")

                    boat_size, changed = self.table_manager.recalculate_table_state(element)

                    self.send_text(msg="Number of boats alive: " + str(boat_size))

                    self.publish_on_server(position_to_receive_attack, "attack_result",
                                           boat_size)

                    print("You have lost madafaca!")
                    self.send_text("Would you like to play again?")
            else:
                self.publish_on_server("You loser, can't kill me madafaca YOU HIT WATTER DUMB!", "text", 0)

    def attack_boat(self, update, context):
        if self.can_attack():
            text = update.message.text
            self.send_text("Attacking user at this position: {0}".format(text), context, update)
            boat_size = self.table_manager.calculate_number_of_boats()

            self.send_text(msg="Number of your boats alive: " + str(boat_size))
            self.publish_on_server(text, "attack_position", boat_size)

            self.show_table(True)
        else:
            self.update_callback_response(update, context)
        return ATTACKING

    def debug(self, update, context):
        self.send_text("Debugging...")
        self.debug = False
        return ATTACKING

    def done(self, update, context):
        self.send_text("Goodbye you looser.")
        return ConversationHandler.END

    def show_table(self, show_opponent_table=False):
        self.send_text("These are your boats")
        f = self.table_manager.show_table()
        self.send_image(msg=f)

        if show_opponent_table and self.opponent_table_manager:
            self.send_text("These are your opponent boats")
            f = self.opponent_table_manager.show_table()
            self.send_image(msg=f)

    def main(self):
        print("Bot Main Called")
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

                # "Yes I need to replace them", "Nooo I don't mind I want war ."
                ASKING_REPLACE: [
                    MessageHandler(Filters.regex("^(Yes I need to replace them)$"), self.on_replace_boats), MessageHandler(Filters.regex("^(Nooo I don't mind I want war .)$"), self.on_yes_pressed)],
                ASKING_WAR: [MessageHandler(Filters.regex('^(War|Nooo I am afraid.)$'), self.ask_for_war)],
                ASKING_BOATS: [MessageHandler(Filters.regex('^({})$'.format(get_coordinates_as_string())),
                                              self.add_boat_to_table)],
                ASKING_ORIENTATION: [
                    MessageHandler(Filters.regex('^(Vertical|Horizontal)$'), self.set_boat_orientation)],
                ATTACKING: [MessageHandler(Filters.regex('^({})$'.format(get_coordinates_as_string())),
                                           self.attack_boat), MessageHandler(Filters.text, self.send_text_to_unity)],
            },

            fallbacks=[MessageHandler(Filters.regex('^Goodbye My Lover, Goodbye my friend$'), self.done),
                       MessageHandler(Filters.regex('^War'), self.ask_for_war)],

            allow_reentry=True
        )

        dp.add_handler(conv_handler)

        try:
            # Start the Bot
            updater.start_polling()
        except telegram.error.NetworkError as e:
            print(e)

        # Run the bot until you press Ctrl-C or the process receives SIGINT,
        # SIGTERM or SIGABRT. This should be used most of the time, since
        # start_polling() is non-blocking and will stop the bot gracefully.
        updater.idle()


if __name__ == "__main__":
    bot = TELEGRAM_BOT(TELEGRAM_TOKEN, False)
