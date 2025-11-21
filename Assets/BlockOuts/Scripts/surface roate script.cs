using UnityEngine;

public class AutoRotate : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    public Vector3 rotationSpeed = new Vector3(0, 50f, 0);  
    // default rotating around Y axis

    void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
