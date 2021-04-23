using System;
using System.Collections.Generic;
using UnityEngine;

namespace DragAnDrop
{
    public class BoatsSlot : MonoBehaviour
    {
        public static BoatsSlot instance;
        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(instance);
            }
        }

        public Transform BoatParent;
        public BoatElement BoatButtonTemplate;
        public List<BoatElement> BoatList;

        private void Start()
        {
            NextBoat();
        }

        public void InstantiateNewBoat(int i)
        {
            Debug.Log("INSTANTIATING BOAT AT " + i);
            
            BoatElement e = Instantiate(BoatButtonTemplate, BoatParent);
            e.SetImage(GameResources.Instance.CurrentAvailableBoats[i]);
            e.gameObject.name = e.gameObject.name + i;
            e.SetIndex(i);
            BoatList.RemoveAt(i);
            //BoatList[i] = e;
            e.transform.SetSiblingIndex(i);
            BoatList.Insert(i,e);
        }
        public void InitBoatSlot()
        {
            for (int i = 0; i < GameResources.Instance.CurrentAvailableBoats.Count; i++)
            {
                BoatElement e = Instantiate(BoatButtonTemplate, BoatParent);
                e.name = e.name + i;
                e.SetImage(GameResources.Instance.CurrentAvailableBoats[i]);
                e.SetIndex(i);
                e.transform.SetSiblingIndex(i);
                BoatList.Add(e);
            }
            
        }

        public int counter = -1;
        
        // Start is called before the first frame update
        public void NextBoat()
        {
            if (counter < BoatList.Count)
            {
                counter++;
            }
            else
            {
                counter = 0;
            }
            
            
            for (int i = 0; i < BoatList.Count; i++)
            {
                BoatList[i].gameObject.SetActive(i == counter);
            }
        }
    }
}
