import tkinter as tk
import BotDemo as bot
from PIL import Image, ImageTk
from tkinter import messagebox
import random
import os
import time
import threading
import tkFont
import sys
import ctypes
from colorama import init
init(strip=not sys.stdout.isatty()) # strip colors if stdout is redirected
from termcolor import cprint
from pyfiglet import figlet_format
LARGE_FONT = ("Verdana", 12) # font's family is Verdana, font's size is 12
panelList = []
app = ""
hilo2 = ""
echo = True
class MainWindow(tk.Tk):
    def __init__(self, *args, **kwargs):
        tk.Tk.__init__(self, *args, **kwargs)
        self.title(bot.warlib.gameTitle) # set the title of the main window+
        self.config(background =  "black")
        #self.geometry("300x300") # set size of the main window to 300x300 pixels
        #self.iconbitmap(r'iconwarship.ico')
        self.iconbitmap(r'icon.ico')
        # this container contains all the pages
        # hilo2 = threading.Thread(target=echoo)
        # hilo2.start()
        photo = ImageTk.PhotoImage(file = "sprites/warshipLogo.png")
        container = tk.Frame(self)
        panelList.append(container)
        container.pack(side="top", fill="both", expand=True)
        container.grid_rowconfigure(0, weight=1)   # make the cell in grid cover the entire window
        container.grid_columnconfigure(0,weight=1) # make the cell in grid cover the entire window
        self.frames = {} # these are pages we want to navigate to
        for F in (StartPage, PageOne): # for each page
            frame = F(container, self) # create the page
            self.frames[F] = frame  # store into frames
            panelList.append(frame)
            frame.grid(row=0, column=0, sticky="nsew") # grid it to container

        self.show_frame(StartPage) # let the first page is StartPage
    def show_frame(self, name):
        frame = self.frames[name]
        frame.tkraise()
def echoo():
    while echo == True:
        bot.echo()
    self.after(2000,echoo)
def doit(arg):
    t = threading.currentThread()
    while getattr(t, "do_run", True):
        bot.echo()
    print("Stopping as you wish.")
    os.system("exit")
# def updateImage():
#     img2 = ImageTk.PhotoImage(Image.open("out.png"))
#     panel = tk.Label(self, image = img2)
#     panel.configure(image=img2)
#     panel.image = img2
class StartPage(tk.Frame):
    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent)
        img2 = ImageTk.PhotoImage(Image.open("sprites/warshipLogo1.png"))
        panel1 = tk.Label(self, image = img2,anchor = tk.CENTER)
        panel1.pack(pady=30, padx=10)
        panel1.image = img2
        #photo = ImageTk.PhotoImage(file = "sprites/warshipLogo.png")
        # label = tk.Label(self, text='Welcome to Warship.py a game about sinking ships...and bots..', font=LARGE_FONT)
        # label.pack(pady=50, padx=10) # center alignment

        # img2 = ImageTk.PhotoImage(Image.open("iconwarship.ico"))
        # panel = tk.Label(self, image = img2)
        # panel.pack(side = "bottom", fill = "both", expand = "yes")
        # panel.configure(image=img2)
        # panel.image = img2

        font = tkFont.Font(size=10, weight="bold")
        button1 = tk.Button(self, text='Play Game',  # when click on this button, call the show_frame method to make PageOne appear
                            command=lambda : startGame(controller), height = 4, width = 45,bg='#FB3640', fg='white', bd=8)
        photo = ImageTk.PhotoImage(file = "sprites/wave_icon.png")
        button1.pack()

        # button = tk.Button(self, text="Click me!")
        # img = ImageTk.PhotoImage(file="sprites/wave_icon.png") # make sure to add "/" not "\"
        # button.config(image=img)
        # button.pack() # Displaying the button
        # pack it in
        # center alignment
        #button1['bg'] = "#FB3640"
        #button1['fg'] = "white"
        
        button1['font'] = font
        button2 = tk.Button(self, text='Exit',  # when click on this button, call the show_frame method to make PageOne appear
                            command=lambda : showQuitDialog(parent), height = 4, width = 20,bd=8)
        button2['bg'] = "#30BCED"
        button2['fg'] = "white"
        button2['font'] = font
        button2.pack(pady=30, padx=10) # pack it in
        label = tk.Label(self, text='Game created by Bruno Moya in Codelearn. www.elblogdebruno.com | @elblogbruno')
        label.pack(pady=20, padx=10) # center alignment
