using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Player
{
    public enum PlayerType
    {
        PCUser = 0,
        Bot = 1,
    }
    public PlayerType playerType;
    public int numOfBoats;
    public string name;
    public string photo_uri;
    //Constructor (not necessary, but helpful)
    public Player(PlayerType playerType, int numOfBoats,string name,string photo_uri)
    {
        this.playerType = playerType;
        this.numOfBoats = numOfBoats;
        this.name = name;
        this.photo_uri = photo_uri;
    }
}
