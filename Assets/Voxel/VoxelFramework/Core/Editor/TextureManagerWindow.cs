using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Zarenikit.Core;
/*
public class TextureManagerWindow : EditorWindow {
	int index = 0;
	TextureAtlas atlas;
	List<Texture2D> sources = new List<Texture2D>();

	[MenuItem("ZareniKit/Texture Atlaser")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(TextureManagerWindow));
	}

	void OnGUI(){
		GUILayout.Label("TEXTURE ATLAS CREATOR",EditorStyles.boldLabel);
		EditorGUILayout.Separator();
		GUILayout.BeginHorizontal();
		if(sources.Count > 0){
			if(GUILayout.Button("<")){
				index = Mathf.Clamp(index-1,0,sources.Count-1);
			}
			if(GUILayout.Button(">")){
				index = Mathf.Clamp(index+1,0,sources.Count-1);
			}
			if(GUILayout.Button("+")){
				sources.Add(Texture2D.whiteTexture);
				index = sources.Count-1;
			}
		}else{
			sources.Add(null);
		}
		GUILayout.EndHorizontal();
		sources[index] = (Texture2D)EditorGUILayout.ObjectField(sources[index], typeof(Texture2D), false);
		if(GUILayout.Button("CREATE ATLAS")){
			atlas = TextureManager.MakeAtlas(sources.ToArray());
		}
		if(atlas == null){
			GUILayout.Label ("NO ATLAS GENERATED");
		}else{
			GUILayout.Label("Atlas Generated Successfully",EditorStyles.miniLabel);
			GUILayout.Label("Size: "+atlas.atlas.width+"X"+atlas.atlas.height,EditorStyles.miniLabel);
			GUILayout.Label("Textures: "+atlas.uvs.Length,EditorStyles.miniLabel);
			GUILayout.Label(atlas.atlas,GUILayout.Width(128),GUILayout.Height(128));

		}
	}
}
*/