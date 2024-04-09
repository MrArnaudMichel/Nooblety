using GameCreator.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;

namespace GameCreator.Editor.Dialogue
{
    public class NodeTypePropertyElement : PropertyElement
    {
        private const string LABEL = "Type";
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public NodeTypePropertyElement(SerializedProperty property) : base(property, LABEL, false)
        { }

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        protected override void CreateBody()
        {
            PropertyField propertyField = new PropertyField(this.m_Property);
            
            this.m_Body.Add(propertyField);
            this.m_Body.Bind(this.m_Property.serializedObject);
        }
    }
}