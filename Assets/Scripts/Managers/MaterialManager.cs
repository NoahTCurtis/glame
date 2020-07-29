using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialManager : Manager
{
	public enum CUTOUT_SOURCE { MainMapAlpha, CustomMap, TwoCustomMaps, ThreeCustomMaps }
	public enum MAPPING { Normal, Triplanar, Screenspace }
	public enum UV_SET { UV0, UV1 }
	public enum SHAPE { Solid, Smooth, Smooth_Squared }
	public enum TEXTURE_TYPE { None, Gradient, MainMap, Custom }
	public enum NOISE_STRENGTH { CutoutSource, MainMapAlpha }
	public enum GLOBAL_CONTROL { None, MaskOnly, MaskAndEdge, All }

	public Shader dissolveShader;

	[Header("General")]
	public bool LocalSpace = true;
	public bool Invert = false;

	[Header("Cutout")]
	public CUTOUT_SOURCE COSource;
	public MAPPING COMapping;
	public float CONoise;
	public Texture COTexture;
	public Vector2 COTiling;
	public Vector2 COOffset;
	public Vector2 COScroll;
	public UV_SET CO_UVSet;

	[Header("Edge")]
	public float EdgeWidth = 0.25f;
	public SHAPE EdgeShape;
	public Color EdgeColor = Color.red;
	public float EdgeIntensity;
	public Texture EdgeTexture;
	public bool EdgeTexReverse = false;
	public bool EdgeEmission;

	[Header("Edge Misc")]
	[Range(-1f, 1f)] public float EdgeAlphaOffset;
	public float EdgePhaseOffset;
	[Range(1, 10)] public float EdgeBlur;
	public bool EdgeIsDynamic;
	public float EdgeGIMultyplier;

	[Header("UV Distortion")]
	public NOISE_STRENGTH UV_NoiseStrength;
	public float UVStrength;

	[Header("Global")]
	public GLOBAL_CONTROL GlobalControl;

	private Dictionary<Material, Material> _lookupDissolveShader = new Dictionary<Material, Material>();
	private Dictionary<Material, Material> _lookupStandardShader = new Dictionary<Material, Material>();

	public Material GetDissolveMaterial(Material mat, Transform trans)
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
			dissolveMat.name = mat.name + " (dissolve)";
			_lookupDissolveShader.Add(mat, dissolveMat);

			dissolveMat.shader = dissolveShader;
			UpdateTextureTypeKeyword(dissolveMat, TEXTURE_TYPE.Custom);
			UpdateShaderData(dissolveMat, trans);

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

	//Set thing like texture, edge width, & LocalSpace
	void UpdateShaderData(Material material, Transform trans)
	{
		Debug.Assert(material != null);

		float scale = (trans.localScale.x + trans.localScale.y + trans.localScale.z) / 3.0f;

		//Mask Shape
		material.DisableKeyword("_DISSOLVEMASK_XYZ_AXIS");
		material.DisableKeyword("_DISSOLVEMASK_PLANE");
		material.DisableKeyword("_DISSOLVEMASK_SPHERE");
		material.DisableKeyword("_DISSOLVEMASK_BOX");
		material.DisableKeyword("_DISSOLVEMASK_CONE");
		material.EnableKeyword("_DISSOLVEMASK_CYLINDER");

		//General Mask params
		material.SetFloat("_DissolveMaskSpace", LocalSpace ? 1 : 0);
		material.SetFloat("_DissolveMaskInvert", Invert ? 1 : -1);

		//Cutout
		material.SetFloat("_DissolveAlphaSource", (float)COSource); //CUTOUT_SOURCE COSource;
		material.SetFloat("_DissolveMappingType", (float)COMapping); //MAPPING COMapping;
		material.SetFloat("_DissolveNoiseStrength", CONoise); //float CONoise;
		material.SetTexture("_DissolveMap1", COTexture); //Texture COTexture;
		///material.SetVector("", ); //Vector2 COTiling;
		///material.SetVector("", ); //Vector2 COOffset;
		///material.SetVector("", ); //Vector2 COScroll;
		///material.SetFloat("", ); //UV_SET CO_UVSet;

		//Edge
		material.SetFloat("_DissolveEdgeWidth", EdgeWidth / scale);
		material.SetFloat("_DissolveEdgeShape", (int)EdgeShape);
		material.SetColor("_DissolveEdgeColor", EdgeColor);
		material.SetFloat("_DissolveEdgeColorIntensity", EdgeIntensity);
		material.SetTexture("_DissolveEdgeTexture", EdgeTexture);
		///material.SetInt("", ); //bool EdgeEmission
		material.SetFloat("_DissolveEdgeTextureReverse", EdgeTexReverse ? 1 : 0);

		//Edge Misc
		material.SetFloat("_DissolveEdgeTextureMipmap", EdgeBlur);
		material.SetFloat("_DissolveEdgeTextureAlphaOffset", EdgeAlphaOffset);
		material.SetFloat("_DissolveEdgeTexturePhaseOffset", EdgePhaseOffset);
		material.SetFloat("_DissolveEdgeTextureIsDynamic", EdgeIsDynamic ? 1 : 0);

		material.SetFloat("_DissolveGIMultiplier", EdgeGIMultyplier > 0 ? EdgeGIMultyplier : 0);
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
