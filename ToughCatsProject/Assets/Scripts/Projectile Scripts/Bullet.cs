using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
            IDamage damagable = other.GetComponent<IDamage>();
            if (damagable != null)
            {
                damagable.takeDamage(damage);
            }
        }

        if (!other.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);
        }
    }
}
