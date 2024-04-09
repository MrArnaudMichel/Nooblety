using System;
using UnityEngine;

namespace GameCreator.Runtime.Dialogue
{
    [Serializable]
    public class ShotCameraList
    {
        // MEMBERS: -------------------------------------------------------------------------------

        [SerializeField] private ShotCameraEntry[] m_Shots = Array.Empty<ShotCameraEntry>();
        
        // PROPERTIES: ----------------------------------------------------------------------------

        public ShotCameraEntry[] Get => this.m_Shots;
    }
}