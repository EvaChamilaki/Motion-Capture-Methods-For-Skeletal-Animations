using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

[CustomEditor(typeof(AnimatorControllerSingleton))]
public class AnimationControllerEditor : Editor
{
    private AnimatorController obj;
    int selected = 0;
    string[] options = new string[] { };

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
            int size = acEd.anim.runtimeAnimatorController.animationClips.Length;

            GUILayout.BeginHorizontal();
            {
                GUILayout.Label("Size:          ");
                EditorGUILayout.SelectableLabel(size.ToString(), EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
            }
            GUILayout.EndHorizontal();

            int i = 0;

            foreach (AnimationClip ac in acEd.anim.runtimeAnimatorController.animationClips)
            {
                //list[i] = EditorGUILayout.ObjectField("Animation " + i, ac, typeof(GameObject), true) as Animation;
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Label("Animation " + i + ":");
                    EditorGUILayout.SelectableLabel(ac.name, EditorStyles.textField, GUILayout.Height(EditorGUIUtility.singleLineHeight));
                }
                GUILayout.EndHorizontal();

                i++;

                list.Add(ac.name);
                Debug.Log("animation clip: " + ac);
            }


            //==============================================ADD ANIMATIONS===============================================


            EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

            GUILayout.Label("Add Animation");

            var input = "";
            var text = EditorGUILayout.TextField("");
            AnimatorController controller = obj as AnimatorController;

            if (GUILayout.Button("Apply"))
            {
                input = text;
                Debug.Log(input);
                AnimationClip m = new AnimationClip { name = text };
                controller.AddMotion(m);
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
            Debug.Log("removed");
        }
    }
}
