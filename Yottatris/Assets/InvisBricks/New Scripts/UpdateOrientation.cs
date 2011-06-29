/**
Written by: Jacques Gresset
jacques.gresset@gmail.com
June 2011 - NCSU CSC591 Mobile Apps
**/
using UnityEngine;
using System;

public class UpdateOrientation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	/*
	// Update is called once per frame
	void Update () {
		Vector3 tempVector = transform.localPosition;
		float x = tempVector.x*10;
		float y = tempVector.y*10;
		float z = tempVector.z*10;
		x = Convert.ToInt32(Math.Round(x));
		y = Convert.ToInt32(Math.Round(y));
		z = Convert.ToInt32(Math.Round(z));
		x=x/10;
		y=y/10;
		z=z/10;
		transform.localPosition = new Vector3(x,y,z);
		tempVector = transform.eulerAngles;
		x = tempVector.x/90;
		y = tempVector.y/90;
		z = tempVector.z/90;
		x = Convert.ToInt32(Math.Round(x));
		y = Convert.ToInt32(Math.Round(y));
		z = Convert.ToInt32(Math.Round(z));
		x = x*90;
		y = y*90;
		z = z*90;
		transform.eulerAngles = new Vector3(x,y,z);
	}
	*/
}
