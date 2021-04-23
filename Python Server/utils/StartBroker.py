import logging
import asyncio
import os
from hbmqtt.broker import Broker


@asyncio.coroutine
def broker_coro():
    config = {
        'listeners': {
            'default': {
                'max-connections': 50000,
                'bind': '127.0.0.1:1883',
                'type': 'tcp',
            },
        },
        'auth': {
            'allow-anonymous': True,
        },
        'plugins': [ 'auth_anonymous' ],
        'topic-check': {
            'enabled': False
        }
    }
    broker = Broker(config)
    yield from broker.start()


if __name__ == '__main__':
    formatter = "[%(asctime)s] :: %(levelname)s :: %(name)s :: %(message)s"
    logging.basicConfig(level=logging.INFO, format=formatter)
    asyncio.get_event_loop().run_until_complete(broker_coro())
    asyncio.get_event_loop().run_forever()