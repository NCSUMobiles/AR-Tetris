using UnityEngine;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps

The  ghost bricks could not be drawn during update because of the way that the game play script was written.
**/
public class GhostBricks : MonoBehaviour {

	public Transform ghostBrickPrefab;
	private Transform myGhostBrick;
	private WrapUp wrapUpScript;
	private bool drawGhost = true;
	
	public void generateGhost() {
		myGhostBrick = (Transform) Instantiate(ghostBrickPrefab, new Vector3(0, 0, 0), Quaternion.identity);		
		wrapUpScript = GameObject.Find("ImageTarget").GetComponent("WrapUp") as WrapUp;	
		drawMe();
	}
	 
	public void drawMe() {
		if (!drawGhost) return;
		int highestCompletedBrickWhereIAm = wrapUpScript.getHighestLevelForXZ(transform.position.x,transform.position.z);
		myGhostBrick.position = new Vector3(transform.position.x,highestCompletedBrickWhereIAm+1,transform.position.z);
	}
	
	public void destroyGhost() {
		Destroy(myGhostBrick.gameObject);
		drawGhost = false;
	}
}
