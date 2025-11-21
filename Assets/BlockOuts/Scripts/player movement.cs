using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement_NoRotation : MonoBehaviour
{
    public float moveSpeed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 1.2f;

    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;

    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        // Ground check
        isGrounded = controller.isGrounded;

        if (isGrounded && velocity.y < 0)
            velocity.y = -2f;

        // INPUT (NO ROTATION)
        float x = Input.GetAxis("Horizontal");  // A/D
        float z = Input.GetAxis("Vertical");    // W/S

        // Move RELATIVE TO WORLD, NOT ROTATION
        Vector3 move = new Vector3(x, 0, z);

        controller.Move(move * moveSpeed * Time.deltaTime);

        // Jump
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        // Gravity
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}
