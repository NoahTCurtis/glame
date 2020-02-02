using UnityEngine;
using System.Collections;

namespace Zarenikit.Core{
	/// <summary>
	/// Represents a texture atlas.
	/// </summary>
	public class TextureAtlas{
		public Texture2D atlas;
		public Rect[] uvs;
	}
	/// <summary>
	/// Performs useful texture optimizations
	/// </summary>
	public static class TextureManager{
		/// <summary>
		/// Makes an atlas out of the given textures.
		/// </summary>
		/// <returns>The texture atlas and coordinates.</returns>
		/// <param name="source">The Source Textures</param>
		public static TextureAtlas MakeAtlas(Texture2D[] source, int padding = 0){
			int atlasx =0;
			int atlasy =0;
			foreach(Texture2D tex in source){
				atlasx +=tex.width+padding;
				atlasy +=tex.height+padding;
			}
			Texture2D atlas = new Texture2D(atlasx,atlasy,TextureFormat.ARGB32,true);
			Rect[] uvs;
			TextureAtlas o = new TextureAtlas();
			uvs = atlas.PackTextures(source,padding,2048,true);
			//atlas.Apply();
			o.atlas = atlas;
			o.uvs = uvs;
			return o;
		}
		/// <summary>
		/// Makes an atlas out of the given textures.
		/// </summary>
		/// <returns>The texture atlas and coordinates.</returns>
		/// <param name="source">The Source Textures.</param>
		/// <param name="def">The default texture to use if a texture is null.</param>
		public static TextureAtlas MakeAtlas(Texture2D[] source, Texture2D def, int padding = 0){
			int atlasx =0;
			int atlasy =0;
			for(int t =0; t<source.Length; t++){
				if(source[t] == null){
					source[t] = def;
				}
			}
			foreach(Texture2D tex in source){
				atlasx +=tex.width+padding;
				atlasy +=tex.height+padding;
			}
			Texture2D atlas = new Texture2D(atlasx,atlasy);
			Rect[] uvs;
			TextureAtlas o = new TextureAtlas();
			uvs = atlas.PackTextures(source,padding,2048,true);
			//atlas.Apply();
			o.atlas = atlas;
			o.uvs = uvs;
			return o;
		}
	}
}