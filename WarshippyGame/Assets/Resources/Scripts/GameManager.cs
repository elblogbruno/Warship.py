using M2MqttUnity.Examples;
using System.Collections;
using TMPro;
using UnityEngine;

using UnityEngine.SceneManagement;

public enum GameState
{
    UserAttacking = 0,
    BotAttacking = 1,
    UserWon = 2,
    BotWon = 3,
    UserPlacingBoats  = 4,
    UserBoatsPlaced = 5
}


public class GameManager : MonoBehaviour
{

    #region Const

    public const string ON_BOT_READY = "bot-player-ready";


    #endregion
  
    #region Variables
    public MqttClient mqttClient;
    public GameState gameState;
    string botPosition ,oldPosition;
    public static GameManager instance = null;
    public ButtonGridSpawner gridSpawner;
    public PlayersPanelControl control;
    public bool isPlayer2Ready = false,isPlayer2NameAvailable = false,hasBeenRandomized =false;
    #endregion

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
        if(mqttClient != null)
        {
            mqttClient.onNewMessageMQTT += onNewMessage;
            mqttClient.onNewMessageMQTTImage += onNewImage;
        }

    }
    private void Start()
    {
        Debug.Log("Starting Game...");
        
        //mqttClient.TestPublish();
        //TelegramServerRequesterHelper.StartBotPhotoBehaviour(this);
        //gridSpawner.StartGrid();
        //control.spawnUsers();
    }

    #endregion

    #region Utils

    public void SetBotAttackPosition(string pos)
    {
        botPosition = pos;
    }
    public GameState GetGameState()
    {
        return gameState;
    }
    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    bool checkCorrectPos(string pos)
    {
        foreach (var coord in gridSpawner.coordenates)
        {
            return pos.Equals(coord);
        }

        return false;
    }

    #endregion

    #region MQTT Handling

    public void onNewMessage(string message)
    {
        Debug.Log("[GameManager] New Message: " + message);
        if (gameState != GameState.UserPlacingBoats)
        {
            if (checkCorrectPos(message))
            {
                botPosition = message;
                ShouldChangeTurnToBot(true, botPosition);
            }
            else
            {
            
            }
        }
    }
    public void onNewImage(string message)
    {
        if (gameState != GameState.BotAttacking || gameState != GameState.UserAttacking)
        {
            Debug.Log("[GameManager] New Message: " + message);
            if (message.Contains("jpg"))
            {
                Debug.Log("[GameManager] Saving bot photo uri: " + message);
                PlayerPrefs.SetString("photo-uri-bot", message);
                PlayersPanelControl.instance.spawnUsers();
                this.GetComponent<HandlePlacingBoats>().startPlacingBoats();
                gameState = GameState.UserPlacingBoats;
            }
            else if (message.Contains(ON_BOT_READY))
            {
                isPlayer2Ready = true;
            }
            else
            {
                Debug.Log("[GameManager] Saving bot username: " + message);
                if (!isPlayer2NameAvailable)
                {
                    PlayerPrefs.SetString("username-bot", message);
                    isPlayer2NameAvailable = true;
                }
            }
        }
        
    }

    #endregion

    #region Game Loop
    
    private void Update()
    {
        if(gameState == GameState.BotAttacking)
        {
            //Debug.Log("Bot Torn");
        }
        else if(gameState == GameState.UserAttacking)
        {
            //Debug.Log("User Torn");
        }
        else if (gameState == GameState.UserBoatsPlaced && isPlayer2Ready)
        {
            if (!hasBeenRandomized)
            {
                StartPlay();
                hasBeenRandomized = true;
            }
        }
    }
    void StartPlay()
    {
        Debug.Log("Both users have placed the boats");
        for (int i = 3; i > 0; i--)
        {
            InfoPanelManager.instance.SpawnInfoMessage($"Game starting in {i+1}");
        }

        int random = Random.Range(0, 1);
        if (random == 1)
        {
            gameState = GameState.BotAttacking;
            InfoPanelManager.instance.SpawnInfoMessage($"Player 2 starts attacking");
        }
        else
        {
            gameState = GameState.UserAttacking;
            InfoPanelManager.instance.SpawnInfoMessage($"Player 1 starts attacking");
        }

        InfoPanelManager.instance.SpawnInfoMessage($"Good luck players!");
    }

    public void ShouldChangeTurnToBot(bool should,string position)
    {
        if (should)
        {
            if(position != null){
                gameState = GameState.BotAttacking;
                Debug.Log("[GameManager] Bot attacking at this position: " + position);
                InfoPanelManager.instance.SpawnInfoMessage("Bot player is attacking you at this position: " + position);
                ButtonGridSpawner.instance.attackAtPosition(position,mqttClient);
            }
            else
            {
                TelegramServerRequesterHelper.SendMessageToBot("There was an error. Please write that again!", this);
            }   
        }
        else
        {
            if(position != null)
            {
                gameState = GameState.UserAttacking;
                InfoPanelManager.instance.SpawnInfoMessage("Attacking bot at this position: " + position);
                ButtonGridSpawner.instance.attackAtPosition(position,mqttClient);
            }
        }
        
    }

    

    #endregion


    #region SceneManagement
    // Start is called before the first frame update
    public void LoadGame()
    {
        StartCoroutine(LoadYourAsyncScene("UserAskInfoScreen"));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void ReturnMenu()
    {
        StartCoroutine(LoadYourAsyncScene("InitialMenu"));
    }
    IEnumerator LoadYourAsyncScene(string levelName)
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(levelName);

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
    #endregion
}
