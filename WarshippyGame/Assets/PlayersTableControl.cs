using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragAnDrop;


public class PlayersTableControl : MonoBehaviour
{
    //create singleton
    public static PlayersTableControl instance;

    public BoatsSlot BoatsSlot;
    public UserTable panelPlayer1;
    public UserTable panelPlayer2;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        BoatsSlot.InitBoatSlot();
    }
    
}
