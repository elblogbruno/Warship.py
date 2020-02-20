using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System;
using UnityEngine.Networking;
using M2MqttUnity.Examples;

public enum ButtonState
{
    Water = 0,
    Ship = 1,
    ShipDown = 2,
    WaterDown = 3,
}
public class ButtonManifest : MonoBehaviour, IPointerEnterHandler
{
    #region Const

    int WIDTH = 452;
        int HEIGHT = 415;
    #endregion
    
    public string ButtonAttackCoordenates;
    public Image ButtonGridImage;
    public Text positionText;
    public bool hasHiddenShip;
    public Sprite BGon;
    public Sprite BGoff;
    [Header("Type of boats.")]
    public Sprite WaterSprite;
    public Sprite ShipSprite;
    public Sprite ShipDownSprite;
    public Sprite WaterDown;
    public Sprite IdleSprite;
    public Player _currentPlayer;

    /*[HideInInspector]
    public HelloClient PythonClient;*/

    public ButtonState _currentButtonState;
    
    public string getCoordenates()
    {
        return ButtonAttackCoordenates;
    }
    public void UpdateState(ButtonState state)
    {
        Sprite CurrentSprite = IdleSprite;
        switch (state)
        {
            case ButtonState.Water:
                CurrentSprite = WaterSprite;
                break;
            case ButtonState.Ship:
                CurrentSprite = ShipSprite;
                break;
            case ButtonState.ShipDown:
                CurrentSprite = ShipDownSprite;
                break;
            case ButtonState.WaterDown:
                CurrentSprite = WaterDown;
                break;
            default:
                break;
        }
        ButtonGridImage.sprite = CurrentSprite;
    }
    public bool hasGotHiddenShip()
    {
        return hasHiddenShip;
    }
    public void setHiddenShip(bool has)
    {
        hasHiddenShip = has;
    }
    public Player getButtonOwner()
    {
        return _currentPlayer;
    }
    public void setButtonOwner(Player playerType)
    {
        _currentPlayer = playerType;
    }
    public void setButtonState(bool state)
    {
        this.GetComponent<Button>().interactable = state;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySingle(SoundManager.SoundType.BUTTON_SOUND);
    }
    public void setText(string text){
        ButtonAttackCoordenates = text;
        positionText.text = text;
    }
    public void TakeScreenshotOfPlay(int width,int height)
    {
        StartCoroutine(TakeSnapshot());
    }
    public void TakeScreenshot()
    {
        var width = 452;
        var height = 415;
        var startX = 168;
        var startY = 10;
        var tex = new Texture2D(width, height, TextureFormat.RGB24, false);

        Rect rex = new Rect(startX, startY, width, height);

        tex.ReadPixels(rex, 0, 0);
        tex.Apply();

        // Encode texture into PNG
        var bytes = tex.EncodeToJPG();
        string bytesstr = Convert.ToBase64String(bytes);
        //PythonClient.SendText(bytesstr);
        Destroy(tex);

        //System.IO.File.WriteAllBytes(Application.dataPath + "SavedScreen.png", bytes);

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
        var bytes = tex.EncodeToJPG();
        TelegramServerRequesterHelper.SendImageToBot(tex.EncodeToJPG(), "name"+System.DateTime.UtcNow,this);
        TelegramServerRequesterHelper.SendMessageToBot("Your Turn...",this);
        TelegramServerRequesterHelper.SendAudioToBot("Your Turn...", this);
        GameManager.instance.ShouldChangeTurnToBot(false, ButtonAttackCoordenates);
        //TelegramServerRequesterHelper.GetMessageFromBot(this);
        Destroy(tex);

    }

    IEnumerator SendRequest(UnityWebRequest www)
    {
        yield return www.SendWebRequest();
        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            var w = www;
            Debug.Log("Success!\n" + www.downloadHandler.text);
        }
    }

    // SERVER //////////////////////////



    public void onClick()
    {
        Debug.Log("Attacking bot at this coordenates: " + ButtonAttackCoordenates);
        //PythonClient.SendText(ButtonAttackCoordenates);
        TakeScreenshotOfPlay(WIDTH,HEIGHT);
        InfoPanelManager.instance.SpawnInfoMessage("Attacking Bot Player at this coordenates: "+ ButtonAttackCoordenates);
    }
}
