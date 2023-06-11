using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranqBullet : MonoBehaviour
{
    [SerializeField] int damage;
    [SerializeField] public int speed;
    [SerializeField] int timer;

    [SerializeField] Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, timer);

        rb.velocity = transform.forward * speed;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ISleep sleepable = other.GetComponent<ISleep>();
            if (sleepable != null)
            {
                sleepable.ReduceStamina(damage);
            }
        }
    }
}
