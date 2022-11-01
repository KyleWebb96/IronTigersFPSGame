using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet : MonoBehaviour
{
    [Header("---- Bullet ----")]
    [SerializeField] Rigidbody rb;

    [Header("---- Bullet info ----")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int timer;

    private void Start()
    {
        rb.velocity = (gameManager.instance.player.transform.position - transform.position) * speed;
        Destroy(gameObject, timer);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.damage(damage);
        }

        Destroy(gameObject);
    }
}
