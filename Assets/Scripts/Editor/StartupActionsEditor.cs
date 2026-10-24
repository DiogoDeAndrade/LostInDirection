using UC.Interaction;
using UC.Interaction.Editor;
using UnityEditor;
using UnityEditorInternal;

// StartupActions inspector: the actions as a list with an "add" menu of every GameAction type, like the Verb's
[CustomEditor(typeof(StartupActions))]
public class StartupActionsEditor : UnityEditor.Editor
{
    ReorderableList actionsList;

    void OnEnable()
    {
        actionsList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("actions"), typeof(GameAction), "Actions", "Add Action", "No actions", rightHeader: "Wait", rightHeaderWidth: BaseGameActionDrawer.WaitColumnWidth);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        actionsList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
