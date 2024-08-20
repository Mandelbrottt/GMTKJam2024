using System.Collections;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour {
    [Header("Projectile Properties")]
    [SerializeField] int damage             = 1;
    [SerializeField] float lifespan         = 2.5f;
    [SerializeField] float velocity         = 100.0f;
    [SerializeField] float accuracy         = 2.5f;
    [SerializeField] float ExplosiveForce   = 30f;
    [SerializeField] float ExplosionRadius  = 5.0f;
    [SerializeField] Color color            = Color.white;

    [Header("Auto-Aim Properties")]
    [SerializeField] float autoAimRange     = 10.0f;
    [SerializeField] float autoAimRadius    = 1.0f;

    [Header("Serialized References")]
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] Rigidbody rigidBody;
    [SerializeField] LayerMask lockOnLayer;

    private Vector3 shipVelocity;

    private void Awake() {
        meshRenderer = GetComponent<MeshRenderer>();
        rigidBody = GetComponent<Rigidbody>();
    }

	private void FixedUpdate() {
        lifespan -= Time.deltaTime;
        if (lifespan <= 0.0f) {
            Destroy(gameObject);
            return;
        }

		rigidBody.MovePosition(transform.position + Time.deltaTime * shipVelocity);
	}

	public void OnFire(Vector3 _position, Vector3 _velocity, Quaternion _rotation) {
        meshRenderer.material.color = color;
        transform.position = _position;
        transform.rotation = _rotation * Quaternion.Euler(0.0f, 90.0f, 90.0f);

        if (Physics.SphereCast(transform.position, autoAimRadius, transform.up, out RaycastHit _hit, autoAimRange, lockOnLayer)) {
            GameObject _target = _hit.collider.gameObject;
            Vector3 _direction = Quaternion.LookRotation(_target.transform.position - transform.position).eulerAngles;
            transform.rotation = Quaternion.Euler(_direction) * Quaternion.Euler(0.0f, 90.0f, 90.0f);
        }

        transform.rotation *= Quaternion.Euler(0.0f, 0.0f, Random.Range(-accuracy, accuracy));

        shipVelocity = _velocity + transform.up * velocity;
    }

	private void OnCollisionEnter(Collision _collision) {
        Rigidbody otherRB = _collision.gameObject.GetComponent<Rigidbody>();
        if (otherRB != null)
        {
            otherRB.AddExplosionForce(ExplosiveForce, _collision.GetContact(0).point, ExplosionRadius);
        }
        PlayerController player = _collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            player.Damage(damage);
        }
		Destroy(gameObject);
    }

    private void OnDrawGizmos() {
		Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position + transform.up * autoAimRange, autoAimRadius);
	}
}
