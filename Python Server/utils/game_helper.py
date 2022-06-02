# -*- coding: utf-8 -*-

import string

coordenates = " "
message = " "


def set_current_message(text):
    global message
    message = text
    print("[GameHelper] Setting current message to: " + message)


def get_messages():
    global message
    # print("[GameHelper] Returning current message to server: " + message)
    return message


def wrong_position(position):
    l = position.split(":")
    if len(l) == 2:
        if str(l[0]) in string.digits and str(l[1]) in string.digits:
            if (len(l[0]) == 1 and len(l[1]) == 1):
                return False
            else:
                return True
        else:
            return True
    else:
        return True


def __there_is_no_number(s):
    sp = s.split(":")
    if sp[0] in string.digits:
        if sp[1] in string.digits:
            return False
    else:
        return True


def is_a_valid_position(s):
    sp = s.split(":")
    for pos in sp:
        if pos >= 5 or pos < 0:
            return False
    return True


def is_correct_coordenate(pos):
    print("CHECKING IF {} IS CORRECT".format(pos))
    if pos:
        if ":" in pos:
            if not __there_is_no_number(pos):
                if not wrong_position(pos):
                    return True
            else:
                return False
        else:
            return False
    else:
        return False


def get_coordenates():
    global coordenates
    return coordenates


def update_position(pos):
    if pos != None:
        if ":" in pos:
            if __there_is_no_number(pos) == False:
                if wrong_position(pos) == False:
                    msg = "Attacking user at this coordenates : " + pos
                    # bot.send_text(msg)
                    global coordenates
                    coordenates = pos
                    return True
            else:
                msg = "This position is not available, Please write a coordenate like this: 2:2 [x,y]"
                # bot.send_text(msg)
                return False

        else:
            msg = "This position is not available, Please write a coordenate like this: 2:2 [x,y]"
            # bot.send_text(msg)
            return False
    else:
        return False


