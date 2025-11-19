#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseEventSO<>), true)]
public class BaseEventSOEditor : Editor
{
    private SerializedProperty _sendersProp;
    private SerializedProperty _maxDisplayProp;

    private void OnEnable()
    {
        _sendersProp = serializedObject.FindProperty("_senders");
        _maxDisplayProp = serializedObject.FindProperty("_maxDisplay");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (_sendersProp.arraySize > 0)
        {
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Recent Senders", EditorStyles.boldLabel);
            
            var rect = EditorGUILayout.GetControlRect();
            EditorGUI.DrawRect(rect, new Color(0.1f, 0.1f, 0.1f, 0.2f));
            
            EditorGUI.BeginDisabledGroup(true);
            for (int i = _sendersProp.arraySize - 1; i >= 0; i--)
            {
                EditorGUILayout.LabelField(_sendersProp.GetArrayElementAtIndex(i).stringValue);
            }
            EditorGUI.EndDisabledGroup();
        }
    }
}
#endif