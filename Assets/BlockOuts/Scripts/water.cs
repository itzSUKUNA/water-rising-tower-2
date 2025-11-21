using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Collider))]
public class RisingWater_Tick_Kill_Debug : MonoBehaviour
{
    [Header("Tick Movement")]
    public float tickInterval = 0.08f;
    public Vector3 tickMovement = new Vector3(0f, 0.03f, 0f);

    [Header("Player & UI")]
    public string playerTag = "Player";
    public GameObject gameOverCanvasObj; // assign in inspector OR will try to find GameOverCanvas
    private GameObject player1;

    [Header("VFX/SFX (optional)")]
    public GameObject splashPrefab;
    public AudioClip splashSfx;
    [Range(0f,1f)] public float splashVolume = 0.8f;

    [Header("Behavior")]
    public bool stopTimeOnDeath = true;
    public bool destroyPlayer = true;
    public float destroyDelayRealtime = 0.12f;

    // internals
    float timer = 0f;
    bool triggered = false;
    AudioSource audioSource;

    void Start(){
        player1 = GameObject.FindGameObjectWithTag("Player");
       // EnableCommonMovementComponents(player1);
    }
    void Awake()
    {
        // ensure collider is trigger
        var col = GetComponent<Collider>();
        if (col != null) col.isTrigger = true;

        // prepare audio if any
        if (splashSfx != null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.clip = splashSfx;
            audioSource.volume = splashVolume;
        }

        // try to auto-find canvas if not assigned
        if (gameOverCanvasObj == null)
        {
            var g = GameObject.Find("GameOverCanvas");
            if (g != null) gameOverCanvasObj = g;
            else
            {
                var c = FindObjectOfType<Canvas>();
                if (c != null) gameOverCanvasObj = c.gameObject;
            }
        }

        if (gameOverCanvasObj != null)
            gameOverCanvasObj.SetActive(false);
    }

    void Update()
    {
        if (triggered) return;
        timer += Time.deltaTime;
        if (timer >= tickInterval)
        {
            timer = 0f;
            transform.position += tickMovement;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        Debug.Log($"[RisingWater] OnTriggerEnter with '{other.gameObject.name}' (tag='{other.tag}')");

        if (!other.CompareTag(playerTag))
        {
            Debug.Log("[RisingWater] collided object is NOT player; ignoring.");
            return;
        }

        // Sanity checks: ensure physics setup is correct
        var playerRb = other.attachedRigidbody;
        if (playerRb == null)
            Debug.LogWarning("[RisingWater] Player has no Rigidbody! OnTrigger may still work if Water had a Rigidbody. Recommended: add non-kinematic Rigidbody to Player.");

        // All good: proceed
        triggered = true;
        HandlePlayerHit(other.gameObject);
    }

    void HandlePlayerHit(GameObject player)
    {
        Debug.Log("[RisingWater] Player hit detected. Disabling inputs and showing UI.");

        // disable common movement components (safer than disabling all MonoBehaviours)
        DisableCommonMovementComponents(player);

        // spawn VFX
        if (splashPrefab != null)
            Instantiate(splashPrefab, player.transform.position, Quaternion.identity);

        // play sfx
        if (audioSource != null && splashSfx != null)
            audioSource.PlayOneShot(splashSfx, splashVolume);

        // show canvas
        if (gameOverCanvasObj != null)
        {
            gameOverCanvasObj.SetActive(true);
            Debug.Log("[RisingWater] Activated Canvas: " + gameOverCanvasObj.name);
        }
        else Debug.LogWarning("[RisingWater] No Canvas assigned or found.");

        // stop time
        if (stopTimeOnDeath) Time.timeScale = 0f;

        // destroy player after short realtime delay so any realtime effects can run
        if (destroyPlayer)
            StartCoroutine(DestroyPlayerRealtime(player, destroyDelayRealtime));
    }

    IEnumerator DestroyPlayerRealtime(GameObject player, float delay)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        if (player != null)
        {
            Debug.Log("[RisingWater] Destroying player GameObject: " + player.name);
            Destroy(player);
        }
    }

