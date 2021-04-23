using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class BoatElement : Draggable
{
   public void SetImage(Sprite boat)
   {
      this.GetComponent<Image>().sprite = boat;
   }
   public void SetIndex(int boatIndex)
   {
      this.GetComponent<Draggable>().boatIndex = boatIndex;
   }
}
