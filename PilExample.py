from PIL import Image, ImageDraw, ImageFont
import time
TOP_CORNER = (20,20)
TOP_CORNER_1 = (140,20)
TOP_CORNER_2 = (260,20)
TOP_CORNER_3 = (375,20)
TOP_CORNER_4 = (490,20)
positions = {"0:0":(20,20),"0:1":(140,20),"0:2":(260,20),"0:3":(375,20),"0:4":(490,20),
            "1:0":(20,135),"1:1":(140,135),"1:2":(260,135),"1:3":(375,135),"1:4":(495,135),
            "2:0":(20,255),"2:1":(140,255),"2:2":(260,255),"2:3":(375,255),"2:4":(495,255),
            "3:0":(20,375),"3:1":(140,375),"3:2":(260,375),"3:3":(375,375),"3:4":(495,375),
            "4:0":(20,495),"4:1":(140,495),"4:2":(260,495),"4:3":(375,495),"4:4":(495,495)}

positions_1 = [[(20,20)],[(140,20)],[(260,20)],[(375,20)],[(490,20)]]
positions_2 = [[(20,135)],[(140,135)],[(260,135)],[(375,135)],[(495,135)]]
positions_3 = [[(20,255)],[(140,255)],[(260,255)],[(375,255)],[(495,255)]]
positions_4 = [[(20,375)],[(140,375)],[(260,375)],[(375,375)],[(495,375)]]
positions_5 = [[(20,495)],[(140,495)],[(260,495)],[(375,495)],[(495,495)]]
# TODO: Create function that draws any image in the certain location passed.

size = 85, 85
grid = Image.open('grid.png', 'r')
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
def pasteIcon(icon,pos,icon_mask):
    grid.paste(icon,pos,icon_mask)
# def pasteWave(pos):
#     grid.paste(wave_icon,pos,wave_mask)
# def deleteShip(pos):
#     grid.paste(white_paint,pos,white_mask)
# def deleteShips():
#     i = 0
#     while i < 5:
#         print(positions_1[i][0])
#         deleteShip(positions_1[i][0])
#         deleteShip(positions_2[i][0])
#         deleteShip(positions_3[i][0])
#         deleteShip(positions_4[i][0])
#         deleteShip(positions_5[i][0])
#         i = i + 1
#         #deleteShip(positions[i][0])
#     grid.save('out.png')
# def pasteShips():
#     i = 0
#     while i < 5:
#         print(positions_1[i][0])
#         print(positions_2[i][0])
#         print(positions_3[i][0])
#         print(positions_4[i][0])
#         print(positions_5[i][0])
#         pasteShip(positions_1[i][0])
#         pasteShip(positions_2[i][0])
#         pasteShip(positions_3[i][0])
#         pasteShip(positions_4[i][0])
#         pasteShip(positions_5[i][0])
#         #deleteShip(positions[i][0])
#         i = i +1
#     grid.save('out.png')
# def pasteWaves():
#     i = 0
#     while i < 5:
#         print(positions_1[i][0])
#         print(positions_2[i][0])
#         print(positions_3[i][0])
#         print(positions_4[i][0])
#         print(positions_5[i][0])
#         pasteWave(positions_1[i][0])
#         pasteWave(positions_2[i][0])
#         pasteWave(positions_3[i][0])
#         pasteWave(positions_4[i][0])
#         pasteWave(positions_5[i][0])
#         #deleteShip(positions[i][0])
#         i = i +1
#     grid.save('out.png')
# def pasteIcons(icon,icon_mask):
#     i = 0
#     while i < 5:
#         pasteIcon(icon,positions_1[i][0],icon_mask)
#         pasteIcon(icon,positions_2[i][0],icon_mask)
#         pasteIcon(icon,positions_3[i][0],icon_mask)
#         pasteIcon(icon,positions_4[i][0],icon_mask)
#         pasteIcon(icon,positions_5[i][0],icon_mask)
#         #deleteShip(positions[i][0])
#         i = i +1
#     grid.save('out.png')
# pasteIcons(ship_icon,ship_mask)
# time.sleep(5)
# deleteShips()
# pasteIcons(sunk_icon,sunk_mask)
# time.sleep(5)
# deleteShips()
# pasteIcons(wave_icon,wave_mask)
# time.sleep(5)
# deleteShips()



r = input("please tell me a position: ")
pos = positions[r]
print(pos)
pasteIcon(sunk_icon,pos,sunk_mask)
grid.save("out.jpg")
