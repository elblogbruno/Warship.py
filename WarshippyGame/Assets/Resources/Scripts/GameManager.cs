using System.Collections;

using UnityEngine;

using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
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
}
