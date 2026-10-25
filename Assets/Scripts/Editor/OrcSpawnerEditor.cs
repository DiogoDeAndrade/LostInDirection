using UC.Interaction;
using UC.Interaction.Editor;
using UnityEditor;
using UnityEditorInternal;

// OrcSpawner inspector: its settings, then the actions it runs between waves and once the orcs are dealt with, as
// lists with an "add" menu of every GameAction type, like the Verb's
[CustomEditor(typeof(OrcSpawner))]
public class OrcSpawnerEditor : UnityEditor.Editor
{
    ReorderableList waveList;
    ReorderableList clearedList;

    void OnEnable()
    {
        waveList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("onWaveCleared"), typeof(GameAction), "Between waves", "Add Action", "No actions", rightHeader: "Wait", rightHeaderWidth: BaseGameActionDrawer.WaitColumnWidth);
        clearedList = ManagedReferenceListHelper.Build(serializedObject, serializedObject.FindProperty("onCleared"), typeof(GameAction), "When the orcs are dealt with", "Add Action", "No actions", rightHeader: "Wait", rightHeaderWidth: BaseGameActionDrawer.WaitColumnWidth);
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawPropertiesExcluding(serializedObject, "m_Script", "onWaveCleared", "onCleared");
        EditorGUILayout.Space(4);
        waveList.DoLayoutList();
        clearedList.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
    }
}
