using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour {

    public static MusicManager instance;

    public AudioClip[] music;
    private AudioSource audioSource;

	// Use this for initialization
	void Awake () {
        if (!instance) {
            instance = this;
        } else {
            Destroy(gameObject);
        }

        audioSource = instance.GetComponent<AudioSource>();

		DontDestroyOnLoad(gameObject);
	}
	
    public void PlayMusic () {
        audioSource.clip = music[Random.Range(0, music.Length)];
        audioSource.Play();
    }

    public void StopMusic () {
        audioSource.Stop();
    }
}
