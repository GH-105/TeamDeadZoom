using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class playerController : MonoBehaviour, IDamage, IPickup
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] CharacterController controller;

    [SerializeField] public int HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;

    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    //[SerializeField] int numProjectiles;
    //[SerializeField] int numChains;
    [SerializeField] GameObject gunModel;

    [SerializeField] AudioSource aud; 
    [SerializeField] AudioClip[] audSteps; 
    [Range(0, 1)][SerializeField] float audStepsVol; 
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol; 
    [SerializeField] AudioClip[] audHurt; 
    [Range(0, 1)][SerializeField] float audHurtVol; 

    Vector3 moveDir;
    Vector3 playerVel;

    int jumpCount;
    public int HPOrig;
    public int gunListPos;
    List<GunListings> playerGunList;

    float shootTimer;

    bool isSprinting;
    bool isPlayingSteps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        spawnPlayer();

        if(PowerUpManager.Instance != null)//checks everytime before applying
        {
            playerGunList = PowerUpManager.Instance.gunList;
            if (playerGunList.Count > 0)
            {
                gunListPos = 0;
            }

            changeGun();

            if (PowerUpManager.Instance.pstat)
            {
                PowerUpManager.Instance.ApplyPstats(this);
                PowerUpManager.Instance.pstat = false;
            }
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        shootTimer += Time.deltaTime;
        
        if(!gameManager.instance.isPaused)
            movement();

        sprint();

    }

    void movement()
    {
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingSteps) //t6
            {
                StartCoroutine(playStep());
            }

            playerVel = Vector3.zero;
            jumpCount = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if(Input.GetButton("Fire1") && playerGunList.Count > 0 && PowerUpManager.Instance.GetCurrentAmmo(gunListPos) > 0 && shootTimer >= shootRate)
        {
            shoot();
        }

        selectGun();
        reload();
    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            speed *= sprintMod;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
        }
    }

    void jump()
    {
        if ((Input.GetButtonDown("Jump")) && jumpCount < jumpCountMax)
        {
            aud.PlayOneShot(audJump[Random.Range(0, audJump.Length)], audJumpVol);
            playerVel.y = jumpSpeed;
            jumpCount++;
        }
    }

    void shoot()
    {
        shootTimer = 0;
        PowerUpManager.Instance.ConsumeAmmo(gunListPos);
        aud.PlayOneShot(playerGunList[gunListPos].baseStats.shootSounds[Random.Range(0, playerGunList[gunListPos].baseStats.shootSounds.Length)], playerGunList[gunListPos].baseStats.shootSoundVol);
        updatePlayerUI();

        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, shootDist, ~ignoreLayer))
        {
            Instantiate(playerGunList[gunListPos].baseStats.hitEffect, hit.point, Quaternion.identity);

            IDamage dmg = hit.collider.GetComponent<IDamage>();
            if(dmg != null)
            {
                dmg.takeDamage(shootDamage);
            }
        }
    }

    void reload()
    {
        if(Input.GetButtonDown("Reload"))
        {
            PowerUpManager.Instance.ReloadCurrentGun(gunListPos);
        }
        updatePlayerUI();
    }

    public void takeDamage(int amount)
    {
        HP -= amount;
        updatePlayerUI();
        StartCoroutine(flashPlayerDmg());
        if(HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    public void updatePlayerUI()
    {
        if(PowerUpManager.Instance.gunList.Count > 0)
        {
            gameManager.instance.ammoCur.text = PowerUpManager.Instance.GetCurrentAmmo(gunListPos).ToString();
            gameManager.instance.ammoMax.text = PowerUpManager.Instance.GetMaxAmmo(gunListPos).ToString();
        }    
    }

    IEnumerator flashPlayerDmg()
    {
        gameManager.instance.playerDamageScreen.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerDamageScreen.SetActive(false);
    }

    public void getGunStats(gunStats gun)
    {
        
        gunListPos = PowerUpManager.Instance.AddGun(gun);
        changeGun();
    }

    void changeGun()
    {
        if (PowerUpManager.Instance == null) return;

        int count = PowerUpManager.Instance.gunList.Count;
        if (count == 0) return;
        gunListPos = Mathf.Clamp(gunListPos, 0, count - 1);

        var (damage, rate, ammo, range) = PowerUpManager.Instance.CalcGunStats(gunListPos);

        shootDamage = damage;
        shootDist = range;
        shootRate = rate;
        //numChains = gunList[gunListPos].maxChains;
        //numProjectiles = gunList[gunListPos].maxProjectiles;

        gunModel.GetComponent<MeshFilter>().sharedMesh = playerGunList[gunListPos].baseStats.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = playerGunList[gunListPos].baseStats.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        updatePlayerUI();
    }

    void selectGun()
    {
        if (PowerUpManager.Instance == null || PowerUpManager.Instance.gunList.Count == 0) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < playerGunList.Count - 1)
        {
            gunListPos++;
            changeGun();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && gunListPos > 0)
        {
            gunListPos--;
            changeGun();
        }
    }

    public void spawnPlayer()
    {
        controller.transform.position = gameManager.instance.playerSpawnPos.transform.position;
        HP = HPOrig;
        updatePlayerUI();
    }

    public int Speed
    {
        get => speed;
        set => speed = value;
    }

    public int JumpCountMax
    {
        get => jumpCountMax;
        set => jumpCountMax = value;    
    }
    IEnumerator playStep() 
    {
        isPlayingSteps = true;
        aud.PlayOneShot(audSteps[Random.Range(0, audSteps.Length)], audStepsVol);

        if (isSprinting)
        {
            yield return new WaitForSeconds(0.3f);
        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }
        isPlayingSteps = false;
    }
}
