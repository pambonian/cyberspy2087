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
        if(canAutoFire)
        {
            shooting = Input.GetMouseButton(0);
        }
        // if it is not auto-fire, don't treat as auto-fire:
        else
        {
            shooting = Input.GetMouseButtonDown(0);
        }
        // if player is actively shooting the weapon:
        if (shooting && readyToShoot && bulletsAvailable > 0 && !reloading)
        {
            readyToShoot = false;

            RaycastHit hit;

            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 200f))
            {
                if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    firePosition.LookAt(hit.point);

                    if (!rocketLauncher)
                    {
                        if (hit.collider.CompareTag("Shootable"))
                        {
                            Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                        }
                        if (hit.collider.CompareTag("WaterLeaker"))
                        {
                            Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                        }
                    }
                }

                if (hit.collider.CompareTag("Enemy") && !rocketLauncher)
                {
                    hit.collider.GetComponent<EnemyHealthSystem>().TakeDamage(damageAmount);
                    Instantiate(bloodEffect, hit.point, Quaternion.LookRotation(hit.normal));
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
            }
            else
            {
                Instantiate(bullet, firePosition.position, firePosition.rotation);
                Instantiate(rocketTrail, firePosition.position, firePosition.rotation);
            }
            

            StartCoroutine(ResetShot());


        }
    }

    public void AddAmmo()
    {
        totalBullets += pickupBulletAmount;
        AudioManager.instance.PlayerSFX(0);
    }

    private void Reload()
    {
        myAnimator.SetTrigger(gunAnimationName);

        AudioManager.instance.PlayerSFX(7);
        
        reloading = true;

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

}
