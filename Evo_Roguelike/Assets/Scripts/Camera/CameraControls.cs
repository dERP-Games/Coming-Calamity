using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEditor.Timeline;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControls : MonoBehaviour
{
    /*
     * This class uses the custom input action mapping so that the player can control the camers
     */

    [SerializeField]
    private float _distanceFromEdgeToScroll = 50f;
    [SerializeField]
    private float _cameraPanSpeed = 5f;
    [SerializeField, Min(0.001f)]
    private float _zoomSpeed = 0.5f;
    [SerializeField, Min(0f)]
    private float _minOrthoSize = 1f;
    [SerializeField, Min(0f)]
    private float _maxOrthoSize = 12f;

    private CustomInput _input = null;
    private Camera _camera;

    private void Awake()
    {
        _input = new CustomInput();
        _camera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.CameraZoom.performed += OnCameraZoom;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.CameraZoom.performed -= OnCameraZoom;
    }

    void Update()
    {
        CheckForCameraPan();
    }

    private void CheckForCameraPan()
    {
        /*
         * This function checks if the camera should start panning
         * Checks both mouse at edge of screen and action mappings
         */

        // Checking if mouse at edges of screen
        Vector2 mousePos = Mouse.current.position.ReadValue();
        if (mousePos.x < _distanceFromEdgeToScroll && mousePos.x > 0)
        {
            transform.position = new Vector3(transform.position.x - _cameraPanSpeed, transform.position.y, transform.position.z);
        }
        else if (mousePos.x > Screen.width - _distanceFromEdgeToScroll && mousePos.x < Screen.width)
        {
            transform.position = new Vector3(transform.position.x + _cameraPanSpeed, transform.position.y, transform.position.z);
        }

        if (mousePos.y < _distanceFromEdgeToScroll && mousePos.y > 0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - _cameraPanSpeed, transform.position.z);
        }
        else if (mousePos.y > Screen.height - _distanceFromEdgeToScroll && mousePos.y < Screen.height)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + _cameraPanSpeed, transform.position.z);
        }

        // Checking for action mappings
        if (_input.Player.CameraPan.IsPressed())
        {
            Vector2 cameraMoveVector = _input.Player.CameraPan.ReadValue<Vector2>();
            transform.position = new Vector3(transform.position.x + (cameraMoveVector.x * _cameraPanSpeed),
                                         transform.position.y + (cameraMoveVector.y * _cameraPanSpeed),
                                         transform.position.z);
        }

    }

    private void OnCameraZoom(InputAction.CallbackContext value)
    {
        /*
         * This function is called when the "CameraZoom" action is performed.
         * Input
         * value : Input data of action 
         */

        if (!IsMouseWithinScreen()) return;

        float axisValue = value.ReadValue<float>();
        axisValue = Mathf.Clamp(axisValue, -1, 1);

        Debug.Log(_camera.orthographicSize);

        _camera.orthographicSize = Mathf.Clamp(_camera.orthographicSize + (axisValue * _zoomSpeed), _minOrthoSize, _maxOrthoSize);
    }

    private bool IsMouseWithinScreen()
    {
        /*
         * Checks if mouse is within viewport/screen
         */

        Vector2 mousePos = Mouse.current.position.ReadValue();
        if(mousePos.x > 0 && mousePos.y > 0 && mousePos.x < Screen.width && mousePos.y < Screen.height) { return true; }

        return false;
    }
}


