using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class ParleyEditor {
	private static string PARLEY_FOLDER=Path.DirectorySeparatorChar+"Assets"+Path.DirectorySeparatorChar+"Parley Assets"+Path.DirectorySeparatorChar+"Editor";
	private static string DEFAULT_WORK_FOLDER=Path.DirectorySeparatorChar+"Assets"+Path.DirectorySeparatorChar+"Parley";

	private static string WORK_FOLDER_KEY="WorkingFolder";

	private static Dictionary<string,string> settings=new Dictionary<string, string>();

	private static string GetConfigFile(){
		return Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"ParleyConfig.properties";
	}
	
	private static void SaveConfig(){
		string[] values=new string[settings.Count];
		
		int count=0;
		foreach (string k in settings.Keys){
			values[count++]=k+"="+settings[k];
			Debug.Log(k+"="+settings[k]);
		}
		
		File.WriteAllLines(GetConfigFile(),values);
	}
	
	private static void LoadConfig(){
		string configFile=GetConfigFile();
		if (File.Exists(configFile)){
			// Load config
			string[] lines=File.ReadAllLines(configFile);
			foreach (string line in lines){
				string l=line.Trim();
				if (!l.StartsWith("#")){
					int i=l.IndexOf("=");
					if (i!=-1){
						string n=l.Substring(0,i).Trim();
						string v=l.Substring(i+1).Trim();
						settings[n]=v;
					}
				}
			}
		}
	}
	
	private static string GetWrokingFolder(){
		string workingFolder=DEFAULT_WORK_FOLDER;
		if (settings.ContainsKey(WORK_FOLDER_KEY)){
			workingFolder=settings[WORK_FOLDER_KEY];
			workingFolder=workingFolder.Replace(Path.AltDirectorySeparatorChar,Path.DirectorySeparatorChar);
		}
		return Directory.GetCurrentDirectory()+workingFolder;
	}
	
	[MenuItem("Parley/Start Editor #&p")]
    public static void StartParley () { 
		System.Diagnostics.Process proc=new System.Diagnostics.Process();
		proc.EnableRaisingEvents=false;
		
		LoadConfig();
		string workingFolder=GetWrokingFolder();

#if UNITY_STANDALONE_WIN
		proc.StartInfo.FileName = Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"parley.exe";
#else
		proc.StartInfo.FileName = Directory.GetCurrentDirectory()+PARLEY_FOLDER+"/parley.jar";
#endif
		proc.StartInfo.Arguments = "\""+workingFolder+"\" \""+Directory.GetCurrentDirectory()+PARLEY_FOLDER+"\"";
		
		Debug.Log(proc.StartInfo.FileName);
		Debug.Log(proc.StartInfo.Arguments);
		
		// Identify is Java is installed. If not then offer to download it.
		try {
			if (!proc.Start()){
				Debug.LogError("Unable to execute '"+proc.StartInfo.FileName+" "+proc.StartInfo.Arguments+"'");
				ShowNeedJavaMessage();
			}
		} catch (Exception e){
			Debug.LogException(e);
			ShowNeedJavaMessage();
		}
	}
	
	private static void ShowNeedJavaMessage(){
		if (EditorUtility.DisplayDialog("Unable to start Parley. Java Required.","Parley requires you have Java installed on your system to run. Would you like to open your web browser to the download page?","Yes Please","No Thanks")){
			Application.OpenURL("http://java.com/en/download/index.jsp");
		}
	}
	
	[MenuItem("Parley/User Guide")]
    public static void OpenParleyUserGuide () { 
		System.Diagnostics.Process proc = new System.Diagnostics.Process();
		proc.EnableRaisingEvents=false; 
		proc.StartInfo.FileName = Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"UserGuide.pdf";
		// Identify is PDF Viewer is installed. If not then offer to download it.
		try {
			if (!proc.Start()){
				ShowNeedPDFMessage();
			}
		} catch (Exception e){
			Debug.LogException(e);
			ShowNeedPDFMessage();
		}
	}
	
	private static void ShowNeedPDFMessage(){
		if (EditorUtility.DisplayDialog("Unable to start open UserGuid. PDF Viewer Required.","The USER Guide is a PDF document and requires you have a PDF Viewer installed on your system to run. Would you like to open your web browser to the download page?","Yes Please","No Thanks")){
			Application.OpenURL("http://get.adobe.com/reader/");
		}
	}
	
	[MenuItem("Parley/Forums")]
    public static void OpenParleyForums () { 
		Application.OpenURL("http://forums.celestial-games.com/viewforum.php?f=9&sid=8c26f366fb848e23d21cbcfda6711a45");
	}
	
	[MenuItem("Parley/Pick Working Folder")]
    public static void PickWorkingFolder () { 
		LoadConfig();
		string workingFolder=GetWrokingFolder();
		
		//string folderName=workingFolder.Substring(workingFolder.LastIndexOf(""+Path.DirectorySeparatorChar)+1);
		
		string newfolder=EditorUtility.SaveFolderPanel("Parley working folder",workingFolder,null);
		
		if (newfolder!=null && newfolder.Length>0){
			newfolder=newfolder.Replace(Path.AltDirectorySeparatorChar,Path.DirectorySeparatorChar);
			Debug.Log(newfolder);
			
			// Test that this is still inside the untiy working structure
			if (newfolder.StartsWith(Directory.GetCurrentDirectory())){
				newfolder=newfolder.Substring(Directory.GetCurrentDirectory().Length);
				
				// Save this folder
				settings.Remove(WORK_FOLDER_KEY);
				settings.Add(WORK_FOLDER_KEY,newfolder);
				SaveConfig();
			}else{
				EditorUtility.DisplayDialog("Invalid Working Folder.","The folder you pick must be inside the project space of your Unity project.","Ok");
			}
		}
		
	}

	/*
	// Add Quests to a GameObject
	[MenuItem("Parley/Add Quests To Gameobject")]
    public static void AddPArleyQuests () { 
		
        var window = GetWindow(typeof(ParleyEditor));
        window.position = new Rect(0,0,180,80);
        window.Show();
	}

	public Parley script;
	public List<string> scenes;
	public int index=0;
	
	public void Init(){
		script=GetParleyObject();
		scenes=GetAllScenes();
	}
	
	public void OnGUI() {
        index = EditorGUI.Popup(
            new Rect(0,0,position.width, 20),
            "Scene:",
            index, 
            scenes.ToArray());
                
        if(GUI.Button(new Rect(0,25,position.width, position.height - 26),"Add Component")){
            //AddComponentToObjects();
	    }
	}
	
	static private Parley GetParleyObject(){
		Parley script=null;
		
		// Find selected Object
		GameObject go=Selection.activeGameObject;
		if (go==null){
			EditorUtility.DisplayDialog("No Scene Object Selected!","You need to select the object in the scene that has the Parley script added to it.", "Ok");
		}else{
			// Find PGame script
			script=(Parley)go.GetComponent(typeof(Parley));
			if (script==null){
				EditorUtility.DisplayDialog("No Parley script found!","The selected object in scene does not include a Parley script attached.", "Ok");
			}
		}
		
		return script;
	}
	
	static private List<string> GetAllScenes(){
		List<string> scenes=new List<string>();
		
		string[] directories=System.IO.Directory.GetDirectories(Directory.GetCurrentDirectory()+"/Assets/Parley");
		List<TextAsset> texts=new List<TextAsset>();
		foreach (string d in directories){
			string[] sceneDirectories=System.IO.Directory.GetDirectories(d);
			foreach (string sd in sceneDirectories){
				Debug.Log(sd);
				scenes.Add(sd);
			}
		}
		
		return scenes;
	}*/

}
