using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultipleTargetCamera : MonoBehaviour {

    public Transform[] targets;

    public List<string> teamList;

    public enum CameraState { RoundStart, RoundIn, RoundEnd };

    public CameraState cameraState;

    public Vector3 offset;

    public int targetsCapacity = 4;

    public float smoothTime = .5f;

    public float minZoom = 80f;
    public float maxZoom = 25f;

    public float minAngle = 40f;
    public float maxAngle = 60f;

    public float minOffsetZ = -25f;
    public float maxOffsetZ = -12f;

    public float mapSize = 50f;

    public float zoomSpeed;

    private Vector3 velocity;

    private Camera cam;

	// Use this for initialization
	void Start () {
        cam = GetComponent<Camera>();
        targets = new Transform[targetsCapacity];
        cameraState = CameraState.RoundStart;
        InitialTeamList();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void LateUpdate()
    {
        switch (cameraState)
        {
            case CameraState.RoundStart:
                cameraState = CameraState.RoundIn;
                break;
            case CameraState.RoundIn:

                if (targets.Length != 0)
                {
                    Move();

                    Zoom();

                    Rotate();

                    OffsetZUpdate();
                }

                break;

            case CameraState.RoundEnd:
                break;
        }

    }

    void OffsetZUpdate(){
        float newOffsetZ = Mathf.Lerp(maxOffsetZ, minOffsetZ, GetGreastestDistance() / mapSize);
        offset.z = Mathf.Lerp(offset.z, newOffsetZ, Time.deltaTime * zoomSpeed);
    }

    void Rotate()
    {
        float newAngle = Mathf.Lerp(maxAngle, minAngle, GetGreastestDistance() / mapSize);
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, Quaternion.Euler(newAngle, 0f, 0f), Time.deltaTime * zoomSpeed);
    }

    void Zoom()
    {
        float newZoom = Mathf.Lerp(maxZoom, minZoom, GetGreastestDistance() / mapSize);
        cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, newZoom, Time.deltaTime * zoomSpeed);
    }

    void Move(){
        if (targets.Length != 0)
        {

            Vector3 centerPoint = GetCenterPoint();

            Vector3 newPosition = centerPoint + offset;

            transform.position = Vector3.SmoothDamp(transform.position, newPosition, ref velocity, smoothTime);
        }
    }



    float GetGreastestDistance(){
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i] != null)
            {
                bounds.Encapsulate(targets[i].position);
            }

        }

        return Mathf.Max(bounds.size.x, bounds.size.z);
    }

    Vector3 GetCenterPoint(){
        if(targets.Length == 1){
            return targets[0].position;
        }

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        for (int i = 0; i < targets.Length; i++){
            if (targets[i] !=null){
                bounds.Encapsulate(targets[i].position);
            }
        }

        return bounds.center;
    }

    private void UpdateTargets(){
        for (int i = 0; i < teamList.Count; i++)
        {
            GameObject player = GameObject.FindWithTag("team " + teamList[i] + " player");
            if (player != null)
            {
                targets[i] = player.transform;
            }
        }
    }

    public void UpdateTargetCapacity(List<string> newTeamList)
    {
        targets = new Transform[newTeamList.Count];
        teamList = newTeamList;
        UpdateTargets();
    }

    private void InitialTeamList()
    {
        teamList = new List<string>(targetsCapacity);
        teamList.Add("green");
        teamList.Add("red");
        teamList.Add("yellow");
        teamList.Add("blue");

        UpdateTargets();
    }

    public void setState(int caseNum)
    {
        switch (caseNum)
        {
            case 0:
                cameraState = CameraState.RoundStart;
                break;
            case 1:
                cameraState = CameraState.RoundIn;
                break;
            case 2:
                cameraState = CameraState.RoundEnd;
                break;
        }
    }
}
