using GameCreator.Editor.Common;
using GameCreator.Runtime.Dialogue.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    [CustomEditor(typeof(DialogueLogUI))]
    public class DialogueLogUIEditor : UnityEditor.Editor
    {
        private VisualElement m_Root;

        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            
            SerializedProperty actor = this.serializedObject.FindProperty("m_ActiveActor");
            SerializedProperty actorName = this.serializedObject.FindProperty("m_ActorName");
            SerializedProperty actorDesc = this.serializedObject.FindProperty("m_ActorDescription");
            SerializedProperty activePortrait = this.serializedObject.FindProperty("m_ActivePortrait");
            SerializedProperty portrait = this.serializedObject.FindProperty("m_Portrait");
            SerializedProperty text = this.serializedObject.FindProperty("m_Text");
            
            this.m_Root.Add(new PropertyField(actor));
            this.m_Root.Add(new PropertyField(actorName));
            this.m_Root.Add(new PropertyField(actorDesc));
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(new PropertyField(activePortrait));
            this.m_Root.Add(new PropertyField(portrait));
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(new PropertyField(text));

            return this.m_Root;
        }
    }
}