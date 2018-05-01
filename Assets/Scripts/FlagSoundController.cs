using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagSoundController : MonoBehaviour {

    // Use this for initialization
    public static FlagSoundController acInstance;
    public AudioSource myAudioSource;
    public AudioClip taken;
    public AudioClip captured;
    public AudioClip retaken;
    void Awake () {
		if(acInstance == null)
        {
            acInstance = this;
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    public void PlayTakenSound()
    {
        myAudioSource.PlayOneShot(taken);
    }

    public void PlayCapturedSound()
    {
        myAudioSource.PlayOneShot(captured);
    }

    public void PlayRetakenSound()
    {
        myAudioSource.PlayOneShot(retaken);
    }


}
