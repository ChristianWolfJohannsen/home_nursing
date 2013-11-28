using UnityEngine;
using System.Collections.Generic;

public class Conversation {
	public const string BREAK="!_@~~@_!";

	public int id=0;
	public int uniqueId=0;
	public int returnId=0;
	public string returnDialogName="";
	public string text="";
	public string repeattext="";
	public bool once=false;
	public bool fallthrough=false;
	public string questevent="";
	//public string playerevent="";
	public Command[] playerCommands;
	public string questrequirement="";
	public string environmentalrequirement="";
	public bool _available=true;
	public bool _seen=false;
	public Option[] options=null;
	
	public Conversation(){
	}
	
	public void UpdateAvailability(){
		_available=!_seen || !once;
		if (_available && questrequirement!=null && questrequirement.Length>0){
			_available=Parley.GetInstance().IsRequirementTrue(questrequirement);
		}
		if (_available && environmentalrequirement!=null && environmentalrequirement.Length>0){
			_available=Parley.GetInstance().IsEnvironmentalRequirementTrue(environmentalrequirement);
		}
	}
}
