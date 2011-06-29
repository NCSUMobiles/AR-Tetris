using UnityEngine;
using System;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps
**/
public class GameOver : MonoBehaviour {

	public Texture2D gameOverTexture;
	public GUISkin mySkin;
	public GUISkin smallerFontSkin;
	private int textureWidth = 480;
	private int textureHeight = 800;
	//private Vector2 scrollPosition = Vector2.zero;
	private String credits = "NCSU Summer 2011 CSC591 Mobile Apps\n\nYottatris Development Team\n---------------------------------------\n\nDesigner - Sam Brubaker\nEngineers - Jacques Gresset & Sonya Hedrick\nInstructors - Ben Watson & Pat FitzGerald\n\nAudio Clips by\nwww.SoundBible.com";
	
	public void OnGUI() {
		float xPos = 2;
		float yPos = 2;
		//Humm, these fonts/skins don't really work on the device.
		GUI.skin = mySkin;
		GUILayout.BeginArea(new Rect(xPos,yPos,textureWidth,textureHeight));
		GUILayout.BeginVertical();
		GUI.Label(new Rect (0, 0, textureWidth, textureHeight), gameOverTexture);
		//scrollPosition = GUI.BeginScrollView(new Rect(65,150,350,150),scrollPosition,new Rect(0,0,325,300));
		GUI.Label(new Rect(80,150,325,300), credits);
		//GUI.EndScrollView();
		GUI.skin = smallerFontSkin;
		if (GUI.Button (new Rect (30, 565, 200, 150), "Play \n Again?")) {
			Application.LoadLevel("Main");
		}
		GUI.skin = mySkin;
		if (GUI.Button (new Rect (255, 565, 200, 150), "Quit")) {
			Application.Quit();
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin = null;
	}	
}
