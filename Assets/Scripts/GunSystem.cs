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

    public Transform firePosition;
    public GameObject muzzleFlash, bulletHole, waterLeak;

    public GameObject bullet;

    // AUTO FIRE
    public bool canAutoFire;
    private bool shooting, readyToShoot = true;

    public float timeBetweenShots;

    public int bulletsAvailable, totalBullets, magazineSize;

    public float reloadTime;
    public bool reloading;


    // Start is called before the first frame update
    void Start()
    {
        totalBullets -= magazineSize;
        bulletsAvailable = magazineSize;

        myUICanvas = FindObjectOfType<UICanvasController>();
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
        GunManager();
        UpdateAmmoText();
    }

    

    private void GunManager()
    {
        if(Input.GetKeyDown(KeyCode.R) && bulletsAvailable < magazineSize && !reloading)
        {
            Reload();
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

            if (Physics.Raycast(myCameraHead.position, myCameraHead.forward, out hit, 100f))
            {
                if (Vector3.Distance(myCameraHead.position, hit.point) > 2f)
                {
                    firePosition.LookAt(hit.point);
                    if (hit.collider.CompareTag("Shootable"))
                    {
                        Instantiate(bulletHole, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    if (hit.collider.CompareTag("WaterLeaker"))
                    {
                        Instantiate(waterLeak, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }
                if (hit.collider.CompareTag("Enemy"))
                {
                    Destroy(hit.collider.gameObject);
                }
            }
            else
            {
                firePosition.LookAt(myCameraHead.position + (myCameraHead.forward * 50f));
            }

            bulletsAvailable--;


            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
            Instantiate(bullet, firePosition.position, firePosition.rotation, firePosition);

            StartCoroutine(ResetShot());


        }
    }

    private void Reload()
    {
        int bulletsToAdd = magazineSize - bulletsAvailable;

        if(totalBullets > bulletsToAdd)
        {
            totalBullets -= bulletsToAdd;
            bulletsAvailable = magazineSize;
        }
        else
        {
            bulletsAvailable += totalBullets;
            totalBullets = 0;
        }
        reloading = true;

        StartCoroutine(ReloadCoroutine());
    }

    IEnumerator ReloadCoroutine()
    {
        yield return new WaitForSeconds(reloadTime);
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
        myUICanvas.totalAmmoText.SetText(totalBullets.ToString());
    }



}
