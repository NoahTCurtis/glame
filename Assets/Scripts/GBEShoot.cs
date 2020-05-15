using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class GBEShoot : MonoBehaviour
{
	void Update()
	{
		if (transform.position.y < -100)
			Respawn();

		//THIS MIGHT AS WELL GO HERE
		if (Input.GetKeyDown(KeyCode.R))
		{
			Respawn();
		}
	}

	void Respawn()
	{
		int scene = SceneManager.GetActiveScene().buildIndex;
		SceneManager.LoadScene(scene, LoadSceneMode.Single);
	}
}
