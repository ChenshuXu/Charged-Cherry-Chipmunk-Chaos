using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonBehavior : MonoBehaviour {
	public enum AnimationMode {
		none, kicking, dashing, stunned
	}
	[Header("Settings")]
	[Header("Player Randerer/Animation settings")]
	public AnimationMode animationMode = AnimationMode.none;

	[Header("Animation booleans")]
	public float kickDuration = 0.2f;
	private float tmp_kickDuration;
	public bool isKicking;
	public bool isDashing;
	public bool isMoving;
	public bool isStunned;

	[Header("Only for debug use")]
	[Header("set dynamically")]
	public GameObject gameManager;
	public GameObject teamController;
    private Transform _trans;
    private Rigidbody _rigid;
    private Renderer _rend;

	// Use this for initialization
	void Start () {
		_trans = gameObject.GetComponent<Transform>();
        _rigid = gameObject.GetComponent<Rigidbody>();
        _rend = gameObject.GetComponent<Renderer>();

		gameManager = GameObject.Find("GameManager");
		teamController = transform.parent.gameObject;

		animationMode = AnimationMode.none;

		// Initialize animation booleans
		isKicking = false;
		isDashing = false;
		isMoving = false;
		isStunned = false;
		tmp_kickDuration = kickDuration;
	}
	
	// Update is called once per frame
	void Update () {

		switch (animationMode)
		{
			case AnimationMode.kicking:
				tmp_kickDuration -= Time.deltaTime;
				if (tmp_kickDuration <= 0){
					SetAnimationMode(AnimationMode.none);
				}
				break;
		}

		if (_rigid.velocity.magnitude > 0.1f)
        {
            isMoving = true;
        }
        else if (_rigid.velocity.magnitude <= 0.1f)
        {
            isMoving = false;
        }
	}

	/*
	 * State machine for animation mode
	 */
	public void SetAnimationMode(AnimationMode mode){
		// Exit state
		switch(animationMode)
		{
			case AnimationMode.dashing:
				isDashing = false;
				break;
			case AnimationMode.kicking:
				isKicking = false;
				break;
			case AnimationMode.stunned:
				isStunned = false;
				break;
			
		}

		animationMode = mode;

		// Enter state
		switch(animationMode)
		{
			case AnimationMode.dashing:
				isDashing = true;
				break;
			case AnimationMode.kicking:
				isKicking = true;
				tmp_kickDuration = kickDuration;
				break;
			case AnimationMode.stunned:
				isStunned = true;
				break;
		}
	}
}
