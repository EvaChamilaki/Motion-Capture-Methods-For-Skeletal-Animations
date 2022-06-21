using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

[ExecuteInEditMode]
public class AnimatorControllerSingleton : MonoBehaviour
{
    private static AnimatorControllerSingleton _instance;
    public AnimatorController Animator_Controller;

    private AnimatorStateMachine animStateMach;

    [HideInInspector]
    public Animator animator;

    [HideInInspector]
    public List<GameObject> Characters;

    [HideInInspector]
    public bool showChars = false, showAnims = false, showSubStateMach = false, showAnimContr = false
        , showDefault = false, idlePlays = true;

    [HideInInspector]
    public bool showBeforeThisAnim = false, showBeforeThisAnimSubSM = false;

    [HideInInspector]
    public bool showAfterThisAnim = false, showAfterThisAnimSubSM = false;

    [HideInInspector]
    public string selected_option;

    [HideInInspector]
    public List<AnimationClip> animClips = new List<AnimationClip>();

    public void Start()
    {
        animStateMach = Animator_Controller.layers[0].stateMachine;
        animator = GetComponent<Animator>();

        foreach(var ch in Characters)
        {
            StartCoroutine(CharacterThread(ch));
        }
    }

    IEnumerator CharacterThread(GameObject character)
    {
        bool idlePlays = true;

        ChildAnimatorState[] subStateMachStates = animStateMach.stateMachines[0].stateMachine.states;
        AnimationClip prevChosen = null;

        while (idlePlays)
        {
            ChildAnimatorState chosenState = subStateMachStates[Random.Range(0, subStateMachStates.Length)];

            AnimationClip chosen = FindAnimationSubSM(animStateMach, chosenState.state.name);

            if (prevChosen != null)
            {
                if (prevChosen.name == chosen.name)
                {
                    chosenState = subStateMachStates[Random.Range(0, subStateMachStates.Length)];
                    chosen = FindAnimationSubSM(animStateMach, chosenState.state.name);
                }
            }

            StartCoroutine(SequenceOfAnimationsSubSM(animStateMach, chosen.name, character));
            yield return new WaitForSeconds(DurationOfAnimsSubSM(animStateMach, chosen.name));

            prevChosen = chosen;
        }
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

    public AnimatorState FindStateSubSM(AnimatorStateMachine animStMach, string stateName)
    {
        foreach (var state in animStateMach.stateMachines[0].stateMachine.states)
        {
            if (state.state.name == stateName)
            {
                return state.state;
            }
        }
        Debug.LogError("Could not find state: " + stateName);
        return null;
    }

    public AnimatorState FindNextStateSubSM(AnimatorStateMachine animStMach, string afterThisState)
    {
        AnimatorState afterThisAnim = FindStateSubSM(animStMach, afterThisState);

        foreach (var trans in afterThisAnim.transitions)
        {
            if (trans.destinationState != null)
            {
                return trans.destinationState;
            }
        }
        Debug.LogError("Could not find a next state.");
        return null;
    }

    public AnimationClip FindAnimation(AnimatorController animCont, string name)
    {
        foreach (AnimationClip clip in animCont.animationClips)
        {
            if (clip.name == name)
            {
                return clip;
            }
        }

        return null;
    }

    public AnimationClip FindAnimationSubSM(AnimatorStateMachine animStMach, string name)
    {
        foreach (var state in animStMach.stateMachines[0].stateMachine.states)
        {
            if (state.state.name == name)
            {
                return state.state.motion as AnimationClip;
            }
        }

        return null;
    }

    IEnumerator SequenceOfAnimationsSubSM(AnimatorStateMachine animStMach, string animName, GameObject character)
    {
        AnimatorState default_state = animStateMach.defaultState;
        AnimationClip default_clip = FindAnimation(Animator_Controller, default_state.name);

        character.GetComponent<Animator>().Play(default_state.name);

        yield return new WaitForSeconds(default_clip.length);

        AnimationClip animcl = FindAnimationSubSM(animStMach, animName);
        bool hasAnother = true;

        character.GetComponent<Animator>().Play(animName);
        yield return new WaitForSeconds(animcl.length);


        while (hasAnother)
        {
            AnimatorState animState = FindStateSubSM(animStMach, animName);
            AnimatorState state = FindNextStateSubSM(animStMach, animName);

            if (!state || animState.transitions[0].isExit) { hasAnother = false; }
            else
            {
                AnimationClip anim = FindAnimationSubSM(animStMach, state.name);

                character.GetComponent<Animator>().Play(anim.name);

                yield return new WaitForSeconds(anim.length);

                animName = state.name;
            }
        }

        character.GetComponent<Animator>().Play(default_state.name);

        yield return new WaitForSeconds(default_clip.length);
    }

    public void PlayAnimationFromAnimatorController(string s, GameObject character)
    {
        idlePlays = false;

        character.GetComponent<Animator>().Play(s);
        StartCoroutine(SequenceOfAnimationsSubSM(animStateMach, s, character));

        idlePlays = true;
        StartCoroutine(CharacterThread(character));
    }

    public float DurationOfAnimsSubSM(AnimatorStateMachine animStMach, string animName)
    {
        AnimationClip animcl = FindAnimationSubSM(animStMach, animName);
        bool hasAnother = true;
        float duration = animcl.length;

        while (hasAnother)
        {
            AnimatorState animState = FindStateSubSM(animStMach, animName);
            AnimatorState state = FindNextStateSubSM(animStMach, animName);

            if (!state || animState.transitions[0].isExit) { Debug.Log("exit"); hasAnother = false; }
            else
            {
                AnimationClip anim = FindAnimationSubSM(animStMach, state.name);

                duration += anim.length;

                animName = state.name;
            }
        }

        AnimatorState default_state = animStateMach.defaultState;
        AnimationClip default_clip = FindAnimation(Animator_Controller, default_state.name);

        duration += default_clip.length;

        return duration;
    }
}
