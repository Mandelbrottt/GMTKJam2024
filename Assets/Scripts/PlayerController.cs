using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField] float ThrottleDelta = 0.05f;
    [SerializeField] float ThrottleBoostScalar = 2.5f;
    [SerializeField] float BoostTime = 3.0f;
    [SerializeField] float BoostCooldown = 10.0f;

    [SerializeField] float Health = 100;
    [SerializeField] float collisionCooldown = 1;
    [SerializeField] public UnityEvent PlayerDeath;

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
    float _throttle;

    bool _boosted;
    float _boostTimer = 100;
    public Vector3 _externalForce;
    float _collisionCooldown;
    int _playerNum;
    GameMode _gameMode;

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

        _throttle = 0.7f;

        Debug.Log("Joined");
        _gameMode = FindAnyObjectByType<GameMode>();
        _playerNum = _gameMode.Join(this);
    }

    void Update()
    {
        //RotateShip();
        UpdateGravity();
        FovMAXXER();
        UpdateThrottle();

        float OldBoostTime = _boostTimer;
        _boostTimer += Time.deltaTime;
        if (OldBoostTime <= 0 && _boostTimer > 0) //The Boost Timer just crossed zero
        {
            _boosted = false;
        }

        _collisionCooldown -= Time.deltaTime;

        Debug.Log("Speed: " + _rb.velocity.magnitude);
    }

    void FixedUpdate()
    {
        Movement();
        RotateShip();
    }

    void Movement()
    {
        Vector3 TargetVelocity = new Vector3(0, 0, _throttle * lateralThrust); //This velocity is in local space

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

            //Debug.Log(grav.magnitude);
            TargetVelocity += grav;
        }

        Vector3 velocity = _rb.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);

        Vector3 velocityChange = TargetVelocity - localVelocity;
        velocityChange.x = Mathf.Clamp(velocityChange.x, -accelationLimit, accelationLimit);
        velocityChange.y = Mathf.Clamp(velocityChange.y, -accelationLimit, accelationLimit);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -accelationLimit, accelationLimit);

        velocityChange += transform.InverseTransformVector(_externalForce);

        _rb.AddRelativeForce(velocityChange, ForceMode.VelocityChange);

        _externalForce = Vector3.zero;
    }

    void RotateShip()
    {
        //Trying to scale the yaw rotation, but it's whacky.
        //Doing it as an input processor instead now.
        //i_RotationDirection.y *= _horizontalRotationSpeedScaling;

        //Scale rot sens with velocity.

        float SpeedScalar = minSpeedForGrav / (_rb.velocity.magnitude + 0.01f);

        Quaternion inputRotation = Quaternion.Euler(i_RotationDirection * _rotationalSensitivity * SpeedScalar);
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

    void UpdateThrottle()
    {
        _throttle += ThrottleDelta * i_MovementMagnitude * Time.deltaTime;
        _throttle = Mathf.Clamp(_throttle, 0.0f, 1.0f);

        if (_boosted)
        {
            _throttle *= ThrottleBoostScalar;
        }
    }

    public void OnBoost()
    {
        if(_boostTimer > BoostCooldown)
        {
            _boosted = true;
            _boostTimer = 0 - BoostTime; //Account for the time of the boost
        }
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

    public void ExternalForceAdd(Vector3 force)
    {
        _externalForce = force;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(_collisionCooldown > 0)
        {
            return;
        }
        float force = collision.impulse.magnitude;
        if(force < 10f)
        {
            Damage(2f);
        } else if (force < 30f)
        {
            Damage(5f);
        } else
        {
            Damage(10f);
        }
        _collisionCooldown = collisionCooldown;
    }

    public void Damage(float damage)
    {
        Health -= damage;
        Debug.Log(Health);

        if (Health < 0)
        {
            PlayerDeath.Invoke();
            GetComponentsInChildren<MeshRenderer>().ToList().ForEach(renderer => renderer.enabled = false);
        }
    }

    public float GetHealth()
    {
        return Health;
    }

    public void SetPlayerNumber(int number)
    {
        _playerNum = number;
    }

    void Reset()
    {
        _rb = GetComponent<Rigidbody>();
        _input = GetComponent<PlayerInput>();
    }
}