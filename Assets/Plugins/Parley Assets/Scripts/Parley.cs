using UnityEngine;
using System.Collections.Generic;

public class Parley : MonoBehaviour {
	
	private static Parley instance;
	
	public static Parley GetInstance(){
		return instance;
	}
	
	public TextAsset[] questList=new TextAsset[0];
	
	public float dampenGuiClose=.1f;
	
	private List<Quest> quests=new List<Quest>();
	private static List<Quest> currentQuests=new List<Quest>(); // All open quests duplicated from the quests list
	private static List<Quest> completedQuests=new List<Quest>(); // All complted quests duplicated from the quests list
	
	private static HashSet<string> questEventsSet=new HashSet<string>();

	private Dictionary<string,List<QuestEventListener>> questTriggerListeners=new Dictionary<string,List<QuestEventListener>>();
	
	private Dictionary<string,Dialog> dialogs=new Dictionary<string,Dialog>();
	
	private HashSet<string> activeEvents=new HashSet<string>();
	
	private Dialog currentDialog=null;
	
	private List<Evaluation> evaluations=new List<Evaluation>();
	
	private static ParleyEnviromentInfo parleyEnviromentInfo;
	
	private bool inGui=false;
	
	public int act=0;
	public int scene=0;
	public bool showEditorQuests=true;
	public bool showDebug=true;
	public bool clearAtStartOfScene=true;

	private float guiCloseTime=0f;
	
	public void Awake(){
		instance=this;
		
		if (clearAtStartOfScene){
			ResetQuests();
		}
		
		// Add all known evalauations to list the order does matter as we must evaulate so that smaller string wont be confused to the longer ones. 
		// EG = could easily be picked up with >= so we make sure >= is in the list first.
		evaluations.Add(new EvaluateEqualOrLessThen());
		evaluations.Add(new EvaluateEqualOrGreaterThen());
		evaluations.Add(new EvaluateLessThen());
		evaluations.Add(new EvaluateGreaterThen());
		evaluations.Add(new EvaluateNotEquals());
		evaluations.Add(new EvaluateEquals());
		
		LoadSceneQuests();
		SyncQuests();
	}
	
	/**
	 * Loads all the quest listed for this scene into the quests list. This will be called at the start of a scene and 
	 * just before quests are loaded for a load game.
	 */
	public void LoadSceneQuests(){
		quests.Clear();
		// Load defined quests
		if (questList.Length>0){
			foreach (TextAsset ta in questList){
				AddQuest(ta);
			}
		}
	}
	
	/**
	 * This brings the current and completed questsw into this scnes quest list. This should be called after a load game or
	 * when game scenes change. Parley will call this automatically after it loads the quests for this scene. You need only call it
	 * when you load quests manually into the completed or current list.
	 */
	public void SyncQuests(){
		
		foreach (Quest q in completedQuests){
			for (int t=0;t<quests.Count;t++){
				Quest q2=quests[t];
				if (q2.uniqueId==q.uniqueId){
					quests[t]=q;
				}
			}
		}
		
		foreach (Quest q in currentQuests){
			for (int t=0;t<quests.Count;t++){
				Quest q2=quests[t];
				if (q2.uniqueId==q.uniqueId){
					quests[t]=q;
				}
			}
		}
	}

	public Dictionary<string,Dialog> GetDialogs(){
		return dialogs;
	}
	
	/**
	 * Active events are a way to manage behavious over a large number of items when a quest or objective is active.
	 */
	public bool IsEventActive(string eventName){
		return activeEvents.Contains(eventName);
	}
	
	/**
	 * Active events are a way to manage behavious over a large number of items when a quest or objective is active.
	 */
	public HashSet<string> GetActiveEvents(){
		return activeEvents;
	}
	
	public void StartEventActive(string eventName){
		if (!activeEvents.Contains(eventName)){
			Debug.Log("Parley: Starting event '"+eventName+"'");
			activeEvents.Add(eventName);
		}else{
			Debug.LogWarning("Parley: Attempt to starting event '"+eventName+"' but that event was allready running.");
		}
	}
	
	public void StopEventActive(string eventName){
		if (activeEvents.Contains(eventName)){
			Debug.Log("Parley: Ending event '"+eventName+"'");
			activeEvents.Remove(eventName);
		}else{
			Debug.LogWarning("Parley: Attempt to end event '"+eventName+"' but that event was not running.");
		}
	}
	
