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

    private void Awake()
    {
    }

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
                    Debug.Log(ac.name + " : " + Animator.StringToHash(ac.name));

                    GUILayout.Label("Animation " + i + ":");
                    EditorGUILayout.SelectableLabel(ac.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUILayout.EndHorizontal();

                i++;

                list.Add(ac.name);
            }


            //==============================================ADD ANIMATIONS===============================================


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("Add Animation");

            animationAdd = EditorGUILayout.ObjectField(animationAdd, typeof(AnimationClip), true) as AnimationClip;
            AnimatorController controller = acEd.Animator_Controller;

            if (GUILayout.Button("Apply"))
            {
                Debug.Log("an:" + animationAdd);
                Debug.Log("an:" + animationAdd.name);

                AnimationClip m = new AnimationClip { name = animationAdd.name };
                controller.AddMotion(m);
                //AnimatorTransition

                Debug.Log("added");
            }

        }

        //==========================================REMOVE ANIMATIONS===============================================

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

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
            //RemoveClip("idle");
            Debug.Log("removed");
        }
    }
}
