using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OperationStart : MonoBehaviour
{
    private Animation anim;
    private GameObject emil = GameObject.Find("Emil_MoCapThesis");
    private void Start()
    {
        Debug.Log("eee");
        anim = emil.GetComponent<Animation>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Here");
            anim.Play("Armature|sit_Armature");
        }
    }
}
