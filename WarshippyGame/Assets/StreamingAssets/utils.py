coordenates_table = [["0:0","0:1","0:2","0:3","0:4"],
                ["1:0","1:1","1:2","1:3","1:4"],
                ["2:0","2:1","2:2","2:3","2:4"],
                ["3:0","3:1","3:2","3:3","3:4"],
                ["4:0","4:1","4:2","4:3","4:4"]]
orientation_table = [["Vertical","Horizontal"]]
yes_table = [["War","Goodbye My Lover, Goodbye my friend"]]
        
def get_coordenates_as_string():
    coordenates = " "
    for l in coordenates_table:
        for coord in l:
            coordenates = coordenates + "|" + coord
    final_coordenates = coordenates[2:]
    return final_coordenates
    
def get_orientation_as_string():
    orientation = " "
    for l in orientation_table:
        for coord in l:
            orientation = orientation + "|" + coord
    final_orientation = orientation[2:]
    return final_orientation
