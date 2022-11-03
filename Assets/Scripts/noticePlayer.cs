using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class noticePlayer : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;

    bool playerInRange;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameManager.instance.playerDeadMenu.activeSelf)
        {
            playerInRange = false;
        }
        if(playerInRange)
        {
            agent.SetDestination(gameManager.instance.player.transform.position);
        }
        else if(!playerInRange)
        {
            agent.SetDestination(agent.transform.position);
        }
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
