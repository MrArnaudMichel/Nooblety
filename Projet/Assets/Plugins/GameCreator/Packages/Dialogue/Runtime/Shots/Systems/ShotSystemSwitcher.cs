using System;
using System.Collections.Generic;
using GameCreator.Runtime.Cameras;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Serializable]
    public class ShotSystemSwitcher : TShotSystem
    {
        public static readonly int ID = nameof(ShotSystemSwitcher).GetHashCode();

        // EXPOSED MEMBERS: -----------------------------------------------------------------------

        [SerializeField] private ShotCameraList m_Shots = new ShotCameraList();
        
        [SerializeField] private bool m_EnsureClearShot;
        [SerializeField] private float m_Timeout = 5f;

        // MEMBERS: -------------------------------------------------------------------------------

        [NonSerialized] private ShotCamera m_CurrentShot;
        [NonSerialized] private int m_CurrentIndex = -1;
        [NonSerialized] private int m_PreviousIndex = -1;

        [NonSerialized] private TCamera m_Camera;
        [NonSerialized] private float m_NextSwitch = -9999f;
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public override int Id => ID;

        public ShotCamera CurrentShot => this.m_CurrentShot;

        // CONSTRUCTORS: --------------------------------------------------------------------------

        public ShotSystemSwitcher() : base()
        { }

        // IMPLEMENTS: ----------------------------------------------------------------------------

        public override void OnAwake(TShotType shotType)
        {
            base.OnAwake(shotType);

            this.m_CurrentShot = null;
            this.m_CurrentIndex = -1;
            this.m_PreviousIndex = -1;
        }

        public override void OnEnable(TShotType shotType, TCamera camera)
        {
            base.OnEnable(shotType, camera);

            this.m_Camera = camera;
            this.SelectNewShot(shotType);
        }

        public override void OnDisable(TShotType shotType, TCamera camera)
        {
            base.OnDisable(shotType, camera);
            
            ShotCamera shot = this.CurrentShot;
            if (shot != null) shot.OnDisableShot(camera);
        }

        public override void OnUpdate(TShotType shotType)
        {
            base.OnUpdate(shotType);

            if (shotType.ShotCamera.TimeMode.Time > this.m_NextSwitch)
            {
                if (shotType.IsActive && this.m_Camera != null)
                {
                    this.m_Camera.Transition.ChangeToShot(shotType.ShotCamera);
                }
            }

            ShotCamera shot = this.CurrentShot;
            if (shot == null) return;
            
            shotType.Position = shot.Position;
            shotType.Rotation = shot.Rotation;
        }
        
        // PRIVATE METHODS: -----------------------------------------------------------------------

        private void SelectNewShot(TShotType shotType)
        {
            this.m_PreviousIndex = this.m_CurrentIndex;

            ShotCameraEntry[] list = this.m_Shots.Get;
            List<ShotCamera> shots = new List<ShotCamera>();
            
            for (int i = list.Length - 1; i >= 0; --i)
            {
                ShotCamera shot = list[i].Get;
                if (this.m_EnsureClearShot && shot.HasObstacle) continue;

                shots.Add(shot);
            }
            
            switch (list.Length)
            {
                case 0: return;
                case 1: this.m_CurrentIndex = 0; break;
                default:
                {
                    this.m_CurrentIndex = UnityEngine.Random.Range(0, shots.Count - 1);
                    if (this.m_CurrentIndex == this.m_PreviousIndex) this.m_CurrentIndex += 1;
                    break;
                }
            }

            this.m_CurrentShot = shots[this.m_CurrentIndex];
            if (this.m_CurrentShot == null) return;
            
            this.m_CurrentShot.OnEnableShot(this.m_Camera);
            this.m_NextSwitch = shotType.ShotCamera.TimeMode.Time + this.m_Timeout;
        }
    }
}