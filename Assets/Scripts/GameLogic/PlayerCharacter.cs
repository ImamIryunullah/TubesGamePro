using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCharacter : MonoBehaviour
{
    public float speed = 1.5f;

    public Camera camera;

    public float sensitivity = 5f;
    public float headMinY = -40f;
    public float headMaxY = 40f;

    public KeyCode jumpButton = KeyCode.Space;
    public float jumpForce = 0.025f;
    public float jumpDistance = 0.025f;

    private Vector3 direction;
    private float h, v;
    private int layerMask;
    private Rigidbody body;
    private float rotationY;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;
        layerMask = 1 << gameObject.layer | 1 << 2;
        layerMask = ~layerMask;
    }

    void FixedUpdate()
    {
        body.AddForce(direction * speed, ForceMode.VelocityChange);

        if (Mathf.Abs(body.linearVelocity.x) > speed)
        {
            body.linearVelocity = new Vector3(Mathf.Sign(body.linearVelocity.x) * speed, body.linearVelocity.y, body.linearVelocity.z);
        }
        if (Mathf.Abs(body.linearVelocity.z) > speed)
        {
            body.linearVelocity = new Vector3(body.linearVelocity.x, body.linearVelocity.y, Mathf.Sign(body.linearVelocity.z) * speed);
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
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        float rotationX = camera.transform.localEulerAngles.y + Input.GetAxis("Mouse X") * sensitivity;
        rotationY += Input.GetAxis("Mouse Y") * sensitivity;
        rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);
        camera.transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);

        direction = new Vector3(h, 0, v);
        direction = camera.transform.TransformDirection(direction);
        direction = new Vector3(direction.x, 0, direction.z);

        if (Input.GetKeyDown(jumpButton) && GetJump())
        {
            body.linearVelocity = new Vector2(0, jumpForce);
        }
    }

    public bool Active
    {
        get
        {
            return camera.enabled;
        }
        set
        {
            gameObject.SetActive(value);
            camera.enabled = value;
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, Vector3.down * jumpDistance);
    }
}