using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.SceneManagement;

public class Game : MonoBehaviour
{
	public static Game Instance { get { return _instance; } }
	public static void Save() { _instance.SaveGame(); }
	public static void Load() { _instance.LoadGame(); }
	public static T Manager<T>() where T : Manager { return _instance?.GetManager<T>(); }
	public static void Register(Manager manager, bool doNotDestroy) { _instance.RegisterManager(manager, doNotDestroy); }
	public static void Unregister(Manager manager) { _instance.UnregisterManager(manager); }

	protected static Game _instance;

	private Dictionary<System.Type, Manager> _managers = new Dictionary<System.Type, Manager>();

	private GameObject _dontDestroyManagerRoot;
	public bool asyncLoadedMapScene = false;

	public T GetManager<T>() where T : Manager
	{
		System.Type t = typeof(T);
		if (_managers.ContainsKey(t))
		{
			return _managers[typeof(T)] as T;
		}
		else
		{
			return null;
		}
	}

	public void SaveGame()
	{

	}

	public void LoadGame()
	{

	}

	public IEnumerator LoadGameScene(string stateToReturnTo)
	{
		/// Leaving this so we can see the loading screen
		//yield return new WaitForSeconds(2.0f); /// XXX
		/*​
		Cleanup();

		AsyncOperation ao = SceneManager.LoadSceneAsync("Game");
		ao.allowSceneActivation = false;
		while (!ao.isDone)
		{
			if (ao.progress < 0.9f)
			{
				ao.allowSceneActivation = true;
			}
			yield return null;
		}
		*/
		yield return null; //remove this line
	}

	public IEnumerator LoadSceneSelect()
	{
		AsyncOperation ao = SceneManager.LoadSceneAsync("SceneSelector");
		ao.allowSceneActivation = false;
		while (!ao.isDone)
		{
			if (ao.progress < 0.9f)
			{
				ao.allowSceneActivation = true;
			}
			yield return null;
		}

		Cleanup();
	}

	public void GoBackToSceneSelect()
	{
		StartCoroutine("LoadSceneSelect");
	}


	public void ReloadLastSave()
	{

	}

	public void NewGame()
	{

	}

	public void Cleanup()
	{
		_managers.Clear();
		Destroy(_dontDestroyManagerRoot);
		_dontDestroyManagerRoot = null;
		InitPersistentManagerObject();
	}


	[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
	private static void Create()
	{
		if (_instance != null)
		{
			return; // already created
		}

		Debug.Assert(
			condition: FindObjectOfType<Game>() == null,
			message: "Game type does not support being added via Editor");

		GameObject obj = new GameObject();
		DontDestroyOnLoad(obj);
		obj.name = typeof(Game).Name;
		_instance = obj.AddComponent<Game>();

		_instance.OnAppInit();
	}

	private static void SetGameViewScale()
	{
#if UNITY_EDITOR
		System.Reflection.Assembly assembly = typeof(UnityEditor.EditorWindow).Assembly;
		System.Type type = assembly.GetType("UnityEditor.GameView");
		UnityEditor.EditorWindow v = UnityEditor.EditorWindow.GetWindow(type);

		var defScaleField = type.GetField("m_defaultScale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

		//whatever scale you want when you click on play
		float defaultScale = 1.5f;

		var areaField = type.GetField("m_ZoomArea", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		var areaObj = areaField.GetValue(v);

		var scaleField = areaObj.GetType().GetField("m_Scale", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
		scaleField.SetValue(areaObj, new Vector2(defaultScale, defaultScale));
#endif
	}

	private void OnAppInit()
	{
		Debug.Assert(Game.Instance != null);
		Debug.Log("Game.OnAppInit");
		
		InitPersistentManagerObject();

		Application.targetFrameRate = 60;

		#if UNITY_EDITOR
			SetGameViewScale();
#endif
	}

	private void InitPersistentManagerObject()
	{
		if (_dontDestroyManagerRoot == null)
		{
			_dontDestroyManagerRoot = new GameObject("PersistentManagers");
			DontDestroyOnLoad(_dontDestroyManagerRoot);
		}
	}

	private void RegisterManager(Manager manager, bool doNotDestroy)
	{
		var t = manager.GetType();

		if (_managers.ContainsKey(t))
		{
			Destroy(manager.gameObject);
		}
		else
		{
			_managers[t] = manager;
			if (doNotDestroy)
			{
				manager.transform.parent = _dontDestroyManagerRoot.transform;
			}
		}
	}

	private void UnregisterManager(Manager manager)
	{
		var type = manager.GetType();
		if (_managers.ContainsKey(type) && manager == _managers[type])
			_managers.Remove(manager.GetType());
		else
			Debug.Log($">>> cannot unregister manager {type}");
	}

	private void Start()
	{
		//------------------------------------------------------------
		// NOTE: USE THIS AREA FOR THINGS WHICH SHOULD NOT
		//       BE INITIALIZED BEFORE THE SCENE LOADS
		//------------------------------------------------------------
	}

}