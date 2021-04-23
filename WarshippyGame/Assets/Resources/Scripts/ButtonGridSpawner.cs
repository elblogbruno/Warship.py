using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using DragAnDrop;
using M2MqttUnity.Examples;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonGridSpawner : MonoBehaviour
{
    #region Variable
    public MqttClient client;
    private ButtonManifest[] ListOfButtons;
    public int NumOfButtonsToSpawn;
    public BoatsSlot BoatsSlot;
    public ButtonManifest ButtonTemplate;

    public string[] coordenates;
    public static ButtonGridSpawner instance = null;
    int x = 0; int y = 0;
    #endregion

    #region Setup
    public void Awake()
    {
        //Check if there is already an instance of SoundManager
        if (instance == null)
            //if not, set it to this.
            instance = this;
        //If instance already exists:
        else if (instance != this)
            //Destroy this, this enforces our singleton pattern so there can only be one instance of SoundManager.
            Destroy(gameObject);
    }
    
    /// <summary>
    /// Starts the grid
    /// </summary>
    public void StartGrid()
    {
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
        SetupButtons();
        BoatsSlot.InitBoatSlot();
    }
    /// <summary>
    /// Setups the grid buttons.
    /// </summary>
    void SetupButtons()
    {
        ListOfButtons = new ButtonManifest[NumOfButtonsToSpawn];

        for (int i = 0; i < ListOfButtons.Length; i++)
        {
            ButtonManifest ButtonInstance = Instantiate(ButtonTemplate, this.transform);
            ListOfButtons[i] = ButtonInstance;
            
            Player buttonOwner = PlayersPanelControl.instance.getPlayer(0);
            ListOfButtons[i].setText(coordenates[i]);
            ListOfButtons[i].setButtonOwner(buttonOwner);
            ListOfButtons[i].UpdateState(ButtonState.Idle);
            
            ListOfButtons[i].ButtonAttackCoordenates = coordenates[i];
        }
    }
    #endregion

    #region Utils

    /// <summary>
    /// Returns a button on the grid by its coordenates.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public ButtonManifest GetButtonByPosition(string pos)
    {
        int index = 0;
        for (int i = 0; i < ListOfButtons.Length; i++)
        {    
            string coord = ListOfButtons[i].getCoordenates();
            if (coord == pos)
            {
                index = i;
                Debug.Log("[ButtonGridSpawner] Button was found with this position: " + pos);
                break;
            }
        }
        return ListOfButtons[index];
    }
    
    

    

    #region ScreenShot

    public void TakeScreenshotOfPlayAndSentToBot()
    {
        StartCoroutine(TakeSnapshot());
    }

    WaitForSeconds waitTime = new WaitForSeconds(0.1F);
    WaitForEndOfFrame frameEnd = new WaitForEndOfFrame();
    public IEnumerator TakeSnapshot()
    {
        yield return waitTime;
        yield return frameEnd;
        /*var width = 452;
        var height = 415;
        var startX = 168;
        var startY = 10;*/

        var width = 700;
        var height = 600;
        var startX = 0;
        var startY = 0;


        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        string enc = System.Convert.ToBase64String(tex.EncodeToPNG());
        //Debug.Log(enc);
        MQTTUnity2Bot.SendImageToBot(enc,client);
        Destroy(tex);
        //GameManager.instance.ShouldChangeTurnToBot(false);
    }

    #endregion
    
    #endregion
    
}
