using UnityEngine.UI;

using UnityEngine;

public struct Player
{
    public enum PlayerVisualState
    {
        Idle,

        Attacking,
        BoatsPlaced,
        Winner
    }
    public enum PlayerType
    {
        PCUser = 0,
        Bot = 1,
    }

    public PlayerInfoPanel playerInfoPanel;
    public PlayerType playerType;
    public PlayerVisualState playerVisualState;
    public int numOfBoats;
    public string name;
    public string photo_uri;

    public Color original_color;
    //Constructor (not necessary, but helpful)
    public Player(PlayerInfoPanel playerInfoPanel, PlayerType playerType, int numOfBoats,string name,string photo_uri)
    {
        this.playerInfoPanel = playerInfoPanel;
        this.playerVisualState = PlayerVisualState.Idle;
        this.playerType = playerType;
        this.numOfBoats = numOfBoats;
        this.name = name;
        this.photo_uri = photo_uri;
        this.original_color = playerInfoPanel.PanelTextHolder.GetComponent<Image>().material.color;
    }

    public void ChangeVisualState(PlayerVisualState playerVisualState)
    {
        this.playerVisualState = playerVisualState;
    }
}
