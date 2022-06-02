using System;
using M2MqttUnity.Examples;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GameState
{
    Player1Attacking = 0,
    Player2Attacking = 1,
    Player1Won = 2,
    Player2Won = 3,
    Player1PlacingBoats  = 4,
    Player1BoatsPlaced = 5
}
public enum GameType
{
    BotVsUnity = 0,
    UnityVsUnity = 1
}

 
public class GameManager : MonoBehaviour
{
    #region Const

    private const string ON_BOT_READY = "bot-player-ready";
    private const string ON_WAITING_FOR_US = "waiting_for_player_1";
    
    private const string ON_WAITING_FOR_BOT = "waiting_for_player_2";
    #endregion
  
    #region Variables
    [Header("Debug")]
    public bool debug;
    [Header("MQTT")]
    [Space]
    public MqttClient mqttClient;
    [Header("Game related")]
    private GameState gameState;
    private GameType gameType = GameType.BotVsUnity;
    
    string botPosition ,oldPosition;
    public static GameManager instance = null;
    
    // [Space]
    // public PlayersTableControl gridSpawner;


    #endregion
    
    #region booleans
    private bool isPlayer2Ready = false,isPlayer2NameAvailable = false;
    private bool hasStartedPlaying =false;

    public bool IsPlayer2Ready
    {
        get => isPlayer2Ready;
        set => isPlayer2Ready = value;
    }

    #endregion

    #region Unity Events
    [Space]
    public UnityEvent OnStart;
    
    [Space]
    public UnityEvent OnPlayFinished;
    
