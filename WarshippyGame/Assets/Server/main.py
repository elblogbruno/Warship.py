import os
import BotDemo
import flaskServer
import argparse

parser = argparse.ArgumentParser()
parser.add_argument('-b', '--bot', help='Starts Bot.')
parser.add_argument('-s', '--server', help='Starts Server.')
args = parser.parse_args()

if args.bot:
    print("Starting bot.")
    BotDemo.main()
    
if args.server:
    print("Starting server.")
    flaskServer.startServer()
