using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TopPanelManager : MonoBehaviour
{
    public static TopPanelManager instance = null;
    
    public TMP_Text currentBoatInfoText;
    public GameObject currentBoatNumberPanel;
    public Button doneButton;
    
    private void Awake()
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

    public void SetInfoPanelText(string text)
    {
        currentBoatInfoText.text = text;
    }
    
    public void SetButtonStatus(bool status)
    {
        doneButton.gameObject.SetActive(status);
    }
    
    
}
