using M2MqttUnity.Examples;
using System.Collections;

using UnityEngine;

using UnityEngine.SceneManagement;

public enum GameState
{
    UserAttacking = 0,
    BotAttacking = 1,
    UserWon = 2,
    BotWon = 3,
}

public class GameManager : MonoBehaviour
{
    public MqttClient mqttClient;
    public GameState gameState;
    string botPosition ,oldPosition;
    public static GameManager instance = null;
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
        mqttClient.onNewMessageMQTT += onNewMessage;
    }
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
    // Start is called before the first frame update
    public void LoadGame()
    {
        StartCoroutine(LoadYourAsyncScene("UserAskInfoScreen"));
    }
    public void ExitGame()
    {
        Application.Quit();
    }
    public void onNewMessage(string message)
    {
        Debug.Log("[GameManager] New Message: " + message);
        botPosition = message;
        ShouldChangeTurnToBot(true);
    }
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
    }
    public void ShouldChangeTurnToBot(bool should)
    {
        if (should)
        {
            gameState = GameState.BotAttacking;
            Debug.Log("[GameManager] Bot attacking at this position: "+ botPosition);
            InfoPanelManager.instance.SpawnInfoMessage("Bot player is attacking you at this position: " + botPosition);
            
            ButtonGridSpawner.instance.attackAtPosition(botPosition);
        }
        else
        {
            gameState = GameState.UserAttacking;
            Debug.Log("User attacking");
        }
        
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
}
