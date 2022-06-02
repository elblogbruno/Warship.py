using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HandleUI : MonoBehaviour
{
    public MainPanelUI mainPanel;
    public GameObject loadingJokesPanel;

    public GameObject chatPanel;
    
    public GameObject playersPanel;

    public GameObject ViewPanelSwitchButton;
    
    public void TogglePlayersPanel(){
        playersPanel.SetActive(!playersPanel.activeSelf);
    }

    public void ToggleChatPanel()
    {
        chatPanel.SetActive(!chatPanel.activeSelf);
    }

    public void StartingUi()
    {
        Debug.Log("Changing UI to Starting UI");
        mainPanel.gameObject.SetActive(false);
        loadingJokesPanel.SetActive(true);
    }

    public void SetPlacingBoatsUi()
    {
        Debug.Log("Changing UI to Placing Boats");
        mainPanel.gameObject.SetActive(true);
        loadingJokesPanel.SetActive(false);

        playersPanel.SetActive(true);
        chatPanel.SetActive(true);
        
        PlayersTableControl.instance.panelPlayer1.StartGrid();
        PlayersTableControl.instance.panelPlayer2.StartGrid();
        PlayersTableControl.instance.panelPlayer2.disable();
        
        ToggleUserPanel();
    }

    public void StartGameUi(){
        playersPanel.SetActive(false);
        chatPanel.SetActive(false);
    }

    public void ToggleUserPanel()
    {
        // if attack button is active switch to attack mode 

        if (ViewPanelSwitchButton.name == "attack")
        {
            ViewPanelSwitchButton.GetComponentInChildren<TMP_Text>().text = "View table";
            ViewPanelSwitchButton.name = "view";
            PlayersTableControl.instance.panelPlayer1.SwitchTableToViewMode();
        }
        else
        {
            ViewPanelSwitchButton.GetComponentInChildren<TMP_Text>().text = "Attack Table";
            ViewPanelSwitchButton.name = "attack";
            PlayersTableControl.instance.panelPlayer1.SwitchTableToAttack();
        }
    }
}
