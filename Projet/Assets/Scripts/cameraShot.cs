using UnityEngine;

namespace Nooblety.core.camera
{
    public class CameraShot : MonoBehaviour
    {
        public Camera mainCamera;
        public bool isMainShot;
        public Projection projection;
        public Transform target;
        public Vector3 offset;

        void Start()
        {
            SetupShot();
        }

        void Update()
        {
            UpdateShot();
        }

        protected virtual void SetupShot()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            switch (projection)
            {
                case Projection.Perspective:
                    mainCamera.orthographic = false;
                    break;
                case Projection.Orthographic:
                    mainCamera.orthographic = true;
                    break;
            }

            if (target != null)
            {
                mainCamera.transform.position = target.position + offset;
                mainCamera.transform.LookAt(target);
            }
        }

        protected virtual void UpdateShot()
        {
            if (target != null)
            {
                mainCamera.transform.position = target.position + offset;
            }
        }
    }
}