using UnityEngine;
using System.Collections.Generic;

public class Dialog : MonoBehaviour {

	public TextAsset conversationAsset;
	
	protected string dialogname="";
	public string description="";
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
	
	private Conversation[] conversations=null;
	private int conversationIndex=0;
	private GameObject playerObject;

	public void Start(){
		LoadDialog();
		playerObject=GameObject.FindWithTag("Player");
		
		// Add this to the parley list
		Parley.GetInstance().GetDialogs().Add(dialogname,this);
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
				o._available=bool.Parse(lines[l++]);
				l++;
			}
			l++;
		}
	}

	public void Update(){
		if (triggerInputKey!=null && triggerInputKey.Length>0 && keyTriggered && Input.GetButtonUp(triggerInputKey) && IsInRange() && !Parley.GetInstance().IsInGui()){
			TriggerDialog();
		}
	}

	public bool IsInRange(){
		return Vector3.Distance(transform.position, playerObject.transform.position)<dialogRange; 
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
}
