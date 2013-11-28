using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.IO;

public class ParleyMenu {
	private static string PARLEY_FOLDER=Path.DirectorySeparatorChar+"Assets"+Path.DirectorySeparatorChar+"Plugins"+Path.DirectorySeparatorChar+"Parley Assets"+Path.DirectorySeparatorChar+"Editor";
	private static string DEFAULT_WORK_FOLDER=Path.DirectorySeparatorChar+"Assets"+Path.DirectorySeparatorChar+"Parley";

	private static string WORK_FOLDER_KEY="WorkingFolder";

	private static Dictionary<string,string> settings=new Dictionary<string, string>();
	
	private static bool CheckFolders(){
#if UNITY_STANDALONE_WIN
		string file = "parley.exe";
#else
		string file = "parley.jar";
#endif
		string directory = Directory.GetCurrentDirectory()+PARLEY_FOLDER;
		
		// Looks like it was moved lets serch for it
		if (!File.Exists(directory+Path.DirectorySeparatorChar+file)){
			string foundIn=SearchDirectory(Directory.GetCurrentDirectory(),file);
			if (foundIn!=null){
				PARLEY_FOLDER=foundIn.Substring(Directory.GetCurrentDirectory().Length);
			}
		}
		
		if (!File.Exists(Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+file)){
			Debug.LogWarning("Parley not found in expected folder, searching project.");
			ShowCanNotFindFilesMessage();
			return false;
		}
		
		return true;
	}
	
	private static string SearchDirectory(string directory,string file){
		string[] files=Directory.GetFiles(directory,file);
		Debug.LogWarning(directory+"="+files.Length);
		if (files.Length==1){
			return directory;
		}
		
		string[] directories=Directory.GetDirectories(directory);
		foreach (string d in directories){
			string found=SearchDirectory(d,file);
			if (found!=null){
				return found;
			}
		}
		return null;
	}
	
	private static string GetConfigFile(){
		return Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"ParleyConfig.properties";
	}
	
	private static void SaveConfig(){
		if (!CheckFolders()){
			return;
		}
		string[] values=new string[settings.Count];
		
		int count=0;
		foreach (string k in settings.Keys){
			values[count++]=k+"="+settings[k];
			Debug.Log(k+"="+settings[k]);
		}
		
		File.WriteAllLines(GetConfigFile(),values);
	}
	
	private static void LoadConfig(){
		if (!CheckFolders()){
			return;
		}
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
	
	private static string GetWorkingFolder(){
		string workingFolder=DEFAULT_WORK_FOLDER;
		if (settings.ContainsKey(WORK_FOLDER_KEY)){
			workingFolder=settings[WORK_FOLDER_KEY];
			workingFolder=workingFolder.Replace(Path.AltDirectorySeparatorChar,Path.DirectorySeparatorChar);
		}
		return Directory.GetCurrentDirectory()+workingFolder;
	}
	
	[MenuItem("Parley/Start Editor #&p")]
    public static void StartParley () { 
		if (!CheckFolders()){
			return;
		}
		System.Diagnostics.Process proc=new System.Diagnostics.Process();
		proc.EnableRaisingEvents=false;
		
		LoadConfig();
		string workingFolder=GetWorkingFolder();

#if UNITY_STANDALONE_WIN
		proc.StartInfo.FileName = Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"parley.exe";
#else
		proc.StartInfo.FileName = Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"parley.jar";
#endif
		proc.StartInfo.Arguments = "\""+workingFolder+"\" \""+Directory.GetCurrentDirectory()+PARLEY_FOLDER+Path.DirectorySeparatorChar+"Dictionary"+Path.DirectorySeparatorChar+"english\"";
		
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
	
	private static void ShowCanNotFindFilesMessage(){
#if UNITY_STANDALONE_WIN
		string file = "parley.exe";
#else
		string file = "parley.jar";
#endif
		EditorUtility.DisplayDialog("Unable to start Parley.","The parley '"+file+"' file can not be found in your project. Did you import it correctly.","Ok");
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
		string workingFolder=GetWorkingFolder();
		
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
	
	private static string[] acts;
	private static Dictionary<string,string[]> actScenes;
	
	private static string GetActSceneDirectory(String act,String scene){
		return GetWorkingFolder()+Path.DirectorySeparatorChar+act+Path.DirectorySeparatorChar+scene;
	}
	
	public static void UpdateActsAndScenes(){
		string folder=GetWorkingFolder();
		Dictionary<string,string[]> acts=new Dictionary<string,string[]>();
		List<string> actList=new List<string>();
		// Tests acts
		foreach (string a in Directory.GetDirectories(folder)){
			List<string> scenes=new List<string>();
			// Tests scenes
			foreach (string s in Directory.GetDirectories(a)){
				bool sceneFolder=false;
				// Look for quests or dialog files to mark this as a scene
				foreach (string f in Directory.GetFiles(s)){
					string fn=f.Substring(s.Length+1);
					if ((fn.StartsWith("ply.") || fn.StartsWith("qst.")) && fn.EndsWith(".txt")){
						sceneFolder=true;
						break;
					}
				}
				if (sceneFolder){
					string scene=s.Substring(a.Length+1);
					scenes.Add(scene);
				}
			}
			
			if (scenes.Count>0){
				string act=a.Substring(folder.Length+1);
				acts.Add(act,scenes.ToArray());
				actList.Add(act);
			}
		}
		ParleyMenu.acts=actList.ToArray();
		ParleyMenu.actScenes=acts;
	}
	
	public static Dictionary<string,string[]> GetActsScenes(){
		UpdateActsAndScenes();
		return actScenes;
	}
	
	public static string[] GetActs(){
		UpdateActsAndScenes();
		return acts;
	}	

	public static List<TextAsset> GetActSceneQuests(String act,String scene){
		string asdirectory=GetActSceneDirectory(act,scene).Substring(Directory.GetCurrentDirectory().Length+1);
		List<TextAsset> quests=new List<TextAsset>();
		
		foreach (string f in Directory.GetFiles(asdirectory)){
			string fn=f.Substring(asdirectory.Length+1);
			if (fn.StartsWith("qst.") && fn.EndsWith(".txt")){
				TextAsset quest=(TextAsset)AssetDatabase.LoadMainAssetAtPath(f);
				quests.Add(quest);
			}
		}
		
		return quests;
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
