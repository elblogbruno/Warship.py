using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HandlePlacingBoats : MonoBehaviour
{
    #region Variables
    public static HandlePlacingBoats instance = null;

    private const int MAX_BOATS = 3;
    public List<ButtonManifest> currentBoats;
    public Action OnBoatsPlaced;
    
    private bool UpdateUI = true;
    public int currBoatsSum = 0;
    
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
        TopPanelManager.instance.doneButton.gameObject.SetActive(false);
        TopPanelManager.instance.doneButton.onClick.AddListener(StopPlacingBoats);
        
        TopPanelManager.instance.currentBoatNumberPanel.SetActive(false);
        currentBoats = new List<ButtonManifest>();
    }

    #endregion
    public void StartPlacingBoats(bool debug = false)
    {
        TopPanelManager.instance.currentBoatNumberPanel.SetActive(true);
        
        if (!debug)
        {
            InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, be careful with player 2!");
            InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, start by placing your first boat!");
        }
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

    /* Places boat on the board */
    public void PlaceBoat(string coordinates, String boatType)
    {
        if (currBoatsSum >= MAX_BOATS)
        {
            InfoPanelManager.instance.SpawnInfoMessage("You can't place more boats!");
            return;
        }

        ButtonManifest clickedButton = PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(coordinates);
        
        EventSystem.current.SetSelectedGameObject(clickedButton.gameObject);

        bool boatExists = BoatOnList(clickedButton);
        
        if (boatExists) //we need to be able to deselect boats
        {
            RemoveBoat(clickedButton);
        }
        
        if (clickedButton.gameObject.GetComponent<Boat>().TryPlaceBoat(boatType))
        {                
            currentBoats.Add(clickedButton);

            UpdateCounter();
        }
        else
        {
            InfoPanelManager.instance.SpawnInfoMessage("Boat can't be placed here!");
        }    
    }

    /* Removes boat from the list and desinitializes it */
    private void RemoveBoat(ButtonManifest boat)
    {
        currentBoats.Remove(boat);

        boat.gameObject.GetComponent<Boat>().DesinitializeBoat();
    }

    /* Updates the counter on the top panel */
    private void UpdateCounter()
    {
        currBoatsSum = currentBoats.Count;
        
        string info =  $"Boat {currBoatsSum} out of {MAX_BOATS}";
        
        TopPanelManager.instance.SetInfoPanelText(info); 
    }

    /* Unplaces a boat from the board */
    public void UnplaceBoat(string coordinates)
    {
        ButtonManifest clickedButton = PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(coordinates);

        RemoveBoat(clickedButton);
    
        UpdateCounter();
    }

    private ButtonManifest button;
    private void Update()
    {
        if (UpdateUI)
        {
            if (currentBoats.Count >= MAX_BOATS)
            {
                TopPanelManager.instance.SetButtonStatus(true);
                TopPanelManager.instance.doneButton.Select();
            }
            else
            {
                TopPanelManager.instance.SetButtonStatus(false);
            }

            // if (currentBoats.Count >= 0)
            // {
            //     if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != null)
            //     {
            //         if (EventSystem.current.currentSelectedGameObject.GetComponent<ButtonManifest>() != null)
            //         {
            //             button = EventSystem.current.currentSelectedGameObject.GetComponent<ButtonManifest>();

            //             if (Input.GetKeyDown(KeyCode.V) && button.ButtonOrientation != "v")
            //             {
            //                 button.RotateButton(false);
            //                 button.gameObject.GetComponent<Boat>().RespawnBoat(button.x, button.y, "h", "v");
            //             }
            //             else if (Input.GetKeyDown(KeyCode.H) && button.ButtonOrientation != "h")
            //             {
            //                 button.RotateButton(true);
            //                 button.gameObject.GetComponent<Boat>().RespawnBoat(button.x, button.y, "v", "h");
            //             }
            //         }
            //         else
            //         {
            //             Debug.Log($"Selected gameobject {EventSystem.current.currentSelectedGameObject.name} is not a correct button!");
                        
            //             if (EventSystem.current.currentSelectedGameObject.GetComponent<ButtonManifest>() != null)
            //             {
            //                 Debug.Log("Deselecting before boat!");
            //                 EventSystem.current.SetSelectedGameObject(null);
            //             }
            //         }
            //     }
            // }
        }
    }

    public void StopPlacingBoats()
    {
        Debug.Log("Stop placing boats");
        UpdateUI = false;
        TopPanelManager.instance.SetButtonStatus(false);
        OnBoatsPlaced?.Invoke();
    }
}
