using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action0 : MonoBehaviour
{
    public AnimatorControllerSingleton acEd;
    private GameObject emily;

    private void Start()
    {
        emily = GameObject.Find("Emily (1)");
    }

    public void PlayWhenPressed() {
        StartCoroutine(acEd.PlayAnimationFromAnimatorController(acEd.selected_option, emily));
    }
}
