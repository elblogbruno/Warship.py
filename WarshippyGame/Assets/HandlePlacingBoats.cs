using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HandlePlacingBoats : MonoBehaviour
{
    public static HandlePlacingBoats instance = null;
    public TMP_Text currentBoatInfoText;
    public const int maxBoats = 3;

    public GameObject Player1BoatsInfoPanel;

    public List<ButtonManifest> currentBoats;
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
    private void Start()
    {
        Player1BoatsInfoPanel.SetActive(false);
    }

    #endregion
    public void startPlacingBoats()
    {
        Player1BoatsInfoPanel.SetActive(true);
        InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, be careful with player 2!");
        InfoPanelManager.instance.SpawnInfoMessage("Player 1 please, start by placing your first boat!");
        currentBoatInfoText.text = "Boat 1 out of 3";
    }

    private int currBoatsSum = 1;
    public void placeBoat(string coordenates)
    {
        Debug.Log(currBoatsSum + " " + maxBoats);
        if (currBoatsSum  <= maxBoats)
        {
            currentBoatInfoText.text = $"Boat {currBoatsSum+1} out of 3";
            ButtonManifest button = ButtonGridSpawner.instance.getButtonByPosition(coordenates);
            button.hasHiddenShip = true;
            currentBoats.Add(button);
            currBoatsSum++;
        }
        else
        {
            GameManager.instance.SetGameState(GameState.UserBoatsPlaced);
        }

    }
}
