using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class PlayerCharacter : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5.0f;
    public float jumpForce = 4.0f; // Kekuatan loncat (sesuaikan jika kurang tinggi)

    [Tooltip("Jarak deteksi tanah. Jika karakter melayang, naikkan sedikit.")]
    public float jumpCheckDistance = 0.3f;

    [Header("Mouse Settings")]
    public float mouseSensitivity = 3.0f;
    public float headMinY = -80f;
    public float headMaxY = 80f;

    [Header("References")]
    public Camera playerCamera;
    public Animator anim;

    private Rigidbody body;
    private Vector3 direction;
    private float h, v;
    private int layerMask;
    private float rotationY = 0f;

    // --- ANTI DOUBLE JUMP VARS ---
    private float jumpCooldown = 0.5f; // Jeda 0.5 detik sebelum bisa loncat lagi
    private float lastJumpTime = 0f;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.freezeRotation = true;

        // Kunci kursor mouse
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (anim == null) anim = GetComponent<Animator>();

        // Layer mask: Ignore layer 'Ignore Raycast' (2) dan layer Player itu sendiri
        // Agar raycast tidak mendeteksi badan sendiri sebagai tanah
        int playerLayer = gameObject.layer;
        layerMask = ~(1 << playerLayer | 1 << 2);
    }

    void Update()
    {
        // 1. INPUT
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");

        if (anim != null)
        {
            anim.SetBool("isWalking", (h != 0 || v != 0));
        }

        // 2. ROTASI BADAN (Mouse X)
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        transform.Rotate(Vector3.up * mouseX);

        // 3. ROTASI KAMERA (Mouse Y)
        rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, headMinY, headMaxY);

        if (playerCamera != null)
        {
            playerCamera.transform.localEulerAngles = new Vector3(-rotationY, 0f, 0f);
        }

        // 4. PERGERAKAN (FPS Style: W selalu maju ke arah pandang)
        direction = (transform.forward * v) + (transform.right * h);
        if (direction.magnitude > 1) direction.Normalize();

        // 5. LOGIKA LOMPAT
        // Syarat: Tombol Spasi + Menyentuh Tanah + Cooldown selesai
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded() && Time.time > lastJumpTime + jumpCooldown)
        {
            DoJump();
        }
    }

    void FixedUpdate()
    {
        // Menggerakkan player via Velocity (Unity 6 compatible)
        Vector3 targetVelocity = direction * speed;

        // Penting: Kita ambil Y (naik/turun) yang sedang terjadi agar gravitasi tetap jalan
        float currentVerticalSpeed = body.linearVelocity.y;

        // Gabungkan kecepatan jalan (X, Z) dengan kecepatan jatuh/lompat (Y)
        body.linearVelocity = new Vector3(targetVelocity.x, currentVerticalSpeed, targetVelocity.z);
    }

    void DoJump()
    {
        // Ambil velocity saat ini
        Vector3 vel = body.linearVelocity;

        // KITA PAKSA kecepatan Y (naik) menjadi sesuai JumpForce.
        // Tidak peduli seberapa berat (Mass) karakternya, dia akan naik dengan kecepatan ini.
        vel.y = jumpForce;

        // Terapkan kembali
        body.linearVelocity = vel;

        // Catat waktu lompat
        lastJumpTime = Time.time;

        if (anim != null) anim.SetTrigger("jump");
    }

    // Logika cek tanah yang LEBIH KUAT
    bool IsGrounded()
    {
        // Kita tembak Raycast dari sedikit di ATAS kaki (0.1f) ke arah BAWAH
        // Ini mencegah raycast gagal deteksi kalau pivot point tenggelam sedikit ke lantai
        Vector3 origin = transform.position + (Vector3.up * 0.1f);

        // Visualisasi garis merah di Scene view (untuk debug)
        Debug.DrawRay(origin, Vector3.down * jumpCheckDistance, Color.red);

        return Physics.Raycast(origin, Vector3.down, jumpCheckDistance, layerMask);
    }

    public bool Active
    {
        get { return playerCamera.enabled; }
        set
        {
            playerCamera.enabled = value;
            this.enabled = value;
            if (!value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }
    }
}