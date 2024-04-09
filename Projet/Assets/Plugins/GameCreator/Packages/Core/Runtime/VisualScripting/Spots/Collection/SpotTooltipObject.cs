using System;
using UnityEngine;
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.VisualScripting
{
    [Title("Activate Object")]
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Yellow)]
    
    [Category("Activate Object")]
    [Description(
        "Activates a game object scene instance when the Hotspot is enabled and " +
        "deactivates it when the Hotspot is disabled"
    )]

    [Serializable]
    public class SpotTooltipObject : Spot
    {
        // EXPOSED MEMBERS: -----------------------------------------------------------------------
        
        [SerializeField] protected GameObject m_GameObject;

        // PROPERTIES: ----------------------------------------------------------------------------

        public override string Title => string.Format(
            "Show {0}",
            this.m_GameObject != null ? this.m_GameObject.name : "(none)"
        );

        // OVERRIDE METHODS: ----------------------------------------------------------------------

        public override void OnUpdate(Hotspot hotspot)
        {
            base.OnUpdate(hotspot);
            
            if (this.m_GameObject == null) return;

            bool isActive = this.EnableInstance(hotspot);
            this.m_GameObject.SetActive(isActive);
        }

        public override void OnDisable(Hotspot hotspot)
        {
            base.OnDisable(hotspot);
            
            if (this.m_GameObject == null) return;
            this.m_GameObject.SetActive(false);
        }

        // VIRTUAL METHODS: -----------------------------------------------------------------------

        protected virtual bool EnableInstance(Hotspot hotspot)
        {
            return hotspot.IsActive;
        }
    }
}