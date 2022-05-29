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

    private AnimationClip animationAdd, addAnimationAfterThisOne, addAnimationBeforeThisOne
        , animationRemove, addDefaultState, animationAddToStateMach, addAnimationAfterThisOneSubSM, addAnimationBeforeThisOneSubSM;
    private AnimatorStateMachine animStateMach, stateMachine;

    public enum PlayModeState
    {
        Playing,
        Paused
    }

    //[InitializeOnLoad]

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //EditorApplication.playModeStateChanged = HandleOnPlayModeChanged;

        List<string> list = new List<string>(options.ToList());
        AnimatorControllerSingleton acEd = (AnimatorControllerSingleton)target;

        //void HandleOnPlayModeChanged()
        //{
        //    // This method is run whenever the playmode state is changed.

        //}



        //=========================================EXISTING ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        AnimatorController controller = acEd.Animator_Controller;

        animStateMach = controller.layers[0].stateMachine;

        acEd.showAnims = EditorGUILayout.Foldout(acEd.showAnims, "Existing Animations", true);

        if (acEd.showAnims)
        {
            int size = acEd.animator.runtimeAnimatorController.animationClips.Length;
            if (animStateMach.stateMachines[0].stateMachine != null)
            {
                size += animStateMach.stateMachines[0].stateMachine.states.Length;
            }
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

            //foreach (var state in animStateMach.stateMachines[0].stateMachine.states)
            //{
            //    GUILayout.BeginHorizontal();
            //    {
            //        GUILayout.Label("Animation " + i + ":");
            //        EditorGUILayout.SelectableLabel(state.state.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            //    }
            //    GUILayout.EndHorizontal();

            //    i++;
            //}
        }


        //===========================================ADD IDLE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        acEd.showSubStateMach = EditorGUILayout.Foldout(acEd.showSubStateMach, "Sub State Machine", true);

        if (GUILayout.Button("TEst"))
        {
            foreach (var i in acEd.animClips)
            {
                Debug.Log($"list elem: {i}");
            }
            //AnimationClip clip = FindAnimation(acEd.animator, "Bored");
            //Debug.Log(clip);
            //SequenceOfAnimations(controller, "StandingScrollingPhone");
            SequenceOfAnimationsSubSM(animStateMach, "Bored", acEd.Character.GetComponent<Animation>());
        }


        if (acEd.showSubStateMach)
        {
            GUILayout.Label("Set as Default Animation:");

            addDefaultState = EditorGUILayout.ObjectField(addDefaultState, typeof(AnimationClip), true) as AnimationClip;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                AnimationClip m = new AnimationClip { name = addDefaultState.name };
                controller.AddMotion(m);

                AnimatorState default_state = FindState(controller, addDefaultState.name);
                animStateMach.defaultState = default_state;

                AnimatorStateMachine ast = animStateMach.AddStateMachine("IdleStates");
                animStateMach.AddStateMachineTransition(ast, default_state);

                foreach (var s in animStateMach.stateMachines)
                {
                    Debug.Log(s.stateMachine.name);
                }
                default_state.AddTransition(ast);

                AddToAnimationListOfCharacter(addDefaultState);
            }

            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            if (animStateMach.stateMachines.Length != 0)
            {
                stateMachine = animStateMach.stateMachines[0].stateMachine;


                GUILayout.Label("Add this Animation to the " + animStateMach.stateMachines[0].stateMachine.name + ":");

                animationAddToStateMach = EditorGUILayout.ObjectField(animationAddToStateMach, typeof(AnimationClip), true) as AnimationClip;

                acEd.showBeforeThisAnimSubSM = EditorGUILayout.Foldout(acEd.showBeforeThisAnimSubSM, "Before this Animation", true);

                if (acEd.showBeforeThisAnimSubSM)
                {
                    addAnimationBeforeThisOneSubSM = EditorGUILayout.ObjectField(addAnimationBeforeThisOneSubSM, typeof(AnimationClip), true) as AnimationClip;
                }

                acEd.showAfterThisAnimSubSM = EditorGUILayout.Foldout(acEd.showAfterThisAnimSubSM, "After this Animation", true);

                if (acEd.showAfterThisAnimSubSM)
                {
                    addAnimationAfterThisOneSubSM = EditorGUILayout.ObjectField(addAnimationAfterThisOneSubSM, typeof(AnimationClip), true) as AnimationClip;
                }

                GUILayout.Space(10);
                if (GUILayout.Button("Apply"))
                {
                    if (animationAddToStateMach == null)
                    {
                        Debug.LogError("Insert Animation Clips to the corresponding fields.");
                    }
                    else if (addAnimationBeforeThisOneSubSM == null && addAnimationAfterThisOneSubSM == null)
                    {
                        SimpleAdditionSubSM(stateMachine);
                        acEd.animClips.Add(animationAddToStateMach);
                        AddToAnimationListOfCharacter(animationAddToStateMach);
                    }
                    else if (addAnimationBeforeThisOneSubSM != null && addAnimationAfterThisOneSubSM == null)
                    {
                        AnimatorState animBefore = FindStateSubSM(stateMachine, addAnimationBeforeThisOneSubSM.name);

                        if (animBefore == null)
                        {
                            Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the sub State Machine.");
                        }
                        else
                        {
                            bool isEntry = false;

                            Debug.Log(stateMachine.entryTransitions.Length);

                            for (int i = 0; i < stateMachine.entryTransitions.Length; i++)
                            {
                                if (stateMachine.entryTransitions[i].destinationState.name == addAnimationBeforeThisOneSubSM.name)
                                {
                                    isEntry = true;
                                    FromEntryTransitionAddStateSubSM(stateMachine, i);
                                }
                            }
                            if (!isEntry)
                            {
                                SimpleBeforeAdditionSubSM(stateMachine);
                            }
                            acEd.animClips.Add(animationAddToStateMach);
                            AddToAnimationListOfCharacter(animationAddToStateMach);
                        }
                    }
                    else if (addAnimationBeforeThisOneSubSM == null && addAnimationAfterThisOneSubSM != null)
                    {
                        AnimatorState animAfter = FindStateSubSM(stateMachine, addAnimationAfterThisOneSubSM.name);

                        if (animAfter == null)
                        {
                            Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the sub State Machine.");
                        }
                        else
                        {
                            AnimatorStateTransition transition = animAfter.transitions[0];

                            if (transition.isExit)
                            {
                                ToExitTransitionAddStateSubSM(stateMachine, animAfter);
                            }
                            else
                            {
                                SimpleAfterAdditionSubSM(stateMachine);
                            }
                            acEd.animClips.Add(animationAddToStateMach);
                        }
                    }
                    else if (addAnimationBeforeThisOneSubSM != null && addAnimationAfterThisOneSubSM != null)
                    {
                        BetweenTwoAnimStatesAddStateSubSM(stateMachine);
                        acEd.animClips.Add(animationAddToStateMach);
                        AddToAnimationListOfCharacter(animationAddToStateMach);
                    }
                }
            }
        }

        //==============================================ADD ANIMATIONS===============================================


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        acEd.showAnimContr = EditorGUILayout.Foldout(acEd.showAnimContr, "Animator Controller", true);

        if (acEd.showAnimContr)
        {
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

            if (GUILayout.Button("Apply"))
            {
                AnimationClip m = new AnimationClip { name = animationAdd.name };

                if (animationAdd == null)
                {
                    Debug.LogError("Insert Animation Clips to the corresponding fields.");
                }
                else if (addAnimationBeforeThisOne == null && addAnimationAfterThisOne == null)
                {
                    SimpleAddition(controller, m);

                    AddToAnimationListOfCharacter(animationAdd);
                }
                else if (addAnimationBeforeThisOne != null && addAnimationAfterThisOne == null)
                {
                    AnimatorState animBefore = FindState(controller, addAnimationBeforeThisOne.name);

                    if (animBefore == null)
                    {
                        Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the AnimatorController.");
                    }
                    else
                    {
                        bool isAnyState = false;

                        for (int i = 0; i < animStateMach.anyStateTransitions.Length; i++)
                        {
                            if (animStateMach.anyStateTransitions[i].destinationState.name == addAnimationBeforeThisOne.name)
                            {
                                isAnyState = true;
                                FromAnyStateTransitionAddState(controller, i);
                            }
                        }
                        if (!isAnyState)
                        {
                            SimpleBeforeAddition(controller);
                        }
                        AddToAnimationListOfCharacter(animationAdd);
                    }
                }
                else if (addAnimationBeforeThisOne == null && addAnimationAfterThisOne != null)
                {
                    AnimatorState animAfter = FindState(controller, addAnimationAfterThisOne.name);
                    AnimatorState default_state = animStateMach.defaultState;

                    if (animAfter == null)
                    {
                        Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the AnimatorController.");
                    }
                    else
                    {
                        AnimatorStateTransition transition = animAfter.transitions[0];

                        if (transition.destinationState == default_state)
                        {
                            ToDefaultTransitionAddState(controller, animAfter, default_state);
                        }
                        else
                        {
                            SimpleAfterAddition(controller);
                        }
                        AddToAnimationListOfCharacter(animationAdd);
                    }
                }
                else if (addAnimationBeforeThisOne != null && addAnimationAfterThisOne != null)
                {
                    BetweenTwoAnimStatesAddState(controller);
                    AddToAnimationListOfCharacter(animationAdd);
                }
            }
        }


        //==========================================REMOVE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        animationRemove = EditorGUILayout.ObjectField(animationRemove, typeof(AnimationClip), true) as AnimationClip;

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
            AnimatorState removable = FindState(controller, animationRemove.name);
            AnimatorState beforeRemovable = FindPreviousState(controller, animationRemove.name);
            AnimatorState afterRemovable = FindNextState(controller, animationRemove.name);
            AnimatorState default_state = animStateMach.defaultState;

            AnimatorStateTransition trans = removable.transitions[0];

            if (beforeRemovable != null && trans.destinationState == default_state)
            {
                removable.RemoveTransition(trans);
                animStateMach.RemoveState(removable);

                beforeRemovable.AddTransition(default_state);

                RemoveFromAnimationListOfCharacter(animationRemove);
            }
            else if (beforeRemovable == null && trans.destinationState == default_state)
            {
                foreach (var asTrans in animStateMach.anyStateTransitions)
                {
                    if (asTrans.destinationState.name == removable.name)
                    {
                        animStateMach.RemoveAnyStateTransition(asTrans);
                        animStateMach.RemoveState(removable);

                        RemoveFromAnimationListOfCharacter(animationRemove);
                    }
                }
            }
            else if (beforeRemovable != null && afterRemovable != null)
            {
                beforeRemovable.AddTransition(afterRemovable);
                animStateMach.RemoveState(removable);

                RemoveFromAnimationListOfCharacter(animationRemove);
            }
            else if (beforeRemovable == null && afterRemovable == null)
            {
                animStateMach.RemoveState(removable);

                RemoveFromAnimationListOfCharacter(animationRemove);
            }
            else if (beforeRemovable == null && afterRemovable != null)
            {
                foreach (var asTrans in animStateMach.anyStateTransitions)
                {
                    if (asTrans.destinationState.name == removable.name)
                    {
                        animStateMach.AddAnyStateTransition(afterRemovable);

                        animStateMach.RemoveAnyStateTransition(asTrans);
                        animStateMach.RemoveState(removable);

                        RemoveFromAnimationListOfCharacter(animationRemove);
                    }
                }
            }
        }


        void AddToAnimationListOfCharacter(AnimationClip acAdd)
        {
            //acAdd.legacy = true;
            acEd.Character.GetComponent<Animation>().AddClip(acAdd, acAdd.name);
            //acAdd.legacy = false;
        }

        void RemoveFromAnimationListOfCharacter(AnimationClip accRem)
        {
            acEd.Character.GetComponent<Animation>().RemoveClip(accRem);
        }
    }



    //=================================================FIND STATE FUNCTIONS======================================================

    public AnimatorState FindStateSubSM(AnimatorStateMachine animStMach, string stateName)
    {
        foreach (var state in animStMach.states)
        {
            if (state.state.name == stateName)
            {
                return state.state;
            }
        }
        Debug.LogError("Could not find state: " + stateName);
        return null;
    }

    public AnimatorState FindPreviousStateSubSM(AnimatorStateMachine stateMachine, string beforeThisState)
    {
        foreach (var state in stateMachine.states)
        {
            foreach (var s in state.state.transitions)
            {
                if (s.destinationState != null)
                {
                    if (s.destinationState.name == beforeThisState)
                        return state.state;
                }
            }
        }

        Debug.LogError("Could not find a previous state.");
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

    public AnimatorState FindState(AnimatorController animCont, string stateName)
    {
        for (int i = 0; i < animCont.layers.Length; i++)
        {
            foreach (var child in animCont.layers[i].stateMachine.states)
            {
                if (child.state.name == stateName)
                {
                    return child.state;
                }
            }
        }
        Debug.LogError("Could not find state: " + stateName);
        return null;
    }

    public AnimatorState FindPreviousState(AnimatorController animCont, string beforeThisState)
    {
        for (int i = 0; i < animCont.layers.Length; i++)
        {
            foreach (var state in animCont.layers[0].stateMachine.states)
            {
                foreach (var s in state.state.transitions)
                {
                    if (s.destinationState != null)
                    {
                        if (s.destinationState.name == beforeThisState)
                            return state.state;
                    }
                }
            }
        }
        Debug.LogError("Could not find a previous state.");
        return null;
    }

    public AnimatorState FindNextState(AnimatorController animCont, string afterThisState)
    {
        AnimatorState afterThisAnim = FindState(animCont, afterThisState);

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



    //=================================================ADD STATE FUNCTIONS(Animator)=======================================================


    public void SimpleAddition(AnimatorController animCont, AnimationClip m)
    {
        AnimatorState default_state = animStateMach.defaultState;
        animCont.AddMotion(m).AddTransition(default_state);

        AnimatorState add = FindState(animCont, animationAdd.name);
        animStateMach.AddAnyStateTransition(add);
    }

    public void SimpleAfterAddition(AnimatorController animCont)
    {
        AnimatorState nextState = FindNextState(animCont, addAnimationAfterThisOne.name);

        if (nextState == null)
        {
            Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the AnimatorController.");
        }
        else
        {
            AnimatorState afterThisState = FindState(animCont, addAnimationAfterThisOne.name);

            AnimationClip m = new AnimationClip { name = animationAdd.name };
            animCont.AddMotion(m).AddTransition(nextState);

            AnimatorState add = FindState(animCont, animationAdd.name);
            afterThisState.AddTransition(add);

            foreach (var trans in afterThisState.transitions)
            {
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == nextState.name)
                    {
                        afterThisState.RemoveTransition(trans);
                    }
                }
            }
        }
    }

    public void SimpleBeforeAddition(AnimatorController animCont)
    {
        AnimatorState previousState = FindPreviousState(animCont, addAnimationBeforeThisOne.name);

        if (previousState == null)
        {
            Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the AnimatorController.");
        }
        else
        {
            AnimatorState beforeThisState = FindState(animCont, addAnimationBeforeThisOne.name);

            AnimationClip m = new AnimationClip { name = animationAdd.name };
            animCont.AddMotion(m).AddTransition(beforeThisState);

            AnimatorState add = FindState(animCont, animationAdd.name);
            previousState.AddTransition(add);

            foreach (var trans in previousState.transitions)
            {
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == beforeThisState.name)
                    {
                        previousState.RemoveTransition(trans);
                    }
                }
            }
        }
    }

    public void ToDefaultTransitionAddState(AnimatorController animCont, AnimatorState animAfter, AnimatorState default_state)
    {
        AnimationClip m = new AnimationClip { name = animationAdd.name };

        animCont.AddMotion(m).AddTransition(default_state);

        AnimatorState add = FindState(animCont, animationAdd.name);
        animAfter.AddTransition(add);

        foreach (var trans in animAfter.transitions)
        {
            if (trans.destinationState == default_state)
            {
                animAfter.RemoveTransition(trans);
            }
        }
    }

    public void BetweenTwoAnimStatesAddState(AnimatorController animCont)
    {
        if (addAnimationBeforeThisOne.name == addAnimationAfterThisOne.name)
        {
            Debug.LogError("You can not add a new Animation State that the parent and the child is the same Animation State.");
        }
        else
        {
            AnimatorState animAfter = FindState(animCont, addAnimationAfterThisOne.name);
            AnimatorState animPrev = FindState(animCont, addAnimationBeforeThisOne.name);

            if (animAfter == null && animPrev != null)
            {
                Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the AnimatorController.");
                return;
            }
            else if (animAfter != null && animPrev == null)
            {
                Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the AnimatorController.");
                return;
            }
            else if (animAfter == null && animPrev == null)
            {
                Debug.LogError("Neither of the animation states you selected as a Before and Next Animation State exist in the AnimatorController.");
                return;
            }
            else
            {
                AnimationClip m = new AnimationClip { name = animationAdd.name };
                animCont.AddMotion(m).AddTransition(animPrev);


                foreach (var trans in animAfter.transitions)
                {
                    if (trans.destinationState != null)
                    {
                        if (trans.destinationState.name == addAnimationBeforeThisOne.name)
                        {
                            animAfter.RemoveTransition(trans);
                        }
                    }
                }

                AnimatorState add = FindState(animCont, animationAdd.name);

                animAfter.AddTransition(add);
            }
        }
    }

    public void FromAnyStateTransitionAddState(AnimatorController animCont, int i)
    {
        AnimationClip m = new AnimationClip { name = animationAdd.name };

        AnimatorState animationStateAddBeforeThis = FindState(animCont, addAnimationBeforeThisOne.name);
        animCont.AddMotion(m).AddTransition(animationStateAddBeforeThis);

        AnimatorState add = FindState(animCont, animationAdd.name);
        animStateMach.AddAnyStateTransition(add);

        animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[i]);
    }


    //==============================================ADD STATE FUNCTIONS(Sub State Machine)=====================================================


    public void SimpleAdditionSubSM(AnimatorStateMachine stateMachine)
    {
        stateMachine.AddState(animationAddToStateMach.name).motion = animationAddToStateMach;

        AnimatorState newStateInSubSM = FindStateSubSM(stateMachine, animationAddToStateMach.name);
        stateMachine.AddEntryTransition(newStateInSubSM);

        //animationAddToStateMach.legacy = true;

        newStateInSubSM.motion = animationAddToStateMach;
        newStateInSubSM.AddExitTransition();
    }

    public void SimpleAfterAdditionSubSM(AnimatorStateMachine stateMachine)
    {
        AnimatorState nextState = FindNextStateSubSM(stateMachine, addAnimationAfterThisOneSubSM.name);

        if (nextState == null)
        {
            Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the AnimatorController.");
        }
        else
        {
            stateMachine.AddState(animationAddToStateMach.name);

            AnimatorState afterThisState = FindStateSubSM(stateMachine, addAnimationAfterThisOneSubSM.name);

            AnimatorState add = FindStateSubSM(stateMachine, animationAddToStateMach.name);
            add.motion = animationAddToStateMach;
            add.AddTransition(nextState);
            afterThisState.AddTransition(add);

            foreach (var trans in afterThisState.transitions)
            {
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == nextState.name)
                    {
                        afterThisState.RemoveTransition(trans);
                    }
                }
            }
        }
    }

    public void SimpleBeforeAdditionSubSM(AnimatorStateMachine stateMachine)
    {
        AnimatorState previousState = FindPreviousStateSubSM(stateMachine, addAnimationBeforeThisOneSubSM.name);

        if (previousState == null)
        {
            Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the sub State Machine.");
        }
        else
        {
            AnimatorState beforeThisState = FindStateSubSM(stateMachine, addAnimationBeforeThisOneSubSM.name);

            stateMachine.AddState(animationAddToStateMach.name);

            AnimatorState newStateInSubSM = FindStateSubSM(stateMachine, animationAddToStateMach.name);

            newStateInSubSM.motion = animationAddToStateMach;
            newStateInSubSM.AddTransition(beforeThisState);

            previousState.AddTransition(newStateInSubSM);

            foreach (var trans in previousState.transitions)
            {
                if (trans.destinationState != null)
                {
                    if (trans.destinationState.name == beforeThisState.name)
                    {
                        previousState.RemoveTransition(trans);
                    }
                }
            }
        }
    }

    public void ToExitTransitionAddStateSubSM(AnimatorStateMachine stateMachine, AnimatorState animAfter)
    {
        stateMachine.AddState(animationAddToStateMach.name);

        AnimatorState add = FindStateSubSM(stateMachine, animationAddToStateMach.name);
        add.motion = animationAddToStateMach;
        add.AddExitTransition();
        animAfter.AddTransition(add);

        foreach (var trans in animAfter.transitions)
        {
            if (trans.isExit)
            {
                animAfter.RemoveTransition(trans);
            }
        }
    }

    public void BetweenTwoAnimStatesAddStateSubSM(AnimatorStateMachine stateMachine)
    {
        if (addAnimationBeforeThisOneSubSM.name == addAnimationAfterThisOneSubSM.name)
        {
            Debug.LogError("You can not add a new Animation State that the parent and the child is the same Animation State.");
        }
        else
        {
            AnimatorState animAfter = FindStateSubSM(stateMachine, addAnimationAfterThisOneSubSM.name);
            AnimatorState animPrev = FindStateSubSM(stateMachine, addAnimationBeforeThisOneSubSM.name);

            if (animAfter == null && animPrev != null)
            {
                Debug.LogError("The animation state you selected as a Next Animation State doesn't exist in the sub State Machine.");
                return;
            }
            else if (animAfter != null && animPrev == null)
            {
                Debug.LogError("The animation state you selected as a Before Animation State doesn't exist in the sub State Machine.");
                return;
            }
            else if (animAfter == null && animPrev == null)
            {
                Debug.LogError("Neither of the animation states you selected as a Before and Next Animation State exist in the sub State Machine.");
                return;
            }
            else
            {
                stateMachine.AddState(animationAddToStateMach.name);

                foreach (var trans in animAfter.transitions)
                {
                    if (trans.destinationState != null)
                    {
                        if (trans.destinationState.name == addAnimationBeforeThisOneSubSM.name)
                        {
                            animAfter.RemoveTransition(trans);
                        }
                    }
                }

                AnimatorState add = FindStateSubSM(stateMachine, animationAddToStateMach.name);
                add.motion = animationAddToStateMach;
                add.AddTransition(animPrev);
                animAfter.AddTransition(add);
            }
        }
    }

    public void FromEntryTransitionAddStateSubSM(AnimatorStateMachine stateMachine, int i)
    {
        stateMachine.AddState(animationAddToStateMach.name);

        AnimatorState newStateInSubSM = FindStateSubSM(stateMachine, animationAddToStateMach.name);
        AnimatorState animationStateAddBeforeThis = FindStateSubSM(stateMachine, addAnimationBeforeThisOneSubSM.name);

        newStateInSubSM.motion = animationAddToStateMach;
        newStateInSubSM.AddTransition(animationStateAddBeforeThis);
        stateMachine.AddEntryTransition(newStateInSubSM);

        stateMachine.RemoveEntryTransition(stateMachine.entryTransitions[i]);
    }


    //========================================================================================================================

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


    public float SequenceOfAnimations(AnimatorController animCont, string animName)
    {
        AnimationClip animcl = FindAnimation(animCont, animName);

        bool hasAnother = true;
        float duration = animcl.length;

        while (hasAnother)
        {
            AnimatorState state = FindNextState(animCont, animName);

            if (!state || state.name == animStateMach.defaultState.name) { hasAnother = false; }
            else
            {
                AnimationClip anim = FindAnimation(animCont, state.name);
                duration += anim.length;
                animName = state.name;
            }
        }
        Debug.Log(duration);

        return duration;
    }


    public float SequenceOfAnimationsSubSM(AnimatorStateMachine animStMach, string animName, Animation animation)
    {
        var StateMachStates = animStateMach.stateMachines[0].stateMachine;
        AnimationClip animcl = FindAnimationSubSM(animStMach, animName);

        bool hasAnother = true;
        float duration = animcl.length;
        animation.PlayQueued(animcl.name);


        while (hasAnother)
        {
            AnimatorState state = FindNextStateSubSM(StateMachStates, animName);

            if (!state || state.transitions[0].isExit) { hasAnother = false; }
            else
            {
                AnimationClip anim = FindAnimationSubSM(animStMach, state.name);
                duration += anim.length;
                animName = state.name;
            }
        }
        Debug.Log(duration);

        return duration;
    }
}
