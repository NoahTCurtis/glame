using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashingPages : MonoBehaviour
{
	public List<Sprite> Pages;

	float timePerPage = 0.1f; //in seconds
	float timeSoFar = 0;

	void Update()
	{
		timeSoFar += Time.deltaTime;

		if(timeSoFar >= timePerPage)
		{
			timeSoFar -= timePerPage;

			//pick a new page
			int i = Random.Range(0, Pages.Count);
			GetComponent<Image>().sprite = Pages[i];
		}
	}
}