    [Space]
    public UnityEvent OnBotImageReceived;

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
        
        
        
    }
    private void Start()
    {
        Debug.Log("Starting Game...");
        TelegramServerRequesterHelper.OnBotStatus = OnNewStatus;
        HandlePlacingBoats.instance.OnBoatsPlaced = OnPlayer1BoatsPlaced;
        
        if (!debug)
        {
            OnStart.Invoke();
        }
        else
        {
            this.GetComponent<HandleUI>().StartingUi();
        }
        
        if (debug)
        {
            OnNewImage("elblogbruno");
            OnNewImage("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");
            //OnNewImage(ON_BOT_READY);
        }

        
    }

    #endregion

    #region Utils
    public GameState GetGameState()
    {
        return gameState;
    }
    
    public void SetGameState(GameState state)
    {
        gameState = state;
    }

    bool CheckCorrectPos(string pos)
    {
        foreach (var coord in PlayersTableControl.instance.panelPlayer1.Coordenates)
        {
            return pos.Equals(coord);
        }

        return false;
    }

    #endregion
    
    #region MQTT Handling
    private MessagePacket lastStatus = null;
    private void OnNewStatus(MessagePacket arg0)
    {
        if (arg0 != lastStatus)
        {
            lastStatus = arg0;
            
            if (arg0.type_message == "no_messages")
            {
                MessagePacket bot1 = new MessagePacket("ack", "tcp");
                TelegramServerRequesterHelper.SendMessageToBot(bot1.ToJson(), this);
                return;
            }

            Debug.Log("New Status: " + arg0.type_message);
            
            MessagePacket bot = new MessagePacket("ack", "tcp");
            TelegramServerRequesterHelper.SendMessageToBot(bot.ToJson(), this);

            if (arg0.type_message.Contains("image"))
            {
                Debug.Log("New Image Messages");
                OnNewImage(arg0.message);
            }
            else if (arg0.type_message == "control")
            {
                OnControlMessage(arg0.message);
            }
            else
            {
                OnNewMessage(arg0);
            }
        }
    }
    /// <summary>
    /// If true it blocks the bot, so it can't attack.
    /// </summary>
    /// <param name="shouldblock"></param>
    void UnblocBlockPlayer2(bool shouldblock)
    {
        if (shouldblock)
        {
            MessagePacket message = new MessagePacket("block","control");
            TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(),this);    
        }
        else
        {
            MessagePacket message = new MessagePacket("unblock","control");
            TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(),this);   
        }
    }

    bool change = false; 
    /// <summary>
    /// Event from mqtt (player 2)
    /// </summary>
    /// <param name="message"></param>
    private void OnNewMessage(MessagePacket message)
    {
        if (GetGameState() == GameState.Player1Attacking || GetGameState() == GameState.Player2Attacking)
        {
            if (message.type_message == "attack_position")
            {
                botPosition = message.message; 
                ShouldChangeTurnToBot(true, botPosition);
            }
            else if (message.type_message == "attack_result")
            {
                Debug.Log("We got an attack result from telegram: " + message.ToString());
                
                change = PlayersPanelControl.instance.SetUserNumberOfBoats(Player.PlayerType.Bot, message.points);
                
                if (change)
                {
                    InfoPanelManager.instance.SpawnInfoMessage($"We turned down ship from {PlayersPanelControl.instance.GetPlayer(Player.PlayerType.Bot).name} at this {message.message} coordenates");
                    InfoPanelManager.instance.SpawnInfoMessage($"{PlayersPanelControl.instance.GetPlayer(Player.PlayerType.Bot).name} has {message.points} boats alive!");
                }else{

                    InfoPanelManager.instance.SpawnInfoMessage($"We hit a ship of {PlayersPanelControl.instance.GetPlayer(Player.PlayerType.Bot).name} (Player 2)!");
                    
                    

                    InfoPanelManager.instance.SpawnInfoMessage($"{PlayersPanelControl.instance.GetPlayer(Player.PlayerType.Bot).name} has {message.points} boats alive!");
                }
                
                // we update the player 2 panel with the new boat states
                ButtonManifest currentButton = PlayersTableControl.instance.panelPlayer2.GetButtonByPosition(message.message);
                currentButton.UpdateState(ButtonState.ShipDown);

                AfterAttackingLogic();
            }
        }
        else
        {
            Debug.Log("Play has finished");
        }

        if (message.type_message == "text")
        {
            InfoPanelManager.instance.SpawnInfoMessage(message.message);

            ChatController.instance.ReceiveMessageFromBot(message.message);
        }
    }
    
    /// <summary>
    /// Event from mqtt (player 2)
    /// </summary>
    /// <param name="message"></param>
    private void OnNewImage(string message)
    {
        if (GetGameState() != GameState.Player2Attacking || GetGameState() != GameState.Player1Attacking)
        {
            Debug.Log("[GameManager] New Message on IMAGE topic: " + message);
            
            if (message.Length > 100) //we have a photo from player2
            {
                HandleImageReceived(message);
                
                SetGameState(GameState.Player1PlacingBoats);
                
                OnBotImageReceived?.Invoke();

                if (!debug)
                {
                    MessagePacket message_to_bot = new MessagePacket(ON_WAITING_FOR_US,"control");

                    TelegramServerRequesterHelper.SendMessageToBot(message_to_bot.ToJson(),this);
                }
            }
            else
            {
                Debug.Log("[GameManager] Saving player2 username: " + message);
                if (!isPlayer2NameAvailable)
                {
                    PlayerPrefs.SetString("username-bot", message);
                    isPlayer2NameAvailable = true;
                }
            }
        }
        
    }
    public void OnControlMessage(string message)
    {
        if (message.Contains(ON_BOT_READY))
        {
            isPlayer2Ready = true;
            InfoPanelManager.instance.SpawnInfoMessage("Player 2 is already prepared to fight, cammon you loser.");
            PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.Bot, Player.PlayerVisualState.BoatsPlaced);
        }
    }
    private void HandleImageReceived(string image_base64)
    {
        Debug.Log("[GameManager] Saving bot photo uri: " + image_base64);
                
        byte[]  imageBytes = Convert.FromBase64String(image_base64);
                
        var dirPath = Application.persistentDataPath;
                
        var filename = dirPath + "/bot-profile-picture.jpg";
                
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
                
        File.WriteAllBytes(filename, imageBytes);
                
        Debug.Log("Written image to this url: " + filename);
                
        PlayerPrefs.SetString("photo-uri-bot", filename);
    }
    
    /// <summary>
    /// This function will be called from the HandlePlacingBoat event that tells player 1 has finished placing its boats 
    /// </summary>
    void OnPlayer1BoatsPlaced()
    {
        if (!isPlayer2Ready)
        {
            MessagePacket myObject = new MessagePacket(ON_WAITING_FOR_BOT, "control");
            TelegramServerRequesterHelper.SendMessageToBot(myObject.ToJson(),this);
        }
        
        PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.PCUser, Player.PlayerVisualState.BoatsPlaced);
        SetGameState(GameState.Player1BoatsPlaced);
    }

   
    #endregion

    #region Game Loop
    
    private void Update()
    {
        switch (GetGameState())
        {
            case GameState.Player2Attacking:
                if (hasStartedPlaying)
                {
                    Debug.Log("Player 2 Torn");
                    TopPanelManager.instance.SetInfoPanelText("Player 2 Torn");
                    PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.PCUser, Player.PlayerVisualState.Idle);
                    PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.Bot, Player.PlayerVisualState.Attacking);
                }
                break;
            case GameState.Player1Attacking:
                if (hasStartedPlaying)
                {
                    Debug.Log("Player 1 Torn");
                    TopPanelManager.instance.SetInfoPanelText("Player 1 Torn");
                    PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.Bot, Player.PlayerVisualState.Idle);
                    PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.PCUser, Player.PlayerVisualState.Attacking);
                }
                break;
            case GameState.Player1BoatsPlaced when !isPlayer2Ready:
                TopPanelManager.instance.SetInfoPanelText("Waiting for player 2!");
                Debug.Log("Waiting for player 2!");
                break;
            case GameState.Player1BoatsPlaced when isPlayer2Ready:
            {
                if (!hasStartedPlaying)
                {
                    TopPanelManager.instance.SetInfoPanelText("Game starting!");
                    StartPlay();
                    hasStartedPlaying = true;
                }

                break;
            }
            case GameState.Player1Won:
                TopPanelManager.instance.SetInfoPanelText("Player 1 won!");
                break;
            case GameState.Player2Won:
                TopPanelManager.instance.SetInfoPanelText("Player 2 won!");
                break;
            case GameState.Player1PlacingBoats:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    #endregion

    #region GameControl
    void StartPlay()
    {
        Debug.Log("Both users have placed the boats");
        
        for (int i = 3; i > 0; i--)
        {
            InfoPanelManager.instance.SpawnInfoMessage($"Game starting in {i}",true);
        }

        int random = Random.Range(0, 2);
        
        if (random == 1)
        {
            UnblocBlockPlayer2(false);
            InfoPanelManager.instance.SpawnInfoMessage($"Player 2 starts attacking",true);
            PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.Bot, Player.PlayerVisualState.Attacking);

            // if they are attacking us, we can't do anything, so panels are blocked.
            PlayersTableControl.instance.panelPlayer1.SwitchTableToViewMode();
            PlayersTableControl.instance.panelPlayer2.SwitchTableToViewMode();
        }
        else
        {   
            UnblocBlockPlayer2(true);
            InfoPanelManager.instance.SpawnInfoMessage($"Player 1 starts attacking",true);
            PlayersPanelControl.instance.ChangePlayerVisualState(Player.PlayerType.PCUser, Player.PlayerVisualState.Attacking);
            
            // if we are attacking we enable attack panel
            PlayersTableControl.instance.panelPlayer1.SwitchTableToViewMode();
            PlayersTableControl.instance.panelPlayer2.SwitchTableToAttack();
        }
        
        InfoPanelManager.instance.SpawnInfoMessage($"Good luck players!",true);
        
        SetGameState((GameState)random);
    }

    void GetPosibleWinner()
    {
        Player player1 = PlayersPanelControl.instance.GetPlayer(0);
        Player player2 = PlayersPanelControl.instance.GetPlayer(1);
        
        Debug.Log($"Current users boats player 1 = {player1.numOfBoats}, player 2 =  {player2.numOfBoats}");
        
        if (player1.numOfBoats == 0) //we have lost because we have 0 boats.
        {
            MessagePacket message = new MessagePacket("Yeah you rock it, YOU WON!", "text");

            TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(),this);
                
            InfoPanelManager.instance.SpawnInfoMessage("You have lost!");
            
            SetGameState(GameState.Player2Won);
            
            OnPlayFinished.Invoke();
        }
        else if (player2.numOfBoats == 0)
        {
            MessagePacket message = new MessagePacket("Yeah you rock it, YOU'VE LOST SUBNORMAL!", "text");

            TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(),this);
                
            InfoPanelManager.instance.SpawnInfoMessage("You have won!");
            
            SetGameState(GameState.Player1Won);
            
            OnPlayFinished.Invoke();
        }
    }

    /// <summary>
    /// If true, the turn changes to player2, if false it is player 1 torn and so allowed to shoot.
    /// </summary>
    /// <param name="should"></param>
    /// <param name="position"></param>
    public void ShouldChangeTurnToBot(bool should, string position = null)
    {
        if (should)
        {
            Debug.Log("[GameManager] Changing to player 2 (bot) turn: " + position);
            SetGameState(GameState.Player2Attacking);

            // if they are attacking us, we can't do anything, so panels are blocked.
            PlayersTableControl.instance.panelPlayer1.disable();
            PlayersTableControl.instance.panelPlayer2.disable();

            AttackAtPosition(position);
        }
        else
        {
            if(position != null)
            {
                Debug.Log("[GameManager] Changing to player 1 (Unity) turn: " + position);
                SetGameState(GameState.Player1Attacking);
                AttackAtPosition(position);

                PlayersTableControl.instance.panelPlayer2.enable();
            }
        }
    }

    bool changed = false;

    /// <summary>
    /// Attacks the player2 (Bot) in the position you pass.
    /// </summary>
    /// <param name="pos"></param>
    void AttackAtPosition(string pos)
    {
        ButtonManifest currentButton = PlayersTableControl.instance.panelPlayer2.GetButtonByPosition(pos);

        Player currentPlayer = currentButton.GetButtonOwner();
        Player otherPlayer = PlayersPanelControl.instance.GetPlayer(1);

        if (currentPlayer.name == otherPlayer.name)
        {
            otherPlayer = PlayersPanelControl.instance.GetPlayer(0);
        }
        
        if (GetGameState() == GameState.Player1Attacking) //Unity is attacking the telegram bot. Telegram handles the logic of bot attacking back.
        {
            Debug.Log("[ButtonGridSpawner] Attacking Player 2 (Bot) at this position: " + pos);
            
            InfoPanelManager.instance.SpawnInfoMessage("Attacking Player 2 (Bot) at this position: " + pos);

            if (gameType == GameType.BotVsUnity)
            {
                UnblocBlockPlayer2(false); //unblock bot so it can attack back

                MessagePacket message = new MessagePacket(pos, "attack_position");

                TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(), this);
            }
        }
        else //we have been attacked by the bot.
        {   
            // we are searching for the button that has the same position as the one we have received.
            currentButton = PlayersTableControl.instance.panelPlayer1.GetButtonByPosition(pos);

            //Bot has attacked unity.
            if (currentButton.HasGotHiddenShip()) 
            {
                changed = calculate_number_of_boats(currentPlayer.numOfBoats, 3);                
                
                Debug.Log("[ButtonGridSpawner] Bot has attacked us at this position: " + pos + " " + changed);

                if (changed)
                {
                    InfoPanelManager.instance.SpawnInfoMessage($"A ship by  {currentPlayer.name} (Player 1) was turned down by {otherPlayer.name} (Player 2) !");

                    if (gameType == GameType.BotVsUnity)
                    {
                        // MessagePacket message = new MessagePacket($"Yeah you rock it, you turn down a ship of  {currentPlayer.name} (Player 1)!", "text");
                        MessagePacket message = new MessagePacket($"{currentButton.ButtonAttackCoordenates}:{currentButton.ButtonOrientation}", "attack_response");

                        TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(), this);
                    }

                }else{

                    InfoPanelManager.instance.SpawnInfoMessage($"{otherPlayer.name} (Player 2) hit your ship {currentPlayer.name} (Player 1)!");
                    
                    if (gameType == GameType.BotVsUnity)
                    {
                        // MessagePacket message = new MessagePacket($"Yeah you rock it, you hit a ship from {currentPlayer.name} (Player 1)!", "text");
                        MessagePacket message = new MessagePacket($"{currentButton.ButtonAttackCoordenates}:{currentButton.ButtonOrientation}", "attack_response");

                        TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(), this);
                    }
                }

                currentButton.UpdateState(ButtonState.ShipDown);
            }
            else
            {
                Debug.Log("[ButtonGridSpawner] Bot has hit water: " + pos);

                InfoPanelManager.instance.SpawnInfoMessage("Player 2 hit watter. What a dumb one!");

                if (gameType == GameType.BotVsUnity)
                {
                    // MessagePacket message = new MessagePacket("You hit watter. What a dumb one!", "text");
                    MessagePacket message = new MessagePacket("missed", "attack_response");

                    TelegramServerRequesterHelper.SendMessageToBot(message.ToJson(), this);
                }

                currentButton.UpdateState(ButtonState.WaterDown);
            }

            
            
            AfterAttackingLogic();
        }

    }

    bool calculate_number_of_boats(int currentBoatNumber, int BOAT_SIZE)
    {
        int table_boat_pieces_count = PlayersTableControl.instance.panelPlayer1.GetNumberOfBoats() / BOAT_SIZE;
        bool changed = false;
        
        if (table_boat_pieces_count % 2 == 0)
        {
            Debug.Log("WE HAVE LOST A SHIP " + table_boat_pieces_count.ToString() +  " " + PlayersTableControl.instance.panelPlayer1.GetNumberOfBoats().ToString());
            changed = true;
            changed = PlayersPanelControl.instance.SetUserNumberOfBoats(Player.PlayerType.PCUser, currentBoatNumber - 1);
        }

        return changed;
    }

    private void AfterAttackingLogic() {
        GetPosibleWinner();
        
        if(GetGameState() != GameState.Player2Won || GetGameState() != GameState.Player1Won)
        {
            if (GetGameState() == GameState.Player2Attacking)
            {
                PlayersTableControl.instance.panelPlayer2.SwitchTableToAttack();
                UnblocBlockPlayer2(true); //block bot so it can't attack
                SetGameState(GameState.Player1Attacking);
            }
            else
            {
                PlayersTableControl.instance.panelPlayer2.SwitchTableToViewMode();
                SetGameState(GameState.Player2Attacking); //it is bot user turn
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
