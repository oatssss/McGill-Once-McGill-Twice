/*using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TransitionTeleport), true)]
public class TransitionTeleportEditor : Editor
{
    string[] _choices = UnityEditorInternal.InternalEditorUtility.tags;
    GUIContent[] _guiChoices;
    int[] _guiChoiceValues;
    int _choiceIndex = 0;

    SerializedProperty interactKey;
    SerializedProperty collisionTag;

    void OnEnable()
    {
        // Setup the SerializedProperties.
        interactKey = serializedObject.FindProperty("InteractKey");
        collisionTag = serializedObject.FindProperty("CollisionTag");
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        base.OnInspectorGUI();
        
        serializedObject.Update();
        
        EditorGUILayout.PropertyField (interactKey, new GUIContent ("Interact Key"));
        //  EditorGUILayout.Popup(collisionTag,)
        _choiceIndex = EditorGUILayout.
        collisionTag.
        
        //  _choiceIndex = EditorGUILayout.Popup(_choiceIndex, _choices);
        //  var transitionTeleport = target as TransitionTeleport;
        // Update the selected choice in the underlying object
        //  transitionTeleport.CollisionTag = _choices[_choiceIndex];
        // Save the changes back to the object
        //  EditorUtility.SetDirty(target);
        // Apply changes to the serializedProperty - always do this in the end of OnInspectorGUI.
		serializedObject.ApplyModifiedProperties();
    }
}
*/