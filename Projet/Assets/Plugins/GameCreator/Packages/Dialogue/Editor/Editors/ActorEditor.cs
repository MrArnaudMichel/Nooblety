using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Dialogue
{
    [CustomEditor(typeof(Runtime.Dialogue.Actor))]
    public class ActorEditor : UnityEditor.Editor
    {
        private VisualElement m_Root;
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();
            
            SerializedProperty actant = this.serializedObject.FindProperty("m_Actant");
            SerializedProperty expressions = this.serializedObject.FindProperty("m_Expressions");
            SerializedProperty typewriter = this.serializedObject.FindProperty("m_Typewriter");
            SerializedProperty skin = this.serializedObject.FindProperty("m_OverrideSpeechSkin");
            SerializedProperty portrait = this.serializedObject.FindProperty("m_Portrait");
            
            PropertyField fieldActant = new PropertyField(actant);
            PropertyField fieldExpressions = new PropertyField(expressions);
            PropertyField fieldTypewriter = new PropertyField(typewriter);
            PropertyField fieldSkin = new PropertyField(skin, "Optional Skin");
            PropertyField fieldPortrait = new PropertyField(portrait, "Default Portrait");

            this.m_Root.Add(fieldActant);
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(fieldExpressions);
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(fieldTypewriter);
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(fieldSkin);
            this.m_Root.Add(fieldPortrait);
            
            return this.m_Root;
        }
    }
}