using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;


public class PlayersPanelControl : MonoBehaviour
{
    private const int MAX_BOATS_PIECES_PER_USER = 9;
    private const int MAX_BOATS_PER_USER = 3;

    [Header("Unity Event")]
    public UnityEvent onPlayersSpawned;
    [Header("List of CurrentPlayers")]
    public List<Player> players;
    
    public PlayerInfoPanel playerInfoPanel1;
    public PlayerInfoPanel playerInfoPanel2;

    public static PlayersPanelControl instance = null;

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

    public void SpawnUsers()
    {
        string name = PlayerPrefs.GetString("PlayerName");
        string url = PlayerPrefs.GetString("PhotoURI");
        
        string name1 = PlayerPrefs.GetString("username-bot");
        string url1 = PlayerPrefs.GetString("photo-uri-bot");
        
        players = new List<Player>();
        players.Add(CreateUser(playerInfoPanel1, name, url, Player.PlayerType.PCUser));
        players.Add(CreateUser(playerInfoPanel2, name1, url1, Player.PlayerType.Bot));
        
        if (onPlayersSpawned != null)
        {
            Debug.Log("Users Spawned");
            onPlayersSpawned.Invoke();
        }
    }

    public void ChangePlayerVisualState(Player.PlayerType player, Player.PlayerVisualState state)
    {
        Player playerToChange = GetPlayer(player);

        playerToChange.ChangeVisualState(state);

        switch (state){
            case Player.PlayerVisualState.BoatsPlaced:
                playerToChange.playerInfoPanel.PanelTextHolder.color = Color.green;
                break;
            case Player.PlayerVisualState.Winner:
                playerToChange.playerInfoPanel.PanelTextHolder.color = Color.red;
                break;
            case Player.PlayerVisualState.Attacking:
                playerToChange.playerInfoPanel.PanelTextHolder.color = Color.blue;
                break;
            default:
                playerToChange.playerInfoPanel.PanelTextHolder.color = playerToChange.original_color;
                break;
        }
    }


    public bool SetUserNumberOfBoats(Player.PlayerType user_type, int num)
    {
        if (user_type == Player.PlayerType.Bot)
        {
            var player = players[1];
            bool changed = num != player.numOfBoats;

            Debug.Log("SetUserNumberOfBoats: " + user_type+ " "+ num.ToString() +  " " + player.numOfBoats);

            if (changed)
            {
                player.numOfBoats = num;
            }
            players[1] = player;

            return changed;
        }
        else
        {
            var player = players[0];
            bool changed = num != player.numOfBoats;
            if (changed)
            {
                player.numOfBoats = num;
            }
            players[0] = player;

            return changed;
        }
    }
    
    public Player GetPlayer(Player.PlayerType user_type)
    {
        foreach (var player in players)
        {
            if (player.playerType == user_type)
            {
                return player;
            }
        }

        return players[0];
    }
    
    public Player GetPlayer(int num)
    {
        return players[num];
    }
    
    Player CreateUser(PlayerInfoPanel playerInfoPanel, string name, string photo_uri,Player.PlayerType type)
    {   
        Player player = new Player(playerInfoPanel, type, MAX_BOATS_PER_USER, name,photo_uri);

        playerInfoPanel.PlayerNameTextHolder.text = name;
        Davinci.get().load(photo_uri).into(playerInfoPanel.PlayerImageHolder).start();
        
        return player;
    }

}
