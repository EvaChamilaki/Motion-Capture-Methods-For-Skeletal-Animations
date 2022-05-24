using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class AnimatorControllerSingleton : MonoBehaviour
{
    private static AnimatorControllerSingleton _instance;
    public Animator animator;
    public AnimatorController Animator_Controller;


    [HideInInspector]
    public bool showAnims = false, showSubStateMach = false, showAnimContr = false;

    [HideInInspector]
    public bool showBeforeThisAnim = false, showBeforeThisAnimSubSM = false;
    
    [HideInInspector]
    public bool showAfterThisAnim = false, showAfterThisAnimSubSM = false;


    public void Start()
    {
        animator = GetComponent<Animator>();
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
