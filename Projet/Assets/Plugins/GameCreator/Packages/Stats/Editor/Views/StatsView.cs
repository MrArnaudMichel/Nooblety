using System;
using GameCreator.Runtime.Common;
using GameCreator.Runtime.Stats;
using UnityEngine.UIElements;

namespace GameCreator.Editor.Stats
{
    public class StatsView : TTraitsView
    {
        private const string TOOLTIP_MODIFIERS = "Total amount contributed by the Stat Modifiers";
        
        // PROPERTIES: ----------------------------------------------------------------------------

        protected override string Label => "Stats";

        // CONSTRUCTOR: ---------------------------------------------------------------------------
        
        public StatsView(Traits traits) : base(traits)
        {
            this.m_Traits.EventChange -= this.Rebuild;
            this.m_Traits.EventChange += this.Rebuild;
        }
        
        ~StatsView()
        {
            if (this.m_Traits == null) return;
            this.m_Traits.EventChange -= this.Rebuild;
        }
        
        // IMPLEMENTATIONS: -----------------------------------------------------------------------
        
        protected override void Rebuild()
        {
            this.m_Body.Clear();

            Class traitsClass = this.m_Traits.Class;
            
            for (int i = 0; i < traitsClass.StatsLength; i++)
            {
                StatItem statItem = traitsClass.GetStat(i);
                if (statItem == null || statItem.Stat == null) continue;
                if (statItem.IsHidden) continue;
                
                RuntimeStatData statData = this.m_Traits
                    .RuntimeStats
                    .Get(statItem.Stat.ID);
                
                if (statData == null) continue;

                VisualElement root = new VisualElement { name = TOverrideDrawer.NAME_ELEM_ROOT };
                VisualElement head = new VisualElement { name = TOverrideDrawer.NAME_ELEM_HEAD };

                Image image = new Image
                {
                    image = OverrideStatsDrawer.ICON.Texture,
                    name = TOverrideDrawer.NAME_HEAD_ICON
                };

                double value = Math.Round(statData.Value, 2);
                
                Label title = new Label
                {
                    text = $"<b>{TextUtils.Humanize(statItem.Stat.ID.String)}:</b> {value}",
                    name = TOverrideDrawer.NAME_HEAD_TEXT
                };

                double modifiersValue = Math.Round(statData.ModifiersValue, 2);
                Label modifiers = new Label
                {
                    text = modifiersValue >= 0 ? $"+{modifiersValue}" : $"{modifiersValue}",
                    name = TOverrideDrawer.NAME_HEAD_INFO,
                    tooltip = TOOLTIP_MODIFIERS
                };

                if (modifiersValue > 0) modifiers.style.color = ColorTheme.Get(ColorTheme.Type.Green);
                if (modifiersValue < 0) modifiers.style.color = ColorTheme.Get(ColorTheme.Type.Red);
                
                Button headButton = new Button();
                
                headButton.Add(image);
                headButton.Add(title);
                headButton.Add(modifiers);
                head.Add(headButton);
                root.Add(head);

                this.m_Body.Add(root);
            }
        }
    }
}