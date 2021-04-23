using System.Collections.Generic;
using UnityEngine;

public class GameResources : MonoBehaviour
{
    public List<Sprite> CurrentAvailableBoats;
    
    public static GameResources Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    
}
