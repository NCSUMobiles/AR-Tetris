using UnityEngine;
using System;
using System.Collections;

/**
Written by: Sonya Hedrick 
sonyahedrick@yahoo.com
June 2011 - NCSU CSC591 Mobile Apps
**/
public class RandomPieceGenerator : MonoBehaviour {

	public Transform piece0; 
	public Transform piece1;  
	public Transform piece2;  
	public Transform piece3;   
	public Transform piece4;  
	public Transform piece5;  
	public Transform piece6;  
	public Transform piece7;  
	public Transform imageTarget;
	public Transform clone;
	public Transform ARCamera;
	
	void Start() {
		generateNewPiece();
	}
	
	public void generateNewPiece() {
		int pieceType = randomlyChoosePiece();
		if (pieceType == 0) clone = (Transform) Instantiate(piece0, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 1) clone = (Transform) Instantiate(piece1, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 2) clone = (Transform) Instantiate(piece2, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 3) clone = (Transform) Instantiate(piece3, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 4) clone = (Transform) Instantiate(piece4, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 5) clone = (Transform) Instantiate(piece5, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 6) clone = (Transform) Instantiate(piece6, new Vector3(0, 0, 0), Quaternion.identity);		
		if (pieceType == 7) clone = (Transform) Instantiate(piece7, new Vector3(0, 0, 0), Quaternion.identity);		
		
		clone.parent = imageTarget;
		clone.localPosition = new Vector3(.05f,1.4f,-.05f);
	}

	private int randomlyChoosePiece() {
		System.Random randomNumber = new System.Random();
		return randomNumber.Next(8); //returns a number from 0 to 7
	}	
}
