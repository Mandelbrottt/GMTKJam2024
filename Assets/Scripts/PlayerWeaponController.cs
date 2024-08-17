using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour {
	[Header("Weapon Properties")]
	[SerializeField] float pelletFireRate = 0.1f;
	[SerializeField] float missileFireRate = 1.5f;
	[SerializeField] int missileAmmoCapacity = 10;

	[Header("Serialized References")]
	[SerializeField] PlayerInput playerInput;
	[SerializeField] Rigidbody rigidBody;
	[SerializeField] Transform leftPelletBarrel;
	[SerializeField] Transform rightPelletBarrel;
	[SerializeField] GameObject pellet;

	private float pelletInputMag = 0.0f;
	private float missileInputMag = 0.0f;

	private float pelletCooldown = 0.0f;
	private float missileCooldown = 0.0f;

	private bool useRightPelletBarrel = false;

	private void Awake() {
		playerInput = GetComponent<PlayerInput>();
		rigidBody = GetComponent<Rigidbody>();
	}

	private void Update() {
		if (pelletInputMag >= 0.5f && pelletCooldown <= 0.0f) {
			pelletCooldown = pelletFireRate;

			Vector3 _position = leftPelletBarrel.position;
			if (useRightPelletBarrel)
				_position = rightPelletBarrel.position;
			useRightPelletBarrel = !useRightPelletBarrel;

			PlayerProjectile _pellet = Instantiate(pellet).GetComponent<PlayerProjectile>();
			_pellet.OnFire(_position, rigidBody.velocity, transform.rotation);
		}
	}

	private void FixedUpdate() {
		if (pelletCooldown > 0.0f)
			pelletCooldown -= Time.deltaTime;
	}

	public void OnFirePellet() {
		pelletInputMag = playerInput.actions["Fire Pellet"].ReadValue<float>();
	}
}
