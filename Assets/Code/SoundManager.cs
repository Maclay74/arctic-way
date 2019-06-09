using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {

	public static SoundManager Instance;
	public  AudioSource SoundSource;
	public AudioSource MusicSource;
    
	public List<AudioClip> clips;

	public AudioClip Music;
    
	// Start is called before the first frame update
	void Start() {
		if (Instance) {
			Destroy(gameObject);
			return;
		}
		
		Instance = this;

		MusicSource.clip = Music;
		MusicSource.Play();
		
		DontDestroyOnLoad(gameObject);
	}
    
	public static void PlaySound(int id) {
		Instance.SoundSource.clip = Instance.clips[id];
		Instance.SoundSource.Play();
	}
	
}