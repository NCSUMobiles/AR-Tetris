/**
Written by: Jacques Gresset
jacques.gresset@gmail.com
June 2011 - NCSU CSC591 Mobile Apps
**/
using UnityEngine;
using System.Collections;

public class CollisionDetection : MonoBehaviour {
	//GamePlay parentObject;
	// Use this for initialization
	//private GamePlay gamePlayObject;
	void Start () {
		//generationScript = GameObject.Find("ImageTarget").GetComponent("RandomPieceGenerator") as RandomPieceGenerator;
		//parentObject = transform.parent.parent.GetComponent("GamePlay") as GamePlay;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	private void OnTriggerEnter(Collider other){
		Transform temp = transform.parent;
		temp = temp.parent;
		GamePlay parentObject = temp.GetComponent("GamePlay") as GamePlay;
		if(parentObject!=null){
			parentObject.colliding();
		}
	}
}
