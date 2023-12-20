using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class InputHandler : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _sensitivity;
    [SerializeField] private float _gravity;
    [SerializeField] private Camera _camera;

    private float _rotationX = 0f;

    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        var xAxis = Input.GetAxis("Horizontal");
        var zAxis = Input.GetAxis("Vertical");

        var moveDirection = transform.TransformDirection(new Vector3(xAxis, 0, zAxis));
        var movement = moveDirection * _speed * Time.deltaTime;

        var rotationY = Input.GetAxis("Mouse X") * _sensitivity;
        transform.Rotate(0, rotationY, 0);

        _rotationX -= Input.GetAxis("Mouse Y") * _sensitivity;
        _rotationX = Mathf.Clamp(_rotationX, -90, 90);
        _camera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);

        if (_characterController.isGrounded is false)
        {
            movement.y -= _gravity * Time.deltaTime;
        }

        _characterController.Move(movement);
    }
}
