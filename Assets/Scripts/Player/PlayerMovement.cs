using UnityEngine;

namespace Player
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField] private float speed;
        [SerializeField] private float rotationSpeed;
        [SerializeField] private Rigidbody playerRigidbody;

        public Vector3 PlayerPosition => transform.position;
        public Vector3 PlayerForward => transform.forward;
        private Vector3 MoveInput => Services.Services.Instance.PlayerInput.MoveInput;
        
        private const float MIN_VALUE_TO_ROTATE = 0.01f;

        private void FixedUpdate()
        {
            Vector3 move = MoveInput * (speed * Time.fixedDeltaTime);
            playerRigidbody.MovePosition(playerRigidbody.position + move);
            
            if (MoveInput.magnitude > MIN_VALUE_TO_ROTATE)
            {
                Quaternion targetRotation = Quaternion.LookRotation(MoveInput);
                playerRigidbody.rotation = Quaternion.Slerp(
                    playerRigidbody.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime);
            }
        }
    }
}