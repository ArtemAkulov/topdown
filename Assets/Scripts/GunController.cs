using UnityEngine;
using System.Collections;

public class GunController : MonoBehaviour {

    public Transform weaponHold;
    public Gun[] arsenal;
    Gun equippedGun;

    void Start() {
        
    }

    public void EquipGun(Gun gunToEquip) {
        if (equippedGun != null) {
            Destroy(equippedGun.gameObject);
        }
        equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation) as Gun;
        equippedGun.transform.parent = weaponHold;
    }

    public void EquipGun(int gunIndex) {
        EquipGun(arsenal[gunIndex]);
    }

    public void OnTriggerDepressed() {
        if (equippedGun != null) {
            equippedGun.OnTriggerDepressed();
        }
    }

    public void OnTriggerReleased() {
        if (equippedGun != null) {
            equippedGun.OnTriggerReleased();
        }
    }

    public float GunHeight {
        get {
            return weaponHold.position.y;
        }
    }

    public void Aim(Vector3 aimPoint) {
        if (equippedGun != null) {
            equippedGun.Aim(aimPoint);
        }
    }

    public void Reload() {
        if (equippedGun != null) {
            equippedGun.Reload();
        }
    }
}
