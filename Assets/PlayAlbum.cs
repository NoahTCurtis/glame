using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAlbum : MonoBehaviour
{
	public List<AudioClip> songs;

	void Start()
	{
		StartCoroutine(playAlbum());
	}
	
	IEnumerator playAlbum()
	{
		int i = 0;
		AudioSource audio = GetComponent<AudioSource>();

		while (true)
		{
			AudioClip nextClip = songs[i];
			i++;
			i = (int)Mathf.Repeat(i, songs.Count);

			audio.clip = nextClip;
			audio.Play();
			yield return new WaitForSeconds(audio.clip.length);
		}
	}
}
