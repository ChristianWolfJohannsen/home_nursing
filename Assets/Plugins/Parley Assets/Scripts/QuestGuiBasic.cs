using UnityEngine;
using System.Collections.Generic;

public class QuestGuiBasic : QuestGuiAbstract {
	
	public Vector2 questSize=new Vector2(800,371);
	public GUISkin questSkin;
	public bool showQuestsSummary=true;
	public float charactersPerSecond=50f;
	
	private Vector2 scrollPosition;
	private Quest currentQuest=null;
	private float svw=800;
	private float svh=256+20+30+50+15;
	
	private string questText="";
	private float questTextStartSec=0f;


	private bool hideStatusInDialog=false;
	

	public void Start(){
	}
	
	public void QuestsStarted(){
		SelectQuest(currentQuest);
	}

	public void QuestsEnded(){
	}
	
	public void OnGUI(){
		if (Parley.GetInstance()==null){
			return;
		}
		
		// Return if we are in a dialog
		if (hideStatusInDialog && Parley.GetInstance().GetCurrentDialog()!=null){
			return;
		}
		if (questSkin!=null){
			GUI.skin=questSkin;
		}
		
		if (MustShowQuests() && Parley.GetInstance().GetCurrentDialog()==null){
			ShowQuestDetails();
		}
		
		if (showQuestsSummary){
			ShowQuestList();
		}
	}
	
	public void ShowQuestDetails(){
		Rect windowRect=new Rect((Screen.width-svw)/2,(Screen.height-svh)/2,svw,svh);
		// Draw Window
		GUILayout.BeginArea(windowRect,currentQuest==null?"Quests":currentQuest.name,GUI.skin.window);
		GUILayout.BeginVertical();
        DrawText(currentQuest);
		// Buttons
		DrawButtons();
		
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}	
	
	private void DrawButtons(){
		List<Quest> quests=Parley.GetInstance().GetQuests();
    	foreach (Quest q in quests){
    		if (q.open){
	  			if (GUILayout.Button(q.name)) {
		  			SelectQuest(q);
		  		}
		  	}
    	}
    	
		if (GUILayout.Button("Bye.")) {
			CloseQuestDialog();
  		}
	}

	private bool DrawText(Quest quest){
		if (quest!=null){
			int textPos=(int)((Time.time-questTextStartSec)*charactersPerSecond);
			textPos=Mathf.Min(questText.Length,textPos);
			scrollPosition=GUILayout.BeginScrollView(scrollPosition,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
	        GUILayout.Label(questText.Substring(0,textPos),new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
			GUILayout.EndScrollView();
			
			return textPos==questText.Length;
		}else{
	        GUILayout.Label("",new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
		}
		return false;
	}

	private void ShowQuestList(){	
		int q=0;
		List<Quest> quests=Parley.GetInstance().GetCurrentQuests();
		for (int t=quests.Count-1;t>=0;t--){
			Quest qq=quests[t];
			if (qq.open && (!qq.completed || Time.time-3<qq.lastEffected)){
				Rect currentQuestDisplay=new Rect(Screen.width-190,Screen.height-70-80*q,180,60);
				GUI.BeginGroup(currentQuestDisplay,qq.name,(Time.time-3<qq.lastEffected)?"QuestSummaryFocus":"QuestSummary");
				GUI.Label(new Rect(3,20,currentQuestDisplay.width-6,currentQuestDisplay.height-23),qq.GetStatus(),"Tiny");
				GUI.EndGroup();
				q++;
			}
		}
	}
	
	public void SelectQuest(Quest q){
		currentQuest=q;
		if (q!=null){
			questText=Parley.GetInstance().EmbedEnviromentalInformation(q.description+"\n\n"+q.GetObjectivesSummary());
			
			questTextStartSec=Time.time;
		}
	}

}