def button():
    b=tk.Button(self,justify = tk.LEFT)

    image = Image.open("sprites/wave_icon.png")
    image = image.resize((90, 90), Image.ANTIALIAS)
    photo=ImageTk.PhotoImage(image)

    b.config(image=photo,width = photo.width(),height=photo.height())
    b.image = photo
    b.pack()
    return b
def startGame(controller):
    controller.show_frame(PageOne)
    bot.send_text(bot.warlib.BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
    bot.send_audio(bot.warlib.BOT_TOKEN,"Hola")
    bot.send_image_url(bot.warlib.BOT_TOKEN,"https://vignette.wikia.nocookie.net/central/images/4/40/Hello_there.jpg")
class PageOne(tk.Frame):
    def __init__(self, parent, controller):
        tk.Frame.__init__(self, parent)
        # bot.sendText(bot.warlib.BOT_TOKEN,"WELCOME TO WARSHIP.PY, A game about sinking ships and winning bots..")
        # bot.send_audio(bot.warlib.BOT_TOKEN,"Hola")
        # bot.send_image_url(bot.warlib.BOT_TOKEN,"https://vignette.wikia.nocookie.net/central/images/4/40/Hello_there.jpg")
        #bot.warlib.startThread()
        label = tk.Label(self, text='Start Page', font=LARGE_FONT)
        label.pack(pady=10, padx=10) # center alignment
        img2 = ImageTk.PhotoImage(Image.open("out.png"))
        panel = tk.Label(self, image = img2)
        panel.pack(side = "bottom", fill = "both", expand = "yes")
        panel.configure(image=img2)
        panel.image = img2
        #hilo2 = threading.Thread(target=checkImageChanged(panel,self))
        #hilo2.start()
        global hilo2
        hilo2 = threading.Thread(target=doit, args=("task",))
        hilo2.start()
        tk.Label(self, text='X').pack(side=tk.LEFT, padx=10)
        xEntry = tk.Entry(self,width=5)
        xEntry.pack(side = tk.LEFT)
        tk.Label(self, text='Y').pack(side=tk.LEFT, padx=10)
        yEntry = tk.Entry(self,width=5)
        yEntry.pack(side = tk.LEFT)
        button1 = tk.Button(self,cursor = 'boat', text='Attack Bot',command= lambda: update_image(xEntry.get(),yEntry.get(),panel), height = 1, width = 10)
        button1.pack(side = tk.LEFT, padx=10) # pack it in
        button1['bg'] = "#FB3640"
        button1['fg'] = "white"
        button3 = tk.Button(self, text='Clear Grid',command=lambda: ClearGrid(panel), height = 1, width = 10)
        button3.pack(side = tk.LEFT, padx=50) # pack it in
        button2 = tk.Button(self, text='Come Back',command=lambda : controller.show_frame(StartPage), height = 1, width = 10)
        button2.pack(side = tk.RIGHT)
        button2['bg'] = "#30BCED"
        button2['fg'] = "white"
        button4 = tk.Button(self, text='Reload Image',command=lambda : updateImage(panel), height = 1, width = 10)
        button4.pack(side = tk.RIGHT)
def pasteIcon(icon,pos,icon_mask):
    grid.paste(icon,pos,icon_mask)
    grid.save("out.png")
class  QuitDialog():

    def __init__(self, instance):

        self.instance = instance

        self.quitDialog = tk.Toplevel()
        img2 = ImageTk.PhotoImage(Image.open("out.png"))
        warnMessage = tk.Label(master=self.quitDialog,
                                text='Are you sure that you want to quit? ').grid(row=5, column=1, columnspan=2)
        quitButton = tk.Button(master= self.quitDialog ,
                                text='Yes',
                                command = self.quitALL).grid(row=2, column=1)

        cancelButton = tk.Button(master= self.quitDialog,
                                text='No',
                                command = lambda: self.quitDialog.destroy()).grid(row=2, column=2)

    def start(self):
        # self.invalidDiag.grab_set() #takes control over the dialog (makes it active)
        self.quitDialog.wait_window()

    def quitALL(self):
        self.quitDialog.destroy()
        self.instance.quit()
        #global panelList
        global app
        global hilo2
        app.destroy()
        hilo2.do_run = False
        hilo2.join()
def terminate_thread(thread):
    """Terminates a python thread from another thread.

    :param thread: a threading.Thread instance
    """
    if not thread.isAlive():
        return

    exc = ctypes.py_object(SystemExit)
    res = ctypes.pythonapi.PyThreadState_SetAsyncExc(
        ctypes.c_long(thread.ident), exc)
    if res == 0:
        raise ValueError("nonexistent thread id")
    elif res > 1:
        # """if it returns a number greater than one, you're in trouble,
        # and you should call it again with exc=NULL to revert the effect"""
        ctypes.pythonapi.PyThreadState_SetAsyncExc(thread.ident, None)
        raise SystemError("PyThreadState_SetAsyncExc failed")
def showQuitDialog(self):
    quitdialog = QuitDialog(self)
    quitdialog.start()
def ClearGrid(panel):
    bot.warlib.ClearGrid(panel)
    #messagebox.showinfo(gameTitle, "Done clearing")
# def checkImageChanged(panel,self):
#     moddate = os.stat("out.png")[8]
#     old_moddate = " "
#     while True:
#         time.sleep(1)
#         moddate = os.stat("out.png")[8]
#         if moddate == old_moddate:
#             print("Panel has changed")
#         else:
#             print("Panel has not changed")
#             old_moddate = moddate
#             img2 = ImageTk.PhotoImage(Image.open("out.png"))
#             panel.configure(image=img2)
#             panel.image = img2
#             file = open("coordenates.txt","r")
#             print("This are the coordenates of the bot user: " + str(file.read()))
#         #time.sleep(2)
#         self.after(2000, checkImageChanged(panel,self))
def updateImage(panel):
    img2 = ImageTk.PhotoImage(Image.open("out.png"))
    #panel = tk.Label(self, image = img2)
    panel.configure(image=img2)
    panel.image = img2
def update_image(x,y,panel):
    pos = x + ":"+y
    print("trying to atack at this position: " + pos)
    #pos = positions[str(random.randint(0,4)) + ":" + str(random.randint(0,4))]
    bot.warlib.update_position_panel(pos,panel)
    # if txt in bot.warlib.positions:
    #     pos = bot.warlib.positions[txt][0]
    #     print ("this is the current position " + str(pos))
    #     bot.warlib.update_position(pos)
    #     #pasteIcon(bot.warlib.wave_icon,pos,bot.warlib.wave_mask)
    #     img2 = ImageTk.PhotoImage(Image.open("out.png"))
    #     #panel = tk.Label(self, image = img2)
    #     panel.configure(image=img2)
    #     panel.image = img2
    #     bot.send_image(BOT_TOKEN,"out.png")
    #     msg = "Where do you want to attack?"
    #     bot.sendText(BOT_TOKEN,msg)
    # else:
    #     messagebox.showinfo(gameTitle, "This position is not available")
    #     msg = "This position is not available"
    #     bot.sendText(BOT_TOKEN,msg)
if __name__ == '__main__':
    global app
    app = MainWindow()
    app.mainloop()
