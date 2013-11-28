using UnityEngine;
using System.Collections;

public class TestGui : MonoBehaviour {

	public GUISkin testSkin=null;
	
	private float width=800;
	private float height=600;
	
	private bool toggle1=true;
	private bool toggle2=false;
	
	private float slider1=0;
	private float slider2=.7f;
	private float slider3=1f;
	private float slider4=.3f;
	
	private string text1="Text Field";
	private string text2="Text Area";
	private Vector2 textScrollpos=new Vector2();
	
	private Vector2 scrollpos=new Vector2();
	
	// Update is called once per frame
	void OnGUI () {
		if (testSkin==null){
			return;
		}
		
		
		GUI.skin=testSkin;
		
		float x=(Screen.width-width)/2;
		float y=(Screen.height-height)/2;
		Rect windowRect=new Rect(x,y,width,height);
		
		// Draw Window
		GUILayout.BeginArea(windowRect,"Window",GUI.skin.window);
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		DrawDetails();
		DrawScroll();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal(GUI.skin.box);
		DrawVerticleSliders();
		DrawHorizontalSliders();
		GUILayout.EndHorizontal();
		DrawButtons();
		GUILayout.EndVertical();

		GUILayout.EndArea();
	}
	
	private void DrawDetails(){
		// Draw Box with Labels
		GUILayout.BeginVertical("Details",GUI.skin.box,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(false)});
		GUILayout.BeginHorizontal();
		GUILayout.Label("Text Label 1");
		text1=GUILayout.TextField(text1);
		GUILayout.EndHorizontal();
		GUILayout.Label("Text Label 2");
		textScrollpos=GUILayout.BeginScrollView(textScrollpos,false,false,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
		text2=GUILayout.TextArea(text2,120,new GUILayoutOption[]{GUILayout.ExpandHeight(true)});
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal();
		toggle1=GUILayout.Toggle(toggle1,"Toggle 1");
		toggle2=GUILayout.Toggle(toggle2,"Toggle 2");
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	private void DrawScroll(){
		// Draw Scroll with Labels
		scrollpos=GUILayout.BeginScrollView(scrollpos,false,false,new GUILayoutOption[]{GUILayout.ExpandHeight(true),GUILayout.ExpandWidth(true)});
		GUILayout.Label(
			"Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.\n\n"+
			"Sed ut perspiciatis unde omnis iste natus error sit voluptatem accusantium doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo inventore veritatis et quasi architecto beatae vitae dicta sunt explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur aut odit aut fugit, sed quia consequuntur magni dolores eos qui ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed quia non numquam eius modi tempora incidunt ut labore et dolore magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut aliquid ex ea commodi consequatur? Quis autem vel eum iure reprehenderit qui in ea voluptate velit esse quam nihil molestiae consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla pariatur?"
		);
		GUILayout.EndScrollView();
	}
	
	private void DrawVerticleSliders(){
		// Draw Sliders
		GUILayout.BeginVertical();
		GUILayout.BeginHorizontal();
		slider1=GUILayout.VerticalSlider(slider1,0,1f);
		slider2=GUILayout.VerticalSlider(slider2,0,1f);
		slider3=GUILayout.VerticalSlider(slider3,0,1f);
		slider4=GUILayout.VerticalSlider(slider4,0,1f);
		GUILayout.EndHorizontal();
		GUILayout.EndVertical();
	}
	
	private void DrawHorizontalSliders(){
		// Draw Sliders
		GUILayout.BeginVertical();
		slider1=GUILayout.HorizontalSlider(slider1,0,1f);
		slider2=GUILayout.HorizontalSlider(slider2,0,1f);
		slider3=GUILayout.HorizontalSlider(slider3,0,1f);
		slider4=GUILayout.HorizontalSlider(slider4,0,1f);
		GUILayout.EndVertical();
	}
	
	private void DrawButtons(){
		// Draw Sliders
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Button("Button1",new GUILayoutOption[]{GUILayout.ExpandHeight(false),GUILayout.ExpandWidth(true)});
		GUILayout.Button("Button2",new GUILayoutOption[]{GUILayout.ExpandHeight(false),GUILayout.ExpandWidth(true)});
		GUILayout.EndVertical();
	}

}
