using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class playerController : MonoBehaviour
{
    [Header("----- Components -----")]
    [SerializeField] CharacterController controller;

    [Header("----- Player Stats -----")]
    [SerializeField] int HP;
    [SerializeField] float playerSpeed;
    [SerializeField] float sprintMod;
    [SerializeField] float jumpHeight;
    [SerializeField] float gravityValue;
    [SerializeField] int jumpsMax;

    [Header("----- Gun Stats -----")]
    [SerializeField] float shootRate;
    [SerializeField] int shootDist;
    [SerializeField] int shootDamage;
    [SerializeField] GameObject gunModel;
    [SerializeField] List<gunStats> gunStatList = new List<gunStats>();
    [SerializeField] GameObject hitEffect; 


    Vector3 move;
    private Vector3 playerVelocity;
    int jumpsTimes;
    float playerSpeedOrig;
    bool isSprinting;
    bool isShooting;
    int HPOrig;
    int selectedGun; 

    private void Start()
    {
        playerSpeedOrig = playerSpeed;
        HPOrig = HP;
        playerRespawn();
        updatePlayerHPBar();
    }

    void Update()
    {
        movement();
        sprint();
        StartCoroutine(shoot());
        gunSelect(); 
    }

    void movement()
    {
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            jumpsTimes = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") +
               transform.forward * Input.GetAxis("Vertical");

        controller.Move(move * Time.deltaTime * playerSpeed);

        if (Input.GetButtonDown("Jump") && jumpsTimes < jumpsMax)
        {
            jumpsTimes++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed *= sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed /= sprintMod;
            isSprinting = false;
        }
    }
    IEnumerator shoot()
    {
        if (gunStatList.Count > 0 && !isShooting && Input.GetButton("Shoot"))
        {
            isShooting = true;

            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist) && shootDamage > 0)
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(shootDamage);
                }

                Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
            }


            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void damage(int dmg)
    {
        HP -= dmg;
        updatePlayerHPBar();

        StartCoroutine(gameManager.instance.playerDamageFlash());

        if(HP <= 0)
        {
            gameManager.instance.playerDeadMenu.SetActive(true);
            gameManager.instance.pauseGame();
        }    
    }

    void updatePlayerHPBar()
    {
        gameManager.instance.HPBar.fillAmount = (float)HP / (float)HPOrig;
    }

    public void gunPickup(gunStats gunStat)
    {
        shootRate = gunStat.shootRate;
        shootDist = gunStat.shootDist;
        shootDamage = gunStat.shootDamage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.gunModel.GetComponent<MeshRenderer>().sharedMaterial; 

        gunStatList.Add(gunStat);
    }

    void gunSelect()
    {
        if(gunStatList.Count > 1)
        {
            if(Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunStatList.Count - 1)
            {
                selectedGun++;
                changeGuns(); 
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)
            {
                selectedGun--;
                changeGuns(); 
            }
        }
    }

    void changeGuns()
    {
        shootRate = gunStatList[selectedGun].shootRate;
        shootDist = gunStatList[selectedGun].shootDist;
        shootDamage = gunStatList[selectedGun].shootDamage;

        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStatList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStatList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void playerRespawn()
    {
        controller.enabled = false;
        transform.position = gameManager.instance.spawnPos.transform.position;
        HP = HPOrig;
        controller.enabled = true;
        updatePlayerHPBar();
    }
}