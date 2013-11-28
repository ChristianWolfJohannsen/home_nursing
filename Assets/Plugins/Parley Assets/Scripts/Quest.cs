using UnityEngine;
using System.Collections.Generic;

public class Quest {
	public int uniqueId=0;
	public string name="";
	public string description="";
	public string handinDescription="";
	public string afterDescription="";

	public string questevent="";
	public string activeevent="";
	public Command[] playerCommands;
	public string questrequirement="";
	
	public bool available=false;
	public bool open=false;
	public bool readyToHandIn=false;
	public bool completed=false;
	public float lastEffected=0f;
	
	public bool boundInDialog=false; // This quest is started and completed from within Dialog not automatically

	public List<Objective> objectives=new List<Objective>();
	public List<QuestChangedListener> questListeners=new List<QuestChangedListener>();
	
	public string status="";
		
	public Quest(){
	}
	
	public Quest(string questText){
		// Load quest from text asset
		string[] lines = questText.Split("\n"[0]);
		int l=0;
		
		uniqueId=int.Parse(lines[l++]);
		
		name=lines[l++];
		
		description=null;			
		for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
			if (description!=null){
				description+="\n";
			}
			description+=text;
		}

		handinDescription=null;			
		for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
			if (handinDescription!=null){
				handinDescription+="\n";
			}
			handinDescription+=text;
		}

		afterDescription=null;			
		for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
			if (afterDescription!=null){
				afterDescription+="\n";
			}
			afterDescription+=text;
		}

		questevent=lines[l++];
		activeevent=lines[l++];

		playerCommands=new Command[int.Parse(lines[l++])];
		for (int ct=0;ct<playerCommands.Length;ct++){
			Command command=new Command();
			playerCommands[ct]=command;
			
			command.objectName=lines[l++];
			command.method=lines[l++];
			command.assignment=("true".Equals(lines[l++]));
			command.paramaters=new string[int.Parse(lines[l++])];
			for (int pt=0;pt<command.paramaters.Length;pt++){
				command.paramaters[pt]=lines[l++];
			}
		}
		
		questrequirement=lines[l++];
		
		boundInDialog=("true".Equals(lines[l++]));
		
		int count=int.Parse(lines[l++]);
		
		for (int t=0;t<count;t++){
			l++;
			Objective o=new Objective();
			
			o.description=null;			
			for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
				if (o.description!=null){
					o.description+="\n";
				}
				o.description+=text;
			}
			
			o.doneDescription=null;			
			for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
				if (o.doneDescription!=null){
					o.doneDescription+="\n";
				}
				o.doneDescription+=text;
			}
			
			o.locationObject=lines[l++];
			o.count=int.Parse(lines[l++]);
			o.optional=bool.Parse(lines[l++]);
			o.objectiveevent=lines[l++];
			o.questevent=lines[l++];
			o.activeevent=lines[l++];
			o.playerCommands=new Command[int.Parse(lines[l++])];
			
			for (int ct=0;ct<o.playerCommands.Length;ct++){
				Command command=new Command();
				o.playerCommands[ct]=command;
				
				command.objectName=lines[l++];
				command.method=lines[l++];
				command.assignment=("true".Equals(lines[l++]));
				command.paramaters=new string[int.Parse(lines[l++])];
				for (int pt=0;pt<command.paramaters.Length;pt++){
					command.paramaters[pt]=lines[l++];
				}
			}
			o.questrequirement=lines[l++];
			
			objectives.Add(o);
			l++;
		}
		
		// Test if open
		available=Parley.GetInstance().IsRequirementTrue(questrequirement);
		
		if (available && !boundInDialog){
			StartQuest();
		}else{
			FireQuestHasChanged();
		}
	}
	
	public void AddQuestChangedListener(QuestChangedListener listener){
		questListeners.Add(listener);
	}

	public void RemoveQuestChangedListener(QuestChangedListener listener){
		questListeners.Remove(listener);
	}
	
	private void FireQuestHasChanged(){
		foreach (QuestChangedListener qcl in questListeners){
			qcl.QuestChanged(this);
		}
	}
	
	public void StartQuest(){
		open=true;
		GameObject.FindWithTag("Player").BroadcastMessage("StartedQuest",this,SendMessageOptions.DontRequireReceiver);
		Parley.GetInstance().QuestStarted(this);
		TestQuestDone();
		lastEffected=Time.time;
		if (activeevent!=null && activeevent.Length>0){
			Parley.GetInstance().StartEventActive(activeevent);
		}
		FireQuestHasChanged();
	}
	
	public void CompleteQuest(){
		if (playerCommands!=null && playerCommands.Length>0){
			Parley.GetInstance().ExecutePlayerCommands(null,playerCommands);
		}
		GameObject.FindWithTag("Player").BroadcastMessage("FinishedQuest",this,SendMessageOptions.DontRequireReceiver);
		
		// Fire quest event
		if (questevent!=null && questevent.Length>0){
			Parley.GetInstance().TriggerQuestEvent(questevent);
		}
		Parley.GetInstance().QuestCompleted(this);
		completed=true;
		lastEffected=Time.time;
		
		if (activeevent!=null && activeevent.Length>0){
			Parley.GetInstance().StopEventActive(activeevent);
		}
		FireQuestHasChanged();
	}
	
	public void TestQuestDone(string questEvent=null){
		// If started test against each quest option
		if (open){
			// Test each option
			bool done=true;
			foreach (Objective o in objectives){
				if (o.open==false && Parley.GetInstance().IsRequirementTrue(o.questrequirement)){
					o.open=true;
					if (o.activeevent!=null && o.activeevent.Length>0){
						Parley.GetInstance().StartEventActive(o.activeevent);
					}
				}
				
				if (questEvent!=null && o.TriggerQuestEvent(questEvent)){
					lastEffected=Time.time;
				}
				if (!o.optional && !o.completed){
					done=false;
				}
			}
			
			// Mark this quest as completed and fire events
			if (readyToHandIn==false && done==true){
				Debug.Log("Parley: Completed quest "+name);
				readyToHandIn=done;
				lastEffected=Time.time;
				status="Completed";
				
				if (!boundInDialog){
					CompleteQuest();
				}else{
					FireQuestHasChanged();
				}
			}else{
				UpdateStatus();
			}
		}
	}
	
	public void TriggerQuestEvent(string questEvent){
		// Do nothing if this quest is finished allready
		if (completed){
			return;
		}
		
		// Check if we can start this quest
		if (!open){
			available=Parley.GetInstance().IsRequirementTrue(questrequirement);
			
			if (available && !boundInDialog){
				StartQuest();
			}
		}

		TestQuestDone(questEvent);
	}
	
	private void UpdateStatus(){
		status=GetObjectivesSummary(true);
	}

	public string GetObjectivesSummary(bool abridged=false){
		string summary="";
		ParleyEnviromentInfo[] infoSets=new ParleyEnviromentInfo[2];
		infoSets[1]=Parley.GetInstance().GetParleyEnviromentInfo();
		ParleyEnviromentInfoCombiner pic=new ParleyEnviromentInfoCombiner(infoSets);
		
		foreach (Objective o in objectives){
			infoSets[0]=o;
			if (o.open){
				if (abridged){
					if (!o.completed){
						summary+=Parley.GetInstance().EmbedEnviromentalInformation(o.GetStatus()+"\n",pic);
					}
				}else{
					if (o.completed){
						summary+=Parley.GetInstance().EmbedEnviromentalInformation(" - "+o.GetStatus()+" ("+(o.optional?"Optional, ":"")+"done)\n",pic);
					}else{
						summary+=Parley.GetInstance().EmbedEnviromentalInformation(" - "+o.GetStatus()+(o.optional?" (Optional)":"")+"\n",pic);
					}
				}
			}
		}
		
		return summary;
	}
	
	public string GetStatus(){
		return status;
	}

}
