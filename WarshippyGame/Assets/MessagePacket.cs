using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MessagePacket
{
    public int points;
    public string type_message;
    public string message;

    MessagePacket()
    {
        
    }

    public override String ToString()
    {
        return "Points: " + points + " Type: " + type_message + " Message: " + message;
    }

    public string ToJson()
    {
        return JsonUtility.ToJson(this);
    }
    
    public MessagePacket(int numeric_message, string message, string typeMessage)
    {
        this.points = numeric_message;
        this.message = message;
        this.type_message = typeMessage;
    }
    public MessagePacket(string message, string typeMessage)
    {
        this.points = 0;
        this.message = message;
        this.type_message = typeMessage;
    }
}
