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

    private AnimationClip animationAdd, animationAfter, animationPrevious;
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
        var states = animStateMach.states;

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
                //list[i] = EditorGUILayout.ObjectField("Animation " + i, ac, typeof(GameObject), true) as Animation;
                GUILayout.BeginHorizontal();
                {
                    //Debug.Log(ac.name + " : " + Animator.StringToHash(ac.name));

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

        GUILayout.Label("Add Animation");

        animationAdd = EditorGUILayout.ObjectField(animationAdd, typeof(AnimationClip), true) as AnimationClip;


        if (GUILayout.Button("Apply"))
        {
            AnimationClip m = new AnimationClip { name = animationAdd.name };
            controller.AddMotion(m).AddExitTransition();
            //var state = animStateMach.AddState(animationAdd.name);
            //state.AddExitTransition();
            //controller.GetStateEffectiveMotion(AnimatorState AnyState);
            AnimatorState add = FindState(controller, animationAdd.name);
            animStateMach.AddAnyStateTransition(add);
        }
        animationPrevious = EditorGUILayout.ObjectField(animationPrevious, typeof(AnimationClip), true) as AnimationClip;

        animationAfter = EditorGUILayout.ObjectField(animationAfter, typeof(AnimationClip), true) as AnimationClip;

        if (GUILayout.Button("Between"))
        {
            AnimatorState animAfter = FindState(controller, animationAfter.name);

            AnimationClip m = new AnimationClip { name = animationAdd.name };
            controller.AddMotion(m).AddTransition(animAfter);

            AnimatorState animPrev = FindState(controller, animationPrevious.name);

            foreach (var trans in animPrev.transitions)
            {
                //Debug.Log("my name is: " + trans.destinationState.name);
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == animationAfter.name)
                    {
                        animPrev.RemoveTransition(trans);
                    }
                }
                else
                {
                    Debug.Log("It's null");
                }
            }

            AnimatorState add = FindState(controller, animationAdd.name);

            animPrev.AddTransition(add);
        }


        if (GUILayout.Button("Exit"))
        {
            foreach (var state in states)
            {
                foreach (var s in state.state.transitions)
                {
                    if (state.state.name == animationAfter.name && s.isExit)
                    {
                        AnimationClip m = new AnimationClip { name = animationAdd.name };
                        controller.AddMotion(m).AddExitTransition();

                        AnimatorState add = FindState(controller, animationAdd.name);
                        AnimatorState animAfter = FindState(controller, animationAfter.name);
                        animAfter.AddTransition(add);

                        foreach (var trans in animAfter.transitions)
                        {
                            if (trans.isExit)
                            {
                                animAfter.RemoveTransition(trans);
                            }
                        }
                    }
                    Debug.Log(state.state.name + " has " + s.isExit);
                }
            }
        }

        if (GUILayout.Button("Clear"))
        {
            animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[3]);

        }

        if (GUILayout.Button("AnyState"))
        {
            Debug.Log(animStateMach.anyStateTransitions.Length);
            for (int i = 0; i < animStateMach.anyStateTransitions.Length; i++)
            {
                Debug.Log(i + ": " + animStateMach.anyStateTransitions[i].destinationState.name);

                if (animStateMach.anyStateTransitions[i].destinationState.name == animationAfter.name)
                {
                    AnimationClip m = new AnimationClip { name = animationAdd.name };

                    AnimatorState animationStateAddBeforeThis = FindState(controller, animationAfter.name);
                    controller.AddMotion(m).AddTransition(animationStateAddBeforeThis);

                    AnimatorState add = FindState(controller, animationAdd.name);
                    animStateMach.AddAnyStateTransition(add);

                    animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[i]);
                }
            }
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
                    //Destroy(child.state);
                    return child.state;
                }
            }
        }
        Debug.LogError("Could not find state: " + stateName);
        return null;
    }

}
