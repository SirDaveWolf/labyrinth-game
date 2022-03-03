using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseHandler : MonoBehaviour
{
    public float HorizontalRotationSpeed = 1f;
    public float VerticalRotationSpeed = 1f;

    public InputActionMap ViewControls;

    private Camera _playerCamera;

    private float _xotation = 0.0f;
    private float _yRotation = 0.0f;

    private bool _isActivated;
    private EscapeMenuControls _escapeMenuControls;

    public void Activate()
    {
        _isActivated = true;
        _playerCamera.enabled = true;
        _playerCamera.GetComponent<AudioListener>().enabled = true;
    }

    public void Deactivate()
    {
        _isActivated = false;
        _playerCamera.enabled = false;
        _playerCamera.GetComponent<AudioListener>().enabled = false;
    }

    public void SetEscapeMenu(EscapeMenuControls escapeMenuControls)
    {
        _escapeMenuControls = escapeMenuControls;
    }

    // Start is called before the first frame update
    void Start()
    {
        _playerCamera = GetComponentInChildren<Camera>();
        Deactivate();
        ViewControls.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (_isActivated && _escapeMenuControls.isActiveAndEnabled == false)
        {
            float mouseX = ViewControls["Mouse X"].ReadValue<float>() * HorizontalRotationSpeed;
            float mouseY = ViewControls["Mouse Y"].ReadValue<float>() * VerticalRotationSpeed;

            _yRotation += mouseX;
            _xotation -= mouseY;
            _xotation = Mathf.Clamp(_xotation, -90, 90);

            _playerCamera.transform.eulerAngles = new Vector3(_xotation, _yRotation, 0.0f);
        }
    }
}
