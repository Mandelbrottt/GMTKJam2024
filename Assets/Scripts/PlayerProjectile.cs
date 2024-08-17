using Unity.VisualScripting;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {
    [Header("Projectile Properties")]
    [SerializeField] int damage             = 1;
    [SerializeField] float lifespan         = 2.5f;
    [SerializeField] float velocity         = 100.0f;
    [SerializeField] float accuracy         = 2.5f;
    [SerializeField] Color color            = Color.white;

    [Header("Auto-Aim Properties")]
    [SerializeField] float autoAimRange     = 10.0f;
    [SerializeField] float autoAimRadius    = 1.0f;

    [Header("Serialized References")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] LayerMask lockOnLayer;

    private GameObject targetObject;
    private Vector3 shipVelocity;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        rigidBody = GetComponent<Rigidbody>();
    }

	private void Update() {
		lifespan -= Time.deltaTime;
        if (lifespan <= 0.0f) {
            Destroy(gameObject);
        }
    }

	private void FixedUpdate() {
		rigidBody.MovePosition(transform.position + Time.deltaTime * shipVelocity);
	}

	public void OnFire(Vector3 _position, Vector3 _velocity, Quaternion _rotation) {
        meshRenderer.material.color = color;
        transform.position = _position;
        transform.rotation = _rotation * Quaternion.Euler(0.0f, 90.0f, 90.0f);
        shipVelocity = _velocity + transform.up * velocity;

        if (Physics.SphereCast(transform.position, autoAimRadius, transform.up, out RaycastHit hit, autoAimRange, lockOnLayer)) {
            targetObject = hit.collider.gameObject;
            Debug.Log("TEST");
        }
    }

	private void OnDrawGizmos() {
		Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up * autoAimRange, autoAimRadius);
	}
}
