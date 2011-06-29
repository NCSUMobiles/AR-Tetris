using UnityEngine;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps
**/
public class StartScreen : MonoBehaviour {

	public Texture2D screenTexture;
	public Texture2D playButtonTexture;
	public Texture2D instructionsButtonTexture;
	public GUISkin mySkin;
	public GUISkin largeLabelSkin;
	private int textureWidth = 480;
	private int textureHeight = 800;
	
	public void OnGUI() { 
		float xPos = 2;
		float yPos = 2;
		GUI.skin = mySkin;
		GUILayout.BeginArea(new Rect(xPos,yPos,textureWidth,textureHeight));
		GUILayout.BeginVertical();
		GUI.Label(new Rect (0, 0, textureWidth, textureHeight), screenTexture);
		if (GUI.Button (new Rect (150, 525, 175, 100), playButtonTexture)) {
			Application.LoadLevel("Main");
		}
		if (GUI.Button (new Rect (150, 650, 175, 65), instructionsButtonTexture)) {
			Application.LoadLevel("InstructionsScreen");
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin = null;
	}
}
