using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----  Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;


    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int speedChase;
    [SerializeField] int sightDist;
    [SerializeField] int sightAngle;
    [SerializeField] GameObject headPos;

    [Header("---- Gun Stats ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;

    Color origColor;
    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;

    // Start is called before the first frame update
    void Start()
    {
        origColor = GetComponent<MeshRenderer>().material.color;
        gameManager.instance.enemiesToKill++;
        gameManager.instance.updateUI();
    }

    // Update is called once per frame
    void Update()
    {


        playerDir = (gameManager.instance.player.transform.position - headPos.transform.position);

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        if (gameManager.instance.playerDeadMenu.activeSelf)
        {
            playerInRange = false;
        }
        if (playerInRange && angleToPlayer <= sightAngle)
        {
            canSeePlayer();
        }
    }

    void canSeePlayer()
    {
        RaycastHit hit;
        if(Physics.Raycast(headPos.transform.position, playerDir, out hit))
        {
            if(hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                facePlayer();

                if (!isShooting)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }

    void facePlayer()
    {
        playerDir.y = 0;

        Quaternion rotation = Quaternion.LookRotation(playerDir);

        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * playerFaceSpeed);
    }

    public void takeDamage(int dmg)
    {
        HP -= dmg;

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.updateEnemyNumber();
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;

        yield return new WaitForSeconds(0.15f);

        model.material.color = origColor;
    }
    IEnumerator shoot()
    {
        isShooting = true;

        Instantiate(bullet, shootPos.position, transform.rotation);

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
        
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
