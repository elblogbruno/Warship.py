using System;
using System.Collections;
using System.Collections.Generic;
using DragAnDrop;
using UnityEngine;

public class MainPanelUI : MonoBehaviour
{
    public GameObject RightUsersPanel;
    public BoatsSlot BoatsSlot;
    public GameObject TopPanel;


    private void OnDisable()
    {
        print("being dissabled");
    }
}
