﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events;
using System.IO;

public class PlayersPanelControl : MonoBehaviour
{
    public UnityEvent onPlayersSpawned;
    [Header("List of CurrentPlayers")]
    public List<Player> players;

    public RawImage PlayerImage;
    public TMP_Text PlayerName;
    public RawImage PlayerImage1;
    public TMP_Text PlayerName1;

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

    public void spawnUsers()
    {
        string name = PlayerPrefs.GetString("PlayerName");
        string url = PlayerPrefs.GetString("PhotoURI");
        string name1 = "BotPlayer";
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
    IEnumerator GetImage(string url,bool who)
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
            PlayerName1.text = name;
            StartCoroutine(GetImage(photo_uri,true));
        }
        else
        {
            PlayerName1.text = name;
            StartCoroutine(GetImage(photo_uri,false));
        }
        
        return player;
    }
}
