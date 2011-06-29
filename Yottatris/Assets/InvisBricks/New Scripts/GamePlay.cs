/**
Written by: Jacques Gresset (ghost bricks and audio by Sonya - sonyahedrick@yahoo.com)
jacques.gresset@gmail.com
June 2011 - NCSU CSC591 Mobile Apps
**/
using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class GamePlay : MonoBehaviour {
	//create variables
	public AudioClip goodMoveClip;
	public AudioClip badMoveClip;
	public AudioClip whooshClip;
	public AudioClip landingClip;
	private Transform physicalPart;
	private Transform ARCamera;
	private bool collision;
	private Mutex movementMutex;
	//private Mutex collisionMutex;
	private WrapUp wrapUpScript;
	private float nextMove;
	private Vector2 finger1;
	private int numTouching;
	//private Vector2 finger2;
	//private int finger1Id;
	//private int finger2Id;
	private int numFingers;
	// Use this for initialization 
	private int quartile;
	private int testTimer;
	private bool isMoving;
	IEnumerator Start () {
		isMoving = false;
		numTouching = 0;
		quartile = 0;
		//ARCamera = transform.parent;
		//ARCamera = ARCamera.GetComponent("ARCamera");
		//ARCamera = GameObject.Find("Camera").transform;
		
		if(GameObject.Find("ARCamera") != null){
			ARCamera = GameObject.Find("ARCamera").transform;
		}
		else{
			ARCamera = GameObject.Find("Camera").transform;
		}
		transform.Find("PhysicalPart").gameObject.SetActiveRecursively(false);
		testTimer = 0;
		numFingers = 0;
		//finger1Id = -1;
		nextMove = 0.0f;
		movementMutex = new Mutex(false);
		//collisionMutex = new Mutex(false);
		wrapUpScript = GameObject.Find("ImageTarget").GetComponent("WrapUp") as WrapUp;
		collision = false;
		physicalPart = transform.Find("PhysicalPart").transform;
		generateGhosts();
		//Act as a constructor
		//while it is not colliding
		while(true){
			if(!collision){
				StartCoroutine(drop());
			}
			//call the drop method every 1 second
			yield return new WaitForSeconds(1.5f);
		}
	}
	
	// Update is called once per frame
	void Update () {
		/*
		int x = Convert.ToInt32(ARCamera.position.x);
		int z = Convert.ToInt32(ARCamera.position.z);
		double gameRatio = Math.Abs((x*1.0d)/(z*1.0d));
		int gameAngle = Convert.ToInt32(Math.Atan(gameRatio)*(180/Math.PI)); //xangle
		print(gameAngle);
		*/
		testTimer++;
		//handles the User Input
		//if the system is not ready for a move
			//return;
		if(Time.time < nextMove){
			return;
		}
		//Parse the Touches
			//Elaborate Touch algorithm
			//Call touchAnalyzer
		foreach(Touch touch in Input.touches){
			if(touch.phase == TouchPhase.Began){
				if(numFingers == 0){
					finger1 = new Vector2(0,0);
				}
				numFingers++;
				numTouching++;
			}
			else if(touch.phase == TouchPhase.Moved){
				//may need to modify for new touch stuff
				finger1 += touch.deltaPosition;
				
			}
			else if(touch.phase == TouchPhase.Ended){
				numTouching--;
				if(numTouching == 0){
					touchAnalyzer(finger1.x, finger1.y, numFingers);
					numFingers = 0;
				}
			
			
			
			}
		
		}
		//Read Keyboard Input
		if(Input.GetButton("A")){
			nextMove = Time.time +.2f;
			sideSwitch(1);
		}
		else if(Input.GetButton("S")){
			nextMove = Time.time +.2f;
			sideSwitch(-2);
		}
		else if(Input.GetButton("D")){
			nextMove = Time.time +.2f;
			sideSwitch(-1);
		}
		else if(Input.GetButton("W")){
			nextMove = Time.time +.2f;
			sideSwitch(2);
		}
		else if(Input.GetButton("1")){
			nextMove = Time.time +.2f;
			sideSwitch(3);
		}
		else if(Input.GetButton("4")){
			nextMove = Time.time +.2f;
			sideSwitch(-3);
		}
		else if(Input.GetButton("2")){
			nextMove = Time.time +.2f;
			sideSwitch(4);
		}
		else if(Input.GetButton("5")){
			nextMove = Time.time +.2f;
			sideSwitch(-4);
		}
		else if(Input.GetButton("3")){
			nextMove = Time.time +.2f;
			sideSwitch(5);
		}
		else if(Input.GetButton("6")){
			nextMove = Time.time +.2f;
			sideSwitch(-5);
		}
		else if(Input.GetButton("Space")){
			nextMove = Time.time + 2.7f;
			StartCoroutine(slamToBottom());
		}
		
		
	}
	void FixedUpdate(){
		
	}
	
	private void touchAnalyzer(float xIn, float yIn, int fingers){
		//TODO tweak
		//Parse the inputs
			//call sideswitch
		float xDelta = xIn;
		float yDelta = yIn;
		
		if(fingers == 1){
			//TODO get rid of invalid inputs
			double screenRatio = Math.Abs((xDelta*1.0d)/(yDelta*1.0d));
			int screenAngle = Convert.ToInt32(Math.Atan(screenRatio)*(180/Math.PI)); //OFFSET FROM TOP
			int quad = 0;
			//quad represents the quadrent in which the swipe vectors were pointing
			if(xDelta > 0){
				if(yDelta >0){
					quad = 1;
				}
				else{
					quad = 4;
				}
			}
			else{
				if(yDelta >0){
					quad = 2;
				}
				else{
					quad = 3;
				}
			}
			int angle = screenAngle;
			
			if(quad == 2|| quad == 4){
				angle = 90-screenAngle;
			}
			//angle now is the normalized rotation of the angle around the screen
			int x = Convert.ToInt32(ARCamera.position.x);
			int z = Convert.ToInt32(ARCamera.position.z);
			double gameRatio = Math.Abs((x*1.0d)/(z*1.0d));
			int gameAngle = Convert.ToInt32(Math.Atan(gameRatio)*(180/Math.PI)); //xangle
			int locQuad = 0;
			if(x<0){
				if(z<0){
					locQuad = 3;
				}
				else{
					locQuad = 2;
				}
			}
			else{
				if(z<0){
					locQuad = 4;
				}
				else{
					locQuad = 1;
				}
			}
			int angleFromPivot = gameAngle;
			if(locQuad == 1 ||locQuad ==3){
				angleFromPivot = 90-gameAngle;
				
			}
			//TODO fix purge
			int inner = 44;
			int outer = 45;
			if(angle>angleFromPivot+inner || angle + inner< angleFromPivot){ //invalid input
				//return;
			}
			
			float yLength = Math.Abs(yDelta);
			float xLength = Math.Abs(xDelta);
			
			
			if(locQuad==4){//in quadrant4 of game
				if(quad ==1){//Touch in quadrant 1
		
					
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveForward());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
						
					}
					
				}
				else if(quad == 2){//Touch in quadrant2
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						
						StartCoroutine(moveLeft());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
						
					}
				}
				else if(quad ==3){
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveBack());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
						
					}
				}
				else{
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveRight());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
						
					}
				}
			}
			
			else if(locQuad==1){
				if(quad ==1){//Touch in quadrant 1
		
					
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveLeft());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
						
					}
					
				}
				else if(quad == 2){//Touch in quadrant2
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						
						StartCoroutine(moveBack());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
						
					}
				}
				else if(quad ==3){
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveRight());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
						
					}
				}
				else{
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveForward());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
						
					}
				}
			}
			
			else if(locQuad==2){//in quadrant4 of game
				if(quad ==1){//Touch in quadrant 1
		
					
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveBack());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
						
					}
					
				}
				else if(quad == 2){//Touch in quadrant2
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						
						StartCoroutine(moveRight());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
						
					}
				}
				else if(quad ==3){
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveForward());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
						
					}
				}
				else{
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveLeft());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
						
					}
				}
			}
			//TODO
			else{
				if(quad ==1){//Touch in quadrant 1
		
					
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveRight());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
						
					}
					
				}
				else if(quad == 2){//Touch in quadrant2
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveRight());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						
						StartCoroutine(moveForward());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveForward());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
						
					}
				}
				else if(quad ==3){
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveForward());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveLeft());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveLeft());
							return;
						}
						
					}
				}
				else{
					if(angleFromPivot<inner){
						if(yLength>xLength){
							StartCoroutine(moveLeft());
							return;
						}
						else{
							StartCoroutine(moveBack());
							return;
						}
					}
					else if(angleFromPivot>=inner && angleFromPivot<=outer){
						//StartCoroutine(slamToBottom());
						StartCoroutine(moveBack());
						return;
					}
					else{
						if(yLength>xLength){
							StartCoroutine(moveBack());
							return;
						}
						else{
							StartCoroutine(moveRight());
							return;
						}
						
					}
				}
			}
			
			
			
		}
		else if(fingers ==2){
			int x = Convert.ToInt32(ARCamera.position.x);
			int z = Convert.ToInt32(ARCamera.position.z);
			double gameRatio = Math.Abs((x*1.0d)/(z*1.0d));
			int gameAngle = Convert.ToInt32(Math.Atan(gameRatio)*(180/Math.PI)); //xangle
			int locQuad = 0;
			if(x<0){
				if(z<0){
					locQuad = 3;
				}
				else{
					locQuad = 2;
				}
			}
			else{
				if(z<0){
					locQuad = 4;
				}
				else{
					locQuad = 1;
				}
			}
			int angleFromPivot = gameAngle;
			if(locQuad == 1 ||locQuad ==3){
				angleFromPivot = 90-gameAngle;
			}
			if(angleFromPivot>45){
				locQuad++;
				locQuad = locQuad%4;
			}
			
			
			if(Math.Abs(xDelta)>Math.Abs(yDelta)){
				if(xDelta<0){
					StartCoroutine(rotateYPositive());
				}
				else{
					StartCoroutine(rotateYNegative());
				}
			}
			else{
				
				if(locQuad == 1){
					if(yDelta>0){
						StartCoroutine(rotateZPositive());
					}
					else{
						StartCoroutine(rotateZNegative());
					}
				}
				else if(locQuad == 2){
					if(yDelta>0){
						StartCoroutine(rotateXNegative());
					}
					else{
						StartCoroutine(rotateXPositive());
					}
				}
				else if(locQuad == 3){
					if(yDelta<0){
						StartCoroutine(rotateZPositive());
					}
					else{
						StartCoroutine(rotateZNegative());
					}
				}
				else{
					if(yDelta<0){
						StartCoroutine(rotateXNegative());
					}
					else{
						StartCoroutine(rotateXPositive());
					}
				}
				
				
			}
			
		}
		else if(fingers == 3){
			//StartCoroutine(slamToBottom());
		}
	}
	
	//Parses the input to determine which move to perform
	private void sideSwitch(int inputNum){
		quartileCalculator();
		//read ARCAMERA rotation and call correct movement according to it
		int temp = inputNum;
		if(quartile == 0){
		}
		else if(quartile == 1){
			//L = B
			if(temp == 1){
				temp = 2;
			}
			//R = F
			else if(temp == -1){
				temp = -2;
			}
			//B = R
			else if(temp == -2){
				temp = 1;
			}
			//F = L
			else if(temp == 2){
				temp = -1;
			}
			//x+ = z+
			else if(temp == 3){
				temp = 5;
			}
			//x- = z-
			else if(temp == -3){
				temp = -5;
			}
			//z+ = x-?
			else if(temp ==5){
				temp = -3;
			}
			//z- = x+?
			else if(temp == -5){
				temp = 3;
			}
		}
		
		else if(quartile == 2){
			if(temp !=4 || temp !=-4){
				temp = -temp;
			}
			//L = R
			//R = L
			//B = F
			//F = B
		}
		
		else if(quartile == 3){
			//L = F
			if(temp == 1){
				temp = -2;
			}
			//R = B
			else if(temp == -1){
				temp = 2;
			}
			//B = L
			else if(temp == -2){
				temp = -1;
			}
			//F = R
			else if(temp == 2){
				temp = 1;
			}
			//x+ = z-
			else if(temp == 3){
				temp = -5;
			}
			//x- = z
			else if(temp == -3){
				temp = 5;
			}
			//z+ = x?
			else if(temp ==5){
				temp = 3;
			}
			//z- = x-?
			else if(temp == -5){
				temp = -3;
			}
			
		}
		//print("temp = "+temp + " quartile = "+quartile);
		
		
		if(temp == 1){
			StartCoroutine(moveLeft());
		}
		else if(temp == -1){
			StartCoroutine(moveRight());
		}
		else if(temp == 2){
			StartCoroutine(moveForward());
		}
		else if(temp == -2){
			StartCoroutine(moveBack());
		}
		else if(temp == 3){
			StartCoroutine(rotateXPositive());
		}
		else if(temp == -3){
			StartCoroutine(rotateXNegative());
		}
		else if(temp == 4){
			StartCoroutine(rotateYPositive());
		}
		else if(temp == -4){
			StartCoroutine(rotateYNegative());
		}
		else if(temp == 5){
			StartCoroutine(rotateZPositive());
		}
		else if(temp == -5){
			StartCoroutine(rotateZNegative());
		}
	}
	//CALLED BY GHOSTBRICK COLLISION
	public void colliding(){
		//nextMove = Time.time +.3f;
		collision = true;
		//print("GOT TO GAMEPLAY "+testTimer+" Updates later");
		//colliding = true
		//determine if this is all the code needed
	}
	
	
	private void finalCollision(){
		//reorients the position
		//activate "hidden" components
		transform.Find("PhysicalPart").gameObject.SetActiveRecursively(true);
		//call wrapUp function;
		wrapUpScript.wrapUp();
	}
	
	//Dropping 
	private IEnumerator drop(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").Translate(Vector3.up*-1f,Space.World);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").Translate(Vector3.up*1f,Space.World);
		//print("Moved Invis Back");

		if(!collision){
			transform.Translate(Vector3.up*-1f,Space.World);
			//movementMutex.ReleaseMutex();		
			isMoving = false;
		}
		else{
			ARCamera.audio.PlayOneShot(landingClip);
			finalCollision();
		}
		}
		// graps a movement mutex
		// drops ghost piece 1 unit
		// if collision occurs
			// STOP ALL MOVEMENT
			// call finalCollision()
			// put collision =false
		// else
			// return ghost piece to original spot (could just move the regular one only)
			// move all pieces down 1 unit
			// release mutex
	}
	
	private IEnumerator moveLeft(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").Translate(Vector3.right*-1f,Space.World);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").Translate(Vector3.right*1f,Space.World);
		//print("Moved Invis Back");
		if(!collision){
			transform.Translate(Vector3.right*-1f,Space.World);
			ARCamera.audio.PlayOneShot(goodMoveClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		//return null;

		drawGhostBricks();
		
	}
	
	private IEnumerator moveRight(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").Translate(Vector3.right*1f,Space.World);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").Translate(Vector3.right*-1f,Space.World);
		//print("Moved Invis Back");
		if(!collision){
			transform.Translate(Vector3.right*1f,Space.World);
			ARCamera.audio.PlayOneShot(goodMoveClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex

		drawGhostBricks();
		
	}
	
	private IEnumerator moveForward(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").Translate(Vector3.forward*1f,Space.World);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").Translate(Vector3.forward*-1f,Space.World);
		//print("Moved Invis Back");
		if(!collision){
			transform.Translate(Vector3.forward*1f,Space.World);
			ARCamera.audio.PlayOneShot(goodMoveClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();
		
	}
	
	private IEnumerator moveBack(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").Translate(Vector3.forward*-1f,Space.World);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").Translate(Vector3.forward*1f,Space.World);
		//print("Moved Invis Back");
		if(!collision){
			transform.Translate(Vector3.forward*-1f,Space.World);
			ARCamera.audio.PlayOneShot(goodMoveClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();
		
	}
	
	private IEnumerator rotateXPositive(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.right,90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.right,-90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.right,90);
			transform.RotateAround(transform.position,Vector3.right,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();		

	}
	
	private IEnumerator rotateXNegative(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.right,-90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.right,90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.right,-90);
			transform.RotateAround(transform.position,Vector3.right,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.right,-22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex

		drawGhostBricks();
		
	}
	
	private IEnumerator rotateYPositive(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.up,90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.up,-90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.up,90);
			transform.RotateAround(transform.position,Vector3.up,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();
		
	}
	
	private IEnumerator rotateYNegative(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.up,-90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.up,90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.up,-90);
			transform.RotateAround(transform.position,Vector3.up,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.up,-22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex

		drawGhostBricks();
		
	}
	
	private IEnumerator rotateZPositive(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.forward,90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.forward,-90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.forward,90);
			transform.RotateAround(transform.position,Vector3.forward,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();
		
	}
	
	private IEnumerator rotateZNegative(){
		while(isMoving){
			yield return new WaitForFixedUpdate();
		}
		isMoving = true;
		lock(movementMutex){
		//movementMutex.WaitOne();
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.forward,-90);
		testTimer=0;
		//print("got to yield");
		yield return new WaitForFixedUpdate();
		yield return new WaitForFixedUpdate();
		//print("Moved Invis Left");
		transform.Find("InvisPart").RotateAround(transform.position,Vector3.forward,90);
		//print("Moved Invis Back");
		if(!collision){
			//transform.RotateAround(transform.position,Vector3.forward,-90);
			transform.RotateAround(transform.position,Vector3.forward,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,-22.5f);
			yield return new WaitForFixedUpdate();
			transform.RotateAround(transform.position,Vector3.forward,-22.5f);
			ARCamera.audio.PlayOneShot(whooshClip);
		}
		else{
			ARCamera.audio.PlayOneShot(badMoveClip);
		}
		collision = false;
		}
		isMoving = false;
		//print("Moved LEFT");
		//movementMutex.ReleaseMutex();
		// grab movement mutex
		// move ghost piece 1 unit
		// undo ghost move
		// if collision occurs
			// put collision =false
			// return;
		// else
			// move all pieces 1 unit
			
		// release mutex
		
		drawGhostBricks();
		
	}
	
	private void quartileCalculator(){
	
	//TODO need to acount for threshold stuff
		
		//TODO may need to handle World orientation
		int rot = Convert.ToInt32(ARCamera.eulerAngles.y);
		//print("ARCamera = " +rot);
		rot+=360;
		rot = rot%360;
		//if y axis rotation is from 226 to 315
		// if y axis rotation is from 316 to 45
		if(rot>315 && rot <=45){
			quartile = 0;
		}
		// if y axis rotation is from 46 to 135
		if(rot>45 && rot <=135){
			 quartile = 1;
		}
		// if y axis rotation is from 136 to 225
		if(rot>135 && rot <=225){
			 quartile = 2;
		}
		if(rot>225 && rot <=315){
			quartile = 3;
		}
	
	}


	private IEnumerator slamToBottom(){
		while(true){
			StartCoroutine(drop());
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
			yield return new WaitForFixedUpdate();
		}
	}

	private void generateGhosts() {
		foreach(Transform brick in physicalPart) {
			GhostBricks ghostScript = brick.gameObject.GetComponent("GhostBricks") as GhostBricks;
			ghostScript.generateGhost();
		}
	}

	private void drawGhostBricks() {
		foreach(Transform brick in physicalPart) {
			GhostBricks ghostScript = brick.gameObject.GetComponent("GhostBricks") as GhostBricks;
			ghostScript.drawMe();
		}
	}




}
