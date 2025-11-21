using UnityEngine;

public class WaterTriggerTest : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER HIT: " + other.name);
    }
}
