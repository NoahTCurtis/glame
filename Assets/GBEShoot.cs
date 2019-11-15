using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GBEShoot : MonoBehaviour
{
	void Start()
	{
		
	}

	void Update()
	{
		//THIS MIGHT AS WELL GO HERE
		if(Input.GetKeyDown(KeyCode.R))
		{
			int scene = SceneManager.GetActiveScene().buildIndex;
			SceneManager.LoadScene(scene, LoadSceneMode.Single);
		}
	}
}
