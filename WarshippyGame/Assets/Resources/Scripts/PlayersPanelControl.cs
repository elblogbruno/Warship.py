using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class PlayersPanelControl : MonoBehaviour
{
    [Header("Unity Event")]
    public UnityEvent onPlayersSpawned;
    [Header("List of CurrentPlayers")]
    public List<Player> players;

    public PlayersInfoPanel playersInfoPanel;

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
        //PanelObject.SetActive(true);
        string name = PlayerPrefs.GetString("PlayerName");
        string url = PlayerPrefs.GetString("PhotoURI");
        
        string name1 = PlayerPrefs.GetString("username-bot");
        string url1 = PlayerPrefs.GetString("photo-uri-bot");
        
        players = new List<Player>();
        players.Add(createUser(name, url, Player.PlayerType.PCUser));
        players.Add(createUser(name1, url1, Player.PlayerType.Bot));
        
        if (onPlayersSpawned != null)
        {
            Debug.Log("Users Spawned");
            onPlayersSpawned.Invoke();
        }
    }
    public void setUserNumberOfBoats(Player user,int num)
    {
        user.numOfBoats = num;
    }
    
    public Player getPlayer(int num)
    {
        return players[num];
    }
    Player createUser(string name,string photo_uri,Player.PlayerType type)
    {   
        Player player = new Player(type,0,name,photo_uri);

        if(type == Player.PlayerType.Bot)
        {
            playersInfoPanel.Player2NameTextHolder.text = name;
            Davinci.get().load(photo_uri).into(playersInfoPanel.Player2ImageHolder).start();
        }
        else
        {
            playersInfoPanel.Player1NameTextHolder.text = name;
            Davinci.get().load(photo_uri).into(playersInfoPanel.Player1ImageHolder).start();
        }
        
        return player;
    }
}
