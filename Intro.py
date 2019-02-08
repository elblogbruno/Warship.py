from __future__ import division
from pyfiglet import Figlet
from WarshipGame import *
import os
from asciimatics.effects import Scroll, Mirage, Wipe, Cycle, Matrix, \
    BannerText, Stars, Print
from asciimatics.particles import DropScreen
from asciimatics.renderers import FigletText, SpeechBubble, Rainbow, Fire
from asciimatics.scene import Scene
from asciimatics.screen import Screen
from asciimatics.exceptions import ResizeScreenError
import sys
playIntro = True
def demo(screen):
        scenes = []
        screen.print_at('Press a to enter',0,0)
        effects = [
        Matrix(screen, stop_frame=100),
        Mirage(
            screen,
            FigletText("Warship.py"),
            screen.height // 2 - 3,
            Screen.COLOUR_GREEN,
            start_frame=100,
            stop_frame=200),
        Wipe(screen, start_frame=150),
        Cycle(
            screen,
            FigletText("Warship.py"),
            screen.height // 2 - 3,
            start_frame=200),
        ]
        scenes.append(Scene(effects, 250, clear=False))
        effects = [
        Mirage(
            screen,
            FigletText("Coded and"),
            screen.height,
            Screen.COLOUR_GREEN),
        Mirage(
            screen,
            FigletText("designed by:"),
            screen.height + 8,
            Screen.COLOUR_GREEN),
        Mirage(
            screen,
            FigletText("Bruno Moya"),
            screen.height + 16,
            Screen.COLOUR_RED),
        Scroll(screen, 3),
        ]
        #playIntro = False
        scenes.append(Scene(effects, (screen.height + 24) * 3))
        screen.play(scenes,stop_on_resize=True, repeat=False)
if __name__ == "__main__":
    while playIntro == True:
        try:
            Screen.wrapper(demo)
            playIntro= False
        except ResizeScreenError:
            pass
    MainMenu()


