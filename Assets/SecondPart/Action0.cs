using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action0 : MonoBehaviour
{
    public AnimatorControllerSingleton acEd;
    private GameObject emily, adam;

    private void Start()
    {
        emily = GameObject.Find("Emily");
        adam = GameObject.Find("Adam");
    }

    public void PlayWhenPressed() {
        StartCoroutine(acEd.PlayAnimationFromAnimatorController("StartWalking", emily));
    }

    public void AdamPlay()
    {
        StartCoroutine(acEd.PlayAnimationFromAnimatorController("Wave", adam));
    }
}
