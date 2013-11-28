using UnityEngine;
using System.Collections;
using System.IO;

public class SaveLoadGui : MonoBehaviour {
	
	private const int NUMBER_OF_GAMES=5;
	
	private bool showMenu=false;

	public GUISkin menuSkin;
	
	private bool[] usedSlots=new bool[NUMBER_OF_GAMES];
	private string[] saveFiles=new string[NUMBER_OF_GAMES];
	
	public GameObject[] packableObjects=new GameObject[0];
	
	enum State {
		Menu,
		Save,
		Load
	}
	
	private State state=State.Menu;
	
	void Start(){
		
		// Check what save games there are
		string basePath=GetSavedGameFolder();
		for (int t=0;t<NUMBER_OF_GAMES;t++){
			saveFiles[t]=basePath+"\\game."+t;
			usedSlots[t]=System.IO.File.Exists(saveFiles[t]);
		}
		
		// Make sure all GameObjects added support PackUnpackable
		foreach (GameObject go in packableObjects){
			PackUnpackableBehaviour[] pus=(PackUnpackableBehaviour[])go.GetComponents<PackUnpackableBehaviour>();
			if (pus==null || pus.Length==0){
				pus=(PackUnpackableBehaviour[])go.GetComponentsInChildren<PackUnpackableBehaviour>(true);
				if (pus==null || pus.Length==0){
					Debug.LogWarning("SaveLoad: "+go.name+" and its children do not implement PackUnpackableBehaviour in any scripts");
				}
			}
		}
	}
	
	private void HideMenu(){
		showMenu=false;
		Parley.GetInstance().SetInGui(false);
	}
	
	void Update () {
		if (!showMenu && Input.GetKey(KeyCode.Escape) && !Parley.GetInstance().IsInGui()){
			showMenu=true;
			state=State.Menu;
			Parley.GetInstance().SetInGui(true);
		}
	}
	
	public string GetSavedGameFolder(){
		string basePath=null;
		if (Application.platform==RuntimePlatform.WindowsEditor || Application.platform==RuntimePlatform.WindowsPlayer){ 
			basePath=System.Environment.GetEnvironmentVariable("APPDATA")+"\\Igg";
			System.IO.Directory.CreateDirectory(basePath);
		} else if (Application.platform==RuntimePlatform.WindowsEditor || Application.platform==RuntimePlatform.WindowsPlayer){ 
			basePath=System.Environment.GetEnvironmentVariable("HOME")+"/Igg";
			System.IO.Directory.CreateDirectory(basePath);
		} else {
			Debug.LogError("Parley: Parley load and save ahs not been implemented for "+Application.platform);
		}

		return basePath;
	}
	
	public bool IsThereASaveGame(int gamenumber){
		return usedSlots[gamenumber];
	}
	
	public void OnGUI(){
		if (!showMenu){
			return;
		}
		
		if (menuSkin!=null){
			GUI.skin=menuSkin;
		}
		
		switch (state){
		case State.Menu:
			ShowMenu();
			break;
		case State.Save:
			ShowSave();
			break;
		case State.Load:
			ShowLoad();
			break;
		}
	}
	
	public void ShowMenu(){
		int y=100;
		int x=Screen.width/2-50;
		
		if (GUI.Button(new Rect(x,y,100,30),"Save Game")){
			state=State.Save;
		}
		y+=40;
		bool somthingsaved=false;
		for (int t=0;!somthingsaved && t<NUMBER_OF_GAMES;t++){
			somthingsaved=IsThereASaveGame(t);
		}
		if (somthingsaved){
			if (GUI.Button(new Rect(x,y,100,30),"Load Game")){
				state=State.Load;
			}
			y+=40;
		}
		if (GUI.Button(new Rect(x,y,100,30),"Close Menu")){
			HideMenu();
		}
		y+=40;
		if (GUI.Button(new Rect(x,y,100,30),"Exit")){
			Application.LoadLevel("An Igg Story Menu");
		}
		y+=40;
	}

	public void ShowSave(){
		int y=100;
		int x=Screen.width/2-50;

		for (int t=0;t<NUMBER_OF_GAMES;t++){
			if (GUI.Button(new Rect(x,y,100,30),IsThereASaveGame(t)?"Overwrite "+(t+1):"Save In "+(t+1))){
				Save(saveFiles[t]);
				usedSlots[t]=true;
				HideMenu();
			}
			y+=40;
		}
		if (GUI.Button(new Rect(x,y,100,30),"Return")){
			state=State.Menu;
		}
	}

	public void ShowLoad(){
		int y=100;
		int x=Screen.width/2-50;

		for (int t=0;t<NUMBER_OF_GAMES;t++){
			if (IsThereASaveGame(t)){
				if (GUI.Button(new Rect(x,y,100,30),"Load Slot "+(t+1))){
					Load(saveFiles[t]);
					HideMenu();
				}
				y+=40;
			}
		}
		if (GUI.Button(new Rect(x,y,100,30),"Return")){
			state=State.Menu;
		}
	}
	
	public void Save(string file){
		FileStream fs = new FileStream(file, FileMode.Create);
		
		StreamPacker sp=new StreamPacker(fs);
		
		// Save Parley
		ParleySaveLoad.Save(sp);
		
		sp.WriteString("All is well");
		
		// Save all the Objects
		foreach (GameObject go in packableObjects){
			PackUnpackableBehaviour[] pus=(PackUnpackableBehaviour[])go.GetComponents<PackUnpackableBehaviour>();
			if (pus!=null && pus.Length>0){
				foreach (PackUnpackableBehaviour pu in pus){
					sp.WriteString(pu.GetType().Name);
					pu.Pack(sp);
				}
			}
			pus=(PackUnpackableBehaviour[])go.GetComponentsInChildren<PackUnpackableBehaviour>(true);
			if (pus!=null && pus.Length>0){
				foreach (PackUnpackableBehaviour pu in pus){
					sp.WriteString(pu.GetType().Name);
					pu.Pack(sp);
				}
			}
		}
		
		fs.Close();
	}

	public void Load(string file){
		FileStream fs = new FileStream(file, FileMode.Open);
		
		StreamUnpacker su=new StreamUnpacker(fs);
		
		// Save Parley
		ParleySaveLoad.Load(su);
		
		Debug.Log(su.ReadString());
		// Save all the Objects
		foreach (GameObject go in packableObjects){
			PackUnpackableBehaviour[] pus=(PackUnpackableBehaviour[])go.GetComponents<PackUnpackableBehaviour>();
			Debug.Log("Parley: Loading details for object "+go.name);
			if (pus!=null && pus.Length>0){
				foreach (PackUnpackableBehaviour pu in pus){
					Debug.Log("Parley: Loading details for script "+pu.GetType().Name);
					Debug.Log("Parley: Saved Was "+su.ReadString());
					pu.Unpack(su);
				}
			}
			pus=(PackUnpackableBehaviour[])go.GetComponentsInChildren<PackUnpackableBehaviour>(true);
			if (pus!=null && pus.Length>0){
				foreach (PackUnpackableBehaviour pu in pus){
					Debug.Log("Parley: Loading details for script "+pu.GetType().Name);
					Debug.Log("Parley: Saved Was "+su.ReadString());
					pu.Unpack(su);
				}
			}
		}
		
		fs.Close();
	}
}
