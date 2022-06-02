from PIL import Image, ImageDraw, ImageFilter, ImageShow
import numpy as np

from config import MAXIMUM_BOATS, MAXIMUM_BOAT_LENGHT
from elements.table_element import TableElement, ElementType


class TableManager:
    def __init__(self, x=5, y=5):
        self.size = x * y
        self.x = x
        self.y = y
        self.table = np.full((x, y), TableElement(ElementType.Water))

        self.number_of_boats = MAXIMUM_BOATS # 3 boats per player

        self.parent_boats = []
        self.boats_position = {}
        self.boat_size = (100, 100)  # (80, 100)

        self.squares = self.init_squares()

        # print(self.squares)

    def init_squares(self):
        squares = {}

        size_x = self.boat_size[0]
        size_y = self.boat_size[1]

        for x in range(0, 5):
            for y in range(0, 5):
                squares[(x, y)] = (size_x * x, size_y * y)

        return squares

    def reset(self):
        self.table = np.full((self.x, self.y), TableElement(ElementType.Water))
        self.parent_boats = []
        self.boats_position = {}

    """
        @brief: Add a boat to the table
        @param: boat: Boat to add
        @return: True if the boat has been added, False otherwise
        
        boat has this structure: [ x, y, orientation]
    """
    def sunk_boat_part(self, x, y ):
        boat = Image.open('image/sunk_ship.png')

        b1 = self.boat_size[0]
        b2 = self.boat_size[1]

        boat.thumbnail((b1, b2), Image.ANTIALIAS)

        table = Image.open('image/table_1.png')

        table.paste(boat, self.squares[(x, y)], boat)

    def spawn_boat_image(self, element, table):
        x, y, orientation = element

        b1 = self.boat_size[0]
        b2 = self.boat_size[1]

        # boat = Image.open('image/atago-ship_01.png')
        boat = Image.open('image/ShipBattleshipHull_01.png')
        boat.thumbnail((b1, b2), Image.ANTIALIAS)
        # boat.show()
        # boat1 = Image.open('image/atago-ship_02.png')
        boat1 = Image.open('image/ShipBattleshipHull_02.png')
        boat1.thumbnail((b1, b2), Image.ANTIALIAS)

        # boat2 = Image.open('image/atago-ship_03.png')
        boat2 = Image.open('image/ShipBattleshipHull_03.png')
        boat2.thumbnail((b1, b2), Image.ANTIALIAS)

        boats_img = [boat, boat1, boat2]

        # boats_img but rotated 90 degrees
        boats_img_rotated = [boat.rotate(90), boat1.rotate(90), boat2.rotate(90)]

        counter = 0

        if orientation == "v":
            for b in range(y, y + 3):
                print(self.squares[(x, b)])
                table.paste(boats_img[counter], self.squares[(x, b)], boats_img[counter])
                counter += 1
            return True
        else:
            for a in range(x, x + 3):
                print(self.squares[(a, y)])
                table.paste(boats_img_rotated[counter], self.squares[(a, y)], boats_img_rotated[counter])
                counter += 1
            return True

        return False

    def add_element_to_table(self, element, original_orientation):
        orientation = original_orientation[0].lower()

        pos = (int(element.x), int(element.y))

        if orientation == "v":
            if self.can_place_boat(int(element.x), int(element.y), orientation):
                self.boats_position[pos] = [int(element.x), int(element.y), orientation]

                self.parent_boats[element.id] = element

                for b in range(int(element.y), int(element.y) + 3):
                    element.is_parent = b == range(int(element.y) + 3)[0]
                    self.table[int(element.x)][b] = element

                return True
            else:
                return False
        else:
            if self.can_place_boat(int(element.x), int(element.y), orientation):
                self.boats_position[pos] = [int(element.x), int(element.y), orientation]

                self.parent_boats[element.id] = element

                for b in range(int(element.x), int(element.x) + 3):
                    print(b)
                    element.is_parent = b == range(int(element.x), int(element.x) + 3)[0]
                    self.table[b][int(element.y)] = element

                return True
            else:
                return False

    def get_number_of_element_type(self, type):
        count = 0
        for i in range(self.x):
            for a in range(self.y):
                if self.table[i][a].type == type:
                    count += 1
        return count

    def get_number_of_element_type_by_id(self, id):
        count = 0
        for i in range(self.x):
            for a in range(self.y):
                if self.table[i][a].type == ElementType.Boat and self.table[i][a].id == id:
                    count += 1
        return count

    def recalculate_table_state(self, element=None):
        self.parent_boats[element.parent_id].number_of_boats -= 1
        self.remove_boat(x=element.x, y=element.y)

        if self.parent_boats[element.parent_id].number_of_boats == 0:
            self.number_of_boats -= 1
            return self.number_of_boats, True

        return self.number_of_boats, False

    def get_element_at_coordenates(self, coordenates=None, x=None, y=None):
        if coordenates:
            a = coordenates.split(":")
            x = int(a[0])
            y = int(a[1])
            return self.table[x][y]
        elif x and y:
            return self.table[x][y]
        else:
            return None

    def remove_element_at_coordenates(self, coordenates=None, x=None, y=None):
        if coordenates:
            a = coordenates.split(":")
            x = int(a[0])
            y = int(a[1])
            self.table[x][y] = TableElement(ElementType.Water)
            return True
        elif x is not None and y is not None:
            self.table[x][y] = TableElement(ElementType.Water)
            return True
        # else:
        #     return False

    def some_box_occupied(self, element):
        print("Checking if {0} already exists".format(str(element)))
        if self.table[int(element.x)][int(element.y)].type == element.type:
            return True
        return False

    def remove_boat(self, x, y):
        print("Removing boat at {0}".format(str(x) + ":" + str(y)))
        self.table[x][y] = TableElement(ElementType.Water)

        self.sunk_boat_part(x, y)

        # if coordenates:
        #     self.remove_element_at_coordenates(x=x, y=y)
        #     del self.boats_position[(x, y)]

    def show_table(self, show_table=False):
        return self._get_current_table_as_image(show_table)

    def __eq__(self, other):
        return other.x == self.x and other.y == self.y and other.orientation == self.orientation

    def _get_current_table_as_image(self, show_table=False):
        table = Image.open('image/table-custom.jpg')
        # water = Image.open('image/water.png')

        # water.thumbnail(self.boat_size, Image.ANTIALIAS)

        # x = 0
        # y = 0

        # for a in range(int(self.x)):
        #     for b in range(int(self.y)):
        #         if self.table[a][b].type == ElementType.Water:
        #             table.paste(water, self.squares[(a, b)])

        for pos in self.boats_position:
            boat = self.boats_position[pos]
            print("{0} {1}".format(pos, boat))
            self.spawn_boat_image(boat, table)

        table.save('image/table_1.png', format='png')

        if show_table:
            ImageShow.show(table)

        return 'image/table_1.png'

    def can_place_boat(self, x, y, orientation):
        if self.table is not None and len(self.table) < 0:
            return False

        print("Checking if boat can be placed on the board at " + str(x) + " " + str(y))

        if orientation == "h":
            if x + 3 > self.x:
                return False

            for b in range(int(x), int(x) + 3):
                if self.table[b][int(y)].type != ElementType.Water:
                    return False

            return True
        else:
            if y + 3 > self.y:
                print("Y is bigger than the table")
                return False

            for a in range(int(y), int(y) + 3):
                print((int(x), a))
                print(self.table[int(x)][a].type)
                if self.table[int(x)][a].type != ElementType.Water:
                    return False

            return True
