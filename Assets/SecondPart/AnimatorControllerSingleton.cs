using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class AnimatorControllerSingleton : MonoBehaviour
{
    private static AnimatorControllerSingleton _instance;
    public Animator anim;

    [HideInInspector]
    public bool showAnims = false;

    public void Start()
    {
        anim = GetComponent<Animator>();
    }

    public static AnimatorControllerSingleton Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("AnimatorControllerSingleton");
                go.AddComponent<AnimatorControllerSingleton>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        _instance = this;
    }
}
