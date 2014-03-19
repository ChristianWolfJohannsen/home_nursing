using UnityEngine;
using System.Collections;

public class Harley_Dialog : MonoBehaviour {
	public string[] answerButtons;
	public string[] questions;
	bool DisplayDialog = false;
	bool ActivateQuest = false;
	
	
	void Start () {
		
	}
	
	void Update () {
		
	}
	
	void OnGUI(){
		GUILayout.BeginArea (new Rect(70, 60, 400, 400));
		if (DisplayDialog && !ActivateQuest) {
			
			
			GUILayout.Label (questions [0]);
			GUILayout.Label (questions [1]);
			if (GUILayout.Button (answerButtons [0])) {
				ActivateQuest = true;
				
				DisplayDialog = false;
				
			}
			
			if (GUILayout.Button (answerButtons [1])) {
				DisplayDialog = false;	
			}
		}
		
		if(DisplayDialog && ActivateQuest){
			GUILayout.Label(questions[2]);
			if(GUILayout.Button(answerButtons[2])){
				DisplayDialog = false;
			}
		}
		
		GUILayout.EndArea ();
		
		
	}
	
	void OnTriggerEnter(){
		DisplayDialog = true;
	}
	
	void OnTriggerExit(){
		DisplayDialog = false;
	}
}
