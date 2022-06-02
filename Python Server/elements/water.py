from elements.table_element import TableElement, ElementType


class Water(TableElement):
    def __init__(self):
        super().__init__(ElementType.Water)