using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    PlayerPrefs playerPrefs;
    InputsActions _playerActions;
    InputAction _move;
    InputAction _look;
    InputAction _crouch;
    Rigidbody _rb;
    float _xRotation = 0f;
    float _colliderOriginalHeight;
    Vector3 _colliderOriginalCenter;

    [Header("Player Settings")]
    [Space(10)]
    [SerializeField] float _moveSpeed = 5f;
    [SerializeField] float _jumpForce = 3f;
    [SerializeField] float _groundCheckRadius = 0.2f;
    [SerializeField] float _groundCheckOffset = 0.05f;
    [SerializeField] LayerMask _groundLayer;
    [SerializeField] CapsuleCollider _playerCollider;
    [Header("Player Camera")]
    [Space(10)]
    [SerializeField, Range(0, 1f)] float _sensitivity = 1f;
    [SerializeField, Range(0, 180f)] float _maxLookAngle = 90f;
    [SerializeField] Camera _playerCamera;
    [SerializeField] Transform _cameraRoot;
    [Header("UI Interaction")]
    [Space(10)]
    [SerializeField] InGameUI _inGameUI;

    private void Awake()
    {
        playerPrefs = PreferencesData.LoadPreferences();
        _playerActions = new InputsActions();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        SetUpInputs();
    }

    private void Start()
    {
        UpdateSettings();
        _colliderOriginalHeight = _playerCollider.height;
        _colliderOriginalCenter = _playerCollider.center;

        InGameUI.OnSettingsChanged += UpdateSettings;
    }

    private void OnDisable()
    {
        DisableAllInputs();

        InGameUI.OnSettingsChanged -= UpdateSettings;
    }

    private void Update()
    {
        if(_inGameUI.isPaused) return;
        Look();
    }

    private void FixedUpdate()
    {
        if(_inGameUI.isPaused) return;
        MoveCamera();
        Move();
        Crouch();
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
    
    void Crouch()
    {
        int isPressed = _crouch.ReadValue<int>();
        switch (isPressed)
        {
            case 1:
                Debug.Log("Crouch started");
                float crouchHeight = _colliderOriginalHeight * 0.5f;

                _playerCollider.height = crouchHeight;
                _playerCollider.center = _colliderOriginalCenter - Vector3.up * (_colliderOriginalHeight - crouchHeight) * 0.5f;
                break;

            case 0:
                Debug.Log("Crouch released");
                _playerCollider.height = _colliderOriginalHeight;
                _playerCollider.center = _colliderOriginalCenter;
                break;
        }
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && IsGrounded())
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

    bool IsGrounded()
    {
        if (_playerCollider == null) {
            Debug.LogWarning("Player collider is not assigned in the inspector.");
            return false;
        }

        Bounds bounds = _playerCollider.bounds;

        Vector3 spherePos = new Vector3(
            bounds.center.x,
            bounds.min.y - _groundCheckOffset,
            bounds.center.z
        );

        return Physics.CheckSphere(spherePos, _groundCheckRadius, _groundLayer);
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
        _crouch = _playerActions.Player.Crouch;
        _crouch.Enable();

        _playerActions.Player.Jump.performed += OnJump;
        _playerActions.Player.Jump.Enable();
        _playerActions.Player.Escape.performed += OnEscape;
        _playerActions.Player.Escape.Enable();
    }
    void DisableAllInputs()
    {
        _move.Disable();
        _look.Disable();
        _crouch.Disable();
        _playerActions.Player.Jump.Disable();
        _playerActions.Player.Escape.Disable();
    }
    void UpdateSettings()
    {
        playerPrefs = PreferencesData.LoadPreferences();
        _sensitivity = playerPrefs.sensitivity;
    }

    private void OnDrawGizmosSelected()
    {
        if (_playerCollider == null) return;

        Bounds bounds = _playerCollider.bounds;

        Vector3 spherePos = new Vector3(
            bounds.center.x,
            bounds.min.y - _groundCheckOffset,
            bounds.center.z
        );

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spherePos, _groundCheckRadius);
    }
}
