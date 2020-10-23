using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class PlayersPanelControl : MonoBehaviour
{
    [Header("Unity Event")]
    public UnityEvent onPlayersSpawned;
    [Header("List of CurrentPlayers")]
    public List<Player> players;
    [Header("Users panel UI")]
    public Image Player1ImageHolder;
    public TMP_Text Player1NameTextHolder;
    public Image Player2ImageHolder;
    public TMP_Text Player2NameTextHolder;
    public GameObject PanelObject;
    
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

        PanelObject.SetActive(false);
    }

    public void spawnUsers()
    {
        PanelObject.SetActive(true);
        string name = PlayerPrefs.GetString("PlayerName");
        string url = PlayerPrefs.GetString("PhotoURI");
        
        string name1 = PlayerPrefs.GetString("username-bot");
        string url1 = Path.Combine(Application.streamingAssetsPath,PlayerPrefs.GetString("photo-uri-bot"));
        
        players = new List<Player>();
        players.Add(createUser(name, url, Player.PlayerType.PCUser));
        players.Add(createUser(name1, url1, Player.PlayerType.Bot));
        
        if (onPlayersSpawned != null)
        {
            Debug.Log("Users Spawned");
            onPlayersSpawned.Invoke();
        }
    }
    /*IEnumerator GetImage(string url,bool who)
    {
        Debug.Log("getting This photo url: " + url);
        WWW www = new WWW(url);
        while (!www.isDone)
            yield return null;
        if (who)
        {
            PlayerImage.texture = www.texture;
        }
        else
        {
            PlayerImage1.texture = www.texture;
        }
       
    }*/
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
            Player2NameTextHolder.text = name;
            Davinci.get().load(photo_uri).into(Player2ImageHolder).start();
            //StartCoroutine(GetImage(photo_uri,true));
        }
        else
        {
            Player1NameTextHolder.text = name;
            Davinci.get().load(photo_uri).into(Player1ImageHolder).start();
            //StartCoroutine(GetImage(photo_uri,false));
        }
        
        return player;
    }
}
