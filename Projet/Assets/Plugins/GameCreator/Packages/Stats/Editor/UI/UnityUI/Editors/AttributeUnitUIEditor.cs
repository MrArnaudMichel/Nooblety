using GameCreator.Editor.Common;
using GameCreator.Editor.Common.UnityUI;
using GameCreator.Runtime.Stats.UnityUI;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats.UnityUI
{
    [CustomEditor(typeof(AttributeUnitUI))]
    public class AttributeUnitUIEditor : UnityEditor.Editor
    {
        private const string TEXT = "Automatically refreshed by Attribute UI parent";
        
        // MEMBERS: -------------------------------------------------------------------------------

        private VisualElement m_Root;

        // PAINT METHOD: --------------------------------------------------------------------------
        
        public override VisualElement CreateInspectorGUI()
        {
            this.m_Root = new VisualElement();

            SerializedProperty common = this.serializedObject.FindProperty("m_Common");
            SerializedProperty max = this.serializedObject.FindProperty("m_ImageFillMax");
            SerializedProperty current = this.serializedObject.FindProperty("m_ImageFillCurrent");
            
            this.m_Root.Add(new SpaceSmaller());
            this.m_Root.Add(new InfoMessage(TEXT));
            
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(new PropertyField(common));
            
            this.m_Root.Add(new SpaceSmall());
            this.m_Root.Add(new LabelTitle("Progress:"));
            this.m_Root.Add(new PropertyField(max));
            this.m_Root.Add(new PropertyField(current));
            
            return this.m_Root;
        }
    }
}