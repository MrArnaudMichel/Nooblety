using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Serializable]
    public class Value : TPolymorphicItem<Value>
    {
        // MEMBERS: -------------------------------------------------------------------------------
        
        [SerializeField] private IdString m_Key = IdString.EMPTY;
        [SerializeField] private PropertyGetString m_Value = GetStringActorName.Create;
        
        [SerializeField] private bool m_InBold;
        [SerializeField] private bool m_InItalic;
            
        [SerializeField] private bool m_UseColor;
        [SerializeField] private PropertyGetColor m_Color = GetColorColorsYellow.Create;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public string Key => this.m_Key.String;
        
        public bool InBold => this.m_InBold;
        public bool InItalic => this.m_InItalic;

        public bool UseColor => this.m_UseColor;

        // PUBLIC METHODS: ------------------------------------------------------------------------
        
        public string GetText(Args args) => this.m_Value.Get(args);
        public Color GetColor(Args args) => this.m_Color.Get(args);
        
        // TO STRING: -----------------------------------------------------------------------------

        public override string ToString() => this.m_Value.ToString();
    }
}