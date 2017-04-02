using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    public enum FireMode {Auto, Burst, SingleShot}
    public FireMode fireMode;

    public Transform[] muzzle;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public float reloadTime = .3f;
    
    public int shotsInBurst;
    int shotsRemainingInBurst;

    public int clipCapacity = 10;
    int bulletsRemainingInClip;
    bool reloading;

    float nextShotTime;

    bool triggerReleasedSinceLastShot = true;

    [Header("Effects")]
    public Transform shell;
    public Transform shellEjector;
    public AudioClip fireAudio;
    public AudioClip reloadAudio;
    MuzzleFlash muzzleFlash;


    [Header ("Recoil")]
    Vector3 recoilVelocity;
    float recoilAngle;
    float recoilRotationVelocity;

    public Vector2 kickBack = new Vector2(.03f, .15f);
    public float kickBackNormalize = .1f;
    public Vector2 kickUp = new Vector2(2, 5);
    public float kickUpNormalize = .1f;

    void Start() {
        reloading = false;
        muzzleFlash = GetComponent<MuzzleFlash>();
        shotsRemainingInBurst = shotsInBurst;
        bulletsRemainingInClip = clipCapacity;
    }

    void LateUpdate() {
        // Recoil animation
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilVelocity, kickBackNormalize);
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotationVelocity, kickUpNormalize);
        transform.localEulerAngles += Vector3.left * recoilAngle;

        if (!reloading && bulletsRemainingInClip < 1) {
            Reload();
        }
    }

    void Shoot() {
        if (!reloading && Time.time > nextShotTime && bulletsRemainingInClip > 0) {
            if (fireMode == FireMode.Burst) {
                if (shotsRemainingInBurst == 0) {
                    return;
                }
                shotsRemainingInBurst--;
            }
            else if (fireMode == FireMode.SingleShot) {
                if (!triggerReleasedSinceLastShot) {
                    return;
                }
            }

            for (int i = 0; i < muzzle.Length; i++) {
                Projectile newProjectile = Instantiate(projectile, muzzle[i].position, muzzle[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
                nextShotTime = Time.time + (msBetweenShots / 1000);
                bulletsRemainingInClip -= 1;
                Instantiate(shell, shellEjector.position, shellEjector.rotation);
                muzzleFlash.Activate();
            }
                        
            transform.localPosition -= Vector3.forward * Random.Range(kickBack.x, kickBack.y);
            recoilAngle += Random.Range(kickUp.x, kickUp.y);
            recoilAngle = Mathf.Clamp(recoilAngle, 0, 30);
            AudioManager.audioManager.PlaySound(fireAudio, transform.position);            
        }
    }

    public void Aim(Vector3 aimPoint) {
        if (!reloading) {
            transform.LookAt(aimPoint);
        }
    }

    public void Reload() {
        if (!reloading && bulletsRemainingInClip != clipCapacity) {
            StartCoroutine(ReloadAnimation());
            AudioManager.audioManager.PlaySound(reloadAudio, transform.position);
        }
    }

    public void OnTriggerDepressed() {
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTriggerReleased() {
        triggerReleasedSinceLastShot = true;
        shotsRemainingInBurst = shotsInBurst;
    }

    IEnumerator ReloadAnimation() {
        reloading = true;
        yield return new WaitForSeconds(.2f);

        float reloadSpeed = 1f / reloadTime;
        float fullyReloaded = 0;
        Vector3 initialRotationValue = transform.localEulerAngles;
        float maxReloadAngle = 30;

        while (fullyReloaded < 1) {
            fullyReloaded += Time.deltaTime * reloadSpeed;
            float interpolation = (-Mathf.Pow(fullyReloaded, 2) + fullyReloaded) * 4;
            float reloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialRotationValue + Vector3.left * reloadAngle; 
            yield return null;    
        }

        reloading = false;
        bulletsRemainingInClip = clipCapacity;
    }

}
