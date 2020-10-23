using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using M2MqttUnity.Examples;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ButtonGridSpawner : MonoBehaviour
{
    #region Variable
    public MqttClient client;
    public ButtonManifest[] ListOfButtons;
    public int NumOfButtonsToSpawn;
    public Sprite WaterImage;
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


    public void StartGrid()
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
        SetupButtons();
    }
    /// <summary>
    /// Setups the grid buttons.
    /// </summary>
    void SetupButtons()
    {
        ListOfButtons = new ButtonManifest[NumOfButtonsToSpawn];
        ButtonTemplate.transform.parent = this.transform;
        ListOfButtons[0] = ButtonTemplate;
       
        ListOfButtons[0].ButtonGridImage.sprite = WaterImage;
        ListOfButtons[0].ButtonAttackCoordenates = coordenates[0];
        ListOfButtons[0].setText(coordenates[0]);
        
        for (int i = 1; i < ListOfButtons.Length; i++)
        {
            ButtonManifest ButtonInstance = Instantiate(ButtonTemplate, this.gameObject.transform);
            ListOfButtons[i] = ButtonInstance;
            
            /*bool randomBoolean = (Random.value > 0.5f);
            Player randomButtonOwner = PlayersPanelControl.instance.getPlayer(Random.Range(0, 1));
            ListOfButtons[i].setHiddenShip(randomBoolean);
            if (randomBoolean)
            {
                ListOfButtons[i].setButtonOwner(randomButtonOwner);
                ListOfButtons[i].setText(randomButtonOwner.ToString());
            }
            else
            {
                ListOfButtons[i].setText(coordenates[i]);
            }*/
            Player buttonOwner = PlayersPanelControl.instance.getPlayer(0);
            ListOfButtons[i].setText(coordenates[i]);
            ListOfButtons[i].setButtonOwner(buttonOwner);
            ListOfButtons[i].ButtonGridImage.sprite = WaterImage;
            ListOfButtons[i].ButtonAttackCoordenates = coordenates[i];
        }
    }
    #endregion
    
    void OnNewMessageReceived(string message)
    {
        InfoPanelManager.instance.SpawnInfoMessage(message);
    }
    
    

    #region Utils

    /// <summary>
    /// Returns a button on the grid by its coordenates.
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    public ButtonManifest getButtonByPosition(string pos)
    {
        int index = 0;
        for (int i = 0; i < ListOfButtons.Length; i++)
        {    
            string coord = ListOfButtons[i].getCoordenates();
            if (coord == pos)
            {
                index = i;
                Debug.Log("[ButtonGridSpawner] Button was found with this position: " + pos);
            }
        }
        return ListOfButtons[index];
    }
    
    /// <summary>
    /// Attacks the player2 (Bot) in the position you pass.
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="client"></param>
    public void attackAtPosition(string pos,MqttClient client)
    {
        ButtonManifest CurrentButton =  getButtonByPosition(pos);
        Debug.Log("[ButtonGridSpawner] Attacking at this position: " + pos);
        Player CurrentPlayer = CurrentButton.getButtonOwner();
        if (CurrentButton.hasGotHiddenShip())
        {
            InfoPanelManager.instance.SpawnInfoMessage("A ship by " + CurrentPlayer.name + " was turned down!");
            //TelegramServerRequesterHelper.SendMessageToBot("A ship by " + CurrentPlayer.name + " was turned down!", this);
            MQTTUnity2Bot.SendMessageToBot("A ship by " + CurrentPlayer.name + " was turned down!",client);
            ButtonState state = ButtonState.ShipDown;
            PlayersPanelControl.instance.setUserNumberOfBoats(CurrentPlayer, 3);
            CurrentButton.UpdateState(state);
        }
        else
        {
            InfoPanelManager.instance.SpawnInfoMessage("You hit watter. What a dumb one!");
            //TelegramServerRequesterHelper.SendMessageToBot("You hit watter. What a dumb one!",this);
            MQTTUnity2Bot.SendMessageToBot("You hit watter. What a dumb one!",client);

            ButtonState state = ButtonState.WaterDown;
            CurrentButton.UpdateState(state);
        }
        CurrentButton.setButtonState(false);
        
    }
    public void TakeScreenshotOfPlay(int width,int height,string ButtonAttackCoordenates)
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

        // Encode texture into PNG
        //var bytes = tex.EncodeToJPG();
        //TelegramServerRequesterHelper.SendImageToBot(tex.EncodeToJPG(), "name"+System.DateTime.UtcNow,this);
        
        //MQTTUnity2Bot.SendImageToBot(tex.EncodeToJPG().ToString(),client);
        string enc = System.Convert.ToBase64String(tex.EncodeToPNG());
        Debug.Log(enc);
        MQTTUnity2Bot.SendImageToBot(enc,client);

        //TelegramServerRequesterHelper.SendMessageToBot("Your Turn...",this);
        MQTTUnity2Bot.SendMessageToBot("Your Turn...",client);
        //TelegramServerRequesterHelper.SendAudioToBot("Your Turn...", this);
        
        //TelegramServerRequesterHelper.GetMessageFromBot(this);
        Destroy(tex);
    }
    #endregion
    
}
