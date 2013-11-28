using UnityEngine;
using System.Collections.Generic;

public class Dialog : PackUnpackableBehaviour, QuestChangedListener {

	public TextAsset conversationAsset;
	
	protected string dialogname="";
	public string description="";
	public bool mouseClickTriggered=false;
	public bool keyTriggered=true;
	public bool restartDialogEachTime=true;
	public string triggerInputKey="Chat";
	public string dialogClass="DialogGuiBasic";
	public float dialogRange=3;
	public GUISkin dialogSkin;
	public Camera dialogCamera=null;
	public Vector2 dialogSize=new Vector2(800,371);
	public Texture2D dialogPortrait=null;
	public float charactersPerSecond=50f;

	public Vector3 questObjectOffset=new Vector3(0,2,0);
	public Vector3 questObjectScale=new Vector3(1,1,1);
	public GameObject questAvailableObject=null;
	public GameObject questTakenObject=null;
	public GameObject questHandinObject=null;
	
	private Conversation[] conversations=null;
	private int conversationIndex=0;
	private GameObject playerObject;
	
	private int lastQuestStatus=0;
	private GameObject currentQuestObject;

	public void Awake(){
		LoadDialog();
		playerObject=GameObject.FindWithTag("Player");
		
		// Add this to the parley list
		if (!Parley.GetInstance().GetDialogs().ContainsKey(dialogname)){
			Parley.GetInstance().GetDialogs().Add(dialogname,this);
		}
	}
	
	public void Start(){
		UpdateQuestIcon(true);
	}

	public string GetDialogName(){
		return dialogname;
	}
	
	public int GetConversationIndex(){
		return conversationIndex;
	}
	
	public void SetConversationIndex(int conversationIndex){
		this.conversationIndex=conversationIndex;
	}
	
	public Conversation[] GetConversations(){
		return conversations;
	}
	
	public void ChangePortrait(string newDialogPortrait){
		dialogPortrait=Resources.Load(newDialogPortrait) as Texture2D;
	}
	
	protected void LoadDialog(){
		dialogname=conversationAsset.name.Substring(4);
		Debug.Log("name=["+dialogname+"]");
		// Load from Text Asset
		string[] lines = conversationAsset.text.Split("\n"[0]);
		int l=0;
		
		conversations=new Conversation[int.Parse(lines[l++])];

		for (int c=0;c<conversations.Length;c++){
			l++;
			Conversation co=new Conversation();
			conversations[c]=co;
			
			co.id=int.Parse(lines[l++]);
			co.uniqueId=int.Parse(lines[l++]);
		
			co.returnId=int.Parse(lines[l++]);
			co.returnDialogName=lines[l++];
			co.text=null;			
			for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
				if (co.text!=null){
					co.text+="\n";
				}
				co.text+=text;
			}
			
			co.repeattext=null;
			for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
				if (co.repeattext!=null){
					co.repeattext+="\n";
				}
				co.repeattext+=text;
			}
			
			co.once=bool.Parse(lines[l++]);
			co.fallthrough=bool.Parse(lines[l++]);
			co.questevent=lines[l++];

			co.playerCommands=new Command[int.Parse(lines[l++])];
			for (int ct=0;ct<co.playerCommands.Length;ct++){
				Command command=new Command();
				co.playerCommands[ct]=command;
				
				command.objectName=lines[l++];
				command.method=lines[l++];
				command.assignment=("true".Equals(lines[l++]));
				command.paramaters=new string[int.Parse(lines[l++])];
				for (int pt=0;pt<command.paramaters.Length;pt++){
					command.paramaters[pt]=lines[l++];
				}
			}
			
			co.questrequirement=lines[l++];
			co.environmentalrequirement=lines[l++];
			co._available=bool.Parse(lines[l++]);

			if (co.repeattext==null || co.repeattext.Length==0) co.repeattext=null;
			if (co.questevent==null || co.questevent.Length==0) co.questevent=null;
			if (co.playerCommands==null || co.playerCommands.Length==0) co.playerCommands=null;
			if (co.questrequirement==null || co.questrequirement.Length==0) co.questrequirement=null;
	
