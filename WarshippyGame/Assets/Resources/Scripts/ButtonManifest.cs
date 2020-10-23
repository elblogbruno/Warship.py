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

    #region Variables
    
    //public MqttClient client;
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
    public ButtonState _currentButtonState;

    #endregion

    #region Utils

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
    
    public void setText(string text){
        ButtonAttackCoordenates = text;
        positionText.text = text;
    }
    
    public void onClick()
    {

        if (GameManager.instance.GetGameState() == GameState.UserAttacking)
        {
            Debug.Log("Attacking bot at this coordenates: " + ButtonAttackCoordenates);

            ButtonGridSpawner.instance.TakeScreenshotOfPlay(WIDTH,HEIGHT,ButtonAttackCoordenates);
        
            GameManager.instance.ShouldChangeTurnToBot(false, ButtonAttackCoordenates);
        
            InfoPanelManager.instance.SpawnInfoMessage("Attacking Bot Player 2 at this coordenates: "+ ButtonAttackCoordenates);
        }
        else if(GameManager.instance.GetGameState() == GameState.UserPlacingBoats)
        {
            Debug.Log("Placing boat at this coordenates: " + ButtonAttackCoordenates);

            HandlePlacingBoats.instance.placeBoat(getCoordenates());
        }
        else
        {
            InfoPanelManager.instance.SpawnInfoMessage("Please don't go to fast you sucker.");
        }
   }
    #endregion

    #region Sound

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySingle(SoundManager.SoundType.BUTTON_SOUND);
    }


    #endregion

    



    
}