    public void DisableCommonMovementComponents(GameObject player)
    {
        // Try to disable specific movement scripts by common names - add your script class names here if needed
        string[] knownMovementScriptNames = { "PlayerMovement_WASD", "ClickToMove_NavMesh3D", "ClickToMove3D_NavMesh", "ClickToMoveComponent", "PlayerMovement_NoRotation", "PlayerMovement_WASD" };

        var monoBehaviours = player.GetComponents<MonoBehaviour>();
        foreach (var mb in monoBehaviours)
        {
            if (mb == null) continue;
            // disable known movement scripts
            foreach (var name in knownMovementScriptNames)
            {
                if (mb.GetType().Name == name)
                {
                    mb.enabled = false;
                    Debug.Log($"[RisingWater] Disabled movement script: {mb.GetType().Name}");
                }
            }
        }

        // disable child movement scripts too
        var childMBs = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (var mb in childMBs)
        {
            if (mb == null) continue;
            foreach (var name in knownMovementScriptNames)
            {
                if (mb.GetType().Name == name)
                {
                    mb.enabled = false;
                    Debug.Log($"[RisingWater] Disabled child movement script: {mb.GetType().Name}");
                }
            }
        }

        // CharacterController
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) { cc.enabled = false; Debug.Log("[RisingWater] Disabled CharacterController."); }

        // NavMeshAgent
        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null) { agent.isStopped = true; agent.updatePosition = false; agent.updateRotation = false; Debug.Log("[RisingWater] Stopped NavMeshAgent."); }

        // Rigidbody: make kinematic to halt physics
        var rb = player.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; Debug.Log("[RisingWater] Set Rigidbody to kinematic."); }
    }

    // Reverse the effects applied by DisableCommonMovementComponents
    public void EnableCommonMovementComponents(GameObject player)
    {
        // Match the same list of common movement script names
        string[] knownMovementScriptNames = { "PlayerMovement_WASD", "ClickToMove_NavMesh3D", "ClickToMove3D_NavMesh", "ClickToMoveComponent", "PlayerMovement_NoRotation", "PlayerMovement_WASD" };

        var monoBehaviours = player.GetComponents<MonoBehaviour>();
        foreach (var mb in monoBehaviours)
        {
            if (mb == null) continue;
            foreach (var name in knownMovementScriptNames)
            {
                if (mb.GetType().Name == name)
                {
                    mb.enabled = true;
                    Debug.Log($"[RisingWater] Re-enabled movement script: {mb.GetType().Name}");
                }
            }
        }

        // re-enable child movement scripts too
        var childMBs = player.GetComponentsInChildren<MonoBehaviour>();
        foreach (var mb in childMBs)
        {
            if (mb == null) continue;
            foreach (var name in knownMovementScriptNames)
            {
                if (mb.GetType().Name == name)
                {
                    mb.enabled = true;
                    Debug.Log($"[RisingWater] Re-enabled child movement script: {mb.GetType().Name}");
                }
            }
        }

        // CharacterController
        var cc = player.GetComponent<CharacterController>();
        if (cc != null) { cc.enabled = true; Debug.Log("[RisingWater] Re-enabled CharacterController."); }

        // NavMeshAgent: resume normal behaviour
        var agent = player.GetComponent<NavMeshAgent>();
        if (agent != null) { agent.isStopped = false; agent.updatePosition = true; agent.updateRotation = true; Debug.Log("[RisingWater] Restarted NavMeshAgent."); }

        // Rigidbody: clear kinematic so physics resumes
        var rb2 = player.GetComponent<Rigidbody>();
        if (rb2 != null) { rb2.isKinematic = false; Debug.Log("[RisingWater] Set Rigidbody to non-kinematic."); }
    }
}
