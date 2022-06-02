from config import MAXIMUM_BOATS
from elements.table_element import TableElement, ElementType


class Boat(TableElement):
    def __init__(self):
        super().__init__(ElementType.Boat)
        self.is_parent = False

        self.parent_id = self.id if self.is_parent == False else None

        self.is_new = False
        self.has_correct_coordinates = False
        self.has_orientation_correctly = False

        self.number_of_boats = MAXIMUM_BOATS

    def __str__(self):
        return 'Coordinates: {0}, Orientation: {1}'.format(str(self.coordinates), str(self.orientation))

    def __eq__(self, other):
        return other.x == self.x and other.y == self.y and other.orientation == self.orientation
