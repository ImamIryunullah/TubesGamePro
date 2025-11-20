using UnityEngine;
using System.Collections;

public class AnimalPatrol : MonoBehaviour
{
    public float speed = 2f;
    public float patrolDistance = 5f;
    public float idleTime = 1.5f; // durasi jeda sebelum berputar

    private Vector3 startPos;
    private Vector3 target;
    private Animator anim;
    private bool isTurning = false;

    void Start()
    {
        anim = GetComponent<Animator>();
        startPos = transform.position;
        target = startPos + transform.forward * patrolDistance;
    }

    void Update()
    {
        if (isTurning) return; // stop gerak dulu ketika idle

        // Bergerak ke target
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // Animasi jalan
        anim.SetFloat("Speed", 1f);

        // Cek jika sudah sampai
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            StartCoroutine(TurnAfterIdle());
        }
    }

    IEnumerator TurnAfterIdle()
    {
        isTurning = true;

        // Set animasi Idle
        anim.SetFloat("Speed", 0f);

        // Tunggu beberapa detik
        yield return new WaitForSeconds(idleTime);

        // Putar 180 derajat
        transform.Rotate(0, 180f, 0);

        // Tentukan target berikutnya
        if (target == startPos + transform.forward * patrolDistance)
        {
            target = startPos - transform.forward * patrolDistance;
        }
        else
        {
            target = startPos + transform.forward * patrolDistance;
        }

        isTurning = false;
    }
}