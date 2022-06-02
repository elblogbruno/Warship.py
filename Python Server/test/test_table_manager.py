import pytest

from elements.TableManager import TableManager
from elements.boat import Boat


@pytest.fixture()
def table_manager():
    def _table_manager():
        return TableManager()

    return _table_manager


class TestTableManager:
    # 1
    def test_show_table(self, table_manager) -> None:
        manager = table_manager()

        for i in range(5):
            boat = Boat()
            boat.coordinates = (0,i)
            boat.x = 0
            boat.y = i
            boat.orientation = "vertical"
            manager.add_element_to_table(boat)

        manager.show_table(show_table=True)

        assert manager.some_box_occupied(boat)
