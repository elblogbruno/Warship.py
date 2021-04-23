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
    Idle = 4
}
public class ButtonManifest : MonoBehaviour, IPointerEnterHandler
{

    #region Variables
    
    //public MqttClient client;
    public string ButtonAttackCoordenates;
    public Image ButtonGridImage;
    public Text positionText;
    public bool hasHiddenShip;
    //public Sprite BGon;
    //public Sprite BGoff;
    [Header("Type of boats.")]
    public Sprite WaterSprite;
    public Sprite ShipSprite;
    public Sprite ShipDownSprite;
    public Sprite WaterDown;
    public Sprite IdleSprite;
    
    public Player _currentPlayer;
    public ButtonState _currentButtonState;
    private Quaternion originalRotation;
    #endregion

    #region Utils

    public string getCoordenates()
    {
        return ButtonAttackCoordenates;
    }

    private void Start()
    {
        originalRotation = ButtonGridImage.transform.localRotation;
    }

    public void UpdateState(ButtonState state)
    {
        Sprite CurrentSprite = IdleSprite;
        switch (state)
        {
            case ButtonState.Idle:
                CurrentSprite = IdleSprite;
                break;
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

        _currentButtonState = state;
        ButtonGridImage.sprite = CurrentSprite;
    }

    public void RotateButton(bool horizontal)
    {
        if (!horizontal)
        {
            ButtonGridImage.transform.localRotation = new Quaternion(0,0,180,0);
        }
        else
        {
            ButtonGridImage.transform.localRotation = originalRotation;
        }
    }
    public ButtonState GetState()
    {
        return _currentButtonState;
    }
    public bool hasGotHiddenShip()
    {
        return hasHiddenShip;
    }
    public void setHiddenShip(bool has)
    {
        if (has)
        {
            hasHiddenShip = true;
            UpdateState(ButtonState.Ship);
        }
        else
        {
            hasHiddenShip = false;
            UpdateState(ButtonState.Idle);
        }
    
    }
    public Player getButtonOwner()
    {
        return _currentPlayer;
    }
    public void setButtonOwner(Player playerType)
    {
        _currentPlayer = playerType;
    }
    public void setButtonInteractable(bool state)
    {
        this.GetComponent<Button>().interactable = state;
    }
    
    public void setText(string text)
    {
        this.name = text;
        ButtonAttackCoordenates = text;
        positionText.text = text;
    }



    public void OnClick()
    {
        if (GameManager.instance.GetGameState() == GameState.UserAttacking)
        {
            GameManager.instance.ShouldChangeTurnToBot(false, ButtonAttackCoordenates);
        
            InfoPanelManager.instance.SpawnInfoMessage("Attacking Bot Player 2 at this coordenates: "+ ButtonAttackCoordenates);
        }
        else if(GameManager.instance.GetGameState() == GameState.UserPlacingBoats)
        {
            Debug.Log("Placing boat at this coordenates: " + ButtonAttackCoordenates);

            HandlePlacingBoats.instance.PlaceBoat(ButtonAttackCoordenates);
        }
        else if (GameManager.instance.GetGameState() == GameState.UserBoatsPlaced && !GameManager.instance.IsPlayer2Ready)
        {
            Debug.Log("Waiting for player 2!");
            InfoPanelManager.instance.SpawnInfoMessage("We are waiting for the slow player 2. YOU SUCKER camm'ooon my mum is faster with no hands");
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
