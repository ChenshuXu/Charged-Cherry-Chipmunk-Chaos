using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class P2CooldownManager : MonoBehaviour {

    public Image kickIcon;
    public Image sprintIcon;
    //public bool kickButton;
    //public bool sprintButton;
    public bool didPlayerSprint;
    public bool didPlayerKick;
    //public float coolDownTime;
    public float kickCooldownTime;
    public float sprintCooldownTime;

    void Start()
    {
        didPlayerSprint = GameObject.Find("chipmunk team B").GetComponent<PlayerBehavior>().isSprinting;
        didPlayerKick = GameObject.Find("chipmunk team B").GetComponent<PlayerBehavior>().isKicking;
        kickCooldownTime = GameObject.Find("chipmunk team B").GetComponent<PlayerBehavior>().kickCoolDownTime;
        sprintCooldownTime = GameObject.Find("chipmunk team B").GetComponent<PlayerBehavior>().sprintCoolDownTime;
    }

    void Update()
    {
        if (didPlayerSprint)
        {
            sprintIcon.fillAmount -= 1.0f / sprintCooldownTime * Time.deltaTime;
        }

        if (didPlayerKick)
        {
            kickIcon.fillAmount -= 1.0f / kickCooldownTime * Time.deltaTime;
        }

        //if (didPlayerKick == true && kickButton)
        //{
        //    icon.fillAmount -= 1.0f / coolDownTime * Time.deltaTime;
        //}
    }
}
