using UnityEngine;

public class Objective : ParleyEnviromentInfo {
	
	public string description;
	public string doneDescription;
	public string locationObject;
	public int count;
	public bool optional=false;

	public string objectiveevent="";
	public string questevent="";
	public string activeevent="";
	public Command[] playerCommands;
	public string questrequirement="";
	
	public bool completed=false;
	public bool open=false;
	
	private GameObject myLocationObject=null;
	
	public GameObject GetLocationObject() {
		if (locationObject==null || locationObject.Length==0){
			return null;
		}
		if (myLocationObject==null){
			// Find my location object
			myLocationObject=GameObject.Find(locationObject);
			if (myLocationObject==null){
				Debug.LogWarning("Parley: Can not find location object '"+locationObject+"' for Objective '"+description+"'");
				locationObject=null;
			}
		}
		return myLocationObject;
	}
	
	public string GetStatus(){
		return (completed && doneDescription!=null && doneDescription.Length>0)?doneDescription:description;//.Replace("<count>",""+count);
	}
	
	public bool TriggerQuestEvent(string questEvent){
		if (!completed && questEvent.Equals(objectiveevent)){
			// If this is a count quest then count down
			if (count>0){
				count--;
			}
			// If we are done then fire quest events and player events
			if (count==0){
				if (questevent!=null && questevent.Length>0){
					Parley.GetInstance().TriggerQuestEvent(questevent);
				}
				Debug.Log("Parley: Completed quest objective "+description);
				completed=true;
				if (activeevent!=null && activeevent.Length>0){
					Parley.GetInstance().StopEventActive(activeevent);
				}
			}
				
			// Send player event
			if (playerCommands!=null && playerCommands.Length>0){
				Parley.GetInstance().ExecutePlayerCommands(null,playerCommands);
			}
			return true;
		}
		
		return false;
	}
	
	public object GetEnviromentInfo(string key){
		if ("count"==key){
			return count;
		}
		return null;
	}
	
	public void SetEnviromentInfo(string key,object value){
	}
}