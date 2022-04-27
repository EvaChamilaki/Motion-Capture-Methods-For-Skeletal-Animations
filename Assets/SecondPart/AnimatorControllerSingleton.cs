using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorControllerSingleton : MonoBehaviour
{
    private static AnimatorControllerSingleton _instance;

    public static AnimatorControllerSingleton Instance
    {
        get
        {
            if(_instance == null)
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
