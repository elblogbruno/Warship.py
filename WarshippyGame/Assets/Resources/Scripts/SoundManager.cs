using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioSource musicSource;                 //Drag a reference to the audio source which will play the music.
    public static SoundManager instance = null;     //Allows other scripts to call functions from SoundManager.             
    //public float lowPitchRange = .95f;              //The lowest a sound effect will be randomly pitched.
    //public float highPitchRange = 1.05f;            //The highest a sound effect will be randomly pitched.
    public List<AudioClip> ListOfAudios = new List<AudioClip>();
    public enum SoundType
    {
        BUTTON_SOUND = 0,
        BACKGROUND_PLAY_MUSIC = 1,
    };
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

        //Set SoundManager to DontDestroyOnLoad so that it won't be destroyed when reloading our scene.
        //DontDestroyOnLoad(gameObject);
    }


    //Used to play single sound clips.
    public void PlaySingle(SoundType id)
    {
       // Debug.Log("PLAYING SOUND WITH ID " + (int)id);
        //Set the clip of our efxSource audio source to the clip passed in as a parameter.
        musicSource.clip = ListOfAudios[(int)id];

        //Play the clip.
        musicSource.Play();
    }
}
