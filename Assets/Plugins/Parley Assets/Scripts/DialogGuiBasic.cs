using UnityEngine;
using System.Collections.Generic;

public class DialogGuiBasic : DialogGuiAbstract {
	
	private Vector2 scrollPosition=new Vector2(0,0);
	private float conversationTextStartSec=0f;
	
	public void Start(){
		conversationTextStartSec=Time.time;
	}
	
	public void OnGUI(){
		Dialog dialog=GetDialog();
		if (dialog.dialogSkin!=null){
			GUI.skin=dialog.dialogSkin;
		}
		
		float svw=dialog.dialogSize.x;
		float svh=dialog.dialogSize.y;
		Rect windowRect=new Rect((Screen.width-svw)/2,(Screen.height-svh)/2,svw,svh);;
		Texture portrait=dialog.dialogPortrait;
		
		// Draw Window
		GUILayout.BeginArea(windowRect,dialog.description,GUI.skin.window);
		GUILayout.BeginVertical();
		if (portrait!=null){
			GUILayout.BeginHorizontal();
	        GUILayout.Label(portrait,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(false)});
		}
        DrawText(dialog);
		if (portrait!=null){
			GUILayout.EndHorizontal();
		}

		// Buttons
		DrawButtons(dialog);
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	private bool DrawText(Dialog dialog){
		string conversationText=GetConversationText();
		
		int textPos=(int)((Time.time-conversationTextStartSec)*dialog.charactersPerSecond);
		textPos=Mathf.Min(conversationText.Length,textPos);
		scrollPosition=GUILayout.BeginScrollView(scrollPosition,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
        GUILayout.Label(conversationText.Substring(0,textPos),new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
		GUILayout.EndScrollView();
		
		return textPos==conversationText.Length;
	}
	
	private void DrawButtons(Dialog dialog){
		List<Option> options=GetCurrentConversationOptions();
    	foreach (Option o in options){
    		if (o._available){
	  			if (GUILayout.Button(o.displaytext)) {
		  			SelectOption(o);
					conversationTextStartSec=Time.time;
		  		}
		  	}
    	}
    	
    	if (HasReturnConversation()){
  			if (GUILayout.Button("Lets talk about something else.")) {
  				GotoReturnConversation();
				conversationTextStartSec=Time.time;
	  		}
    	}else{
  			if (GUILayout.Button("Bye.")) {
  				EndDialog();
	  		}
	  	}
	}
}
