using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GunSystem : MonoBehaviour
{
    // PISTOL
    public Transform myCameraHead;
    private UICanvasController myUICanvas;

    public Animator myAnimator;

    public Transform firePosition;
    public GameObject muzzleFlash, bulletHole, waterLeak, bloodEffect, rocketTrail;

    public GameObject bullet;

    // AUTO FIRE
    public bool canAutoFire;
    private bool shooting, readyToShoot = true;

    public float timeBetweenShots;

    public int bulletsAvailable, totalBullets, magazineSize;

    public float reloadTime;
    public bool reloading;

    // ADS Control
    public Transform aimPosition;
    private float aimSpeed = 2f;
    private Vector3 gunStartPosition;
    public float zoomAmount;

    public int damageAmount;
    public string gunName;

    public bool rocketLauncher;

    string gunAnimationName;

    public int pickupBulletAmount;

    public int pistolShotSFXIndex;
    public int rifleShotSFXIndex;
    public int sniperShotSFXIndex;
    public int rocketLauncherShotSFXIndex;


    // Start is called before the first frame update
    void Start()
    {
        totalBullets -= magazineSize;
        bulletsAvailable = magazineSize;

        gunStartPosition = transform.localPosition;

        myUICanvas = FindObjectOfType<UICanvasController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (PauseMenu.gameIsPaused)
        {
            return;
        }


        Shoot();
        GunManager();
        UpdateAmmoText();
        AnimationManager();
    }

    private void AnimationManager()
    {
        switch(gunName)
        {
            case "Pistol":
                gunAnimationName = "Pistol Reload";
                break;

            case "Rifle":
                gunAnimationName = "Rifle Reload";
                break;

            case "Sniper":
                gunAnimationName = "Sniper Reload";
                break;

            case "Rocket Launcher":
                gunAnimationName = "Rocket Launcher Reload";
                break;

            default:
                break;
        }
    }

    

    private void GunManager()
    {
        if(Input.GetKeyDown(KeyCode.R) && bulletsAvailable < magazineSize && !reloading)
        {
            Reload();
        }

        if(Input.GetMouseButton(1))
        {
            transform.position = Vector3.MoveTowards(transform.position, aimPosition.position, aimSpeed * Time.deltaTime);
        } else
        {
            transform.localPosition = Vector3.MoveTowards(transform.localPosition, gunStartPosition, aimSpeed * Time.deltaTime);
        }

        if(Input.GetMouseButtonDown(1))
        {
            FindObjectOfType<CameraMove>().ZoomIn(zoomAmount);
        }
        if (Input.GetMouseButtonUp(1))
        {
            FindObjectOfType<CameraMove>().ZoomOut();
        }
    }

    private void Shoot()
    {
        // check the fire rate of the weapon:
        if (canAutoFire)
        {
            shooting = Input.GetMouseButton(0);
        }
        // if it is not auto-fire, don't treat as auto-fire:
        else
        {
            shooting = Input.GetMouseButtonDown(0);

        }
        Debug.Log($"Shooting: {shooting}, ReadyToShoot: {readyToShoot}, BulletsAvailable: {bulletsAvailable}, Reloading: {reloading}");
        // if player is actively shooting the weapon:
        if (shooting && readyToShoot && bulletsAvailable > 0 && !reloading)
        {
            readyToShoot = false;
            

            // Apply recoil here
            GetComponent<WeaponRecoil>().ApplyRecoil();

            RaycastHit hit;

            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 1000f))
            {
                if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    firePosition.LookAt(hit.point);

                    if (!rocketLauncher)
                    {
                        if (hit.collider.CompareTag("Shootable"))
                        {
                            GameObject hole = Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                            AdjustDecalTransform(hole, hit);
                        }
                        if (hit.collider.CompareTag("WaterLeaker"))
                        {
                            GameObject leak = Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                            AdjustDecalTransform(leak, hit);
                        }
                    }
                }

                if (hit.collider.CompareTag("Enemy") && !rocketLauncher)
                {
                    hit.collider.GetComponent<EnemyHealthSystem>().TakeDamage(damageAmount);
                    GameObject blood = Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    AdjustDecalTransform(blood, hit);
                }
            }
            else
            {
                firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
            }

            bulletsAvailable--;

            if (!rocketLauncher)
            {
                Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
                Instantiate(bullet, firePosition.position, firePosition.rotation, firePosition);
                PlayGunshotSound();
                StartCoroutine(ResetShot());
            }
            else
            {
                Instantiate(bullet, firePosition.position, firePosition.rotation);
                Instantiate(rocketTrail, firePosition.position, firePosition.rotation);
                PlayGunshotSound();
                StartCoroutine(ResetShot());
            }
            

            // StartCoroutine(ResetShot());
            


        }
        else
        {
            // Optionally, add a log here to know when the condition is not met
            Debug.Log("Condition for shooting not met");
        }

    }

    private void AdjustDecalTransform(GameObject decal, RaycastHit hit)
    {
        // Move the decal slightly off the surface to prevent z-fighting
        decal.transform.position += decal.transform.forward * 0.01f;

        // If the hit object should parent the decal (e.g., it's an enemy), parent the decal to it
        if (hit.collider.CompareTag("Enemy"))
        {
            decal.transform.SetParent(hit.collider.transform);
        }
        else
        {
            // Otherwise, just position the decal at the hit point with the correct orientation
            decal.transform.position = hit.point;
            decal.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
    }


    private void PlayGunshotSound()
    {
        switch (gunName)
        {
            case "Pistol":
                AudioManager.instance.PlayerSFX(8);
                break;
            case "Rifle":
                AudioManager.instance.PlayerSFX(9);
                break;
            case "Sniper":
                AudioManager.instance.PlayerSFX(10);
                break;
            case "Rocket Launcher":
                AudioManager.instance.PlayerSFX(11);
                break;
            default:
                // Optionally, log an error or warning if the gun type is unrecognized
                
                break;
        }
    }


    public void AddAmmo()
    {
        totalBullets += pickupBulletAmount;
        AudioManager.instance.PlayerSFX(0);
    }

    private void Reload()
    {
        if (reloading)
        {
            Debug.Log("Already reloading.");
            return;
        }

        Debug.Log("Reload started.");
        reloading = true;

        myAnimator.SetTrigger(gunAnimationName);

        AudioManager.instance.PlayerSFX(7);

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
        int bulletsToAdd = magazineSize - bulletsAvailable;

        if (totalBullets > bulletsToAdd)
        {
            totalBullets -= bulletsToAdd;
            bulletsAvailable = magazineSize;
        }
        else
        {
            bulletsAvailable += totalBullets;
            totalBullets = 0;
        }
        reloading = false;
    }

    IEnumerator ResetShot()
    {
        yield return new WaitForSeconds(timeBetweenShots);

        readyToShoot = true;

    }

    private void UpdateAmmoText()
    {
        myUICanvas.ammoText.SetText(bulletsAvailable + "/ " + magazineSize);
        myUICanvas.totalAmmoText.SetText("Rounds: " + totalBullets.ToString());
    }

    public void CancelReload()
    {
        if (reloading)
        {
            reloading = false;
            // Stop the reload animation if there is one
            myAnimator.ResetTrigger(gunAnimationName);
            // Optionally, stop reload sound effect or play a sound effect for reload interruption
            // AudioManager.instance.PlayerSFX(reloadCancelSFXIndex); // if you have a sound effect for cancelling reload
            Debug.Log("Reload cancelled.");
        }
    }

    public void CancelShotReset()
    {
        // Set readyToShoot to true when weapon is switched
        readyToShoot = true;
        StopCoroutine(ResetShot()); // Stop the ResetShot coroutine if it's running
        Debug.Log("Shot reset cancelled.");
    }

}
