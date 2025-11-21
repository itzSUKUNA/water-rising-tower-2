using UnityEngine;

public class ScalePulse : MonoBehaviour
{
    public float speed = 2f;        // how fast it scales
    public float scaleAmount = 0.3f; // how much it scales

    private Vector3 baseScale;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        float s = 1 + Mathf.Sin(Time.time * speed) * scaleAmount;
        transform.localScale = baseScale * s;
    }
}
