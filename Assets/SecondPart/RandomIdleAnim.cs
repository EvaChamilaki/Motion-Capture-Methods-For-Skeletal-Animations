using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleAnim : MonoBehaviour
{
    private Animator anim;
    private HashManager hm;
    private int doSthHash = Animator.StringToHash("DoSomething");
    private int walkHash = Animator.StringToHash("Walk");
    private int sitHash = Animator.StringToHash("Sit");

    IEnumerator Start()
    {
        anim = GetComponent<Animator>();

        while (true)
        {
            yield return new WaitForSeconds(10.0f);

            anim.SetInteger("IdleStateNum", Random.Range(0, 3));
            anim.SetTrigger("Idles");
        }
    }

    private void Update()
    {
        if (anim != null)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetTrigger(doSthHash);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                anim.SetTrigger(walkHash);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                anim.SetTrigger(sitHash);
            }
        }
    }
    IEnumerator WaitForAnim(AnimationState animclip, float spd)
    {
        float tempTime = animclip.length * (1 / spd);
        yield return new WaitForSeconds(tempTime);
    }
}
