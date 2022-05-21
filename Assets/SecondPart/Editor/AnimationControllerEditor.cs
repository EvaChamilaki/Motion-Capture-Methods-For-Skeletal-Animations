using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimatorControllerSingleton))]
public class AnimationControllerEditor : Editor
{
    int selected = 0;
    string[] options = new string[] { };

    private AnimationClip animationAdd, addAnimationAfterThisOne, addAnimationBeforeThisOne;
    private Animation animationRemove;
    AnimatorStateMachine animStateMach;


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        List<string> list = new List<string>(options.ToList());


        AnimatorControllerSingleton acEd = (AnimatorControllerSingleton)target;


        //=========================================EXISTING ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        AnimatorController controller = acEd.Animator_Controller;

        animStateMach = controller.layers[0].stateMachine;
        ChildAnimatorState[] states = animStateMach.states;

        acEd.showAnims = EditorGUILayout.Foldout(acEd.showAnims, "Existing Animations", true);

        if (acEd.showAnims)
        {
            int size = acEd.animator.runtimeAnimatorController.animationClips.Length;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Size:          ");
                EditorGUILayout.SelectableLabel(size.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            GUILayout.EndHorizontal();

            int i = 0;

            foreach (AnimationClip ac in acEd.animator.runtimeAnimatorController.animationClips)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Animation " + i + ":");
                    EditorGUILayout.SelectableLabel(ac.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUILayout.EndHorizontal();

                i++;

                list.Add(ac.name);
            }
        }


        //==============================================ADD ANIMATIONS===============================================


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("Add this Animation:");

        animationAdd = EditorGUILayout.ObjectField(animationAdd, typeof(AnimationClip), true) as AnimationClip;

        GUILayout.Space(10);

        acEd.showBeforeThisAnim = EditorGUILayout.Foldout(acEd.showBeforeThisAnim, "Before this Animation", true);

        if (acEd.showBeforeThisAnim)
        {
            addAnimationBeforeThisOne = EditorGUILayout.ObjectField(addAnimationBeforeThisOne, typeof(AnimationClip), true) as AnimationClip;

        }

        acEd.showAfterThisAnim = EditorGUILayout.Foldout(acEd.showAfterThisAnim, "After this Animation", true);

        if (acEd.showAfterThisAnim)
        {
            addAnimationAfterThisOne = EditorGUILayout.ObjectField(addAnimationAfterThisOne, typeof(AnimationClip), true) as AnimationClip;
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Simple"))
        {
            SimpleAddition(controller);
        }

        if (GUILayout.Button("Between"))
        {
            BetweenTwoAnimStates(controller);
        }


        if (GUILayout.Button("Exit"))
        {
            ToExitTransition(controller, states);
        }

        if (GUILayout.Button("Clear"))
        {
            animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[3]);

        }

        if (GUILayout.Button("AnyState"))
        {
            FromAnyStateTransition(controller);
        }

        //==========================================REMOVE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        animationRemove = EditorGUILayout.ObjectField(animationRemove, typeof(Animation), true) as Animation;

        EditorGUI.BeginChangeCheck();

        options = list.ToArray();
        this.selected = EditorGUILayout.Popup("Remove Animation", selected, options);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log(options[selected]);
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Remove"))
        {
            //animationRemove.RemoveClip(options[selected]);
            AnimatorState removable = FindState(controller, options[selected]);
            animStateMach.RemoveState(removable);

            //RemoveClip("idle");
            Debug.Log("removed");
        }
    }

    public AnimatorState FindState(AnimatorController animCont, string stateName)
    {
        for (int i = 0; i < animCont.layers.Length; i++)
        {
            foreach (var child in animCont.layers[i].stateMachine.states)
            {
                if (child.state.name == stateName)
                {
                    Debug.Log("Found it: " + child.state);
                    return child.state;
                }
            }
        }
        Debug.LogError("Could not find state: " + stateName);
        return null;
    }
    
    public AnimatorState FindNextState(AnimatorController animCont)
    {
        AnimatorState afterThisAnim = FindState(animCont, addAnimationAfterThisOne.name);

        foreach (var trans in afterThisAnim.transitions)
        {
            if (trans.destinationState != null)
            {
                return trans.destinationState;
            }
            else
            {
                Debug.Log("It's null");
            }
        }
        return null;
    }

    public AnimatorState FindPreviousState(AnimatorController animCont)
    {
        for (int i = 0; i < animCont.layers.Length; i++)
        {
            foreach (var state in animCont.layers[0].stateMachine.states)
            {
                foreach (var s in state.state.transitions)
                {
                    if (s.destinationState != null)
                    {
                        if (s.destinationState.name == addAnimationBeforeThisOne.name)
                            return state.state;
                    }
                }
            }
        }
        return null;
    }

    public void SimpleAddition(AnimatorController animCont)
    {
        AnimationClip m = new AnimationClip { name = animationAdd.name };
        animCont.AddMotion(m).AddExitTransition();

        AnimatorState add = FindState(animCont, animationAdd.name);
        animStateMach.AddAnyStateTransition(add);
    }

    public void ToExitTransition(AnimatorController animCont, ChildAnimatorState[] states)
    {
        foreach (var state in states)
        {
            foreach (var s in state.state.transitions)
            {
                if (state.state.name == addAnimationAfterThisOne.name && s.isExit)
                {
                    AnimationClip m = new AnimationClip { name = animationAdd.name };
                    animCont.AddMotion(m).AddExitTransition();

                    AnimatorState add = FindState(animCont, animationAdd.name);
                    AnimatorState animAfter = FindState(animCont, addAnimationAfterThisOne.name);
                    animAfter.AddTransition(add);

                    foreach (var trans in animAfter.transitions)
                    {
                        if (trans.isExit)
                        {
                            animAfter.RemoveTransition(trans);
                        }
                    }
                }
            }
        }
    }

    public void BetweenTwoAnimStates(AnimatorController animCont)
    {
        if (addAnimationBeforeThisOne.name == addAnimationAfterThisOne.name) {
            Debug.LogError("You can not add a new Animation State that the parent and the child is the same Animation State");
        }
        else {
            AnimatorState animAfter = FindState(animCont, addAnimationAfterThisOne.name);

            AnimationClip m = new AnimationClip { name = animationAdd.name };
            animCont.AddMotion(m).AddTransition(animAfter);

            AnimatorState animPrev = FindState(animCont, addAnimationBeforeThisOne.name);

            foreach (var trans in animPrev.transitions)
            {
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == addAnimationAfterThisOne.name)
                    {
                        animPrev.RemoveTransition(trans);
                    }
                }
                else
                {
                    Debug.Log("It's null");
                }
            }

            AnimatorState add = FindState(animCont, animationAdd.name);

            animPrev.AddTransition(add);
        }
    }

    public void FromAnyStateTransition(AnimatorController animCont)
    {
        for (int i = 0; i < animStateMach.anyStateTransitions.Length; i++)
        {
            Debug.Log(i + ": " + animStateMach.anyStateTransitions[i].destinationState.name);

            if (animStateMach.anyStateTransitions[i].destinationState.name == addAnimationBeforeThisOne.name)
            {
                AnimationClip m = new AnimationClip { name = animationAdd.name };

                AnimatorState animationStateAddBeforeThis = FindState(animCont, addAnimationBeforeThisOne.name);
                animCont.AddMotion(m).AddTransition(animationStateAddBeforeThis);

                AnimatorState add = FindState(animCont, animationAdd.name);
                animStateMach.AddAnyStateTransition(add);

                animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[i]);
            }
        }
    }

}
