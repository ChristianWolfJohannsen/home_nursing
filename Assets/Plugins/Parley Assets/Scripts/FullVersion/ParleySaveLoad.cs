using UnityEngine;
using System.IO;
using System.Collections.Generic;

/**
 * This script facilitates save and load of a Parley Scene.
 * 
 * This is included as part of the Full Version of Parley and not the free edition.
 */
public class ParleySaveLoad {
	public static void Save(Stream outputStream){
		StreamPacker sp=new StreamPacker(outputStream);
		Save(sp);
	}

	public static void Save(StreamPacker sp){
		
		// Pack all quests
		List<Quest> quests=Parley.GetInstance().GetCompletedQuests();
		sp.WriteInt(quests.Count);		
		foreach(Quest q in quests){
			PackQuest(sp,q);
		}
		
		quests=Parley.GetInstance().GetCurrentQuests();
		sp.WriteInt(quests.Count);		
		foreach(Quest q in quests){
			PackQuest(sp,q);
		}
		sp.WriteString("Pack1");
		
		// Pack all events
		HashSet<string> questEvents=Parley.GetInstance().GetQuestEventSet();
		string[] allEvents=new string[questEvents.Count];
		int c=0;
		foreach (string s in questEvents){
			allEvents[c++]=s;
		}
		sp.WriteStrings(allEvents);
		
		sp.WriteString("Pack2");
		
		// Pack all active quest events
		sp.WriteInt(Parley.GetInstance().GetActiveEvents().Count);
		foreach (string s in Parley.GetInstance().GetActiveEvents()){
			sp.WriteString(s);
		}
		sp.WriteString("Pack3");
	}
	
	public static void Load(Stream inputStream){
		StreamUnpacker su=new StreamUnpacker(inputStream);
		Load (su);
	}

	public static void Load(StreamUnpacker su){
		// Reset Scene quests 
		Parley.GetInstance().LoadSceneQuests();
		
		int n=su.ReadInt();
		Parley.GetInstance().GetCompletedQuests().Clear();
		for (int t=0;t<n;t++){
			Parley.GetInstance().GetCompletedQuests().Add(UnpackQuest(su));
		}
		
		n=su.ReadInt();
		Parley.GetInstance().GetCurrentQuests().Clear();
		for (int t=0;t<n;t++){
			Parley.GetInstance().GetCurrentQuests().Add(UnpackQuest(su));
		}
		
		// Bring quests in line
		Parley.GetInstance().SyncQuests();
		
		Debug.LogError(su.ReadString());
	
		// Unpack all events
		string[] allEvents=su.ReadStrings();
		HashSet<string> questEvents=Parley.GetInstance().GetQuestEventSet();
		questEvents.Clear();
		foreach (string s in allEvents){
			questEvents.Add(s);
		}

		Debug.LogError(su.ReadString());
		
		int activeEvents=su.ReadInt();
		Parley.GetInstance().GetActiveEvents().Clear();
		for (int t=0;t<activeEvents;t++){
			Parley.GetInstance().GetActiveEvents().Add(su.ReadString());
		}
		Debug.LogError(su.ReadString());
	}
	
	private static Quest UnpackQuest(StreamUnpacker su){
		Quest q=new Quest();
		q.name=su.ReadString();
		q.uniqueId=su.ReadInt();
		q.description=su.ReadString();
		q.handinDescription=su.ReadString();
		q.afterDescription=su.ReadString();
		q.questevent=su.ReadString();
		q.activeevent=su.ReadString();
		q.playerCommands=(Command[])su.ReadObjects(typeof(Command));

		q.questrequirement=su.ReadString();
		q.open=su.ReadBool();
		q.completed=su.ReadBool();
		q.status=su.ReadString();
		q.lastEffected=Time.time+su.ReadFloat();
		
		int n=su.ReadInt();
		for (int t=0;t<n;t++){
			Objective o=new Objective();
			q.objectives.Add(o);
			o.description=su.ReadString();
			o.doneDescription=su.ReadString();
			o.locationObject=su.ReadString();
			o.count=su.ReadInt();
			o.optional=su.ReadBool();
			o.objectiveevent=su.ReadString();
			o.questevent=su.ReadString();
			o.activeevent=su.ReadString();
			o.playerCommands=(Command[])su.ReadObjects(typeof(Command));
			o.questrequirement=su.ReadString();
			o.completed=su.ReadBool();
			o.open=su.ReadBool();
		}
		return q;
	}

	private static void PackQuest(StreamPacker sp,Quest q){
		sp.WriteString(q.name);
		sp.WriteInt(q.uniqueId);
		sp.WriteString(q.description);
		sp.WriteString(q.handinDescription);
		sp.WriteString(q.afterDescription);
		sp.WriteString(q.questevent);
		sp.WriteString(q.activeevent);
		sp.WriteObjects(q.playerCommands);
		sp.WriteString(q.questrequirement);
		sp.WriteBool(q.open);
		sp.WriteBool(q.completed);
		sp.WriteString(q.status);
		sp.WriteFloat(q.lastEffected-Time.time);

		sp.WriteInt(q.objectives.Count);
		foreach (Objective o in q.objectives){
			sp.WriteString(o.description);
			sp.WriteString(o.doneDescription);
			sp.WriteString(o.locationObject);
			sp.WriteInt(o.count);
			sp.WriteBool(o.optional);
			sp.WriteString(o.objectiveevent);
			sp.WriteString(o.questevent);
			sp.WriteString(o.activeevent);
			sp.WriteObjects(o.playerCommands);
			sp.WriteString(o.questrequirement);
			sp.WriteBool(o.completed);
			sp.WriteBool(o.open);
		}
	}

}
