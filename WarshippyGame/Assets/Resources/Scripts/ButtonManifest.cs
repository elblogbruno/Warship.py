using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public enum ButtonState
{
    Water = 0,
    Ship = 1,
    ShipDown = 2,
    WaterDown = 3,
    Idle = 4
}
public class ButtonManifest : MonoBehaviour, IPointerEnterHandler, IDeselectHandler, ISelectHandler
{

    #region Variables
    public int x;
    public int y;

    public string ButtonAttackCoordenates;
    public string ButtonOrientation;
    private string BoatOwnerName;

    public Image ButtonGridImage;
    public TMP_Text positionText;
    public GameObject orientationPanel;
    public TMP_Text orientationText;

    public bool hasHiddenShip;
    public bool hasSink;
    
    
    [Header("Type of boats.")]
    public Sprite WaterSprite;
    public Sprite ShipSprite;
    public Sprite ShipDownSprite;
    public Sprite WaterDown;
    public Sprite IdleSprite;
    
    private Player _currentPlayer;
    private ButtonState _currentButtonState;
    private Quaternion originalRotation;


    #region Owner Logic Variables
        public bool isOwner;
        public int ownerX = -1;
        public int ownerY = -1;
    #endregion
    
    public Color selectColor;
    private Color originalColor;
    
    private UserTable _userTable;
    
    private Boat _boat;
    public Boat boat{
        /* if null set to boat else return boat*/
        get{
            if (_boat == null)
            {
                _boat = GetComponentInParent<Boat>();
            }
            return _boat;
        }
    }
    #endregion

    #region Utils

    private UserTable GetPanelParent()
    {
        return _userTable;
    }

    public void SetPanelParent(UserTable table){
        _userTable = table;
    }
    public string getCoordenates()
    {
        return ButtonAttackCoordenates;
    }

    private void Start()
    {
        originalRotation = ButtonGridImage.transform.localRotation;
        originalColor = ButtonGridImage.color;
        orientationPanel.SetActive(false);
    }

    public void SetOwnerCoordenates(int x, int y)
    {
        ownerX = x;
        ownerY = y;
    }
    
    public string GetNameOfBoatOwner(string name)
    {
        return BoatOwnerName;
    }
    
    public void SetNameOfBoatOwner(string name)
    {
        BoatOwnerName = name;
    }

    public void SetCustomSprite(Sprite sprite, bool vertical = false)
    {
        ButtonGridImage.sprite = sprite;
        
        if (!vertical)
        {
            ButtonGridImage.transform.localRotation = originalRotation;
        }
        else
        {
            ButtonGridImage.transform.localRotation = Quaternion.Euler(0, 0, 90); // -180
        }
    }
    
    public void UpdateState(ButtonState state)
    {
        Sprite CurrentSprite = IdleSprite;
        switch (state)
        {
            case ButtonState.Idle:
                CurrentSprite = IdleSprite;
                break;
            case ButtonState.Water:
                CurrentSprite = WaterSprite;
                break;
            case ButtonState.Ship:
                CurrentSprite = ShipSprite;
                break;
            case ButtonState.ShipDown:
                CurrentSprite = ShipDownSprite;
                hasSink = true;
                break;
            case ButtonState.WaterDown:
                CurrentSprite = WaterDown;
                break;
            default:
                break;
        }
        
        _currentButtonState = state;
        ButtonGridImage.sprite = CurrentSprite;
    }

    public void RotateButton(bool horizontal)
    {
        if (!horizontal)
        {
            Debug.Log("Rotating to vertical");
            //ButtonGridImage.transform.localRotation = new Quaternion(0,0,0,90);
            ButtonOrientation = "v";        
        }
        else
        {
            Debug.Log("Rotating to horizontal");
            //ButtonGridImage.transform.localRotation = originalRotation;
            ButtonOrientation = "h";
        }
        
        orientationText.text = ButtonOrientation;
    }
    public ButtonState GetState()
    {
        return _currentButtonState;
    }
    public bool HasGotHiddenShip()
    {
        return hasHiddenShip;
    }
    
    public void OnSelect(BaseEventData data) 
    {
        boat.buttonCrosshair.SetActive(true);
        orientationPanel.SetActive(true);
    }
    public void OnDeselect (BaseEventData data) 
    {
        if (!isOwner)
        {
            boat.buttonCrosshair.gameObject.SetActive(false);
            orientationPanel.SetActive(false);
        }
    }
    
    public void RestoreButtonState()
    {
        if (hasSink)
            UpdateState(ButtonState.ShipDown);
        else
            UpdateState(ButtonState.Ship);
    }
    public void SetHiddenShip(bool has, bool setColor = false)
    {
        if (has)
        {
            hasHiddenShip = true;
            //UpdateState(ButtonState.Ship);
            if (setColor)
                ButtonGridImage.color = selectColor;
        }
        else
        {
            hasHiddenShip = false;
            UpdateState(ButtonState.Idle);
            if (setColor)
                ButtonGridImage.color = originalColor;
        }
    
    }
    
    public Player GetButtonOwner()
    {
        return _currentPlayer;
    }
    public void SetButtonOwner(Player playerType)
    {
        _currentPlayer = playerType;
    }
    public void SetButtonInteractable(bool state)
    {
        this.GetComponent<Button>().interactable = state;
    }
    public void SetText(string text)
    {
        this.name = text;
        ButtonAttackCoordenates = text;
        positionText.text = text;
    }

    public void OnClick()
    {
        if (GameManager.instance.GetGameState() == GameState.Player2Attacking)
        {
            InfoPanelManager.instance.SpawnInfoMessage("Please don't go to fast you sucker. Wait for player 2 to shoot");
        }
        else if(GameManager.instance.GetGameState() == GameState.Player1PlacingBoats && hasHiddenShip) // we are only able to unplace boats that have a boat in it
        {
            Debug.Log("Unplacing boat at this coordenates: " + ButtonAttackCoordenates);
            HandlePlacingBoats.instance.UnplaceBoat(ButtonAttackCoordenates);
        }
        else if (GameManager.instance.GetGameState() == GameState.Player1BoatsPlaced && !GameManager.instance.IsPlayer2Ready)
        {
            Debug.Log("Waiting for player 2!");
            InfoPanelManager.instance.SpawnInfoMessage("We are waiting for the slow player 2. YOU SUCKER camm'ooon my mum is faster with no hands");
        }
        else 
        {
            GameManager.instance.ShouldChangeTurnToBot(false, ButtonAttackCoordenates);
        }
    }
    #endregion

    #region Sound

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundManager.instance.PlaySingle(SoundManager.SoundType.BUTTON_SOUND);
    }


    #endregion

    



    
}
