import json


class MessagePacket:
    def __init__(self, message, type_message, points):
        self.dic = {
            "message": str(message),
            "type_message": str(type_message),
            "points": str(points)
        }

    def to_json(self, text):
        a = json.loads(text)
        self.type_message = a['type_message']
        self.message = a['message']
        self.points = a['points']

    def to_string(self):
        json_response = json.dumps(self.dic, ensure_ascii=False)
        return json_response

