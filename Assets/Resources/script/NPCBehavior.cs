using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehavior : MonoBehaviour {
	public enum MovingMode{
		stunned, patrol
	}

	[Header("Settings")]

	[Header("Stun settings")]
	public float stunDuration = 1.0f;
	private float tmp_stunDuration;

	[Header("Only for debug use")]
	[Header("set dynamically")]
	public MovingMode movingMode = MovingMode.patrol;
	public GameObject gameManager;
	public GameObject teamController;

	// All components
    private Transform _trans;
    private Rigidbody _rigid;
    private Renderer _rend;
	private CommonBehavior _common;

	// Use this for initialization
	void Start () {
		_trans = gameObject.GetComponent<Transform>();
        _rigid = gameObject.GetComponent<Rigidbody>();
        _rend = gameObject.GetComponent<Renderer>();
		_common = this.GetComponent<CommonBehavior>();

		gameManager = GameObject.Find("GameManager");
		teamController = transform.parent.gameObject;

		tmp_stunDuration = stunDuration;
	}
	
	// Update is called once per frame
	void Update () {
		switch (movingMode)
		{
			case MovingMode.stunned:
				Stun();
				break;

		}
	}
	/*
	 * behavior during being stunned period
	 */
	private void Stun(){
		tmp_stunDuration -= Time.deltaTime;

		if (tmp_stunDuration < 0){
			SetMovingMode(MovingMode.patrol);
		}
		Debug.Log("up");
	}

	/*
     * Push the character away based on fromPosition
     * @param fromPosition The position that the push force comes
     */
    public void PushedAway(Vector3 fromPosition, float kickForce) {
        Vector3 direction = transform.position - fromPosition;
        direction.Normalize();
        _rigid.AddForce(direction * kickForce, ForceMode.VelocityChange);
    }


	/*
	 * being stunned
	 */
	public void Stunned(){
		_rigid.AddForce(0, 500.0f, 0);
		Debug.Log("up");
		SetMovingMode(MovingMode.stunned);
	}

	private void SetMovingMode(MovingMode mode){
		// Exit state
		switch (movingMode)
		{
			case MovingMode.stunned:
				_common.SetAnimationMode(CommonBehavior.AnimationMode.none);
				break;

		}
		movingMode = mode;
		
		// Entry state
		switch (movingMode)
		{
			case MovingMode.stunned:
				tmp_stunDuration = stunDuration;
				_common.SetAnimationMode(CommonBehavior.AnimationMode.stunned);
				break;
		}
	}

	/*
	 * be exploded by others
	 */
	public void BeExploded() {
		Debug.Log("kill NPC");
		Destroy(gameObject);
	}

}
