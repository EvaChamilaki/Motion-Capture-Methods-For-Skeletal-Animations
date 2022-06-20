using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action0 : MonoBehaviour
{
    public AnimatorControllerSingleton acEd;

    public void PlayWhenPressed() {
        StartCoroutine(acEd.PlayAnimationFromAnimatorController(acEd.selected_option));
    }
}
