using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Stats
{
    [CreateAssetMenu(
        fileName = "Attribute", 
        menuName = "Game Creator/Stats/Attribute",
        order    = 50
    )]
    
    [Icon(EditorPaths.PACKAGES + "Stats/Editor/Gizmos/GizmoAttribute.png")]
    
    public class Attribute : ScriptableObject
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private IdString m_ID = new IdString("attribute-id");
        [SerializeField] private AttributeData m_Data = new AttributeData();
        [SerializeField] private AttributeInfo m_Info = new AttributeInfo();

        // PROPERTIES: ----------------------------------------------------------------------------
        
        public IdString ID => m_ID;
        public Color Color => this.m_Info.Color;
        
        public float StartPercent => m_Data.StartPercent;
        
        public double MinValue => this.m_Data.MinValue;
        public Stat MaxValue => this.m_Data.MaxValue;

        // METHODS: -------------------------------------------------------------------------------

        public string GetAcronym(Args args) => this.m_Info.m_Acronym.Get(args);
        public string GetName(Args args) => this.m_Info.m_Name.Get(args);
        public string GetDescription(Args args) => this.m_Info.m_Description.Get(args);
        
        public Sprite GetIcon(Args args) => this.m_Info.GetIcon(args);
    }
}
