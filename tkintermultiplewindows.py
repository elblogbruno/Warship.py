import tkinter as tk
import BotDemo as bot
from PIL import Image, ImageTk
from tkinter import messagebox
import random
import os
import time
import threading

LARGE_FONT = ("Verdana", 12) # font's family is Verdana, font's size is 12
gameTitle = "Welcome to Warship.py"
BOT_TOKEN  ="729316731:AAEAoHTXtMSSbRAh38rBZW6y-O-H5vESoEk"
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

class MainWindow(tk.Tk):
    def __init__(self, *args, **kwargs):
        tk.Tk.__init__(self, *args, **kwargs)
        self.title(gameTitle) # set the title of the main window
        #self.geometry("300x300") # set size of the main window to 300x300 pixels
        self.iconbitmap(r'iconwarship.ico')
        # this container contains all the pages
        container = tk.Frame(self)
        container.pack(side="top", fill="both", expand=True)
        container.grid_rowconfigure(0, weight=1)   # make the cell in grid cover the entire window
        container.grid_columnconfigure(0,weight=1) # make the cell in grid cover the entire window
        self.frames = {} # these are pages we want to navigate to

        for F in (StartPage, PageOne): # for each page
            frame = F(container, self) # create the page
            self.frames[F] = frame  # store into frames
            frame.grid(row=0, column=0, sticky="nsew") # grid it to container

        self.show_frame(StartPage) # let the first page is StartPage

    def show_frame(self, name):
        frame = self.frames[name]
        frame.tkraise()

class StartPage(tk.Frame):
    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent)
        label = tk.Label(self, text='Start Page', font=LARGE_FONT)
        label.pack(pady=10, padx=10) # center alignment

        button1 = tk.Button(self, text='Play Game',  # when click on this button, call the show_frame method to make PageOne appear
                            command=lambda : controller.show_frame(PageOne))
        button1.pack() # pack it in

class PageOne(tk.Frame):
    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent)
        bot.sendText(BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
        #bot.send_audio(BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
        bot.send_image_url(BOT_TOKEN,"https://vignette.wikia.nocookie.net/central/images/4/40/Hello_there.jpg")

        #hilo1 = threading.Thread(target=echoo())
        #hilo1.start()
        label = tk.Label(self, text='Start Page', font=LARGE_FONT)
        label.pack(pady=10, padx=10) # center alignment
        img2 = ImageTk.PhotoImage(Image.open("out.png"))
        panel = tk.Label(self, image = img2)
        panel.pack(side = "bottom", fill = "both", expand = "yes")
        panel.configure(image=img2)
        panel.image = img2
        hilo2 = threading.Thread(target=checkImageChanged(panel,self))
        txt = tk.Entry(self,width=10)
        txt.pack( expand=True,side = tk.LEFT)
        button1 = tk.Button(self, text='Attack Bot',command= lambda: update_image(txt.get(),panel,self))
        button1.pack(side = tk.LEFT) # pack it in
        button2 = tk.Button(self, text='Come Back',command=lambda : controller.show_frame(StartPage))
        button2.pack(side = tk.RIGHT)
def pasteIcon(icon,pos,icon_mask):
    grid.paste(icon,pos,icon_mask)
    grid.save("out.png")
def ClearGrid():
    for position in positions:
        print(position)
        pasteIcon(white_paint,positions[position],white_mask)
    img2 = ImageTk.PhotoImage(Image.open("out.png"))
    panel = tk.Label(self, image = img2)
    panel.configure(image=img2)
    panel.image = img2
    #messagebox.showinfo(gameTitle, "Done clearing")
def checkImageChanged(panel,self):
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
        #time.sleep(2)
        self.after(2000, hilo2.start())
def echoo():
    while True:
        time.sleep(2)
        bot.echo()
def update_image(txt,panel,self):
    global tkimg1
    print("trying to attaack at this position: " + txt)
    #pos = positions[str(random.randint(0,4)) + ":" + str(random.randint(0,4))]
    if txt in positions:
        pos = positions[txt]
        print (pos)
        pasteIcon(wave_icon,pos,wave_mask)
        img2 = ImageTk.PhotoImage(Image.open("out.png"))
        panel = tk.Label(self, image = img2)
        panel.configure(image=img2)
        panel.image = img2
        bot.send_image(BOT_TOKEN,"out.png")
        msg = "Where do you want to attack?"
        bot.sendText(BOT_TOKEN,msg)
    else:
        messagebox.showinfo(gameTitle, "This position is not available")
        msg = "This position is not available"
        bot.sendText(BOT_TOKEN,msg)
if __name__ == '__main__':
    app = MainWindow()
    app.mainloop()
