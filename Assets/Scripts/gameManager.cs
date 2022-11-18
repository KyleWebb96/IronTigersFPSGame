using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("----- Player Stuff -----")]
    public GameObject player;
    public playerController playerScript;

    [Header("----- UI -----")]
    public GameObject pauseMenu;
    public GameObject playerDeadMenu;
    public GameObject winMenu;
    public GameObject playerDamageScreen;
    public TextMeshProUGUI enemiesLeft;
    public TextMeshProUGUI ammoLeft;
    public TextMeshProUGUI totalAmmo;
    public Image HPBar;
    public Image HPBarAnim;
    public GameObject getKills;
    public GameObject Reloading;
    public GameObject Reload;


    public int enemiesToKill;
    public GameObject spawnPos;

    public bool isPaused;

    float HPTimer = 0f;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        spawnPos = GameObject.FindGameObjectWithTag("Spawn Pos");


    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && !playerDeadMenu.activeSelf && !winMenu.activeSelf)
        {
            isPaused = !isPaused;
            pauseMenu.SetActive(isPaused);

            if (isPaused)
                pauseGame();
            else
                unPauseGame();
        }

        if (HPBarAnim.fillAmount != HPBar.fillAmount)
        {
            HPBarAnim.fillAmount = Mathf.Lerp(HPBarAnim.fillAmount, HPBar.fillAmount, HPTimer);
            HPTimer += 0.25f * Time.deltaTime;
        }
        else { HPTimer = 0f; }
    }

    public void pauseGame()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public IEnumerator getFiveKills()
    {
        getKills.SetActive(true);
        yield return new WaitForSeconds(5);
        getKills.SetActive(false);

    }

    public void unPauseGame()
    {
        Time.timeScale = 1;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public IEnumerator playerDamageFlash()
    {
        playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        playerDamageScreen.SetActive(false);
    }

    public void youWin()
    {
        winMenu.SetActive(true);
        pauseGame();
    }

    public void updateUIEnemyKills()
    {
        enemiesToKill = playerScript.gunStatList[0].kills;
        enemiesLeft.text = enemiesToKill.ToString("F0");
    }

    public void updateUIAmmo()
    {
        ammoLeft.text = playerScript.currAmmo.ToString("F0");
        totalAmmo.text = playerScript.totalAmmo.ToString("F0");
    }
}