	/**
	 * The in gui methods allow various Gui menu's to know weather or not to show. This is critical
	 * when the mouse buttons used to show the gui are the same sones used to click buttons.
	 * There is a damnpen value defaulting to .1 seconds that will continue to show the gui is up for that long after.
	 * This prevents the one gui show ihen you click close on the other.
	 * 
	 * It is recommended you manage all your GUI's with this system of implement you own solution and work that into
	 * these funtions or change the GUI scripts that come with Parley.
	 */
	public void SetInGui(bool inGui){
		if (inGui==true){
			this.inGui=true;
			guiCloseTime=0;
		}else{
			this.inGui=false;
			guiCloseTime=Time.time+dampenGuiClose;
		}
	}
	
	public bool IsInGui(){
		if (inGui==true){
			return true;
		}else{
			if (Time.time>guiCloseTime){
				return false;
			}
			return true;
		}
	}
	
	public void ExecutePlayerCommands(GameObject gameObject,Command[] commands){
		GameObject player=null;
		foreach (Command c in commands){
			GameObject o=gameObject;
			if ("player".Equals(c.objectName)){
				if (player==null){
					player=GameObject.FindWithTag("Player");
				}
				o=player;
			}else if ("parley".Equals(c.objectName)){
				o=this.gameObject;
			}
			object param=c.CalculateParam();
			if (c.assignment){
				Debug.Log("Parley: Executing '"+c.method+"="+c.GetFullParam()+";'");
				parleyEnviromentInfo.SetEnviromentInfo(c.method,param);
			}else{
				Debug.Log("Parley: Executing '"+c.objectName+"."+c.method+"("+c.GetFullParam()+");'");
				if (param==null){
					o.BroadcastMessage(c.method,SendMessageOptions.RequireReceiver);
				}else{
					o.BroadcastMessage(c.method,param,SendMessageOptions.RequireReceiver);
				}
			}
		}
	}
	
	/**
	 * This reset all static game data to a fresh set for a enw game. 
	 * This MUST be called in the first scene of your game.
	 */
	public void NewGameData(){
		currentQuests.Clear();
		completedQuests.Clear();
		questEventsSet.Clear();
	}
	
	public void SetParleyEnviromentInfo(ParleyEnviromentInfo parleyEnviromentInfo){
		Parley.parleyEnviromentInfo=parleyEnviromentInfo;
	}
	
	public ParleyEnviromentInfo GetParleyEnviromentInfo(){
		return parleyEnviromentInfo;
	}

	public string EmbedEnviromentalInformation(string source){
		return EmbedEnviromentalInformation(source,parleyEnviromentInfo);
	}

	public string EmbedEnviromentalInformation(string source,ParleyEnviromentInfo enviromentInfo){
		if (enviromentInfo==null){
			return source;
		}

		for (int p=source.IndexOf("<");p!=-1;p=source.IndexOf("<",p)){
			// Find end tag.
			int e=source.IndexOf(">",p);
			if (e==-1){
				break;
			}
			
			string term=source.Substring(p+1,e-p-1);
			
			object o=enviromentInfo.GetEnviromentInfo(term);

			// If not a direct value then perhaps a calculation
			if (o==null && term.IndexOfAny("+-/*".ToCharArray())!=-1){
				// Use the command code to assign a value to 'v' this wont run it just aids compilation
				Command c=new Command("v="+term);

				Debug.Log("Parley: Calculating value for EI '"+term+"'");
				o=c.CalculateParam(enviromentInfo);
			}
			
			if (o==null){
				Debug.Log("Parley could not find the term <"+term+"> from the Enviroment");
				p++;
			} else {
				source=source.Substring(0,p)+o.ToString()+source.Substring(e+1);
			}
		}
		return source;
	}

	public void AddTriggerListener(GameObject go, string questEvent,string message){
		// Have we allready fired this trigger
		if (questEventsSet.Contains(questEvent)){
			return;
		}
		
		// Make sure there is a list in the dictionary
		if (!questTriggerListeners.ContainsKey(questEvent)){
			questTriggerListeners.Add (questEvent,new List<QuestEventListener>());
		}
		
		// Add this listener
		QuestEventListener qel=new QuestEventListener();
		qel.go=go;
		qel.message=message;
		questTriggerListeners[questEvent].Add(qel);
	}
	
