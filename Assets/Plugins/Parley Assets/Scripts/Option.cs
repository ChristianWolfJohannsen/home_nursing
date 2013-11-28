using UnityEngine;
using System.Collections.Generic;

public class Option {
	public enum OptionType{
		Conversation,
		QuestReturn,
		QuestComplete,
		QuestAccept,
	};

	public string text="";
	public int destinationId=0;
	public string destinationDialogName="";
	public bool _available=true;
	public bool quest=false;
	public bool choosenb4=false;
	
	// This is not loaded or saved it is updated with environmentla infor each time the conversation initializes
	public string displaytext="";
	
	// This is not loaded but created as the optinos are created for the Quest dialog
	public OptionType type=OptionType.Conversation;
}
