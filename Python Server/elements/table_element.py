from enum import Enum
import uuid


class ElementType(Enum):
    Water = 0
    Boat = 1


class TableElement:
    def __init__(self, element_type):
        self.type = element_type
        self.coordinates = " "
        self.orientation = " "
        self.x = 0
        self.y = 0
        self.id = str(uuid.uuid4())

    def get_type(self):
        return self.type

    def __str__(self):
        return 'Element of type: {0}, Coordinates: {1}, Orientation: {2}'.format(str(self.type), str(self.coordinates), str(self.orientation))
