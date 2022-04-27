using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomIdleAnim : MonoBehaviour
{
    private Animator anim;

    IEnumerator Start()
    {
        anim = GetComponent<Animator>();

        while(true)
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
                anim.SetTrigger("DoSomething");
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                anim.SetTrigger("Walk");
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                anim.SetTrigger("Sit");
            }
        }
    }
    IEnumerator WaitForAnim(AnimationState animclip, float spd)
    {
        float tempTime = animclip.length * (1 / spd);
        yield return new WaitForSeconds(tempTime);
    }
}
