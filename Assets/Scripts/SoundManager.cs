using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

    public AudioSource efxSource;
    public AudioSource MusicSource;
    public static SoundManager Instance = null;

    public float LowPitchRange = 0.95f;
    public float HighPitchRange = 1.05f;

	// Use this for initialization
	void Awake () {
		if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

	}


    public void PlaySingle(AudioClip Clip)
    {
        efxSource.clip = Clip;
        efxSource.Play();
    }

    public void RandomiseSFX(params AudioClip[] Clips)
    {
        int RandomIndex = Random.Range(0, Clips.Length);
        float RandomPitch = Random.Range(LowPitchRange, HighPitchRange);

        efxSource.pitch = RandomPitch;
        efxSource.clip = Clips[RandomIndex];
        efxSource.Play();
    }


}
