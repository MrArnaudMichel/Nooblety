using UnityEditor;
using UnityEngine.UIElements;
using GameCreator.Runtime.VisualScripting;

namespace GameCreator.Editor.VisualScripting
{
    [CustomPropertyDrawer(typeof(InstructionList))]
    public class InstructionListDrawer : PropertyDrawer
    {
        public const string NAME_INSTRUCTIONS = "m_Instructions";
        
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            InstructionListTool instructionListTool = new InstructionListTool(
                property
            );
            
            return instructionListTool;
        }
    }
}