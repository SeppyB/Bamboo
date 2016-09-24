﻿using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {

	public GameObject ball;
	public Color color;
	public float runVelocity;
	public float jumpVelocity;
	public float dashVelocity;
	public float maxSpeed;
	public float ballPullModifier;
	public float ballAttractionRadius;
	public LayerMask layerMask;
	private Rigidbody2D playerRigidbody;
	private Rigidbody2D ballRigidbody;
	public bool canDoubleJump = false;
	public bool IsTouchingGround = false;
	public bool tryingToCatchBall = false;

	private Ball ballScript;

	// Use this for initialization
	void Start () {
		playerRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
		ballRigidbody = ball.gameObject.GetComponent<Rigidbody2D>();
		ballScript = ball.gameObject.GetComponent<Ball>();
	}
	
	// Update is called once per frame
	void Update () {
		IsTouchingGround = TouchingGround();
		if(Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)){
			//playerRigidbody.velocity = new Vector2(0, playerRigidbody.velocity.y);
		}

		if(Input.GetKey(KeyCode.DownArrow)){
			playerRigidbody.AddForce(new Vector3(0, -dashVelocity, 0));
		}

		else if(Input.GetKeyDown(KeyCode.Space) && (IsTouchingGround || canDoubleJump)){
			playerRigidbody.velocity = new Vector2(playerRigidbody.velocity.x, 0);
			playerRigidbody.AddForce(new Vector3(0, jumpVelocity, 0));

			if(!IsTouchingGround){
				canDoubleJump = false;
			}
		}

		playerRigidbody.AddForce(new Vector3(Input.GetAxis("Horizontal") * runVelocity, 0, 0));

		CapMaxSpeed();
		CatchBall();
	}

	void CatchBall(){
		tryingToCatchBall = Input.GetKey(KeyCode.LeftControl);
		Vector2 differenceBetweenPlayerAndBallPos = (playerRigidbody.transform.position - ball.transform.position);
	
		if(tryingToCatchBall && differenceBetweenPlayerAndBallPos.magnitude < ballAttractionRadius){
			ballRigidbody.AddForce(differenceBetweenPlayerAndBallPos * ballPullModifier);
		}

		if(Input.GetKeyDown(KeyCode.LeftAlt)) ballScript.Release(playerRigidbody.velocity);
	}

	void OnTriggerEnter2D(Collider2D collision){
		if(tryingToCatchBall && collision.gameObject == ball){
			ballScript.Caught(this.transform);
			ball.transform.position = this.transform.position + this.transform.up;
		}
	}

	void OnDrawGizmos(){
		Gizmos.DrawWireSphere(this.gameObject.transform.position, ballAttractionRadius);
	}

	bool TouchingGround(){
		RaycastHit2D hit = Physics2D.Raycast(playerRigidbody.transform.position, -playerRigidbody.transform.up, 0.7f, layerMask);

		if (hit.collider) canDoubleJump = true;

		return hit;
	}

	void CapMaxSpeed(){
		if(playerRigidbody.velocity.x > maxSpeed){
			playerRigidbody.velocity = new Vector2(maxSpeed, playerRigidbody.velocity.y);
		}
		else if(playerRigidbody.velocity.x < -maxSpeed){
			playerRigidbody.velocity = new Vector2(-maxSpeed, playerRigidbody.velocity.y);
		}
	}
}
