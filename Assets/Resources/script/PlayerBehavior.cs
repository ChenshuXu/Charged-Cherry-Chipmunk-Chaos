using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBehavior : MonoBehaviour {
	public enum MovingMode {
    	control, sprint, stunned
    }

	public MovingMode movingMode = MovingMode.control;

    public enum teamSetting
    {
		teamRed = 1,
        teamBlue = 2,
        teamYellow = 3,
        teamGreen = 4,
	}

	[Header("team settings")]
	public teamSetting team;

	[Header("Moving speed settings")]
	public float normalSpeed = 5;
    public float rotAmt = 100;

	[Header("Sprint settings")]
	public float sprintDuration = 0.2f;
	public float sprintCoolDownTime = 1.0f;
	private float tmp_sprintDuration;
	private float tmp_sprintCoolDownTime;
	public float sprintSpeed = 4.0f;
    public bool isSprinting;

	[Header("Kick settings")]
	public float kickCoolDownTime = 1.0f;
	public float kickRange = 8.0f;
	public float kickForce = 2.5f;
	private float tmp_kickCoolDownTime;
    public bool isKicking;

	[Header("Stun settings")]
	public float stunDuration = 1.0f;
	private float tmp_stunDuration;

	[Header("Explosion settings")]
	public float explisionRadius = 5.0f;
	public float explosionPower = 10f;

	[Header("Only for debug use")]
	[Header("Input name settings (set dynamically)")]
	public string horizontalInputName;
	public string verticalInputName;
	public string sprintInputName;
	public string kickInputName;
	
	// All components
    private Transform _trans;
    private Rigidbody _rigid;
    private Renderer _rend;
	private CommonBehavior _common;

	private float _vx;
	private float _vy;

	

	// Use this for initialization
	void Start () {
		_trans = gameObject.GetComponent<Transform>();
        _rigid = gameObject.GetComponent<Rigidbody>();
        _rend = gameObject.GetComponent<Renderer>();
		_common = this.GetComponent<CommonBehavior>();

		movingMode = MovingMode.control;
		

		// Initialize tmp variables
		tmp_sprintDuration = sprintDuration;
		tmp_sprintCoolDownTime = sprintCoolDownTime;
		tmp_kickCoolDownTime = kickCoolDownTime;
		tmp_stunDuration = stunDuration;

		// Initialize input names
		horizontalInputName = "Horizontal_P" + ((int)team).ToString();
		verticalInputName = "Vertical_P" + ((int)team).ToString();
		sprintInputName = "Sprint_P" + ((int)team).ToString();
		kickInputName = "Kick_P" + ((int)team).ToString();

		
	}
	
	// Update is called once per frame
	void Update () {

		switch (movingMode)
		{
			// move
			case MovingMode.control:
				Control();
				break;
			
			case MovingMode.sprint:
				Sprint();
				break;
			
			case MovingMode.stunned:
				Stun();
				break;

		}

	}

	private void Control(){
		_vx = Input.GetAxis(horizontalInputName);
		_vy = Input.GetAxis(verticalInputName);

		Vector3 vel = new Vector3(_vx, 0, _vy);
		if (vel.magnitude > 1) {
			vel.Normalize();
		}

		_rigid.velocity = vel * normalSpeed * 2.5f;
		//_rigid.AddForce(vel * normalSpeed * 10.0f);

		// rotate to face the move direction
		if (_vx != 0 || _vy != 0) {
			float z_angle = Mathf.Atan(_vx / _vy) * Mathf.Rad2Deg;
			if (_vy < 0) {
				z_angle -= 180;
			}
			_trans.rotation = Quaternion.Euler(new Vector3(0, z_angle, 0));
		}

		// sprint
		if (tmp_sprintCoolDownTime > 0){
			tmp_sprintCoolDownTime -= Time.deltaTime;
		}

		if (Input.GetButton(sprintInputName) && tmp_sprintCoolDownTime <= 0){
			Debug.Log("spring");
			SetMovingMode(MovingMode.sprint);
		}

		// kick
		if (tmp_kickCoolDownTime > 0){
			tmp_kickCoolDownTime -= Time.deltaTime;
		}

		if (Input.GetButton(kickInputName) && tmp_kickCoolDownTime <= 0){
			Debug.Log("kick");
			Kick();
		}
	}

	private void Sprint(){
		tmp_sprintDuration -= Time.deltaTime;
        isSprinting = true;

		if (tmp_sprintDuration < 0){
			SetMovingMode(MovingMode.control);
		}

		Vector3 vel = _rigid.velocity;
		vel.Normalize();
		_rigid.AddForce(vel * sprintSpeed, ForceMode.VelocityChange);
        isSprinting = false;
	}

	private void Kick(){
        isKicking = true;

		_common.SetAnimationMode(CommonBehavior.AnimationMode.kicking);
		for (float angle = -35; angle < 35; angle++) {
			float x = Mathf.Sin(angle);
			float y = Mathf.Cos(angle);
			Vector3 dir = new Vector3(x, 0, y);
			dir += transform.forward;

			RaycastHit[] hitFronts = Physics.RaycastAll(transform.position, dir, kickRange);

			foreach (RaycastHit hitFront in hitFronts) {
				GameObject hit = hitFront.collider.gameObject;

				// kick player
				if (hit.GetComponent<PlayerBehavior>().enabled) {
					Debug.Log("player");
					hit.GetComponent<PlayerBehavior>().PushedAway(transform.position, kickForce-1);
					hit.GetComponent<PlayerBehavior>().Stunned();
				}
				// kick NPC
				else if (hit.GetComponent<NPCBehavior>().enabled){
					Debug.Log("NPC");
					hit.GetComponent<NPCBehavior>().PushedAway(transform.position, kickForce);
					hit.GetComponent<NPCBehavior>().Stunned();
				}
			}
		}
		tmp_kickCoolDownTime = kickCoolDownTime;

        isKicking = false;
	}

	/*
	 * behavior during being stunned period
	 */
	private void Stun(){
		tmp_stunDuration -= Time.deltaTime;

		if (tmp_stunDuration < 0){
			SetMovingMode(MovingMode.control);
		}

		// use Addforce to move
		_vx = Input.GetAxis(horizontalInputName);
		_vy = Input.GetAxis(verticalInputName);

		Vector3 vel = new Vector3(_vx, 0, _vy);
		if (vel.magnitude > 1) {
			vel.Normalize();
		}
		_rigid.AddForce(vel * normalSpeed * 10.0f);
	}

	/*
	 * being stunned
	 */
	public void Stunned(){
		//_rigid.AddForce(0, 500.0f, 0);
		Debug.Log("up");
		SetMovingMode(MovingMode.stunned);
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
	 * State machine for moving mode
	 */
	private void SetMovingMode(MovingMode mode){
		// Exit state
		switch (movingMode)
		{
			case MovingMode.sprint:
				_common.SetAnimationMode(CommonBehavior.AnimationMode.none);
				break;
			case MovingMode.stunned:
				_common.SetAnimationMode(CommonBehavior.AnimationMode.none);
				break;

		}
		
		movingMode = mode;

		// Entry state
		switch (movingMode)
		{
			// reset sprint cool down time and sprint duration when enter sprint mode
			case MovingMode.sprint:
				tmp_sprintCoolDownTime = sprintCoolDownTime;
				tmp_sprintDuration = sprintDuration;
				_common.SetAnimationMode(CommonBehavior.AnimationMode.dashing);
				break;
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
        if (gameObject != null)
        {
            Destroy(gameObject);
        }
	}
}