			co.options=new Option[int.Parse(lines[l++])];
		
			for (int n=0;n<co.options.Length;n++){
				l++;
				Option o=new Option();
				co.options[n]=o;
				o.text=lines[l++];
				for (string text=lines[l++];!text.Equals(Conversation.BREAK);text=lines[l++]){
					if (o.text!=null){
						o.text+="\n";
					}
					o.text+=text;
				}
				
				o.destinationId=int.Parse(lines[l++]);
				o.destinationDialogName=lines[l++];
				o.quest=bool.Parse(lines[l++]);
				o._available=bool.Parse(lines[l++]);
				l++;
			}
			l++;
		}
	}

	private void UpdateQuestIcon(bool addToQuestListener=false){
		HashSet<int> testedIds=new HashSet<int>();
		int questStatus=GetQuestStatus(0,conversations[0],testedIds,this,addToQuestListener);
		// Build or replace quest object if one is needed.
		if (questStatus!=lastQuestStatus){
			lastQuestStatus=questStatus;
			if (currentQuestObject!=null){
				Destroy(currentQuestObject);
				currentQuestObject=null;
			}
			switch (lastQuestStatus){
			case 3:
				if (questHandinObject!=null){
					Debug.Log("Parley: Create new quest ready to hand in icon.");
					currentQuestObject=(GameObject)Instantiate(questHandinObject);
				}
				break;
			case 2:
				if (questAvailableObject!=null){
					Debug.Log("Parley: Create new quest available icon.");
					currentQuestObject=(GameObject)Instantiate(questAvailableObject);
				}
				break;
			case 1:
				if (questTakenObject!=null){
					Debug.Log("Parley: Create new quest ongoing icon.");
					currentQuestObject=(GameObject)Instantiate(questTakenObject);
				}
				break;
			}
			if (currentQuestObject!=null){
				currentQuestObject.transform.parent=transform;
				currentQuestObject.transform.position=transform.position+questObjectOffset;
				currentQuestObject.transform.localScale=questObjectScale;
			}
		}
	}
	
	private int GetQuestStatus(int questStatus,Conversation conversation,HashSet<int> testedIds,Dialog sourceDialog,bool addToQuestListener){
		if (testedIds.Contains(conversation.uniqueId)){
			return questStatus;
		}
		testedIds.Add(conversation.uniqueId);
		
		foreach (Option o in conversation.options){
			if (o._available){
				if (o.quest){
					// Test quest
					if (o._available){
						Quest quest=Parley.GetInstance().GetQuest(o.destinationId);
						if (quest!=null){
							if (addToQuestListener){
								quest.AddQuestChangedListener(sourceDialog);
							}
							
							if (quest.readyToHandIn && !quest.completed){
								questStatus=3;
							} else if (quest.available && !quest.open && questStatus<2){
								questStatus=2;
							} else if (quest.open && questStatus<1){
								questStatus=1;
							}
						}
					} else {
						Debug.LogError("Parley: Dialog linked to quest id #"+o.destinationId+" but that quest can not be found in this scene");
					}
				} else {
					if (o.destinationDialogName!=null && o.destinationDialogName.Length>0){
						// find that dialog
						Dialog d=Parley.GetInstance().GetDialogs()[o.destinationDialogName];
						if (d!=null){
							if (testedIds.Contains(d.conversations[o.destinationId].uniqueId)){
								questStatus=d.GetQuestStatus(questStatus,d.conversations[o.destinationId],testedIds,sourceDialog,addToQuestListener);
							}
						}else{
							Debug.LogError("Parley: Can not find nested dialog '"+o.destinationDialogName+"' as seen in '"+dialogname+"'");
						}
					}else{
						if (testedIds.Contains(conversations[o.destinationId].uniqueId)){
							questStatus=GetQuestStatus(questStatus,conversations[o.destinationId],testedIds,sourceDialog,addToQuestListener);
						}
					}
				}
			}
		}
		return questStatus;
	}
	
	public void QuestChanged(Quest quest){
		UpdateQuestIcon();
	}

	public void Update(){
		if (triggerInputKey!=null && triggerInputKey.Length>0 && keyTriggered && Input.GetButtonUp(triggerInputKey) && IsInRange() && !Parley.GetInstance().IsInGui()){
			TriggerDialog();
		}
	}
	
    public void OnMouseDown() {
        if (mouseClickTriggered && IsInRange()){
			TriggerDialog();
		}
    }

	public bool IsInRange(){
		return Parley.GetInstance().GetCurrentDialog()==null && Vector3.Distance(transform.position, playerObject.transform.position)<dialogRange; 
	}

	public void TriggerDialog(){
		TriggerDialog(restartDialogEachTime?0:conversationIndex);
	}
	
	public void TriggerDialog(int start){
		conversationIndex=start;
		if (dialogClass!=null && dialogClass.Trim().Length>0){
			// Make sure we only ever add one gui instance
			MonoBehaviour dialogGuiInstance=(MonoBehaviour)GetComponent(dialogClass);
			if (dialogGuiInstance==null){
				// Add dialog gui now
				gameObject.AddComponent(dialogClass);
			}
		}
		SendMessage("StartDialog",this,SendMessageOptions.RequireReceiver);
	}
	
	public void TriggerDialogEnd(){
		if (dialogClass!=null && dialogClass.Trim().Length>0){
			MonoBehaviour dialogGuiInstance=(MonoBehaviour)GetComponent(dialogClass);
			if (dialogGuiInstance!=null){
				// Delete the Dialog Gui script
				Destroy(dialogGuiInstance);
			}
		}
	}

	public void OnDrawGizmos(){
		if (conversationAsset!=null){
			Gizmos.DrawIcon (transform.position-new Vector3(0,-dialogRange/3,0),"ParleyDialog.png");
		}
	}
	
	public void OnDrawGizmosSelected () {
		if (conversationAsset!=null){
		    Gizmos.color = Color.yellow;
		    Gizmos.DrawWireSphere (transform.position, dialogRange);
		}
	}
	
	override public void Pack(StreamPacker sp){
		sp.WriteInt(conversations.Length);
		
		foreach (Conversation c in conversations){
			sp.WriteInt(c.id);
			sp.WriteInt(c.returnId);
			sp.WriteInt(c.uniqueId);
			sp.WriteString(c.text);
			sp.WriteString(c.repeattext);
			sp.WriteBool(c.once);
			sp.WriteBool(c.fallthrough);
			sp.WriteString(c.questevent);
			sp.WriteObjects(c.playerCommands);
			sp.WriteString(c.questrequirement);
			sp.WriteString(c.environmentalrequirement);
			sp.WriteBool(c._available);
			sp.WriteBool(c._seen);
			
			sp.WriteInt(c.options.Length);
			foreach (Option o in c.options){
				sp.WriteString(o.text);
				sp.WriteInt(o.destinationId);
				sp.WriteBool(o._available);
				sp.WriteBool(o.choosenb4);
				sp.WriteBool(o.quest);
			}
		}
	}
	
	override public void Unpack(StreamUnpacker su){
		int cnum=su.ReadInt();
		conversations=new Conversation[cnum];
		for (int t=0;t<cnum;t++){
			Conversation c=new Conversation();
			conversations[t]=c;
			c.id=su.ReadInt();
			c.returnId=su.ReadInt();
			c.uniqueId=su.ReadInt();
			c.text=su.ReadString();
			c.repeattext=su.ReadString();
			c.once=su.ReadBool();
			c.fallthrough=su.ReadBool();
			c.questevent=su.ReadString();
			c.playerCommands=(Command[])su.ReadObjects(typeof(Command));
			c.questrequirement=su.ReadString();
			c.environmentalrequirement=su.ReadString();
			c._available=su.ReadBool();
			c._seen=su.ReadBool();
			
			int onum=su.ReadInt();
			c.options=new Option[onum];
			
			for (int u=0;u<onum;u++){
				Option o=new Option();
				c.options[u]=o;
				o.text=su.ReadString();
				o.destinationId=su.ReadInt();
				o._available=su.ReadBool();
				o.choosenb4=su.ReadBool();
				o.quest=su.ReadBool();
			}
		}
		
		UpdateQuestIcon(true);
	}
}
