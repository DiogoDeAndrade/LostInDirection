using UC.Interaction;
using UC.Interaction.Editor;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

// Verb inspector: the conditions and actions as lists with an "add" menu of every Condition / GameAction type,
// the same way UnityCommon's Interactable editor does it
[CustomEditor(typeof(Verb))]
public class VerbEditor : UnityEditor.Editor
{
    SerializedProperty  displayName;
    ReorderableList     conditionsList;
    ReorderableList     actionsList;

    void OnEnable()
    {
        displayName = serializedObject.FindProperty("_displayName");
        conditionsList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("conditions"), typeof(Condition), "Conditions", "Add Condition", "No conditions (always available)");
        actionsList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("actions"), typeof(GameAction), "Actions", "Add Action", "No actions", rightHeader: "Wait", rightHeaderWidth: BaseGameActionDrawer.WaitColumnWidth);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(displayName, new GUIContent("Display Name"));
        EditorGUILayout.Space(4);
        conditionsList.DoLayoutList();
        actionsList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
