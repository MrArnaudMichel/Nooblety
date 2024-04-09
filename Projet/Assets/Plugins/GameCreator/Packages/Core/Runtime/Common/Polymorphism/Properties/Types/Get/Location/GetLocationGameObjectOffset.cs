using System;
using UnityEngine;

namespace GameCreator.Runtime.Common
{
    [Title("Game Object with Offset")]
    [Category("Game Objects/Game Object with Offset")]
    
    [Image(typeof(IconCubeSolid), ColorTheme.Type.Blue, typeof(OverlayArrowRight))]
    [Description("The position and rotation of a Game Object plus an offset in local space")]

    [Serializable]
    public class GetLocationGameObjectOffset : PropertyTypeGetLocation
    {
        [SerializeField]
        private PropertyGetGameObject m_GameObject = GetGameObjectInstance.Create();
        
        [SerializeField] private bool m_Rotate = true;
        
        [SerializeField]
        private Vector3 m_LocalOffset = Vector3.forward;
        
        public override Location Get(Args args)
        {
            GameObject gameObject = this.m_GameObject.Get(args);
            if (gameObject == null) return new Location();

            Marker marker = gameObject.Get<Marker>();
            return marker != null 
                ? new Location(marker, this.m_LocalOffset) 
                : new Location(gameObject.transform, Space.Self, this.m_LocalOffset, this.m_Rotate, Quaternion.identity);
        }

        public static PropertyGetLocation Create => new PropertyGetLocation(
            new GetLocationGameObjectOffset()
        );

        public override string String => $"{this.m_GameObject}";
    }
}