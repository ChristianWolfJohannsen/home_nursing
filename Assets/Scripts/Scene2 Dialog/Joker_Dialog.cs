using UnityEngine;
using System.Collections;


public class Joker_Dialog : MonoBehaviour {
	public string[] answerButtons;
	public string[] questions;
	bool DisplayDialog = false;
	public bool QuestionAsked = false;

	public bool QuestAccepted = false;
	public bool QuestAcceptedWife = false;
	int randomQuest;
	
	
	void Start () {
		
	}
	
	void Update () {
		
	}

	void OnGUI(){


		GUILayout.BeginArea (new Rect (10, 10, 400, 400));



		if (DisplayDialog && !QuestionAsked) {



			GUILayout.Label (questions[randomQuest]);

			

			if(GUILayout.Button(answerButtons[0])){
				Debug.Log("button clicked");
				QuestionAsked = true;


			}
				
			if(GUILayout.Button (answerButtons[1])){

				QuestionAsked = true;
			}

		}

		if (DisplayDialog && QuestionAsked) {

			GUILayout.Label (questions[3]);

			if(GUILayout.Button(answerButtons[2])){
				Debug.Log("button clicked");
				DisplayDialog = false;
				QuestionAsked = false;
				QuestAccepted = true;
				
				
			}
			
			if(GUILayout.Button (answerButtons[3])){
				DisplayDialog = false;
				QuestionAsked = false;
				QuestAcceptedWife = true;
			}
		
		}


		GUILayout.EndArea ();

	}


	
	void OnTriggerEnter(){
		DisplayDialog = true;
		randomQuest = Random.Range (0, 3);

	}
	
	void OnTriggerExit(){
		DisplayDialog = false;
	}
}
