using System;
using GameCreator.Runtime.Common;
using UnityEngine;

namespace GameCreator.Runtime.Cameras
{
    [Title("Main Camera Child Index")]
    [Category("Cameras/Main Camera Child Index")]
    
    [Description("Reference to the N-th child of the Main Camera game object")]
    [Image(typeof(IconCamera), ColorTheme.Type.Green, typeof(OverlayDot))]

    [Serializable]
    public class GetGameObjectCamerasMainIndex : PropertyTypeGetGameObject
    {
        [SerializeField] 
        private PropertyGetInteger m_Index = new PropertyGetInteger();

        public override GameObject Get(Args args) => this.GetObject(args);

        private GameObject GetObject(Args args)
        {
            if (ShortcutMainCamera.Instance == null) return null;
            Transform transform = ShortcutMainCamera.Instance.transform;

            int index = (int) Math.Clamp(m_Index.Get(args), 0, transform.childCount - 1);
            Transform child = transform.GetChild(index);
            
            return child != null ? child.gameObject : null;
        }

        public static PropertyGetGameObject Create => new PropertyGetGameObject(
            new GetGameObjectCamerasMainIndex()
        );

        public override string String => $"Main Camera/{this.m_Index}";
        
        public override GameObject SceneReference
        {
            get
            {
                if (!int.TryParse(this.m_Index.ToString(), out int index)) return null;
                MainCamera instance = UnityEngine.Object.FindObjectOfType<MainCamera>();
                
                index = Math.Clamp(index, 0, instance.transform.childCount - 1);
                return index < instance.transform.childCount 
                    ? instance.transform.GetChild(index).gameObject 
                    : null;
            }
        }
    }
}