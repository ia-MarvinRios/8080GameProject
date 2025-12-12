using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    InputsActions _playerActions;
    InputAction _move;
    InputAction _look;
    Rigidbody _rb;
    float _xRotation = 0f;

    [Header("Player Settings")]
    [Space(10)]
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 3f;
    [Header("Player Camera")]
    [Space(10)]
    [SerializeField, Range(0, 2f)] float _sensitivity = 1f;
    [SerializeField, Range(0, 180f)] float _maxLookAngle = 90f;
    [SerializeField] Camera _playerCamera;
    [SerializeField] Transform _cameraRoot;
    [Header("UI Interaction")]
    [Space(10)]
    [SerializeField] InGameUI _inGameUI;

    private void Awake()
    {
        _playerActions = new InputsActions();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        SetUpInputs();
    }

    private void OnDisable()
    {
        DisableAllInputs();
    }

    private void Update()
    {
        if(_inGameUI.isPaused) return;
        Look();
        MoveCamera();
    }

    private void FixedUpdate()
    {
        if(_inGameUI.isPaused) return;
        Move();
    }

    void Look()
    {
        if (_playerCamera == null) {
            Debug.LogWarning("Player Camera is not assigned in the inspector.");
            return;
        }

        Vector2 lookDir = _look.ReadValue<Vector2>() * _sensitivity;

        // horizontal
        transform.Rotate(Vector3.up, lookDir.x);

        // clamped vertical
        _xRotation -= lookDir.y;
        _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);

        _cameraRoot.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }
    void Move()
    {
        Vector2 moveDir = _move.ReadValue<Vector2>();

        Vector3 direction =
            (transform.forward * moveDir.y) +
            (transform.right * moveDir.x);

        direction.y = 0f;

        _rb.linearVelocity = new Vector3(direction.x * _moveSpeed, _rb.linearVelocity.y, direction.z * _moveSpeed);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _rb.AddForce(Vector3.up * _jumpForce, ForceMode.Impulse);
        }
    }

    void OnEscape(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (_inGameUI == null) { 
                Debug.LogWarning("InGameUI is not assigned in the inspector.");
                return;
            }

            _inGameUI.TogglePauseMenu();
        }
    }

    void MoveCamera()
    {
        if (_playerCamera != null)
        {
            _playerCamera.transform.position = _cameraRoot.position;
            _playerCamera.transform.rotation = _cameraRoot.rotation;
        }
    }

    void SetUpInputs()
    {
        _move = _playerActions.Player.Move;
        _move.Enable();
        _look = _playerActions.Player.Look;
        _look.Enable();

        _playerActions.Player.Jump.performed += OnJump;
        _playerActions.Player.Jump.Enable();
        _playerActions.Player.Escape.performed += OnEscape;
        _playerActions.Player.Escape.Enable();
    }
    void DisableAllInputs()
    {
        _move.Disable();
        _look.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Escape.Disable();
    }
}
