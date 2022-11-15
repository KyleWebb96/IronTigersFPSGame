using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("----  Components ----")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject UI;
    [SerializeField] Image HPBar;
    [SerializeField] Image HPBarAnim;
    [SerializeField] AudioSource aud;


    [Header("---- Enemy Stats ----")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int speedChase;
    [SerializeField] int sightDist;
    [SerializeField] int sightAngle;
    [SerializeField] int roamDist;
    [SerializeField] GameObject headPos;

    [Header("---- Gun Stats ----")]
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPos;
    [SerializeField] float shootRate;

    [Header("---- Audio ----")]
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;
    [SerializeField] AudioClip[] audShoot;
    [Range(0, 1)][SerializeField] float audShootVol;

    Color origColor;
    bool isShooting;
    bool playerInRange;
    Vector3 playerDir;
    float angleToPlayer;
    float distToPlayer;
    Vector3 startingPos;
    float stoppingDistOrig;
    int HPOrig;

    float HPTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        HPOrig = HP;
        startingPos = transform.position;
        stoppingDistOrig = agent.stoppingDistance;
        origColor = GetComponent<MeshRenderer>().material.color;
        updateHPBar();
    }

    // Update is called once per frame
    void Update()
    {
        playerDir = (gameManager.instance.player.transform.position - headPos.transform.position);

        angleToPlayer = Vector3.Angle(playerDir, transform.forward);

        distToPlayer = Vector3.Distance(playerDir, transform.forward);

        //Debug.Log(distToPlayer);

        if (agent.enabled)
        {
            if (playerInRange)
            {
                canSeePlayer();
            }
            else if (agent.remainingDistance < 0.1f && agent.destination != gameManager.instance.player.transform.position)
            {
                roam();
            }
        }

        if(HPBarAnim.fillAmount != HPBar.fillAmount)
        {
            HPBarAnim.fillAmount = Mathf.Lerp(HPBarAnim.fillAmount, HPBar.fillAmount, HPTimer);
            HPTimer += 0.25f * Time.deltaTime;
        }
        else { HPTimer = 0f; }
    }

    void canSeePlayer()
    {
        RaycastHit hit;

        if(Physics.Raycast(headPos.transform.position, playerDir, out hit))
        {
            Debug.DrawRay(headPos.transform.position, playerDir);

            //Debug.Log(agent.remainingDistance);

            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.stoppingDistance = stoppingDistOrig;

                agent.SetDestination(gameManager.instance.player.transform.position);

                if (agent.remainingDistance < agent.stoppingDistance)
                {
                    facePlayer();
                }

                if (!isShooting && distToPlayer <= sightDist)
                {
                    StartCoroutine(shoot());
                }
            }
        }
    }

    void roam()
    {
        agent.stoppingDistance = 0;

        Vector3 randomDir = Random.insideUnitSphere * roamDist;

        randomDir += startingPos;

        NavMeshHit hit;

        NavMesh.SamplePosition(new Vector3(randomDir.x, 0, randomDir.z), out hit, 1, 1);

        NavMeshPath path = new NavMeshPath();

        agent.CalculatePath(hit.position, path);

        agent.SetPath(path);
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

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);

        updateHPBar();

        agent.stoppingDistance = 0;
        agent.SetDestination(gameManager.instance.player.transform.position);

        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            gameManager.instance.updateEnemyNumber();
            Destroy(gameObject);
        }
    }

    void updateHPBar()
    {
        HPBar.fillAmount = (float)HP / (float)HPOrig;
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

        aud.PlayOneShot(audShoot[0], audShootVol);

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
