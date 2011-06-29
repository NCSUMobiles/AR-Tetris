using UnityEngine;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps
**/
public class InstructionsScreen : MonoBehaviour {

	public Texture2D screenTexture;
	public Texture2D backButtonTexture;
	public GUISkin mySkin;
	private int textureWidth = 480;
	private int textureHeight = 800;
	
	public void OnGUI() { 
		float xPos = 2;
		float yPos = 2;
		GUI.skin = mySkin;
		GUILayout.BeginArea(new Rect(xPos,yPos,textureWidth,textureHeight));
		GUILayout.BeginVertical();
		GUI.Label(new Rect (0, 0, textureWidth, textureHeight), screenTexture);
		if (GUI.Button (new Rect (175, 695, 105, 60), backButtonTexture)) {
			Application.LoadLevel("StartScreen");
		}
		GUILayout.EndVertical();
		GUILayout.EndArea();
		GUI.skin = null;
	}
}
