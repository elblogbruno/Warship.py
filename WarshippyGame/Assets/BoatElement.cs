using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using DragAnDrop;

public class BoatElement : Draggable, IDraggable
{
   public String boatType;

   public void OnDragEnd()
   {
      BoatsSlot.instance.InstantiateNewBoat(boatIndex);
      HandlePlacingBoats.instance.PlaceBoat(this.transform.parent.name, boatType);
         
      Destroy(this.gameObject);
   }

   public void SetImage(Sprite boat)
   {
      this.GetComponent<Image>().sprite = boat;
   }
   
   public void SetIndex(int boatIndex)
   {
      this.GetComponent<Draggable>().boatIndex = boatIndex;
   }
}
