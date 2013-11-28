using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(Parley))]
public class ParleyEditor : Editor {
	
	private SerializedProperty actProperty;
	private SerializedProperty sceneProperty;
	private SerializedProperty showDebugProperty;
	private SerializedProperty clearDebugProperty;
	private SerializedProperty questListProperty;

	void OnEnable () {
		actProperty = serializedObject.FindProperty("act");
		sceneProperty = serializedObject.FindProperty("scene");
		showDebugProperty = serializedObject.FindProperty("showDebug");
		clearDebugProperty = serializedObject.FindProperty("clearAtStartOfScene");
		questListProperty = serializedObject.FindProperty("questList");
	}
	
	public override void OnInspectorGUI() {
		serializedObject.Update();
		
		Parley parley = (Parley)target;

		bool showDebug=EditorGUILayout.Toggle("Show Debug Info",parley.showDebug,new GUILayoutOption[0]);
		bool clearFlag=EditorGUILayout.Toggle("Clear At Start",parley.clearAtStartOfScene,new GUILayoutOption[0]);
		
		int oldAct=actProperty.intValue;
		int oldScene=sceneProperty.intValue;
		
		int newAct=EditorGUILayout.Popup("Act",oldAct,ParleyMenu.GetActs(),new GUILayoutOption[0]);
		int newScene=EditorGUILayout.Popup("Scene",newAct!=oldAct?0:oldScene,ParleyMenu.GetActsScenes()[ParleyMenu.GetActs()[newAct]],new GUILayoutOption[0]);

		if (oldAct!=newAct || oldScene!=newScene || showDebugProperty.boolValue!=showDebug || clearDebugProperty.boolValue!=clearFlag){
			Undo.RegisterSceneUndo("Change Act and Scene");
			sceneProperty.intValue=newScene;
			actProperty.intValue=newAct;
			showDebugProperty.boolValue=showDebug;
			clearDebugProperty.boolValue=clearFlag;
			
			// Set new resources
			LoadQuests();
			
			if (serializedObject.ApplyModifiedProperties() || (Event.current.type == EventType.ValidateCommand && Event.current.commandName == "UndoRedoPerformed")){
				serializedObject.Update();
			}
		}
		
		if (GUILayout.Button("Reload Quests for scene")){
			Undo.RegisterSceneUndo("Reload Parley quests");
			LoadQuests();
			serializedObject.ApplyModifiedProperties();
		}
		
		parley.showEditorQuests=EditorGUILayout.Foldout(parley.showEditorQuests, "Quests");
		if (parley.showEditorQuests){
			for(int x = 0; x < questListProperty.arraySize; x++) {
				EditorGUILayout.ObjectField((TextAsset)questListProperty.GetArrayElementAtIndex(x).objectReferenceValue, typeof(TextAsset));
			}
		}
		
	}
	
	private void LoadQuests(){
		// Reset properties
		actProperty = serializedObject.FindProperty("act");
		sceneProperty = serializedObject.FindProperty("scene");
		questListProperty = serializedObject.FindProperty("questList");
		
		TextAsset[] questsList=ParleyMenu.GetActSceneQuests(ParleyMenu.GetActs()[actProperty.intValue],ParleyMenu.GetActsScenes()[ParleyMenu.GetActs()[actProperty.intValue]][sceneProperty.intValue]).ToArray();
		
		questListProperty.ClearArray();
		
		for (int x=0;x<questsList.Length;x++){
			questListProperty.InsertArrayElementAtIndex(x);
			questListProperty.GetArrayElementAtIndex(x).objectReferenceValue=questsList[x];
		}
	}

}