	public bool IsRequirementTrue(string requirement){
		if (requirement==null || requirement.Length==0){
			return true;
		}
		// Split into parts around
		string[] parts=requirement.Split(',');
		foreach (string r in parts){
			if (r.StartsWith("!")){
				if (questEventsSet.Contains(r.Substring(1))){
					return false;
				}
			}else{
				if (!questEventsSet.Contains(r)){
					return false;
				}

			}
		}
		return true;
	}
	
	public bool IsEnvironmentalRequirementTrue(string requirement){
		if (requirement==null || requirement.Length==0){
			return true;
		}
		// Split into parts around "or"
		string[] orparts=requirement.Split(new string[]{" or "},System.StringSplitOptions.None);
		foreach (string orpart in orparts){
			string[] andparts=orpart.Split(new string[]{" and "},System.StringSplitOptions.None);
			bool andbool=true;
			foreach (string andpart in andparts){
				bool foundMatch=false;
				// Find operator
				foreach (Evaluation e in evaluations){
					if (e.Matches(andpart)){
						andbool=e.Evaluate();
						foundMatch=true;
						break;
					}
				}
				if (!foundMatch){
					Debug.LogWarning("Parley: Could not find a match for '"+andpart+"' in '"+requirement+"'");
				}
				if (!andbool){
					break;
				}
			}		
			if (andbool){
				return true;
			}
		}
		return false;
	}
	
	/**
	 * This returns the quests that are currently active.
	 * 
	 * This list persistes between scenes
	 */
	public List<Quest> GetCurrentQuests(){
		return currentQuests;
	}
	
	/**
	 * This returns the completed quests.
	 * 
	 * This list persistes between scenes
	 */
	public List<Quest> GetCompletedQuests(){
		return completedQuests;
	}

	/**
	 * This returns all the quests available in this scene.
	 */
	public List<Quest> GetQuests(){
		return quests;
	}
	
	public Quest GetQuest(int uniqueId){
		foreach (Quest quest in quests){
			if (quest.uniqueId==uniqueId){
				return quest;
			}
		}
		Debug.LogWarning("Parley: Searching for Quest by uniqueId<"+uniqueId+"> quest not found in scene");
		return null;
	}
	
	public List<Quest> GetAllQuests(){
		return quests;
	}
	
	public void SetCurrentDialog(Dialog currentDialog){
		this.currentDialog=currentDialog;
	}

	public Dialog GetCurrentDialog(){
		return currentDialog;
	}
	
	public void AddQuest(TextAsset ta){
		quests.Add(new Quest(ta.text));
	}
	
	public void AddQuest(string ta){
		quests.Add(new Quest(ta));
	}
	
	public HashSet<string> GetQuestEventSet(){
		return questEventsSet;
	}
		
	public void SetQuestEventSet(HashSet<string> questEventsSet){
		Parley.questEventsSet=questEventsSet;
	}

	public void TriggerQuestEvent(string questEvent){
		if (questEvent==null || questEvent.Length==0){
			return;
		}
		
		Debug.Log("Parley Quest Event:"+questEvent);
		if (questEventsSet.Contains(questEvent)){
			for (int t=2;t<1000;t++){
				if (!questEventsSet.Contains(questEvent+t)){
					questEventsSet.Add(questEvent+t);
					break;
				}
			}
		}else{
			questEventsSet.Add(questEvent);
		}

		// effect appropriate quests
		foreach (Quest q in quests){
			q.TriggerQuestEvent(questEvent);
		}
		
		// Fire off quest event listeners
		if (questTriggerListeners.ContainsKey(questEvent)){
			foreach (QuestEventListener qel in questTriggerListeners[questEvent]){
				qel.go.SendMessage(qel.message,SendMessageOptions.RequireReceiver);
			}
			questTriggerListeners.Remove(questEvent);
		}
	
	}
	
	public void QuestStarted(Quest quest){
		currentQuests.Add(quest);
	}
	
	public void QuestCompleted(Quest quest){
		currentQuests.Remove(quest);
		completedQuests.Add(quest);
	}
	
	public void ResetQuests(){
		questEventsSet.Clear();
	}

	public class QuestEventListener {
		public GameObject go;
		public string message;
	}
	
	public abstract class Evaluation {
		private string[] operatorTerms;
		
		private object leftTerm;
		private object rightTerm;
		
