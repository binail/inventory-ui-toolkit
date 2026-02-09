using UnityEngine;

namespace Camera
{
    public class ObserverCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset;
        [SerializeField] private float smoothTime;
        [SerializeField] private float lookHeight;

        private Vector3 _velocity;
        
        private Vector3 CameraTargetPosition => target.position + offset;

        private void OnEnable()
        {
            transform.position = CameraTargetPosition;
        }
        
        private void LateUpdate()
        {
            Vector3 targetPos = CameraTargetPosition;
            transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref _velocity, smoothTime);
            transform.LookAt(target.position + Vector3.up * lookHeight);
        }
    }
}