using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ButtonGridSpawner : MonoBehaviour
{
    public ButtonManifest[] ListOfButtons;
    public int NumOfButtonsToSpawn;
    public Sprite WaterImage;
    public ButtonManifest ButtonTemplate;
    public string[] coordenates;
    //public HelloClient HelloClient;
    // Start is called before the first frame updateç
    int x = 0;
    int y = 0;
    void Start()
    {
        //HelloClient.OnNewMessageReceived = OnNewMessageReceived;
        coordenates = new string[25];
        coordenates[0] = (0 + ":" + 0);
        for (int i = 1; i < coordenates.Length; i++)
        {
            y++;
            if(y == 5)
            {
                y = 0;
                x = x +1;
            }
            coordenates[i] = (x+ ":" + y);
        }
        startButtons();
    }
    void OnNewMessageReceived(string message)
    {
        InfoPanelManager.instance.SpawnInfoMessage(message);
    }
    void startButtons()
    {
        ListOfButtons = new ButtonManifest[NumOfButtonsToSpawn];
        ButtonTemplate.transform.parent = this.transform;
        ListOfButtons[0] = ButtonTemplate;
       
        //ListOfButtons[0].PythonClient = HelloClient;
        ListOfButtons[0].ButtonGridImage.sprite = WaterImage;
        ListOfButtons[0].ButtonAttackCoordenates = coordenates[0];
        ListOfButtons[0].setText(coordenates[0]);
        for (int i = 1; i < ListOfButtons.Length; i++)
        {
            //Debug.Log(i);
            ButtonManifest ButtonInstance = Instantiate(ButtonTemplate, this.gameObject.transform);
            ListOfButtons[i] = ButtonInstance;
            //ListOfButtons[i].PythonClient = HelloClient;
            ListOfButtons[i].ButtonGridImage.sprite = WaterImage;
            ListOfButtons[i].ButtonAttackCoordenates = coordenates[i];
            ListOfButtons[i].setText(coordenates[i]);
        }
    }
}
