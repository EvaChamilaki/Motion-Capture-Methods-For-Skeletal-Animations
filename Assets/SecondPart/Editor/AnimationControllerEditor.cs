using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimatorControllerSingleton))]

public class AnimationControllerEditor : Editor
{
    int selected = 0;

    private AnimationClip animationAdd, addAnimationAfterThisOne, addAnimationBeforeThisOne
        , addDefaultState, animationAddToStateMach, addAnimationAfterThisOneSubSM, addAnimationBeforeThisOneSubSM
        , replaceDefaultState, replaceThisAnimation, withThisAnimation;
    private GameObject addCharacter, removeCharacter;
    private AnimatorStateMachine animStateMach, stateMachine;
    string[] options = new string[] { };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUIStyle style = new GUIStyle();
        style.richText = true;

        AnimatorControllerSingleton acEd = (AnimatorControllerSingleton)target;

        List<string> list = new List<string>(options.ToList());
        AnimatorController controller = acEd.Animator_Controller;

        //==============================================CHARACTERS===================================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        acEd.showChars = EditorGUILayout.Foldout(acEd.showChars, "Characters", true);

        int c = 0;
        if (acEd.showChars)
        {
            foreach (GameObject character in acEd.Characters)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Character " + c + ":");
                    EditorGUILayout.SelectableLabel(character.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUILayout.EndHorizontal();

                c++;
            }

            GUILayout.Space(10);

            GUILayout.Label("<color=#ffffffff> <b>Add</b> Character:</color>", style);

            addCharacter = EditorGUILayout.ObjectField(addCharacter, typeof(GameObject), true) as GameObject;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                acEd.Characters.Add(addCharacter);
            }

            GUILayout.Space(10);

            GUILayout.Label("<color=#ffffffff><b>Remove</b> Character:</color>", style);

