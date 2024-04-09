using System;
using System.Globalization;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Stats
{
    [Title("Attribute Max Value")]
    [Category("Stats/Attribute Max Value")]

    [Image(typeof(IconAttr), ColorTheme.Type.Blue)]
    [Description("Returns the maximum value of an Attribute")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringAttributeMaxValue : PropertyTypeGetString
    {
        [SerializeField] private PropertyGetGameObject m_Traits = GetGameObjectPlayer.Create();
        [SerializeField] protected Attribute m_Attribute;

        public override string Get(Args args)
        {
            if (this.m_Attribute == null) return string.Empty;
            
            Traits traits = this.m_Traits.Get<Traits>(args);
            if (traits == null) return string.Empty;

            return traits.RuntimeAttributes
                .Get(this.m_Attribute.ID)?
                .MaxValue.ToString("0", CultureInfo.InvariantCulture) ?? string.Empty;
        }

        public override string String => string.Format(
            "{0}[{1}]", 
            this.m_Traits, 
            this.m_Attribute != null ? TextUtils.Humanize(this.m_Attribute.ID.String) : ""
        );
    }
}