using UnityEngine;

/// <summary>
/// Attach to a stair (or any GameObject with a Collider).
/// Click the object in-game to rotate it through the angles in `angles`.
/// Can use absolute angles relative to the initial rotation or relative to the current rotation.
/// </summary>
[RequireComponent(typeof(Collider))]
public class ClickRotateCycle : MonoBehaviour
{
    [Header("Rotation settings")]
    public Vector3 rotationAxis = Vector3.up;       // axis to rotate around (local space)
    public float rotateSpeed = 6f;                 // higher = faster
    public bool useRelative = false;               // false = angles are relative to initial rotation; true = rotate relative to current rotation

    [Header("Angles to cycle through (degrees)")]
    public float[] angles = new float[] { 90f, 180f, 270f, 360f };

    // internal
    Quaternion initialRotation;
    Quaternion targetRotation;
    int currentIndex = 0;
    bool rotating = false;

    void Start()
    {
        initialRotation = transform.rotation;
        targetRotation = transform.rotation;
        if (rotationAxis == Vector3.zero) rotationAxis = Vector3.up;
    }

    // Using OnMouseDown so clicks work without extra raycast code (object must have Collider)
    void OnMouseDown()
    {
        if (rotating) return; // ignore clicks while rotating (optional)
        if (angles == null || angles.Length == 0) return;

        float angle = angles[currentIndex % angles.Length];

        if (useRelative)
        {
            // rotate relative to current rotation
            targetRotation = transform.rotation * Quaternion.Euler(rotationAxis.normalized * angle);
        }
        else
        {
            // absolute relative to initial rotation (so 90 -> 180 -> 270 -> 360 -> 90 ...)
            targetRotation = initialRotation * Quaternion.Euler(rotationAxis.normalized * angle);
        }

        currentIndex = (currentIndex + 1) % angles.Length;
        rotating = true;
    }

    void Update()
    {
        if (!rotating) return;

        // Smoothly rotate towards target
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);

        // If close enough, snap and stop
        if (Quaternion.Angle(transform.rotation, targetRotation) < 0.3f)
        {
            transform.rotation = targetRotation;
            rotating = false;

            // Optional: if the target angle was 360 and you prefer to normalize back to initial rotation:
            // if (!useRelative && Mathf.Approximately(Mathf.Abs(angles[(currentIndex + angles.Length - 1) % angles.Length]) % 360f, 0f))
            //     transform.rotation = initialRotation;
        }
    }

    // Editor/Scene visualization
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.12f);
        Gizmos.color = Color.cyan;
        Vector3 axisWorld = transform.TransformDirection(rotationAxis.normalized);
        Gizmos.DrawLine(transform.position, transform.position + axisWorld);
    }
}