            removeCharacter = EditorGUILayout.ObjectField(removeCharacter, typeof(GameObject), true) as GameObject;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                acEd.Characters.Remove(removeCharacter);
            }
        }

        //=========================================EXISTING ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        animStateMach = controller.layers[0].stateMachine;

        acEd.showAnims = EditorGUILayout.Foldout(acEd.showAnims, "Existing Animations", true);

        int l = 0;

        foreach (AnimationClip ac in acEd.Animator_Controller.animationClips)
        {
            list.Add(ac.name);

            l++;
        }

        if (acEd.showAnims)
        {
            int size = acEd.Animator_Controller.animationClips.Length;
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Size:          ");
                EditorGUILayout.SelectableLabel(size.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            GUILayout.EndHorizontal();

            int i = 0;

            foreach (AnimationClip ac in acEd.Animator_Controller.animationClips)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Animation " + i + ":");
                    EditorGUILayout.SelectableLabel(ac.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUILayout.EndHorizontal();

                i++;
            }
        }


        //===========================================ADD IDLE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        acEd.showDefault = EditorGUILayout.Foldout(acEd.showDefault, "Default animation", true);

        if (acEd.showDefault)
        {
            GUILayout.Space(10);

            GUILayout.Label("<color=#ffffffff>Set as <b>Default</b> Animation:</color>", style);

            addDefaultState = EditorGUILayout.ObjectField(addDefaultState, typeof(AnimationClip), true) as AnimationClip;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                AnimationClip m = new AnimationClip { name = addDefaultState.name };

                animStateMach.AddState(m.name).motion = addDefaultState;
                AnimatorState default_state = FindState(controller, addDefaultState.name);

                default_state.motion = addDefaultState;
                list.Add(addDefaultState.name);
                animStateMach.defaultState = default_state;

                AnimatorStateMachine ast = animStateMach.AddStateMachine("IdleStates");
                animStateMach.AddStateMachineTransition(ast, default_state);

                default_state.AddTransition(ast);
            }

            GUILayout.Space(10);

            GUILayout.Label("<color=#ffffffff><b>Replace</b> Default Animation with this Animation:</color>", style);

            replaceDefaultState = EditorGUILayout.ObjectField(replaceDefaultState, typeof(AnimationClip), true) as AnimationClip;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply"))
            {
                AnimatorState default_state = animStateMach.defaultState;

                default_state.motion = replaceDefaultState;
                default_state.name = replaceDefaultState.name;
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        acEd.showSubStateMach = EditorGUILayout.Foldout(acEd.showSubStateMach, "Sub State Machine (random animations)", true);

        if (acEd.showSubStateMach)
        {
            if (animStateMach.stateMachines.Length != 0)
            {
                stateMachine = animStateMach.stateMachines[0].stateMachine;
                GUILayout.Space(10);


                GUILayout.Label("<color=#ffffffff><b>Add</b> this Animation to the " + animStateMach.stateMachines[0].stateMachine.name + ":</color>", style);

                animationAddToStateMach = EditorGUILayout.ObjectField(animationAddToStateMach, typeof(AnimationClip), true) as AnimationClip;
                GUILayout.Space(10);

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
                        }
                    }
                    else if (addAnimationBeforeThisOneSubSM != null && addAnimationAfterThisOneSubSM != null)
                    {
                        BetweenTwoAnimStatesAddStateSubSM(stateMachine);
                    }
                }
            }
        }

        //==============================================ADD ANIMATIONS===============================================


        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        acEd.showAnimContr = EditorGUILayout.Foldout(acEd.showAnimContr, "Animator Controller", true);

        if (acEd.showAnimContr)
        {
            GUILayout.Space(10);

            GUILayout.Label("<color=#ffffffff><b>Add</b> this Animation:</color>", style);

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
                    }
                }
                else if (addAnimationBeforeThisOne != null && addAnimationAfterThisOne != null)
                {
                    BetweenTwoAnimStatesAddState(controller);
                }
            }
        }


        //==========================================REMOVE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        GUILayout.Label("<color=#ffffffff><b>Replace</b> this animation:</color>", style);
        replaceThisAnimation = EditorGUILayout.ObjectField(replaceThisAnimation, typeof(AnimationClip), true) as AnimationClip;

        GUILayout.Space(10);

        GUILayout.Label("<color=#ffffffff>with <b>this</b> animation:</color>", style);
        withThisAnimation = EditorGUILayout.ObjectField(withThisAnimation, typeof(AnimationClip), true) as AnimationClip;

        GUILayout.Space(10);

        if (GUILayout.Button("Replace"))
        {
            AnimatorState stateInBase = FindState(controller, replaceThisAnimation.name);
            if (stateInBase == null)
            {
                stateMachine = animStateMach.stateMachines[0].stateMachine;

                AnimatorState stateInSubSM = FindStateSubSM(stateMachine, replaceThisAnimation.name);

                if (stateInSubSM == null)
                {
                    Debug.LogError("The animation you're trying to replace doesn't exist.");
                }
                else
                {
                    stateInSubSM.motion = withThisAnimation;
                    stateInSubSM.name = withThisAnimation.name;
                }
            }
            else {
                stateInBase.motion = withThisAnimation;
                stateInBase.name = withThisAnimation.name;
            }
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);


        EditorGUI.BeginChangeCheck();

        options = list.ToArray();

        //this.selected = EditorGUILayout.Popup("Play This Animation", selected, options);


        this.selected = EditorGUILayout.Popup("Remove Animation", selected, options);

        if (EditorGUI.EndChangeCheck())
        {
            Debug.Log(options[selected]);
            acEd.selected_option = options[selected];
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Remove"))
        {
            AnimatorState removable = FindState(controller, options[selected]);
            if (removable == null)
            {
                stateMachine = animStateMach.stateMachines[0].stateMachine;

                AnimatorState removeSt = FindStateSubSM(stateMachine, options[selected]);
                AnimatorState beforeRemovablesubSM = FindPreviousStateSubSM(stateMachine, options[selected]);
                AnimatorState afterRemovablesubSM = FindNextStateSubSM(stateMachine, options[selected]);

                AnimatorStateTransition trans = removeSt.transitions[0];

                if (afterRemovablesubSM == null && trans.isExit)
                {
                    removeSt.RemoveTransition(trans);
                    stateMachine.RemoveState(removeSt);

                    beforeRemovablesubSM.AddExitTransition();
                }
                else if (beforeRemovablesubSM != null && afterRemovablesubSM == null)
                {
                    foreach (var asTrans in stateMachine.entryTransitions)
                    {
                        if (asTrans.destinationState.name == removeSt.name)
                        {
                            stateMachine.RemoveEntryTransition(asTrans);
                            stateMachine.RemoveState(removeSt);
                        }
                    }
                }
                else if (beforeRemovablesubSM != null && afterRemovablesubSM != null)
                {
                    beforeRemovablesubSM.AddTransition(afterRemovablesubSM);
                    stateMachine.RemoveState(removeSt);
                }
                else if (beforeRemovablesubSM == null && afterRemovablesubSM == null)
                {
                    stateMachine.RemoveState(removeSt);
                }
                else if (beforeRemovablesubSM == null && afterRemovablesubSM != null)
                {
                    foreach (var asTrans in stateMachine.entryTransitions)
                    {
                        if (asTrans.destinationState.name == removeSt.name)
                        {
                            stateMachine.AddEntryTransition(afterRemovablesubSM);

                            stateMachine.RemoveEntryTransition(asTrans);
                            stateMachine.RemoveState(removeSt);
                        }
                    }
                }
            }
            else
            {
                AnimatorState beforeRemovable = FindPreviousState(controller, options[selected]);
                AnimatorState afterRemovable = FindNextState(controller, options[selected]);
                AnimatorState default_state = animStateMach.defaultState;

                AnimatorStateTransition trans = removable.transitions[0];


                if (beforeRemovable != null && trans.destinationState == default_state)
                {
                    removable.RemoveTransition(trans);
                    animStateMach.RemoveState(removable);

                    beforeRemovable.AddTransition(default_state);
                }
                else if (beforeRemovable == null && trans.destinationState == default_state)
                {
                    foreach (var asTrans in animStateMach.anyStateTransitions)
                    {
                        if (asTrans.destinationState.name == removable.name)
                        {
                            animStateMach.RemoveAnyStateTransition(asTrans);
                            animStateMach.RemoveState(removable);
                        }
                    }
                }
                else if (beforeRemovable != null && afterRemovable != null)
                {
                    beforeRemovable.AddTransition(afterRemovable);
                    animStateMach.RemoveState(removable);
                }
                else if (beforeRemovable == null && afterRemovable == null)
                {
                    animStateMach.RemoveState(removable);
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
                        }
                    }
                }
            }
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

        animStateMach.AddState(m.name).motion = animationAdd;
        AnimatorState add = FindState(animCont, animationAdd.name);

        add.motion = animationAdd;
        add.AddTransition(default_state);

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

            animStateMach.AddState(m.name).motion = animationAdd;
            AnimatorState add = FindState(animCont, animationAdd.name);

            add.motion = animationAdd;
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

            animStateMach.AddState(m.name).motion = animationAdd;
            AnimatorState add = FindState(animCont, animationAdd.name);

            add.motion = animationAdd;
            add.AddTransition(beforeThisState);
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

        animStateMach.AddState(m.name).motion = animationAdd;
        AnimatorState add = FindState(animCont, animationAdd.name);

        add.motion = animationAdd;
        add.AddTransition(default_state);
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

                animStateMach.AddState(m.name).motion = animationAdd;
                AnimatorState add = FindState(animCont, animationAdd.name);

                add.motion = animationAdd;
                add.AddTransition(animPrev);
                animAfter.AddTransition(add);

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
            }
        }
    }

    public void FromAnyStateTransitionAddState(AnimatorController animCont, int i)
    {
        AnimationClip m = new AnimationClip { name = animationAdd.name };

        AnimatorState animationStateAddBeforeThis = FindState(animCont, addAnimationBeforeThisOne.name);

        animStateMach.AddState(m.name).motion = animationAdd;
        AnimatorState add = FindState(animCont, animationAdd.name);

        add.motion = animationAdd;
        add.AddTransition(animationStateAddBeforeThis);
        animStateMach.AddAnyStateTransition(add);

        animStateMach.RemoveAnyStateTransition(animStateMach.anyStateTransitions[i]);
    }


    //==============================================ADD STATE FUNCTIONS(Sub State Machine)=====================================================


    public void SimpleAdditionSubSM(AnimatorStateMachine stateMachine)
    {
        stateMachine.AddState(animationAddToStateMach.name).motion = animationAddToStateMach;

        AnimatorState newStateInSubSM = FindStateSubSM(stateMachine, animationAddToStateMach.name);
        stateMachine.AddEntryTransition(newStateInSubSM);

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
