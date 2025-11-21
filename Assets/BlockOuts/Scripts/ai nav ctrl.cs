using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove_NavMesh3D : MonoBehaviour
{
    public Camera cam;               // The camera used to raycast (drag your Main Camera)
    public LayerMask groundMask;     // Which layers are valid to click on
    public Animator animator;        // Optional (idle/walk animation)
    public string animBool = "isWalking"; // Animator bool name

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if (cam == null)
            cam = Camera.main; // auto assign main camera
    }

    void Update()
    {
        // Ignore clicks on UI elements
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // Left-click sets a new destination
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            // Raycast to ground
            if (Physics.Raycast(ray, out hit, 200f, groundMask))
            {
                agent.SetDestination(hit.point);
            }
        }

        // Animation (optional)
        if (animator != null)
        {
            bool moving = agent.velocity.magnitude > 0.1f;
            animator.SetBool(animBool, moving);
        }
    }
}
