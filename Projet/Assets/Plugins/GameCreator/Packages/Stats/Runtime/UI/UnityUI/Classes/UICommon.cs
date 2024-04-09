using System;
using GameCreator.Runtime.Common.UnityUI;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Stats.UnityUI
{
    [Serializable]
    public class UICommon
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Graphic m_Color;
        
        [SerializeField] private TextReference m_Name;
        [SerializeField] private TextReference m_Acronym;
        [SerializeField] private TextReference m_Description;

        // PROPERTIES: ----------------------------------------------------------------------------

        public Image Icon => this.m_Icon;
        public Graphic Color => this.m_Color;

        public TextReference Name => this.m_Name;
        public TextReference Acronym => this.m_Acronym;
        public TextReference Description => this.m_Description;
    }
}