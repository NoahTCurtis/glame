using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SplashControl : MonoBehaviour
{
	public GameObject toha;
	public GameObject vdo;
	public GameObject blame;
	public GameObject text1;
	public GameObject text2;

	public Image FillBar;
	private float charge = 0;

	void Start()
	{
		toha.SetActive(true);
		vdo.SetActive(true);
		blame.SetActive(true);
		text1.SetActive(true);
		text2.SetActive(true);
		StartCoroutine(introAnim());
	}
	
	IEnumerator introAnim()
	{
		yield return new WaitForSeconds(8.0f);
		toha.SetActive(false);
		yield return new WaitForSeconds(4.5f);
		vdo.SetActive(false);
		yield return new WaitForSeconds(7);
		blame.SetActive(false);

		while (Input.anyKeyDown == false) yield return null;
		text1.SetActive(false);

		while (true)
		{
			Debug.Log("waiting for charge");
			float secondsItTakesToCharge = 1.75f;
			float secondsItTakesToUncharge = 0.75f;
			if (Input.GetMouseButton(1))
			{
				charge += Time.deltaTime / secondsItTakesToCharge;
			}
			else
			{
				charge -= Time.deltaTime / secondsItTakesToUncharge;
			}
			charge = Mathf.Clamp(charge, 0, 1);

			FillBar.fillAmount = charge;

			if (Input.GetMouseButtonDown(0) && charge > 0.99f)
				break;

			yield return null;
		}

		SceneManager.LoadScene("TestScenePlsIgnore", LoadSceneMode.Single);
	}
}
