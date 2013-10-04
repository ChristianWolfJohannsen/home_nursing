using UnityEngine;
using System.Collections.Generic;

public class Parley : MonoBehaviour {
	
	private static Parley instance;
	
	public static Parley GetInstance(){
		return instance;
	}
	
	public List<TextAsset> questList;
	
	public float dampenGuiClose=.1f;
	
	private List<Quest> quests=new List<Quest>();
	
	private HashSet<string> questEventsSet=new HashSet<string>();

	private Dictionary<string,List<QuestEventListener>> questTriggerListeners=new Dictionary<string,List<QuestEventListener>>();
	
	private Dictionary<string,Dialog> dialogs=new Dictionary<string,Dialog>();
	
	private Dialog currentDialog=null;
	
	private List<Evaluation> evaluations=new List<Evaluation>();
	
	private ParleyEnviromentInfo parleyEnviromentInfo;
	
	private bool inGui=false;
	
	private float guiCloseTime=0f;
	
	public void Start(){
		instance=this;
		
		// Add all known evalauations to list the order does matter as we must evaulate so that smaller string wont be confused to the longer ones. 
		// EG = could easily be picked up with >= so we make sure >= is in the list first.
		evaluations.Add(new EvaluateEqualOrLessThen());
		evaluations.Add(new EvaluateEqualOrGreaterThen());
		evaluations.Add(new EvaluateLessThen());
		evaluations.Add(new EvaluateGreaterThen());
		evaluations.Add(new EvaluateNotEquals());
		evaluations.Add(new EvaluateEquals());
		
		// Load defined quests
		if (questList != null && questList.Count>0){
			foreach (TextAsset ta in questList){
				AddQuest(ta);
			}
		}
	}
	
	public Dictionary<string,Dialog> GetDialogs(){
		return dialogs;
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
			Debug.Log("Parley: Executing '"+c.objectName+"."+c.method+"("+c.GetFullParam()+");'");
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
				parleyEnviromentInfo.SetEnviromentInfo(c.method,param);
			}else{
				if (param==null){
					o.BroadcastMessage(c.method,SendMessageOptions.RequireReceiver);
				}else{
					o.BroadcastMessage(c.method,param,SendMessageOptions.RequireReceiver);
				}
			}
		}
	}
	
	public void SetParleyEnviromentInfo(ParleyEnviromentInfo parleyEnviromentInfo){
		this.parleyEnviromentInfo=parleyEnviromentInfo;
	}
	
	public ParleyEnviromentInfo GetParleyEnviromentInfo(){
		return parleyEnviromentInfo;
	}
	
	public string EmbedEnviromentalInformation(string source){
		if (parleyEnviromentInfo==null){
			return source;
		}

		for (int p=source.IndexOf("<");p!=-1;p=source.IndexOf("<",p)){
			// Find end tag.
			int e=source.IndexOf(">",p);
			if (e==-1){
				break;
			}
			
			string term=source.Substring(p+1,e-p-1);
			object o=parleyEnviromentInfo.GetEnviromentInfo(term);
			if (o==null){
				Debug.LogWarning("Parley could not find the term <"+term+"> from the Enviroment");
				p++;
			}else{
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
	
	public List<Quest> GetQuests(){
		return quests;
	}
	
	public void SetCurrentDialog(Dialog currentDialog){
		this.currentDialog=currentDialog;
	}

	public Dialog GetCurrentDialog(){
		return currentDialog;
	}
	
	public void AddQuest(TextAsset ta){
		quests.Add(new Quest(ta));
	}
	
	public HashSet<string> GetQuestEventSet(){
		return questEventsSet;
	}
		
	public void SetQuestEventSet(HashSet<string> questEventsSet){
		this.questEventsSet=questEventsSet;
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
