using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ammoObjectPickup : MonoBehaviour
{
    [SerializeField] ammoPickup ammo;
    [SerializeField] float degreesPerSecond;
    [SerializeField] float range;
    [SerializeField] float speed;

    private float destroyTimer;
    private float yOrigin;
    private float yPos;

    void Start()
    {
        destroyTimer = ammo.timer;
        yOrigin = transform.position.y;
    }

    void Update()
    {
        destroyTimer -= Time.deltaTime;

        if(destroyTimer <= 0)
            Destroy(gameObject);
    }

    void FixedUpdate()
    {
        transform.Rotate(0, Time.deltaTime * degreesPerSecond, 0);

        yPos = Mathf.PingPong(Time.time * speed, 1) * range + yOrigin;
        transform.position = new Vector3(transform.position.x, yPos, transform.position.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.ammoObjectPickup(ammo);
            Destroy(gameObject);
        }
    }
}