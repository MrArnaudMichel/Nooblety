using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Stats
{
    [Title("Status Effect Name")]
    [Category("Stats/Status Effect Name")]

    [Image(typeof(IconStatusEffect), ColorTheme.Type.Green)]
    [Description("Returns the name of a Status Effect")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringStatusEffectName : PropertyTypeGetString
    {
        [SerializeField] protected StatusEffectSelector m_StatusEffect = new StatusEffectSelector();

        public override string Get(Args args) => this.m_StatusEffect.Get != null 
            ? this.m_StatusEffect.Get.GetName(args) 
            : string.Empty;

        public override string String => this.m_StatusEffect.ToString();
    }
}