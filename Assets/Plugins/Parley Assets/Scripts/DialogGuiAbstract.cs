using UnityEngine;
using System.Collections.Generic;

public abstract class DialogGuiAbstract : MonoBehaviour {
	
	private Dialog dialog;
	
	private Camera oldCamera=null;
	
	private string conversationText=null;
	private Conversation conversation=null;
	private List<Option> currentOptions=null;
	protected int uniqueId;
	
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
			
			// Try calling the camera dolly
			dialog.dialogCamera.gameObject.SendMessage("SwitchCamera",SendMessageOptions.DontRequireReceiver);
			
			if (!dialog.dialogCamera.gameObject.activeSelf){
				oldCamera.gameObject.SetActive(false);
				dialog.dialogCamera.gameObject.SetActive(true);
			}
		}
		
		// Broadcast to player and this object that we have started a dialog
		GameObject.FindWithTag("Player").BroadcastMessage("DialogStarted",dialog,SendMessageOptions.DontRequireReceiver);
		BroadcastMessage("DialogStarted",dialog,SendMessageOptions.DontRequireReceiver);
		Parley.GetInstance().SetCurrentDialog(dialog);
		
		// Start at the correct dialog
		GotoDialogue(null,dialog.GetConversationIndex()); 
	}
	
	/**
	 * This returns true if its suitable to show debug information on screen in game.
	 */
	protected bool MustShowDebugInformation(){
		return Debug.isDebugBuild && Parley.GetInstance().showDebug;
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
	 * This creates a 'fake' Dialog for the quest. Placing the appropriate information
	 * in the text and creating the suitable options for accept/hand in and complete.
	 * 
	 * fromId is the originating conversationId never the fake on.
	 * uniqueId is the quest unique id.
	 * type is used to determin the state of teh quest dialog. Ultimatly it is the request.
	 */
	private void GotoQuest(int fromId,int uniqueId, Option.OptionType type){
		// Find the quest
		Quest quest=Parley.GetInstance().GetQuest(uniqueId);
		if (quest!=null){
			
			// Create Conversation
			Conversation questConversation=new Conversation();
			questConversation.id=conversation.id; // We use this so any return will go to the original conversation.
			questConversation.returnId=-1; // We do not have a standard return since we can not trust all implementaitons will support return.
			questConversation.returnDialogName=null;
			questConversation.uniqueId=uniqueId; // Borrow from the Quest uniqueId since they are unique across all quests and conversations
			questConversation._available=true;
			questConversation._seen=true; //
			
			// Create options list
			currentOptions=new List<Option>();
			
			// Quest can be in one of three states.
			// Available, Busy or completed ready for handin.
			if (type==Option.OptionType.QuestComplete){
				// Complete quest and show completed text
				quest.CompleteQuest();
				questConversation.text=quest.afterDescription;
				
			} else if (type==Option.OptionType.QuestAccept){
				// Start quest
				quest.StartQuest();
				// Now return to other conversation
				GotoDialogue(null,fromId);
				return;
			} else if (type==Option.OptionType.Conversation){
				questConversation.text=quest.description;
				
				// This should create two options Accept or Complete
				if (quest.readyToHandIn){
					questConversation.text=quest.handinDescription;
					
					Option acceptOption=new Option();
					acceptOption.choosenb4=true;
					acceptOption.destinationDialogName=null;
					acceptOption.destinationId=quest.uniqueId;
					acceptOption.displaytext="Hand In";
					acceptOption.quest=true;
					acceptOption.type=Option.OptionType.QuestComplete;
					currentOptions.Add(acceptOption);
				}else{
					questConversation.text=quest.description+"\n\n"+quest.GetObjectivesSummary();
					
					// Should we still accept this quest
					if (quest.available && !quest.open){
						Option acceptOption=new Option();
						acceptOption.choosenb4=true;
						acceptOption.destinationDialogName=null;
						acceptOption.destinationId=quest.uniqueId;
						acceptOption.displaytext="Accept";
						acceptOption.quest=true;
						acceptOption.type=Option.OptionType.QuestAccept;
						currentOptions.Add(acceptOption);
					}
				}
			}
			
			Option returnOption=new Option();
			returnOption.choosenb4=true;
			returnOption.destinationDialogName=null;
			returnOption.destinationId=conversation.id;
			returnOption.displaytext="Return";
			returnOption.quest=false;
			returnOption.type=Option.OptionType.Conversation;
			
			currentOptions.Add(returnOption);
			
			// Set up conversation in dialog
			conversation=questConversation;
			conversationText=Parley.GetInstance().EmbedEnviromentalInformation(conversation.text);
		} else {
			Debug.LogError("Parley: Quest not found #"+uniqueId);
		}
	}

	/**
	 * Goto Dialog sets the current Dialog to a specific Conversataion state. The
	 * ID is in face the Array location of the conversation. This will typically come from
	 * the Options.
	 * 
	 * When a new dialog is selected it will fire off the game and player events and flag itself as 
	 * seen.
	 * 
	 * Additionally if will fall through if needed.
	 * 
	 */
	private void GotoDialogue(string dialogName,int index){
		// Is this a jump to aouther dialog?
		if (dialogName!=null && dialogName.Length>0 && !dialog.name.Equals(dialogName)){
			EndDialog();
			
			Dialog d=null;
			
			// We should search our siblings first if we can not find the dialog there we should cehck the Parley singlton.
			if (gameObject.transform.parent!=null){
				int siblings=gameObject.transform.parent.GetChildCount();
				for (int t=0;t<siblings;t++){
					GameObject sibling=gameObject.transform.parent.GetChild(t).gameObject;
					Dialog siblingDialog=sibling.GetComponent<Dialog>();
					if (siblingDialog!=null && siblingDialog.GetDialogName()==dialogName){
						Debug.Log("Dialog'"+dialogName+"' was found in siblings");
						d=siblingDialog;
						break;
					}
				}
			}
			
			// The dialog needed was not found in siblings request from Parley
			if (d==null){
				d=Parley.GetInstance().GetDialogs()[dialogName];
			}
			
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
			}
			
			// Refresh each dialog option from this one.
			int availableOptions=0;
			int firstDestination=-1;
			string firstDestinationName="";
			int optionNumer=0;
			int questStatus=0; // 0 = none, 1=active, 2=available, 3=turnin.
			foreach (Option o in conversation.options){
				Dialog odialog=dialog;
				if (o.destinationDialogName!=null && o.destinationDialogName.Length>0 && !dialog.name.Equals(o.destinationDialogName)){
					if (Parley.GetInstance().GetDialogs().ContainsKey(o.destinationDialogName)){
						odialog=Parley.GetInstance().GetDialogs()[o.destinationDialogName];
					}else{
						Debug.LogError("The dialog is looking for a remote dialog '"+o.destinationDialogName+"' but that dialog is not currently attached to any scene objects");
					}
				}
				
				if (odialog==null){
					Debug.LogError("Parley: Could not find linked dialog '"+o.destinationDialogName+"' in scene. Ignoring Option.");
				}else{
					if (o.quest){
						Quest quest=Parley.GetInstance().GetQuest(o.destinationId);
						if (quest!=null){
							o._available=quest.available && !quest.completed;
						}else{
							o._available=false;
							Debug.LogError("Parley: Conversation #"+conversation.uniqueId+" option "+optionNumer+" leads to Quest #"+o.destinationId+" which can not be found in the scene.");
						}
					}else{
						odialog.GetConversations()[o.destinationId].UpdateAvailability();
						o._available=odialog.GetConversations()[o.destinationId]._available;
					}
					if (o._available){
						availableOptions++;
						if (firstDestination==-1){
							firstDestination=o.destinationId;
							firstDestinationName=o.destinationDialogName;
						}
					}
				}
				optionNumer++;
			}
			
			// Test if fall through. 
			// Also code to test for cyclic dependancies here or at least limit the number of recusions
			if (conversation.fallthrough && ((availableOptions==1 && conversation.returnId==-1) || (availableOptions==0 && conversation.returnId!=-1))){
				int id=conversation.returnId==-1?firstDestination:conversation.returnId;
				string dname=conversation.returnId==-1?firstDestinationName:conversation.returnDialogName;
				cycleCount++;
				if (cycleCount<100){
					GotoDialogue(dname,id);
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
		
		uniqueId=conversation.uniqueId;
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
		
		// Check if this is a quest accept/complete or decline option.
		if (o.quest){
			Debug.Log("Parley: Selecting Quest "+o.destinationId);
			GotoQuest(conversation.id, o.destinationId,o.type);
		}else{
			Debug.Log("Parley: Selecting Conversation "+o.destinationId);
			GotoDialogue(o.destinationDialogName,o.destinationId);
		}
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
		if (currentOptions==null && conversation.options!=null){
			currentOptions=new List<Option>();
        	foreach (Option o in conversation.options){
        		if (o._available){
					o.displaytext=Parley.GetInstance().EmbedEnviromentalInformation(o.text);
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
		if (MustShowDebugInformation() && conversation!=null){
			return conversationText+" <#"+conversation.uniqueId+">";
		}
		return conversationText;
	}
}
