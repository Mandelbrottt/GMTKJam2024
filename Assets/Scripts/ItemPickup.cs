using UnityEngine;

enum ItemIndex {
	None,
	Health,
	Ammo
}

public class ItemPickup : MonoBehaviour {
	[SerializeField] ItemIndex index = ItemIndex.None;
	[SerializeField] int amount = 10;

	private readonly float collectDistance = 1.5f;
	private readonly float velocity = 0.1f;

	private GameObject target;
	private bool isCollected = false;

	private void FixedUpdate() {
		if (!isCollected)
			return;

		// Get the vector for the pickup's "forward" vector and for determining the current distance between the item pickup and player
		// based on the magnitude of the resulting vector.
		Vector3 _distance = target.transform.position - transform.position;
		transform.LookAt(Quaternion.LookRotation(_distance).eulerAngles);

		// If the distance meets the required amount for collection; apply its effect onto the player. Otherwise, update the pickup's
		// position based on the distance vector and current velocity.
		if (_distance.magnitude < collectDistance) {
			OnCollect();
			return;
		}
		transform.position += _distance * velocity;
		transform.LookAt(new Vector3());
	}

	// Tells the pickup to begin moving towards the player for collection.
	public void SetToCollect(GameObject _player) {
		target = _player;
		isCollected = true;
	}

	// Applies the pickup's effect onto the player before the pickup destroys itself.
	public void OnCollect() {
		switch (index) {
			case ItemIndex.Health:
				PlayerController _pController = target.GetComponent<PlayerController>();
				if (_pController != null) {
					_pController.UpdateHealth(amount);
					Debug.Log("Health collected!");
				}
				break;
			case ItemIndex.Ammo:
				PlayerWeaponController _pWeaponController = target.GetComponent<PlayerWeaponController>();
				if (_pWeaponController != null) {
					_pWeaponController.UpdateAmmoCount(amount);
					Debug.Log("Ammo collected!");
				}
				break;
		}

		Destroy(gameObject);
	}
}
