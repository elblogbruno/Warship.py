using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class HandlePlacingBoats : MonoBehaviour
{
    #region Variables
    public static HandlePlacingBoats instance = null;
    public BoatPlacementPanel BoatPlacementPanel;


    private const int maxBoats = 3;
    public List<ButtonManifest> currentBoats;
    public Action OnBoatsPlaced;

    public Image boatImageCursor;
    public float movementSpeed = 0.1f;
    
    private bool UpdateUI = true;
    #endregion

    #region Setup

    private void Awake()
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
    private void Start()
    {
        BoatPlacementPanel.doneButton.gameObject.SetActive(false);
        BoatPlacementPanel.doneButton.onClick.AddListener(StopPlacingBoats);
        
        BoatPlacementPanel.currentBoatNumberPanel.SetActive(false);
        currentBoats = new List<ButtonManifest>();
    }

    #endregion
    public void StartPlacingBoats(bool debug = false)
    {
        BoatPlacementPanel.currentBoatNumberPanel.SetActive(true);
        if (!debug)
        {
            InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, be careful with player 2!");
            InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, start by placing your first boat!");
        }
        //currentBoatInfoText.text = "Boat 0 out of 3";
    }

    private bool BoatOnList(ButtonManifest button)
    {
        for (int i = 0; i < currentBoats.Count; i++)
        {
            if (currentBoats[i] == button)
            {
                return true;
            }
        }

        return false;
    }
    public int currBoatsSum = 0;
    private bool allowPlacing = true;
    public void PlaceBoat(string coordinates,Sprite boatSprite)
    {
        Debug.Log(currBoatsSum + " " + maxBoats);
        ButtonManifest clickedButton = ButtonGridSpawner.instance.GetButtonByPosition(coordinates);
        bool isAlreadySelected = BoatOnList(clickedButton);
        
        if (isAlreadySelected) //we need to be able to deselect boats
        {
            Debug.Log("Deselecting boat");
            
            clickedButton.setHiddenShip(false);
            currentBoats.Remove(clickedButton);
            
            //allowPlacing = true;
        }
        else
        {
            if (allowPlacing)
            {
                //clickedButton.gameObject.SetActive(false);

                clickedButton.setHiddenShip(true);
                clickedButton.ButtonGridImage.sprite = boatSprite;
                currentBoats.Add(clickedButton);
            }
        }
        
        currBoatsSum = currentBoats.Count;
        BoatPlacementPanel.currentBoatInfoText.text = $"Boat {currBoatsSum} out of 3";
    }
    public void PlaceBoat(string coordinates)
    {
        Debug.Log(currBoatsSum + " " + maxBoats);
        ButtonManifest clickedButton = ButtonGridSpawner.instance.GetButtonByPosition(coordinates);
        bool isAlreadySelected = BoatOnList(clickedButton);
        
        if (isAlreadySelected) //we need to be able to deselect boats
        {
            Debug.Log("Deselecting boat");
            
            clickedButton.setHiddenShip(false);
            currentBoats.Remove(clickedButton);
            
            //allowPlacing = true;
        }
        else
        {
            if (allowPlacing)
            {
                //clickedButton.gameObject.SetActive(false);

                clickedButton.setHiddenShip(true);
                currentBoats.Add(clickedButton);
            }
        }
        
        currBoatsSum = currentBoats.Count;
        BoatPlacementPanel.currentBoatInfoText.text = $"Boat {currBoatsSum} out of 3";
    }
    private void Update()
    {
        if (UpdateUI)
        {
            if (currentBoats.Count >= maxBoats)
            {
                BoatPlacementPanel.doneButton.gameObject.SetActive(true);
                BoatPlacementPanel.doneButton.Select();
                allowPlacing = false;
            }
            else
            {
                BoatPlacementPanel.doneButton.gameObject.SetActive(false);
                allowPlacing = true;
            }
            //if(Input.mousePresent)
            //    boatImageCursor.transform.position = Vector3.Lerp(boatImageCursor.transform.position,Input.mousePosition,movementSpeed);
            
            if (Input.GetKeyDown(KeyCode.V))
            {
                currentBoats[currentBoats.Count-1].RotateButton(false);
            }
            else if(Input.GetKeyDown(KeyCode.H))
            {
                currentBoats[currentBoats.Count-1].RotateButton(true);
            }
        }

        
        
    }

    public void StopPlacingBoats()
    {
        Debug.Log("Stop placing boats");
        UpdateUI = false;
        BoatPlacementPanel.doneButton.gameObject.SetActive(false);
        OnBoatsPlaced?.Invoke();
        for(int i = 0; i < currentBoats.Count; i++)
        {
            currentBoats[i].UpdateState(ButtonState.Idle);
        }
    }
}
