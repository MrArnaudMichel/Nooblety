using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Stats
{
    [Title("Status Effect Acronym")]
    [Category("Stats/Status Effect Acronym")]

    [Image(typeof(IconStatusEffect), ColorTheme.Type.Green)]
    [Description("Returns the acronym of a Status Effect")]
    
    [Serializable] [HideLabelsInEditor]
    public class GetStringStatusEffectAcronym : PropertyTypeGetString
    {
        [SerializeField] protected StatusEffectSelector m_StatusEffect = new StatusEffectSelector();

        public override string Get(Args args) => this.m_StatusEffect.Get != null 
            ? this.m_StatusEffect.Get.GetAcronym(args) 
            : string.Empty;

        public override string String => this.m_StatusEffect.ToString();
    }
}