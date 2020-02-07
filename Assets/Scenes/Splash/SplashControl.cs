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
	public GameObject text3;

	public AudioSource audioSource;
	public AudioClip endClip;

	public RectTransform beamImage;

	public Image FillBar;
	private float charge = 0;

	void Start()
	{
		toha.SetActive(true);
		vdo.SetActive(true);
		blame.SetActive(true);
		text1.SetActive(true);
		text2.SetActive(true);
		text3.SetActive(true);
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

		yield return new WaitForSeconds(0.5f);
		while (Input.anyKeyDown == false) yield return null;
		text1.SetActive(false);

		yield return new WaitForSeconds(0.5f);
		while (Input.anyKeyDown == false) yield return null;
		text2.SetActive(false);

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

		text3.SetActive(false);

		audioSource.Stop();
		audioSource.clip = endClip;
		audioSource.Play();

		beamImage.sizeDelta = Vector2.one * Mathf.Max(Screen.width, Screen.height);

		float startTime = Time.unscaledTime;
		float duration = 1;
		for (float t = startTime; t < startTime + duration; t = Time.unscaledTime)
		{
			float T = Mathf.InverseLerp(startTime, startTime + duration, t);
			beamImage.sizeDelta = Vector2.one * Mathf.Max(Screen.width, Screen.height) * (1.0f - T);
			yield return null;
		}
		beamImage.gameObject.SetActive(false);

		yield return new WaitForSeconds(audioSource.clip.length - duration);

		SceneManager.LoadScene("TestScenePlsIgnore", LoadSceneMode.Single);
	}
}
