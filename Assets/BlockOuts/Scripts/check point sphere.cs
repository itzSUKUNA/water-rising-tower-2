using UnityEngine;

public class FloatAndSpin : MonoBehaviour
{
    [Header("Floating Settings")]
    public float floatAmplitude = 0.25f;   // How high it moves up/down
    public float floatSpeed = 2f;          // How fast it floats

    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0, 30f, 0);  // Slow spin

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // FLOATING EFFECT
        float newY = Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
        transform.position = new Vector3(startPos.x, startPos.y + newY, startPos.z);

        // SPINNING EFFECT
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
