using UnityEngine;

public class SkyboxRotator : MonoBehaviour
{
    public float rotationSpeed = 1.0f;

    void Update()
    {
        // Rotates the skybox around the y-axis
        RenderSettings.skybox.SetFloat("_Rotation", Time.time * rotationSpeed);
    }
}
