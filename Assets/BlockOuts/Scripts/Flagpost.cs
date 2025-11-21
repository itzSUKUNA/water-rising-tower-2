using UnityEngine;

public class Flagpost : MonoBehaviour
{
    public RisingWater_Tick_Kill_Debug wa;
    public GameObject lvClr;
    private GameObject player;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void OnTriggerEnter(Collider other)
    {
        wa.DisableCommonMovementComponents(player);
        lvClr.SetActive(true);
    }
}
