using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCharacter : MonoBehaviour
{
    public float speed = 1.5f;

    public Camera playerCamera;
    public Animator anim;

    public float sensitivity = 5f;
    public float headMinY = -40f;
    public float headMaxY = 40f;

    public KeyCode jumpButton = KeyCode.Space;
    public float jumpForce = 3f;
    public float jumpDistance = 0.25f;

    public float rotationSpeed = 120f;   // ← dari PlayerController

    private Vector3 direction;
    private float h, v;
    private int layerMask;
    private Rigidbody body;
    private float rotationY;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;

        anim = GetComponent<Animator>();

        layerMask = (1 << gameObject.layer) | (1 << 2);
        layerMask = ~layerMask;
    }

    void FixedUpdate()
    {
        // Movement physics
        body.AddForce(direction * speed, ForceMode.VelocityChange);

        // Clamp velocity
        if (Mathf.Abs(body.linearVelocity.x) > speed)
        {
            body.linearVelocity = new Vector3(
                Mathf.Sign(body.linearVelocity.x) * speed,
                body.linearVelocity.y,
                body.linearVelocity.z
            );
        }

        if (Mathf.Abs(body.linearVelocity.z) > speed)
        {
            body.linearVelocity = new Vector3(
                body.linearVelocity.x,
                body.linearVelocity.y,
                Mathf.Sign(body.linearVelocity.z) * speed
            );
        }
    }

    bool GetJump()
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out hit, jumpDistance, layerMask))
        {
            return true;
        }
        return false;
    }

    void Update()
    {
        // Input
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        anim.SetBool("isWalking", (h != 0 || v != 0));

        // ------------------------------ 
        // 🔥 Player Rotation (from PlayerController)
        // ------------------------------ 
        float turn = h * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, turn, 0);

        // ------------------------------ 
        // 🔥 Camera Rotation (your FPS camera)
        // ------------------------------
        float rotationX = playerCamera.transform.localEulerAngles.y +
                          Input.GetAxis("Mouse X") * sensitivity;

        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);

        playerCamera.transform.localEulerAngles =
            new Vector3(-rotationY, rotationX, 0);

        // ------------------------------ 
        // 🔥 Movement Direction (PlayerController style)
        // W/S = maju/mundur mengikuti arah badan
        // ------------------------------
        direction = transform.forward * v;

        // Jump
        if (Input.GetKeyDown(jumpButton) && GetJump())
        {
            Vector3 newVel = body.linearVelocity;
            newVel.y = jumpForce;
            body.linearVelocity = newVel;

            anim.SetTrigger("jump");
        }
    }

    public bool Active
    {
        get { return playerCamera.enabled; }
        set
        {
            playerCamera.enabled = value;
            this.enabled = value;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * jumpDistance);
    }
}