import threading
import time
def worker():
    i = 0
    while True:
        i = i + 1
        print (threading.currentThread().getName(), 'Worker ' + str(i))
        print (threading.currentThread().getName(), 'Worker '+ str(i))
def worker1():
    i = 0
    while True:
        i = i + 1
        print (threading.currentThread().getName(), 'Worker1 ' + str(i))
        print (threading.currentThread().getName(), 'Worker1 '+ str(i))

z = threading.Thread(target=worker)
z.start()
w = threading.Thread(target=worker1)
w.start()