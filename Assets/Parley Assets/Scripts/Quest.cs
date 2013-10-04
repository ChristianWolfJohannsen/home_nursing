using UnityEngine;
using System.Collections.Generic;

public class Quest {
	public string name="";
	public string description="";
	public string handinDescription="";
	public string afterDescription="";
	public int xp=0;
	public int ep=0;

	public string questevent="";
	//public string playerevent="";
	public Command[] playerCommands;
	public string questrequirement="";
	
	public bool open=false;
	public bool completed=false;
	public float lastEffected=0f;

	public List<Objective> objectives=new List<Objective>();
	
	public string status="";
	
	public Quest(){
	}
	
	public Quest(TextAsset ta){
		// Load quest from text asset
		string[] lines = ta.text.Split("\n"[0]);
		int l=0;
		
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

		xp=int.Parse(lines[l++]);
		ep=int.Parse(lines[l++]);
		questevent=lines[l++];
		//playerevent=lines[l++];
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
			o.optionalXpPerCount=int.Parse(lines[l++]);
			o.optionalEpPerCount=int.Parse(lines[l++]);
			o.optional=bool.Parse(lines[l++]);
			o.objectiveevent=lines[l++];
			o.questevent=lines[l++];
			//o.playerevent=lines[l++];
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
		open=Parley.GetInstance().IsRequirementTrue(questrequirement);
	}
	
	public void TriggerQuestEvent(string questEvent){
		// Do nothing if this quest is finished allready
		if (completed){
			return;
		}
		
		// Check if we can start this quest
		if (!open){
			open=Parley.GetInstance().IsRequirementTrue(questrequirement);
			
			if (open){
				GameObject.FindWithTag("Player").BroadcastMessage("StartedQuest",this,SendMessageOptions.DontRequireReceiver);
			}
		}
		
		// If started test against each quest option
		if (open){
			// Test each option
			bool done=true;
			foreach (Objective o in objectives){
				if (o.open==false && Parley.GetInstance().IsRequirementTrue(o.questrequirement)){
					o.open=true;
				}
				
				if (o.TriggerQuestEvent(questEvent)){
					lastEffected=Time.time;
				}
				if (!o.optional && !o.completed){
					done=false;
				}
			}

			// Mark this quest as completed and fire events
			if (completed==false && done==true){
				Debug.Log("Parley: Completed quest "+name);
				completed=done;
				lastEffected=Time.time;
				status="Completed";
				
				// Send player messages
				if (xp!=0){
					GameObject.FindWithTag("Player").BroadcastMessage("AddXp",xp,SendMessageOptions.RequireReceiver);
				}
				if (ep!=0){
					GameObject.FindWithTag("Player").BroadcastMessage("AddEp",ep,SendMessageOptions.RequireReceiver);
				}
				if (playerCommands!=null && playerCommands.Length>0){
					Parley.GetInstance().ExecutePlayerCommands(null,playerCommands);
				}
				GameObject.FindWithTag("Player").BroadcastMessage("FinishedQuest",this,SendMessageOptions.DontRequireReceiver);
				
				// Fire quest event
				if (questevent!=null && questevent.Length>0){
					Parley.GetInstance().TriggerQuestEvent(questevent);
				}
			}else{
				status="";
				foreach (Objective o in objectives){
					if (o.open==true && o.completed==false){
						if (status.Length>0){
							status+="\n";
						}
						status+=o.GetStatus();
					}
				}		
			}
		}
	}
	
	public string GetStatus(){
		return status;
	}

}
