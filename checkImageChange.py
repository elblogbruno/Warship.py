

FNAME = "out.png"

import os, time
moddate = os.stat(FNAME)[8]
old_moddate = " "
while True:
    time.sleep(1)
    moddate = os.stat(FNAME)[8]
    if moddate == old_moddate:
        print("Nothin has changed")
    else:
        print("Thing has changed")
        old_moddate = moddate
