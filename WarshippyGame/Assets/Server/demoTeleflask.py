__version__ = '0.1.0'

import re
import logging
from time import sleep, time
from logging.config import dictConfig
from concurrent.futures import ProcessPoolExecutor, ThreadPoolExecutor

import telegram
from telegram.error import TelegramError

# # #from utils.chatter_bot_api import ChatterBotFactory
# # from conf.log_conf import LOG_CONF
# # from conf import configurations as cfg

# logger = logging.getLogger(__name__)


def get_updates(bot, update_id):
    last_update_id = 0
    if update_id:
        last_update_id = update_id
    try:
        updates = bot.getUpdates(offset=update_id, timeout=2)
    except TelegramError:
        logging.exception('Could not get updates')
        return [], update_id

    if updates:
        last_update_id = updates[-1].update_id

    for update in updates:
        text = update.message.text
        if text is None:
            continue

    return [], last_update_id


def process_messages(bot):
    update_id = None
    last_request = 0
    with ThreadPoolExecutor(3) as executor:
        while True:
            update, update_id = get_updates(bot, update_id)
            if update:
                message = update.message.text
                


def main():
    bot = telegram.Bot(token=cfg.TELEGRAM_TOKEN)
    #bot.chatter = ChatterBotFactory().create(type=1).create_session()

    # reset accumulated messages
    updates = bot.getUpdates()
    if updates:
        last_update_id = updates[-1].update_id
        bot.getUpdates(offset=last_update_id+1)

    with ProcessPoolExecutor(1) as executor:
        executor.submit(process_messages(bot))


if __name__ == '__main__':
    dictConfig(LOG_CONF)
    try:
        main()
    except Exception:
        logger.exception('sleeping due to unhandled exception')
        sleep(cfg.SLEEP_BETWEEN_EXCEPTIONS)
        main()