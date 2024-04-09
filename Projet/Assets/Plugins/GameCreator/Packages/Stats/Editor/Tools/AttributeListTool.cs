using System.Collections.Generic;
using GameCreator.Editor.Common;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEditor;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats
{
    public class AttributeListTool : TPolymorphicListTool
    {
        private const string NAME_BUTTON_ADD = "GC-Attr-List-Foot-Add";
        
        private static readonly IIcon ICON_ADD = new IconAttr(ColorTheme.Type.TextLight);

        // MEMBERS: -------------------------------------------------------------------------------

        private readonly AttributeList m_AttributeList;
        
        private Button m_ButtonAdd;

        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string ElementNameHead => "GC-Attr-List-Head";
        protected override string ElementNameBody => "GC-Attr-List-Body";
        protected override string ElementNameFoot => "GC-Attr-List-Foot";
        
        protected override List<string> CustomStyleSheetPaths => new List<string>
        {
            EditorPaths.PACKAGES + "Stats/Editor/StyleSheets/Attr-List"
        };

        public override bool AllowReordering => true;
        public override bool AllowDuplicating => false;
        public override bool AllowDeleting  => true;
        public override bool AllowContextMenu => true;
        public override bool AllowCopyPaste => false;
        public override bool AllowInsertion => true;
        public override bool AllowBreakpoint => false;
        public override bool AllowDisable => false;
        public override bool AllowDocumentation => false;
        
        // CONSTRUCTOR: ---------------------------------------------------------------------------

        public AttributeListTool(SerializedProperty property)
            : base(property, "m_Attributes")
        {
            this.SerializedObject.Update();
            
            this.m_AttributeList = property.GetValue<AttributeList>();
        }

        // PROTECTED METHODS: ---------------------------------------------------------------------

        protected override VisualElement MakeItemTool(int index)
        {
            return new AttributeItemTool(this, index);
        }

        protected override void SetupHead()
        { }

        protected override void SetupFoot()
        {
            base.SetupFoot();

            this.m_ButtonAdd = new Button(() =>
            {
                int insertIndex = this.PropertyList.arraySize;
                this.InsertItem(insertIndex, new AttributeItem());
            });

            this.m_ButtonAdd.name = NAME_BUTTON_ADD;
            this.m_ButtonAdd.Add(new Image { image = ICON_ADD.Texture });
            this.m_ButtonAdd.Add(new Label { text = "Add Attribute..." });

            this.m_Foot.Add(this.m_ButtonAdd);
        }
    }
}