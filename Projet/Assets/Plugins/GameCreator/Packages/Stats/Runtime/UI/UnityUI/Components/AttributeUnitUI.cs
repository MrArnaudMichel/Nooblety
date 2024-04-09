using System.Globalization;
using GameCreator.Runtime.Common;
using UnityEngine;
using UnityEngine.UI;

namespace GameCreator.Runtime.Stats.UnityUI
{
    [AddComponentMenu("Game Creator/UI/Stats/Attribute Unit UI")]
    [Icon(EditorPaths.PACKAGES + "Stats/Editor/Gizmos/GizmoAttributeUnit.png")]
    public class AttributeUnitUI : MonoBehaviour
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private UICommon m_Common = new UICommon();

        [SerializeField] private Image m_ImageFillMax;
        [SerializeField] private Image m_ImageFillCurrent;

        // PUBLIC METHODS: ------------------------------------------------------------------------

        public void Refresh(Attribute attribute, float maxValue, float currentValue, Args args)
        {
            if (this.m_Common.Icon != null) this.m_Common.Icon.overrideSprite = attribute.GetIcon(args);
            if (this.m_Common.Color != null) this.m_Common.Color.color = attribute.Color;

            this.m_Common.Name.Text = attribute.GetName(args);
            this.m_Common.Acronym.Text = attribute.GetAcronym(args);
            this.m_Common.Description.Text = attribute.GetDescription(args);

            if (this.m_ImageFillMax != null) this.m_ImageFillMax.fillAmount = maxValue;
            if (this.m_ImageFillCurrent != null) this.m_ImageFillCurrent.fillAmount = currentValue;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------
        
        private static string FromDouble(double value, string format = "")
        {
            return value.ToString(format, CultureInfo.InvariantCulture);
        }
    }
}
