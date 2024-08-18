using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWeaponController : MonoBehaviour {
	[Header("Weapon Properties")]
	[SerializeField] float bulletFireRate = 0.1f;
	[SerializeField] float missileFireRate = 2.5f;
	[SerializeField] int missileAmmoCapacity = 10;

	[Header("Serialized References")]
	[SerializeField] PlayerInput playerInput;
	[SerializeField] Rigidbody rigidBody;
	[SerializeField] Transform leftButlletBarrel;
	[SerializeField] Transform rightBulletBarrel;
	[SerializeField] Transform missileBarrel;
	[SerializeField] GameObject bullet;
	[SerializeField] GameObject missile;

	private float bulletInputMag = 0.0f;
	private float missileInputMag = 0.0f;

	private float bulletCooldown = 0.0f;
	private float missileCooldown = 0.0f;

	private int curMissileAmmo = 0;
	private bool useRightBarrel = false;

	private void Awake() {
		playerInput = GetComponent<PlayerInput>();
		rigidBody = GetComponent<Rigidbody>();
		curMissileAmmo = missileAmmoCapacity;
	}

	private void Update() {
		if (bulletInputMag >= 0.2f && bulletCooldown <= 0.0f) {
			bulletCooldown = bulletFireRate;

			Vector3 _position = leftButlletBarrel.position;
			if (useRightBarrel)
				_position = rightBulletBarrel.position;
			useRightBarrel = !useRightBarrel;

			PlayerProjectile _bullet = Instantiate(bullet).GetComponent<PlayerProjectile>();
			_bullet.OnFire(_position, rigidBody.velocity, transform.rotation);
		}

		if (curMissileAmmo > 0 && missileInputMag >= 0.2f && missileCooldown <= 0.0f) {
			missileCooldown = missileFireRate;

			PlayerProjectile _missile = Instantiate(missile).GetComponent<PlayerProjectile>();
			_missile.OnFire(missileBarrel.position, rigidBody.velocity, transform.rotation);

			curMissileAmmo--;
		}
	}

	private void FixedUpdate() {
		if (bulletCooldown > 0.0f)
			bulletCooldown -= Time.deltaTime;

		if (missileCooldown > 0.0f)
			missileCooldown -= Time.deltaTime;
	}

	public void OnFireBullet() {
		bulletInputMag = playerInput.actions["Fire Bullet"].ReadValue<float>();
	}

	public void OnFireMissile() {
		missileInputMag = playerInput.actions["Fire Missile"].ReadValue<float>();
	}
}
