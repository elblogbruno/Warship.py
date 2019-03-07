from tkinter import *
import BotDemo as bot
from PIL import Image, ImageTk
from tkinter import messagebox
import random
import os
import time
import threading
root = Tk()
BOT_TOKEN  ="729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk"
gameTitle = "Welcome to Warship.py"
root.title(gameTitle)
root.iconbitmap(r'iconwarship.ico')
path = "C:/Users/elblo/Desktop/Proyectos/Python/WarShipGame/warshippy/"
menu = Menu(root)
new_item = Menu(menu)
new_item.add_command(label='Exit',command=exit)
menu.add_cascade(label='Settings', menu=new_item)
root.config(menu=menu)
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

img = ImageTk.PhotoImage(Image.open("out.png"))
panel = Label(root, image = img)
panel.pack(side = "bottom", fill = "both", expand = "yes")
def pasteIcon(icon,pos,icon_mask):
    grid.paste(icon,pos,icon_mask)
    grid.save("out.png")
def ClearGrid():
    for position in positions:
        print(position)
        pasteIcon(white_paint,positions[position],white_mask)
    img2 = ImageTk.PhotoImage(Image.open("out.png"))
    panel.configure(image=img2)
    panel.image = img2
    #messagebox.showinfo(gameTitle, "Done clearing")
def update_image():
    #pos = positions[str(random.randint(0,4)) + ":" + str(random.randint(0,4))]
    if txt.get() in positions:
        pos = positions[txt.get()]
        print (pos)
        pasteIcon(wave_icon,pos,wave_mask)
        img2 = ImageTk.PhotoImage(Image.open("out.png"))
        panel.configure(image=img2)
        panel.image = img2
        bot.send_image(BOT_TOKEN,"out.png")
        msg = "Where do you want to attack?"
        bot.sendText(BOT_TOKEN,msg)
    else:
        messagebox.showinfo(gameTitle, "This position is not available")
        msg = "This position is not available"
        bot.sendText(BOT_TOKEN,msg)
def update_position(pos):
    positions[pos]
    print (pos)
    pasteIcon(ship_icon,pos,ship_mask)
    img2 = ImageTk.PhotoImage(Image.open("out.png"))
    panel.configure(image=img2)
    panel.image = img2
def checkImageChanged():
    moddate = os.stat("out.png")[8]
    old_moddate = " "
    while True:
        time.sleep(1)
        moddate = os.stat("out.png")[8]
        if moddate == old_moddate:
            print("Panel has changed")
        else:
            print("Panel has not changed")
            old_moddate = moddate
            img2 = ImageTk.PhotoImage(Image.open("out.png"))
            panel.configure(image=img2)
            panel.image = img2
            file = open("coordenates.txt","r")
            print("This are the coordenates of the bot user: " + str(file.read()))

        root.after(2000, checkImageChanged())
def echoo():
    while True:
        root.after(2000, bot.echo())
new_item.add_command(label='Clear Grid',command=ClearGrid)
btn = Button(root, text="Attack Bot",command=update_image)
btn.pack(anchor=CENTER, expand=True)
txt = Entry(root,width=10)
txt.pack(anchor=CENTER, expand=True)
bot.main()
time.sleep(5)
ClearGrid()
hilo1 = threading.Thread(target=echoo)
hilo1.start()
hilo2 = threading.Thread(target=checkImageChanged)
hilo2.start()
#root.after(2000, echoo )
#bot.main()
bot.sendText(BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
#bot.send_audio(BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
bot.send_image_url(BOT_TOKEN,"https://vignette.wikia.nocookie.net/central/images/4/40/Hello_there.jpg")
root.mainloop()