		protected Evaluation(string operatorTerm){
			this.operatorTerms=operatorTerm.Split(',');
		}
		
		public bool Matches(string instruction){
			foreach (string ot in operatorTerms){
				int i=instruction.IndexOf(ot);
				if (i!=-1){
					// Get terms
					string l=instruction.Substring(0,i).Trim();
					string r=instruction.Substring(i+ot.Length).Trim();
					leftTerm=GetTerm(l);
					rightTerm=GetTerm(r);
					return true;
				}
			}
			return false;
		}
		
		private object GetTerm(string termString){
			// Is this a string
			if (termString.StartsWith("'") || termString.StartsWith("\"")){
				return termString.Substring(1,termString.Length-2);
			} else if ("0123456789-+".IndexOf(termString.Substring(0,1))!=-1){
				// Ok we are a number
				// Int or float?
				if (termString.IndexOf(".")!=-1){
					return float.Parse(termString);
				}else{
					return int.Parse(termString);
				}
			} else {
				// This is hopefully an enviromental term
				ParleyEnviromentInfo info=Parley.GetInstance().GetParleyEnviromentInfo();
				if (info!=null){
					object o=info.GetEnviromentInfo(termString);
					if (o==null){
						Debug.LogWarning("Parley: Enviromental Term requested and not found '"+termString+"'");
						return -1;
					}
					return o;
				}else{
					Debug.LogWarning("Parley: Enviromental Term requested but not ParleyEnviromentInfo service has been registered.");
					return -1;
				}
			}
		}
		
		public bool Evaluate(){
			return Evaluate(leftTerm,rightTerm);
		}
		
		abstract protected bool Evaluate(object left,object right);
	}
	
	public class EvaluateEquals : Evaluation {
		public EvaluateEquals():base("==,="){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return ((string)left).Equals(right.ToString());
			}else if (left is int){
				if (right is int){
					return ((int)left)==((int)right);
				}else{
					return ((int)left)==int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)==((float)right);
				}else{
					return ((float)left)==float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+"=="+right);
			return false;
		}
	}

	public class EvaluateNotEquals : Evaluation {
		public EvaluateNotEquals():base("<>,!="){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return !((string)left).Equals(right.ToString());
			}else if (left is int){
				if (right is int){
					return ((int)left)!=((int)right);
				}else{
					return ((int)left)!=int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)!=((float)right);
				}else{
					return ((float)left)!=float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+"!="+right);
			return false;
		}
	}
	
	public class EvaluateGreaterThen : Evaluation {
		public EvaluateGreaterThen():base(">"){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return ((string)left).CompareTo(right.ToString())>0;
			}else if (left is int){
				if (right is int){
					return ((int)left)>((int)right);
				}else{
					return ((int)left)>int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)>((float)right);
				}else{
					return ((float)left)>float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+">"+right);
			return false;
		}
	}
	
	public class EvaluateLessThen : Evaluation {
		public EvaluateLessThen():base("<"){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return ((string)left).CompareTo(right.ToString())<0;
			}else if (left is int){
				if (right is int){
					return ((int)left)<((int)right);
				}else{
					return ((int)left)<int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)<((float)right);
				}else{
					return ((float)left)<float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+"<"+right);
			return false;
		}
	}
	
	public class EvaluateEqualOrGreaterThen : Evaluation {
		public EvaluateEqualOrGreaterThen():base(">="){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return ((string)left).CompareTo(right.ToString())>=0;
			}else if (left is int){
				if (right is int){
					return ((int)left)>=((int)right);
				}else{
					return ((int)left)>=int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)>=((float)right);
				}else{
					return ((float)left)>=float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+">="+right);
			return false;
		}
	}
	
	public class EvaluateEqualOrLessThen : Evaluation {
		public EvaluateEqualOrLessThen():base("<="){
		}
		
		override protected bool Evaluate(object left,object right){
			if (left is string){
				return ((string)left).CompareTo(right.ToString())<=0;
			}else if (left is int){
				if (right is int){
					return ((int)left)<=((int)right);
				}else{
					return ((int)left)<=int.Parse(right.ToString());
				}
			}else if (left is float){
				if (right is float){
					return ((float)left)<=((float)right);
				}else{
					return ((float)left)<=float.Parse(right.ToString());
				}
			}
			Debug.LogWarning("Parley: Term types dont match "+left+"<="+right);
			return false;
		}
	}
}
