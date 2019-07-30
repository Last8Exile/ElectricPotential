using System;
using System.Linq;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(FieldBinding))]
public class FieldBindingEditor : Editor {

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        var property = serializedObject.FindProperty("mSettingName");
        var typeInd = serializedObject.FindProperty("mType").enumValueIndex;
        var bindingTarget = serializedObject.FindProperty("mTarget")?.objectReferenceValue;
        if (bindingTarget == null)
            return;
        Type type;
        switch (typeInd)
        {
            case 0:
                type = typeof(int);
                break;
            case 1:
                type = typeof(float);
                break;
            case 2:
                type = typeof(Vector2);
                break;
            case 3:
                type = typeof(Vector3);
                break;
            case 4:
                type = typeof(Vector4);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        var names = bindingTarget.GetType().GetProperties().Where(x => x.PropertyType == type).Select(x => x.Name).ToArray();

        var index = EditorGUILayout.Popup(names.IndexOf(property.stringValue), names);
        if (index >= 0)
        {
            property.stringValue = names[index];
            serializedObject.ApplyModifiedProperties();
        }
    }
}
