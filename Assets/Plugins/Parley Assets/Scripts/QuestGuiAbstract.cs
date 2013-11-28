using UnityEngine;
using System.Collections.Generic;

public class QuestGuiAbstract : MonoBehaviour {
	public string showQuestsAction="ShowQuests";
	
	private bool showQuests=false;
	
	public void Update(){
		if (!showQuests && Input.GetButtonUp(showQuestsAction) && !Parley.GetInstance().IsInGui()){
			Parley.GetInstance().SetInGui(true);
			showQuests=true;
			SendMessage("QuestsStarted",SendMessageOptions.RequireReceiver);
		}
	}
	
	public bool MustShowQuests(){
		return showQuests;
	}
	
	public void CloseQuestDialog(){
		showQuests=false;
		Parley.GetInstance().SetInGui(false);
		SendMessage("QuestsEnded",SendMessageOptions.RequireReceiver);
	}

}
