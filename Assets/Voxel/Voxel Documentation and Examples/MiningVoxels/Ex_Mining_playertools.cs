using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum MiningExampleToolType{
	Hands,
	Pickaxe,
	Shovel
}
public class Ex_Mining_playertools : MonoBehaviour {
	public static MiningExampleToolType tool;
	public Text toolReadout;
	void Awake(){
		tool = MiningExampleToolType.Hands;
		if(toolReadout){
			toolReadout.text = "Tool: "+tool.ToString()+ "\n No multipliers.";
		}
	}
	void Update(){
		if(Input.GetKeyDown("1")){
			tool = MiningExampleToolType.Hands;
			if(toolReadout){
				toolReadout.text = "Tool: "+tool.ToString()+ "\n No multipliers.";
			}
		}
		if(Input.GetKeyDown("2")){
			tool = MiningExampleToolType.Pickaxe;
			if(toolReadout){
				toolReadout.text = "Tool: "+tool.ToString()+ "\n Uses PickMult attribute.";
			}
		}
		if(Input.GetKeyDown("3")){
			tool = MiningExampleToolType.Shovel;
			if(toolReadout){
				toolReadout.text = "Tool: "+tool.ToString()+ "\n Uses ShovelMult attribute.";
			}
		}
	}
}