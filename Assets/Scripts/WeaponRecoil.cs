using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    // Tunable parameters for recoil
    public Vector3 recoilKickback = new Vector3(-0.1f, 0f, 0f); // How much the weapon kicks back
    public Vector3 recoilRotation = new Vector3(10f, 5f, 7f); // How much the weapon rotates
    public float returnSpeed = 5f; // How quickly the weapon returns to original position

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    // Use this for initialization
    void Start()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        // Return the weapon to its original position smoothly over time
        transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition, returnSpeed * Time.deltaTime);
        transform.localRotation = Quaternion.Lerp(transform.localRotation, originalRotation, returnSpeed * Time.deltaTime);
    }

    // Call this method to apply recoil
    public void ApplyRecoil()
    {
        // Add recoil kickback and rotation
        transform.localPosition -= recoilKickback;
        transform.localRotation *= Quaternion.Euler(recoilRotation);

        // Optionally, you can add some randomness to the recoil here
    }
}
