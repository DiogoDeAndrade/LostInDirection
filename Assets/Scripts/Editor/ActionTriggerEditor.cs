using UC.Interaction;
using UC.Interaction.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// ActionTrigger inspector: the conditions and actions as lists with an "add" menu of every Condition / GameAction
// type, like the Verb's
[CustomEditor(typeof(ActionTrigger))]
public class ActionTriggerEditor : UnityEditor.Editor
{
    SerializedProperty  once;
    ReorderableList     conditionsList;
    ReorderableList     actionsList;

    void OnEnable()
    {
        once = serializedObject.FindProperty("once");
        conditionsList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("conditions"), typeof(Condition), "Conditions", "Add Condition", "No conditions (always fires)");
        actionsList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("actions"), typeof(GameAction), "Actions", "Add Action", "No actions", rightHeader: "Wait", rightHeaderWidth: BaseGameActionDrawer.WaitColumnWidth);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(once, new GUIContent("Once"));
        EditorGUILayout.Space(4);
        conditionsList.DoLayoutList();
        actionsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
