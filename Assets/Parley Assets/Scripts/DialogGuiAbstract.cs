using UnityEngine;
using System.Collections.Generic;

public abstract class DialogGuiAbstract : MonoBehaviour {
	
	private Dialog dialog;
	
	private Camera oldCamera=null;
	
	private string conversationText=null;
	private Conversation conversation=null;
	private List<Option> currentOptions=null;
	
	private int cycleCount=0;
	
	
	/** 
	 * Start Dialog is called from Dialog as the trigger to get the DialogGui 
	 * going. 
	 * 
	 * This is broadcast into he GameObject after the DialogGui class is created 
	 * and added to the GameObject. You should not need to use extends or alter 
	 * this method if you are using the DialogGuiAbstract as your base.
	 * 
	 * This broadcasts DialogStarted into the GameObject after it is called.	 
	 * 
	 */
	public void StartDialog(Dialog dialog){
		Parley.GetInstance().SetInGui(true);
			
		this.dialog=dialog;
		
		// Change camera
		if (dialog.dialogCamera!=null){
			oldCamera=Camera.main;
			oldCamera.gameObject.SetActive(false);
			dialog.dialogCamera.gameObject.SetActive(true);
		}
		
		// Broadcast to player and this object that we have started a dialog
		GameObject.FindWithTag("Player").BroadcastMessage("DialogStarted",dialog,SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("DialogStarted",dialog,SendMessageOptions.DontRequireReceiver);
		Parley.GetInstance().SetCurrentDialog(dialog);
		
		// Start at the correct dialog
		GotoDialogue(null,dialog.GetConversationIndex()); 
	}
	
	/**
	 * End Dialog is called by you when the Player decides to End the Dialog. 
	 * 
	 * Either through a close button or by moving too far away. Any number of situations.
	 * You can casually call EndDialog from within your code all Dialog end cleanup 
	 * code should be added to your method implementing DialogEnded. 
	 * 
	 */
	public void EndDialog(){
		// change camera back
		if (oldCamera!=null){
			dialog.dialogCamera.gameObject.SetActive(false);
			oldCamera.gameObject.SetActive(true);
		}

		// Broadcast to player and this object that we have finished a dialog
		BroadcastMessage("DialogEnded",dialog,SendMessageOptions.DontRequireReceiver);
		GameObject.FindWithTag("Player").BroadcastMessage("DialogEnded",dialog,SendMessageOptions.DontRequireReceiver);
		Parley.GetInstance().SetCurrentDialog(null);
		
		Parley.GetInstance().SetInGui(false);
		
		SendMessage("TriggerDialogEnd",this,SendMessageOptions.RequireReceiver);
	}
	
	/**
	 * Goto Fialog setds the current Dialog to a specific Conversataion state. The
	 * ID is in face the Array location of the conversation. This will typically come from
	 * the Options.
	 * 
	 * When a new dialog is selected it will fire off the gamne and player events and flag itself as 
	 * seen.
	 * 
	 * Additionally if will fall through if needed.
	 * 
	 */
	private void GotoDialogue(string dialogName,int index){
		// Is this a jump to aouther dialog?
		if (dialogName!=null && dialogName.Length>0 && !dialog.name.Equals(dialogName)){
			EndDialog();
			Dialog d=Parley.GetInstance().GetDialogs()[dialogName];
			if (d!=null){
				d.TriggerDialog(index);
			}else{
				Debug.LogError("Parley: Could not find linked dialog '"+dialogName+"' in scene. Falling out of dialog.");
			}
			
		}else{
			dialog.SetConversationIndex(index);
			
			conversation=dialog.GetConversations()[index];
			conversation._seen=true;
			currentOptions=null;
			
			if (conversation.options==null){
				conversation.options=new Option[0];
			}
		
			conversationText=conversation.text;
			if (conversation.repeattext!=null){
				conversation.text=conversation.repeattext;
			}
			
			if (conversation.questevent!=null){
				Parley.GetInstance().TriggerQuestEvent(conversation.questevent);
				conversation.questevent=null;
			}
			
			if (conversation.once){
				conversation._available=false;
				conversation.questrequirement="";
				Parley.GetInstance().TriggerQuestEvent(conversation.questevent);
			}
		
			if (conversation.playerCommands!=null){
				Parley.GetInstance().ExecutePlayerCommands(gameObject,conversation.playerCommands);
				conversation.playerCommands=null;
			}
			
			// Refresh each dialog option from this one.
			int availableOptions=0;
			int firstDestination=-1;
			string firstDestinationName="";
			foreach (Option o in conversation.options){
				Dialog odialog=dialog;
				if (o.destinationDialogName!=null && o.destinationDialogName.Length>0 && !dialog.name.Equals(o.destinationDialogName)){
					odialog=Parley.GetInstance().GetDialogs()[o.destinationDialogName];
				}
				
				if (odialog==null){
					Debug.LogError("Parley: Could not find linked dialog '"+o.destinationDialogName+"' in scene. Ignoring Option.");
				}else{
					odialog.GetConversations()[o.destinationId].UpdateAvailability();
					o._available=odialog.GetConversations()[o.destinationId]._available;
					if (o._available){
						availableOptions++;
						if (firstDestination==-1){
							firstDestination=o.destinationId;
							firstDestinationName=o.destinationDialogName;
						}
					}
				}
			}
			
			// Test if fall through. 
			// Also code to test for cyclic dependancies here or at least limit the number of recusions
			if (conversation.fallthrough && availableOptions==1){
				cycleCount++;
				if (cycleCount<100){
					GotoDialogue(firstDestinationName,firstDestination);
				}else{
					Debug.LogError("Parley: Conversation recursion above 100 items. One of your conversations in Parley has hit a more then 100's cycle falltrhough. This can happen when you create a cyclic falltrhough between a series of dialogs when each one will push the dialog to the next. We assume 100 deep is more then enough and you must have ment to do somthing else.");
					conversationText=Parley.GetInstance().EmbedEnviromentalInformation(conversationText);
				}
				cycleCount--;
			}else {
				// Replace tags
				conversationText=Parley.GetInstance().EmbedEnviromentalInformation(conversationText);
			}
		}
	}
	
	/**
	 * Select option will be called by you when the player chooses one of the currently available 
	 * Conversation Options. 
	 * 
	 * You will get a list of these Options from  GetCurrentConversationOptions when one is 
	 * chosen call this method.
	 * 
	 */
	protected void SelectOption(Option o){
		o.choosenb4=true;
		GotoDialogue(o.destinationDialogName,o.destinationId);
	}
	/**
	 * This returns the currently active Dialog instance.
	 * 
	 */
	protected Dialog GetDialog(){
		return dialog;
	}
	
	/**
	 * This returns the currently active Conversation instance. 
	 * 
	 * Don’t get the Text from here since it would be raw without any of the Environmental 
	 * information embedded yet.
	 * 
	 */
	protected Conversation GetCurrentConversation(){
		return conversation;
	}
	
	/**
	 * This returns all the current Options available at this stage in the Dialog. 
	 * 
	 * These need to presented to the user.
	 * 
	 */
	protected List<Option> GetCurrentConversationOptions(){
		if (currentOptions==null){
			currentOptions=new List<Option>();
        	foreach (Option o in conversation.options){
        		if (o._available){
					currentOptions.Add(o);
			  	}
        	}
		}
		return currentOptions;
	}
	
	/**
	 * This instructs the DialogGui to go back to the Return Dialog as defined in the Parley editor. 
	 * 
	 * You only need to use this if HasReturnConversation returns true.
	 * 
	 */
	protected void GotoReturnConversation(){
		if (HasReturnConversation()){
			GotoDialogue(conversation.returnDialogName,conversation.returnId);
		}
	}
	
	/**
	 * This returns a boolean letting you know if this Dialog has a return dialog.
	 * 
	 */ 
	protected bool HasReturnConversation(){
		return conversation.returnId!=-1 && conversation.returnId!=conversation.id;
	}
	
	/**
	 * This returns the current conversation Text. 
	 * 
	 * All environmental information is embedded into this text already. The text does not update live, 
	 * so the information embedded will be static after the choice was first made. However if the 
	 * player moves away from this Conversation and back the text will refresh.
	 * 
	 */ 
	protected string GetConversationText(){
		return conversationText;
	}
}
