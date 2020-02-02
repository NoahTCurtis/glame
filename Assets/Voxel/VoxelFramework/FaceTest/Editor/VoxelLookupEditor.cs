using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Zarenikit.Voxels.FaceTest;
using Zarenikit.Core;
using ZKAPI;

public class VoxelLookupEditor : EditorWindow {
	string tablename = "Lookuptable";
	VoxelLookup table;
	List<VoxelType> temp;
	List<Texture2D> temptex;
	bool tableLock = false;
	int index =0;
	bool showVoxels = false;
	Vector2 scroll = Vector2.zero;
	Vector2 scrollfloats = Vector2.zero;
	Vector2 scrollbools = Vector2.zero;
	List<string> tfloatnames;
	List<string> tboolnames;
	


	[MenuItem("Tools/ZareniKit/Voxel Lookup Editor")]
	public static void ShowWindow () {
		EditorWindow.GetWindow(typeof(VoxelLookupEditor));
	}

	void OnGUI(){
		if(!tableLock){
			GUILayout.Label("1. Select an existing lookup table:", EditorStyles.boldLabel);
			table = (VoxelLookup)EditorGUILayout.ObjectField(table,typeof(VoxelLookup),false);
			EditorGUILayout.Separator();
			GUILayout.Label("Or create new lookup table:", EditorStyles.boldLabel);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Name: ");
			tablename = GUILayout.TextField(tablename);
			GUILayout.EndHorizontal();
			if(GUILayout.Button("Create Lookup Table")){
				if(AssetDatabase.LoadAssetAtPath("Assets/"+tablename+".asset",typeof(VoxelLookup)) == null){
					table = ScriptableObject.CreateInstance<VoxelLookup>();
					table.name = tablename;
					table.types = new VoxelType[0];
					table.floatAttribNames = new string[0];
					table.boolAttribNames = new string[0];
					table.sources = new Texture2D[0];
					AssetDatabase.CreateAsset(table,"Assets/"+tablename+".asset");
				}else{
					if(!EditorUtility.DisplayDialog("Overwrite?","A lookup table already exists with this name. Overwrite the original?","No, just select it.","Yes. (Will clear original data!)")){
						table = ScriptableObject.CreateInstance<VoxelLookup>();
						table.name = tablename;
						table.types = new VoxelType[0];
						table.floatAttribNames = new string[0];
						table.boolAttribNames = new string[0];
						table.sources = new Texture2D[0];
						AssetDatabase.CreateAsset(table,"Assets/"+tablename+".asset");
					}else{
						table = (VoxelLookup)AssetDatabase.LoadAssetAtPath("Assets/"+tablename+".asset",typeof(VoxelLookup));
					}
				}
			}
			EditorGUILayout.Separator();
			if(table != null){ 
				GUILayout.Label("2. Open the table for editing:", EditorStyles.boldLabel);
				if(GUILayout.Button("Open Table")){
					tablename = table.name;
					temp = table.types.ToList();
					temptex = table.sources.ToList();
					tfloatnames = table.floatAttribNames.ToList();
					tboolnames = table.boolAttribNames.ToList();
					tableLock = true;
					index = temp.Count-1;
				}
			}
		}else{
			GUILayout.Label("VOXEL TYPE EDITOR",EditorStyles.boldLabel);
			EditorGUILayout.Separator();
			GUILayout.Label("Table: "+table.name,EditorStyles.miniLabel);
			if(temp != null){
				GUILayout.Label("Types: "+temp.Count,EditorStyles.miniLabel);
			}else{
				GUILayout.Label("Types: 0",EditorStyles.miniLabel);
			}
			EditorGUILayout.Separator();
			if(index > -1){
				GUILayout.BeginHorizontal();
				if(index==0){
					GUI.color = Color.grey;
					GUILayout.Button("|<");
					GUI.color = Color.white;
				}else{
					if(GUILayout.Button("<")){
						index--;
					}
				}
				index=Mathf.Clamp(EditorGUILayout.IntField(index),0,temp.Count-1);
				if(index==temp.Count-1){
					GUI.color = Color.green;
					if(GUILayout.Button("+")){
						temp.Add(new VoxelType());
						temptex.Add(null);
						index++;
						for(int a =0; a<tfloatnames.Count; a++){
							temp[index].floatAttribs.Add(0f);
						}
						for(int a =0; a<tboolnames.Count; a++){
							temp[index].boolAttribs.Add(false);
						}
					}
					GUI.color = Color.white;
				}else{
					if(GUILayout.Button(">")){
						index++;
					}
				}
				GUILayout.EndHorizontal();
			}else{
				GUILayout.Label("3. Add Some Voxel Types:", EditorStyles.boldLabel);
				GUI.color = Color.green;
				if(GUILayout.Button("Start Editing")){
					temp.Add(new VoxelType());
					temptex.Add(null);
					index = 0;
					for(int a =0; a<tfloatnames.Count; a++){
						temp[index].floatAttribs.Add(0f);
					}
					for(int a =0; a<tboolnames.Count; a++){
						temp[index].boolAttribs.Add(false);
					}
				}
				GUI.color = Color.white;
			}

			if(index>-1){
				temp[index].name = EditorGUILayout.TextField("Name: ",temp[index].name);
				temp[index].visible = EditorGUILayout.ToggleLeft("Visible ",temp[index].visible);
				temp[index].trans = EditorGUILayout.ToggleLeft("Transparent ",temp[index].trans);
				if(temp[index].visible && temp[index].trans){
					temp[index].doubleside = EditorGUILayout.ToggleLeft("Double Sided ",temp[index].doubleside);
				}
				temp[index].solid = EditorGUILayout.ToggleLeft("Solid ",temp[index].solid);
				//temp[index].color = EditorGUILayout.ColorField("Color: ", temp[index].color);
				temptex[index] = (Texture2D)EditorGUILayout.ObjectField("Texture: ",temptex[index],typeof(Texture2D),false);
				GUILayout.Label ("VOXEL TYPE ATTRIBUTES",EditorStyles.largeLabel);
				GUILayout.BeginHorizontal();
				if(GUILayout.Button("ADD FLOAT")){
					tfloatnames.Add("Unnamed");
					for(int v =0;v <temp.Count;v++){
						temp[v].floatAttribs.Add(0f);
					}
				}
				if(GUILayout.Button("ADD BOOL")){
					tboolnames.Add("Unnamed");
					for(int v =0;v <temp.Count;v++){
						temp[v].boolAttribs.Add(false);
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				scrollfloats = EditorGUILayout.BeginScrollView(scrollfloats,GUIStyle.none,GUI.skin.verticalScrollbar);
				if(temp[index].floatAttribs.Count>0){
					for(int a =0; a<tfloatnames.Count; a++){
						GUILayout.BeginHorizontal();
						GUILayout.Label(a+"- ");
						tfloatnames[a] = EditorGUILayout.TextField(tfloatnames[a]);
						temp[index].floatAttribs[a] = EditorGUILayout.FloatField(temp[index].floatAttribs[a]);
						GUI.color = Color.red;
						if(GUILayout.Button("X")){
							tfloatnames.RemoveAt(a);
							for(int v =0;v <temp.Count;v++){
								temp[v].floatAttribs.RemoveAt(a);
							}
						}
						GUI.color = Color.white;
						GUILayout.EndHorizontal();
					}
				}
				EditorGUILayout.EndScrollView();
				GUILayout.FlexibleSpace();
				scrollbools = EditorGUILayout.BeginScrollView(scrollbools,GUIStyle.none,GUI.skin.verticalScrollbar);
				if(temp[index].boolAttribs.Count>0){
					for(int a =0; a<tboolnames.Count; a++){
						GUILayout.BeginHorizontal();
						GUILayout.Label(a+"- ");
						tboolnames[a] = EditorGUILayout.TextField(tboolnames[a]);
						temp[index].boolAttribs[a] = EditorGUILayout.Toggle(temp[index].boolAttribs[a]);
						GUI.color = Color.red;
						if(GUILayout.Button("X")){
							tboolnames.RemoveAt(a);
							for(int v =0;v <temp.Count;v++){
								temp[v].boolAttribs.RemoveAt(a);
							}
						}
						GUI.color = Color.white;
						GUILayout.EndHorizontal();
					}
				}
				GUILayout.EndHorizontal();
				EditorGUILayout.EndScrollView();
				EditorGUILayout.Separator();
				GUILayout.Label("VOXEL INSTANCE ATTRIBUTES",EditorStyles.largeLabel);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Floats: "+temp[index].instfloats);
				if(GUILayout.Button("+")){
					temp[index].instfloats++;
				}
				if(temp[index].instfloats>0){
					if(GUILayout.Button("-")){
						temp[index].instfloats--;
					}
				}
				EditorGUILayout.Separator();
				GUILayout.Label("Booleans: "+temp[index].instbools);
				if(GUILayout.Button("+")){
					temp[index].instbools++;
				}
				if(temp[index].instbools>0){
					if(GUILayout.Button("-")){
						temp[index].instbools--;
					}
				};
				GUILayout.EndHorizontal();
				EditorGUILayout.Separator();
				GUI.color = Color.red;
				if(GUILayout.Button("DELETE VOXEL TYPE")){
					if(temp[index].name == "UNNAMED_VOXEL"){
						temp.RemoveAt(index);
						temptex.RemoveAt(index);
					}else{
						if(EditorUtility.DisplayDialog("Delete this VoxelType?","Are you sure you would like to delete this voxel type? \n WARNING: This will shift type id numbers!","Delete","Keep")){
							temp.RemoveAt(index);
						}
					}
				}
				GUI.color = Color.white;
			}
			EditorGUILayout.Separator();
			showVoxels = EditorGUILayout.Foldout(showVoxels, "Voxels", EditorStyles.foldout);
			if(showVoxels){
				scroll = EditorGUILayout.BeginScrollView(scroll);
				for(int i =0; i< temp.Count; i++){
					if(GUILayout.Button(">"+i+"- "+temp[i].name, EditorStyles.miniButtonLeft)){
						index = i;
					}
				}
				EditorGUILayout.EndScrollView();
			}
			EditorGUILayout.Separator();
			GUILayout.FlexibleSpace();
			GUI.color = Color.green;
			if(GUILayout.Button("Save Table")){
				TextureAtlas atlas = TextureManager.MakeAtlas(temptex.ToArray(),new Texture2D(1,1,TextureFormat.DXT5,false),4);
				for(int t =0; t < temp.Count; t++){
					temp[t].texCoords = atlas.uvs[t];
				}
				Undo.RecordObject(table,"Save Lookup Table");
				AssetDatabase.CreateAsset(atlas.atlas,"Assets/"+tablename+"_atlas.asset");
				AssetDatabase.StartAssetEditing();
				table.boolAttribNames = tboolnames.ToArray();
				table.floatAttribNames = tfloatnames.ToArray();
				table.types = temp.ToArray();
				table.sources = temptex.ToArray();
				table.atlas = atlas.atlas;
				AssetDatabase.StopAssetEditing();
				EditorUtility.SetDirty(table);
				AssetDatabase.SaveAssets();
				tableLock = false;
			}
			GUI.color = Color.white;
		}
	}
}
