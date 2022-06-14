using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action0 : MonoBehaviour
{
    public AnimatorControllerSingleton acEd;

    public void PlayWhenPressed() {
        Debug.Log(acEd.selected_option);
        //acEd.GetComponent<Animator>().Play();
        StartCoroutine(PlayAnim(acEd.selected_option));
    }

    IEnumerator PlayAnim(string s)
    {
        acEd.idlePlays = false;

        acEd.Character.GetComponent<Animator>().Play(s);
        yield return new WaitForSeconds(acEd.FindAnimation(acEd.Animator_Controller, s).length);

        acEd.idlePlays = true;
    }
}
