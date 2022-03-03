using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MovementHandler : MonoBehaviour
{
    public float MovementSpeed = 1;
    public float Gravity = 9.8f;

    private CharacterController _characterController;
    private Camera _playerCamera;

    public InputActionMap PlayerControls;

    private float _velocity = 0;
    private bool _isSprinting = false;

    private bool _isActivated;

    private GameObject _currentTile;

    private Vector3? _newPosition;

    private EscapeMenuControls _escapeMenuControls;

    public void Activate()
    {
        _isActivated = true;
    }

    public void Deactivate()
    {
        _isActivated = false;
    }

    public GameObject GetCurrentTile()
    {
        return _currentTile;
    }

    public void SetNewPosition(Vector3 newPosition)
    {
        _newPosition = newPosition;
    }

    public void SetEscapeMenu(EscapeMenuControls escapeMenuControls)
    {
        _escapeMenuControls = escapeMenuControls;
    }

    // Start is called before the first frame update
    void Start()
    {
        _isActivated = false;
        _characterController = GetComponent<CharacterController>();
        _playerCamera = GetComponentInChildren<Camera>();
        _newPosition = null;

        PlayerControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActivated && _escapeMenuControls.isActiveAndEnabled == false)
        {
            _isSprinting = PlayerControls["Sprint"].IsPressed();

            var horizontalSpeed = PlayerControls["Horizontal"].ReadValue<float>() * MovementSpeed;
            var verticalSpeed = PlayerControls["Vertical"].ReadValue<float>() * MovementSpeed;

            var xMovement = _playerCamera.transform.right * horizontalSpeed;
            var yMovement = _playerCamera.transform.forward * verticalSpeed;

            if (_isSprinting)
                yMovement *= 2;

            var xyMovement = xMovement + yMovement;

            _characterController.Move(xyMovement * Time.deltaTime);

            if (_characterController.isGrounded)
            {
                _velocity = 0;
            }
            else
            {
                _velocity -= Gravity * Time.deltaTime;
                _characterController.Move(new Vector3(0, _velocity, 0));
            }
        }

        DoSetNextPosition();
    }

    private void DoSetNextPosition()
    {
        if(_newPosition != null)
        {
            transform.position = _newPosition.Value;
            _newPosition = null;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Contains("Collectable") ||
            other.tag.Contains("PlayerStart"))
        {
            var playerId = Globals.GetPlayerId(tag);
            GameRules.SetPlayerLastTouched(playerId, other.tag);
        }
        else
        {
            _currentTile = other.gameObject;
        }
    }
}
