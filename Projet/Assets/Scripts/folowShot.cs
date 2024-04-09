using UnityEngine;

namespace Nooblety.core.camera
{
    public class FollowShot : CameraShot
    {
        public Vector3 distance;

        protected override void SetupShot()
        {
            base.SetupShot();
        }

        protected override void UpdateShot()
        {
            base.UpdateShot();
            if (target != null)
            {
                transform.position = target.position + distance;
            }
        }
    }
}
