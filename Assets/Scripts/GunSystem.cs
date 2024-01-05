using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSystem : MonoBehaviour
{

    public Transform myCameraHead;
    public Transform firePosition;
    public GameObject muzzleFlash, bulletHole, waterLeak;
    public GameObject bullet;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Shoot();
    }

    private void Shoot()
    {
        if (Input.GetMouseButtonDown(0))
        {
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

            Instantiate(muzzleFlash, firePosition.position, firePosition.rotation, firePosition);
            Instantiate(bullet, firePosition.position, firePosition.rotation, firePosition);
        }
    }


}
