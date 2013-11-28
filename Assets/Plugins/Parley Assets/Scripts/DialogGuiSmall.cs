using UnityEngine;
using System.Collections.Generic;

public class DialogGuiSmall : DialogGuiAbstract {
	
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
		Rect windowRect=new Rect((Screen.width)/2 - svw/2,(Screen.height)/2 - svh/2,svw,svh);
		Texture portrait=dialog.dialogPortrait;
		
		// Draw Window
		GUILayout.BeginArea(windowRect,GUI.skin.window);
		scrollPosition=GUILayout.BeginScrollView(scrollPosition,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
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
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	private bool DrawText(Dialog dialog){
		string conversationText=GetConversationText();
		
		int textPos=(int)((Time.time-conversationTextStartSec)*dialog.charactersPerSecond);
		textPos=Mathf.Min(conversationText.Length,textPos);
        GUILayout.Label(conversationText.Substring(0,textPos),new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
		
		return textPos==conversationText.Length;
	}
	
	private void DrawButtons(Dialog dialog){
		List<Option> options=GetCurrentConversationOptions();
    	foreach (Option o in options){
    		if (o._available){
	  			if (GUILayout.Button(o.text)) {
		  			SelectOption(o);
					conversationTextStartSec=Time.time;
		  		}
		  	}
    	}
    	
    	if (HasReturnConversation()){
  			if (GUILayout.Button("Do something different")) {
  				GotoReturnConversation();
				conversationTextStartSec=Time.time;
	  		}
    	}else{
  			if (GUILayout.Button("Leave")) {
  				EndDialog();
	  		}
	  	}
	}
}
