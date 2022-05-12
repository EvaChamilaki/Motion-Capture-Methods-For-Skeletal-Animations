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

    private AnimationClip animationAdd;
    private Animation animationRemove;
    AnimatorStateMachine animStateMach;
    

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        List<string> list = new List<string>(options.ToList());

        AnimatorControllerSingleton acEd = (AnimatorControllerSingleton)target;


        //=========================================EXISTING ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

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
        AnimatorController controller = acEd.Animator_Controller;

        animStateMach = controller.layers[0].stateMachine;

        if (GUILayout.Button("Apply"))
        {
            AnimationClip m = new AnimationClip { name = animationAdd.name };
            controller.AddMotion(m).AddExitTransition();
            AnimatorState add = FindState(controller, animationAdd.name);
            animStateMach.AddAnyStateTransition(add);
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
            //Debug.Log(anim.GetComponents<Animation>());
            animationRemove.RemoveClip(options[selected]);
            AnimatorState removable = FindState(controller, animationRemove.name);
            //AnimatorStateMachine.RemoveState(removable);

            //RemoveClip("idle");
            Debug.Log("removed");
        }
    }

    public AnimatorState FindState(AnimatorController _animCont, string _stateName)
    {
        for (int i = 0; i < _animCont.layers.Length; i++)
        {
            foreach (var child in _animCont.layers[i].stateMachine.states)
            {
                if (child.state.name == _stateName)
                {
                    Debug.Log("FOund it: " + child.state);
                    //Destroy(child.state);
                    return child.state;
                }
            }
        }
        Debug.LogError("Could not find state: " + _stateName + " in: " + _animCont.name);
        return null;
    }

}
