using UnityEngine;
using System;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps
**/
public class WrapUp : MonoBehaviour {

	public AudioClip explodeClip; //Downloaded from www.soundbible.com
	private RandomPieceGenerator generationScript;
	private ArrayList level0 = new ArrayList();
	private ArrayList level1 = new ArrayList();
	private ArrayList level2 = new ArrayList();
	private ArrayList level3 = new ArrayList();
	private ArrayList level4 = new ArrayList();
	private ArrayList level5 = new ArrayList();
	private ArrayList level6 = new ArrayList();
	private ArrayList level7 = new ArrayList();
	private ArrayList level8 = new ArrayList(); 
	private ArrayList level9 = new ArrayList();
	private ArrayList level10 = new ArrayList();
	private ArrayList level11 = new ArrayList();
	private const int MAX_LEVEL = 11;
	private int levelsComplete = 0;
	private bool levelsWereDestroyed = false;

	void Start () {
		generationScript = GameObject.Find("ImageTarget").GetComponent("RandomPieceGenerator") as RandomPieceGenerator;
		levelsComplete = 0;
		levelsWereDestroyed = false;
	}

	void OnGUI() {
		GUILayout.BeginArea(new Rect(0,0,200,200));
		GUILayout.BeginVertical();
		GUI.Label(new Rect (2, 2, 100, 30), "Levels Complete");
		GUI.Label(new Rect (32, 35, 32, 30), Convert.ToString(levelsComplete));
		GUILayout.EndVertical();
		GUILayout.EndArea();
	}
	
	public int getHighestLevelForXZ(float x, float z) {
		int roundedX = Convert.ToInt32(x * 10);
		int roundedZ = Convert.ToInt32(z * 10);
		int highestLevel = -1;
		for (int level = 0; level < MAX_LEVEL; level++) {
			ArrayList levelArray = getArray(level);
			foreach(GameObject brick in levelArray) {
				if (Convert.ToInt32(brick.transform.position.x * 10) == roundedX
				&& Convert.ToInt32(brick.transform.position.z * 10) == roundedZ) {
					if (highestLevel < level) highestLevel = level;
					break;
				}
			}
		}
		return highestLevel;
	}
	
	public void wrapUp(){
		StartCoroutine(wrapUp1());
	}
	//Called by other Class
	private IEnumerator wrapUp1() {
		print("Got to Wrap Up");
		if (generationScript.clone.gameObject.transform.localPosition.y >= 1.3f) { 
			print("Game over!");
			Application.LoadLevel("GameOverScreen");
				
		}
		else{
		Transform physicalPart = generationScript.clone.Find("PhysicalPart").transform;
		foreach(Transform child in physicalPart){
			if (child.name == "BaseBlock") {				
				int level = Convert.ToInt32(Math.Round(child.transform.position.y));
				ArrayList levelArray = getArray(level);  //Get array for level (y-value) of the cube
				levelArray.Add(child.gameObject);
				GhostBricks ghostScript = child.gameObject.GetComponent("GhostBricks") as GhostBricks;
				ghostScript.destroyGhost();
			}
		}
		

		while(physicalPart.childCount > 0) {
			foreach(Transform child in physicalPart){
				if (child.name == "BaseBlock") child.parent = null;
			}
		}

		Destroy(generationScript.clone.gameObject);

		//Destroy complete levels
		levelsWereDestroyed = false;
		destroyLevel(0);
		destroyLevel(1);
		destroyLevel(2);
		destroyLevel(3);
		destroyLevel(4);
		destroyLevel(5);
		destroyLevel(6);
		destroyLevel(7);
		destroyLevel(8);
		destroyLevel(9);
		destroyLevel(10);
		destroyLevel(11);
		
		if (levelsWereDestroyed) {
			if(GameObject.Find("ARCamera")!=null){
				GameObject.Find("ARCamera").audio.PlayOneShot(explodeClip);
			}
			else{
				GameObject.Find("Camera").audio.PlayOneShot(explodeClip);
			}
			yield return new WaitForSeconds(.75f);
		}
		
		consolidateLevels();	
		generationScript.generateNewPiece();
		}
	}
	
	private void destroyLevel(int levelNumber) {
		ArrayList levelArray = getArray(levelNumber);
		if(levelArray.Count!=0){
			print("Level " + levelNumber + " has " +levelArray.Count);
		}
		if (levelArray.Count < 36) return;
		foreach (GameObject cube in levelArray) {
			Destroy(cube);
		}

		levelArray.Clear();	
		levelsComplete++;
		levelsWereDestroyed = true;
	}	
	
	private void consolidateLevels() {
		int emptyLevel = getLowestEmptyLevel(0);
		int nonEmptyLevel = MAX_LEVEL + 1;
		while (emptyLevel < MAX_LEVEL) {
			for (int ctr=emptyLevel+1;ctr<=MAX_LEVEL;ctr++) {
				if (getArray(ctr).Count == 0) continue;
				nonEmptyLevel = ctr;
				break;
			}
			if (nonEmptyLevel <= MAX_LEVEL) copyLevel(emptyLevel, nonEmptyLevel);
			emptyLevel = getLowestEmptyLevel(emptyLevel+1);
			nonEmptyLevel = MAX_LEVEL + 1;
		}
	}
	
	private int getLowestEmptyLevel(int startLevel) {
		for (int ctr=startLevel;ctr<=MAX_LEVEL;ctr++) {
			if (getArray(ctr).Count == 0) return ctr;
		}
		
		return MAX_LEVEL + 1;
	}
	
	private void copyLevel(int lowest, int nextLowest) {
		ArrayList lowestLevel = getArray(lowest);
		lowestLevel.Clear();
		ArrayList nextLevel = getArray(nextLowest);
		foreach (GameObject cube in nextLevel) {
			lowestLevel.Add(cube);
			cube.transform.localPosition = new Vector3(cube.transform.localPosition.x,cube.transform.localPosition.y - (nextLowest - lowest), cube.transform.localPosition.z);
		}		
		nextLevel.Clear();
	}

	private ArrayList getArray(int levelNumber) {
		if (levelNumber == 0) return level0;
		if (levelNumber == 1) return level1;
		if (levelNumber == 2) return level2;
		if (levelNumber == 3) return level3;
		if (levelNumber == 4) return level4;
		if (levelNumber == 5) return level5;
		if (levelNumber == 6) return level6;
		if (levelNumber == 7) return level7;
		if (levelNumber == 8) return level8;
		if (levelNumber == 9) return level9;
		if (levelNumber == 10) return level10;
		return level11;
	}
	
}
