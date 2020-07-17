using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : Manager
{
	public enum TEXTURE_TYPE { None, Gradient, MainMap, Custom }
	public enum SHAPE { Solid, Smooth, Smooth_Squared }

	public Shader dissolveShader;
	
	[Header("Dissolve Settings")]
	[Space(10)]
	public float width = 0.25f;
	public SHAPE shape;
	public Color color = Color.green;
	public float intensity;

	[Space(10)]
	public Texture texture;
	public bool reverse;
	[Range(-1f, 1f)]
	public float alphaOffset;
	public float phaseOffset;
	[Range(1, 10)]
	public float blur;
	public bool isDynamic;

	[Space(10)]
	public float GIMultyplier;

	private Dictionary<Material, Material> _lookupDissolveShader = new Dictionary<Material, Material>();
	private Dictionary<Material, Material> _lookupStandardShader = new Dictionary<Material, Material>();

	public Material GetDissolveMaterial(Material mat)
	{
		if (_lookupDissolveShader.ContainsKey(mat))
		{
			return _lookupDissolveShader[mat];
		}
		/*else if(_lookupStandardShader.ContainsKey(mat))
		{
			return mat; //Looks like it's already a dissolve mat
		}*/
		else
		{
			Material dissolveMat = new Material(mat);
			_lookupDissolveShader.Add(mat, dissolveMat);

			dissolveMat.shader = dissolveShader;
			UpdateTextureTypeKeyword(dissolveMat, TEXTURE_TYPE.Custom);
			UpdateShaderData(dissolveMat);

			return dissolveMat;
		}
	}

	public Material GetStandardMaterial(Material mat)
	{
		if(_lookupStandardShader.ContainsKey(mat))
		{
			return _lookupStandardShader[mat];
		}
		/*else if(_lookupDissolveShader.ContainsKey(mat))
		{
			return mat; //It's actually a standard mat already
		}*/
		else
		{
			Debug.LogWarning("MaterialManager.GetStandardShader(): This material doesn't have a standard counterpart");
			return mat;
		}
	}

	void UpdateShaderData(Material material)
	{
		Debug.Assert(material != null);

		material.SetFloat("_DissolveEdgeWidth", width);
		material.SetFloat("_DissolveEdgeShape", (int)shape);
		material.SetColor("_DissolveEdgeColor", color);
		material.SetFloat("_DissolveEdgeColorIntensity", intensity);

		material.SetTexture("_DissolveEdgeTexture", texture);
		material.SetFloat("_DissolveEdgeTextureReverse", reverse ? 1 : 0);
		material.SetFloat("_DissolveEdgeTextureMipmap", blur);
		material.SetFloat("_DissolveEdgeTextureAlphaOffset", alphaOffset);
		material.SetFloat("_DissolveEdgeTexturePhaseOffset", phaseOffset);
		material.SetFloat("_DissolveEdgeTextureIsDynamic", isDynamic ? 1 : 0);

		material.SetFloat("_DissolveGIMultiplier", GIMultyplier > 0 ? GIMultyplier : 0);
	}

	public void UpdateTextureTypeKeyword(Material material, TEXTURE_TYPE textureType)
	{
		Debug.Assert(material != null);

		//Enable proper texture type keyword
		material.DisableKeyword("_DISSOLVEEDGETEXTURESOURCE_GRADIENT");
		material.DisableKeyword("_DISSOLVEEDGETEXTURESOURCE_MAIN_MAP");
		material.DisableKeyword("_DISSOLVEEDGETEXTURESOURCE_CUSTOM");

		switch (textureType)
		{
			case TEXTURE_TYPE.None:
					//For material editor to select proper name inside dropdown
					material.SetFloat("_DissolveEdgeTextureSource", 0);
				break;

			case TEXTURE_TYPE.Gradient:
					material.EnableKeyword("_DISSOLVEEDGETEXTURESOURCE_GRADIENT");

					//For material editor to select proper name inside dropdown
					material.SetFloat("_DissolveEdgeTextureSource", 1);
				break;

			case TEXTURE_TYPE.MainMap:
					material.EnableKeyword("_DISSOLVEEDGETEXTURESOURCE_MAIN_MAP");

					//For material editor to select proper name inside dropdown
					material.SetFloat("_DissolveEdgeTextureSource", 2);
				break;

			case TEXTURE_TYPE.Custom:
					material.EnableKeyword("_DISSOLVEEDGETEXTURESOURCE_CUSTOM");

					//For material editor to select proper name inside dropdown
					material.SetFloat("_DissolveEdgeTextureSource", 3);
				break;
		}
	}
}
