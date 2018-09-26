using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P4CooldownManager : MonoBehaviour {

    public Image icon;
    public bool coolingDown = false;
    public float coolDownTime = 1f;

    void Start()
    {
        //coolingDown = GameObject.Find("chipmunk team A").GetComponent<PlayerBehavior>().isCounting;

    }

    void Update()
    {
        if (coolingDown == true)
        {
            icon.fillAmount -= 1.0f / coolDownTime * Time.deltaTime;
        }
    }
}
