using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Timeline.TimelinePlaybackControls;

public class PlayerController : MonoBehaviour
{
    #region Editor Parameters
    [SerializeField] protected bool debugPrint;
    [SerializeField] protected bool enableGrav;

    [SerializeField] float lateralThrust = 1000;
    //[SerializeField] float verticalThrust = 600;
    [SerializeField] float accelationLimit = 10;

    [SerializeField] float minSpeedForGrav = 10;

    [SerializeField] float _rotationalSnapStrength = 3f;
    [SerializeField] float _rotationalSensitivity = 0.05f;

    [SerializeField] float VerticalGravity = 9.81f;
    [SerializeField] float HorizontalGravity = 3f;

    [SerializeField] float startingFov = 63.0f;
    [SerializeField] float FOVIncrease = 15.0f;
    [SerializeField] float MaxSpeedForCamera = 50.0f;
    #endregion
    #region Serialized References
    [Header("Serialized References")]
    [SerializeField] Rigidbody _rb;
    [SerializeField] CinemachineVirtualCamera _camera;
    [SerializeField] PlayerInput _input;
    #endregion
    #region Internal Values

    Quaternion _desiredRotation;
    float _gravity;

    #region Input Values

    float i_MovementMagnitude = 0;
    Vector3 i_RotationDirection = Vector3.zero;

    #endregion
    #endregion

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _desiredRotation = transform.rotation;
    }

    void Update()
    {
        RotateShip();

        UpdateGravity();

        if (debugPrint)
        {
            Debug.Log("Velcity: " + _rb.velocity.magnitude);
        }

        FovMAXXER();
    }

    void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        Vector3 TargetVelocity = new Vector3(0, 0, i_MovementMagnitude * lateralThrust); //This velocity is in local space

        if (enableGrav)
        {
            Vector3 grav = new Vector3(0, _rb.mass * -_gravity * Time.deltaTime, 0);
            grav = transform.InverseTransformDirection(grav);
            
            if(_rb.velocity.magnitude < minSpeedForGrav)
            {
                grav = Vector3.Slerp(grav, Vector3.zero, _rb.velocity.magnitude / minSpeedForGrav);
            }

            else
            {
                grav = Vector3.zero;
            }

            Debug.Log(grav.magnitude);
            TargetVelocity += grav;
        }

        Vector3 velocity = _rb.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        Vector3 velocityChange = TargetVelocity - localVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -accelationLimit, accelationLimit);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -accelationLimit, accelationLimit);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -accelationLimit, accelationLimit);

        _rb.AddRelativeForce(velocityChange, ForceMode.VelocityChange);
    }

    void RotateShip()
    {
        //Trying to scale the yaw rotation, but it's whacky.
        //Doing it as an input processor instead now.
        //i_RotationDirection.y *= _horizontalRotationSpeedScaling;
        Quaternion inputRotation = Quaternion.Euler(i_RotationDirection * _rotationalSensitivity);
        _desiredRotation = _desiredRotation * inputRotation;
        transform.rotation = Quaternion.SlerpUnclamped(transform.rotation, _desiredRotation, Time.deltaTime * _rotationalSnapStrength);
    }

    void FovMAXXER()
    {
        _camera.m_Lens.FieldOfView = Mathf.SmoothStep(startingFov, startingFov + FOVIncrease, _rb.velocity.magnitude / MaxSpeedForCamera);
    }

    void UpdateGravity()
    {
        _gravity = Mathf.Lerp(HorizontalGravity, VerticalGravity, Mathf.Abs(Vector3.Dot(Vector3.up, transform.forward)));
    }

    #region Input Signals
    public void OnMovementDirection()
    {
        i_MovementMagnitude = _input.actions["Movement Direction"].ReadValue<float>();
    }

    public void OnLookDirection()
    {
        i_RotationDirection = _input.actions["Look Direction"].ReadValue<Vector3>();
    }
    #endregion

    void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
    }
}