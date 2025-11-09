using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
public class playerController : MonoBehaviour, IDamage, IPickup, IStatusDamageReceiver
{
    [SerializeField] LayerMask ignoreLayer;
    [SerializeField] LayerMask aimMask;
    [SerializeField] CharacterController controller;

    [SerializeField] public float HP;
    [SerializeField] int speed;
    [SerializeField] int sprintMod;
    [SerializeField] int jumpSpeed;
    [SerializeField] int jumpCountMax;
    [SerializeField] int gravity;
    [SerializeField] int underwaterGravity;
    [SerializeField] int underwaterSpeed;
    [SerializeField] int underwaterJumpSpeed;
    [SerializeField] float groundbuffer = .1f;

    [SerializeField] int dashDist;
    [SerializeField] public int maxAirDash;
    statusController status;


    [SerializeField] int shootDamage;
    [SerializeField] int shootDist;
    [SerializeField] float shootRate;
    [SerializeField] int numProjectiles;
    [SerializeField] int numRicochet;
    [SerializeField] int bulletSpeed;
    [SerializeField] GameObject gunModel;
    [SerializeField] Transform firePos;
    [SerializeField] GameObject bullet;

    [SerializeField] TextMeshProUGUI reloadText;

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] audSteps;
    [Range(0, 1)][SerializeField] float audStepsVol;
    [SerializeField] AudioClip[] audJump;
    [Range(0, 1)][SerializeField] float audJumpVol;
    [SerializeField] AudioClip[] audHurt;
    [Range(0, 1)][SerializeField] float audHurtVol;

    Vector3 moveDir;
    Vector3 playerVel;
    Vector3 dashDir;
    Quaternion baseRot;

    private int currDash = 0;

    int jumpCount;
    public float HPOrig;
    public int gunListPos;
    public int gravOrig;
    public int speedOrig;
    public int jumpSpeedOrig;
    public int bulletDamage;
    float stepDeg = 6f;
    float lastGrounTime;
    

    float shootTimer;

    bool isSprinting;
    bool isPlayingSteps;
    bool isUnderWater;
    bool isInAir;
    bool isDashing = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        HPOrig = HP;
        gravOrig = gravity;
        speedOrig = speed;
        jumpSpeedOrig = jumpSpeed;
        spawnPlayer();

        controller = GetComponent<CharacterController>();
        status = GetComponent<statusController>();

        if (PowerUpManager.Instance != null)//checks everytime before applying
        {
            if (PowerUpManager.Instance.gunList.Count > 0)
            {
                if (PowerUpManager.Instance == null) return;
                if (PowerUpManager.Instance.gunList.Count == 0) return;

                gunListPos = 0;
                changeGun();
            }

            if (PowerUpManager.Instance.pstat)
            {
                PowerUpManager.Instance.ApplyPstats(this);
                PowerUpManager.Instance.pstat = false;
            }
        }
        if (reloadText != null)
            reloadText.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            lastGrounTime = Time.time;
        }
        isInAir = !controller.isGrounded;
        Debug.DrawRay(Camera.main.transform.position, Camera.main.transform.forward * shootDist, Color.red);
        shootTimer += Time.deltaTime;

        if (!gameManager.instance.isPaused)
            movement();

        sprint();

        if (Input.GetButtonDown("Sprint") && Time.time > lastGrounTime + groundbuffer && currDash < maxAirDash && !isDashing)
        {
            startDash();
        }
    }

    void movement()
    {
        if (isDashing)
        {
            controller.Move(playerVel*Time.deltaTime);
            return;
        }
        if (controller.isGrounded)
        {
            if (moveDir.normalized.magnitude > 0.3f && !isPlayingSteps) //t6
            {
                StartCoroutine(playStep());
            }

            playerVel = Vector3.zero;
            jumpCount = 0;
            currDash = 0;
        }
        else
        {
            playerVel.y -= gravity * Time.deltaTime;
        }

        moveDir = Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward;
        controller.Move(moveDir * speed * Time.deltaTime);

        jump();
        controller.Move(playerVel * Time.deltaTime);

        if (Input.GetButton("Fire1") && PowerUpManager.Instance.gunList.Count > 0 && PowerUpManager.Instance.GetCurrentAmmo(gunListPos) > 0 && shootTimer >= shootRate)
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
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            speed /= sprintMod;
            isSprinting = false;
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

        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        Vector3 aimPoint;
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, aimMask))
            aimPoint = hit.point;
        else
            aimPoint = ray.origin + ray.direction * 1000f;

        Vector3 dir = (aimPoint - firePos.position).normalized;
        Quaternion aimRotation = Quaternion.LookRotation(dir, firePos.up);

        if (PowerUpManager.Instance.GetCurrentAmmo(gunListPos) <= 1)
        {
            if (reloadText != null)
                reloadText.gameObject.SetActive(true);
        }

        PowerUpManager.Instance.ConsumeAmmo(gunListPos);
        aud.PlayOneShot(PowerUpManager.Instance.gunList[gunListPos].baseStats.shootSounds[Random.Range(0, PowerUpManager.Instance.gunList[gunListPos].baseStats.shootSounds.Length)], PowerUpManager.Instance.gunList[gunListPos].baseStats.shootSoundVol);
        updatePlayerUI();

        for (int i = 0; i < numProjectiles; i++)
        {
            float center = (numProjectiles - 1) * 0.5f;
            float offsetIndex = i - center;
            float angle = offsetIndex * stepDeg;

            Quaternion rot = Quaternion.AngleAxis(angle, firePos.up) * aimRotation;

            GameObject newBullet = Instantiate(bullet, firePos.position, rot);
            var d = newBullet.GetComponent<Damage>();
            d.Init(
            shooter: gameObject,
            dmg: shootDamage,
            speed: bulletSpeed,
            effects: PowerUpManager.Instance.gunList[gunListPos].effects
            );
            
        }
        
    }

    void reload()
    {
        if (Input.GetButtonDown("Reload"))
        {
            PowerUpManager.Instance.ReloadCurrentGun(gunListPos);
            updatePlayerUI();
            reloadText.gameObject.SetActive(false);
        }
        
    }

    public void takeDamage(in DamageContext context, IReadOnlyList<EffectInstance> effects)
    {
        if (HP <= 0f) return;

        float finalHit = ComputeFinalHit(context.baseHitDamage, context.source);
        ApplyHP(finalHit, context.source);
        StartCoroutine(flashPlayerDmg());
        if (status != null && effects != null)
        {
            for (int i = 0; i < effects.Count; i++)
            {
                var e = effects[i];

                float mag = ScaleMagnitude(e.effect, e.magnitude, in context);
                if (mag <= 0f) continue;

                status.ApplyEffect(e.effect, in context, mag);
                
            }
        }

        aud.PlayOneShot(audHurt[Random.Range(0, audHurt.Length)], audHurtVol);
        updatePlayerUI();
    }

    public void ApplyDot(float amount, DamageEffects effect, GameObject source)
    {
        HP -= amount;
        StartCoroutine(flashPlayerDmg());
        updatePlayerUI();
        if (HP <= 0f)
        {
            gameManager.instance.youLose();
        }
    }

    float ComputeFinalHit(float baseHit, GameObject source)
    {
        return baseHit;
    }

    public void updatePlayerUI()
    {
        if (PowerUpManager.Instance.gunList.Count > 0)
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

    public void changeGun()
    {
        if (PowerUpManager.Instance == null) return;

        int count = PowerUpManager.Instance.gunList.Count;
        if (count == 0) return;
        gunListPos = Mathf.Clamp(gunListPos, 0, count - 1);

        var (damage, rate, range) = PowerUpManager.Instance.CalcGunStats(gunListPos);

        shootDamage = damage;
        shootDist = range;
        shootRate = rate;
        //numChains = gunList[gunListPos].maxChains;
        //numProjectiles = gunList[gunListPos].maxProjectiles;

        gunModel.GetComponent<MeshFilter>().sharedMesh = PowerUpManager.Instance.gunList[gunListPos].baseStats.gunModel.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = PowerUpManager.Instance.gunList[gunListPos].baseStats.gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        if (PowerUpManager.Instance.GetCurrentAmmo(gunListPos) <= 1)
        {
            if (reloadText != null)
                reloadText.gameObject.SetActive(true);
            //Debug.Log("Reload");

        }
        else
        {
            if (reloadText != null)
                reloadText.gameObject.SetActive(false);
        }

            updatePlayerUI();
    }

    void selectGun()
    {
        if (PowerUpManager.Instance == null || PowerUpManager.Instance.gunList.Count == 0) return;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && gunListPos < PowerUpManager.Instance.gunList.Count - 1)
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = true;
            speed = underwaterSpeed;
            jumpSpeed = underwaterJumpSpeed;
            gravity = underwaterGravity;
            if (isSprinting)
                speed *= sprintMod;
            gameManager.instance.playerUWScreen.SetActive(true);

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = false;
            speed = speedOrig;
            jumpSpeed = jumpSpeedOrig;
            gravity = gravOrig;
            if (isSprinting)
                speed *= sprintMod;
            gameManager.instance.playerUWScreen.SetActive(false);
        }
    }

    public void spawnPlayer()
    {
        controller.enabled = false;
        controller.transform.position = gameManager.instance.playerSpawnPos.transform.position + Vector3.up * 1f;
        controller.enabled = true;
        //controller.transform.position = gameManager.instance.playerSpawnPos.transform.position;
        HP = HPOrig;
        updatePlayerUI();
    }
    Vector3 GetDashDir()
    {
        Vector3 camForward = Camera.main.transform.forward;
        camForward.y = 0f;
        if(camForward.y < 0f)
        {
            return transform.forward;
        }
        return camForward.normalized;
    }
    
    void ApplyHP(float amount, GameObject source)
    {
        HP -= amount;
        flashPlayerDmg();
        if (HP <= 0)
        {
            gameManager.instance.youLose();
        }
    }

    float ScaleMagnitude(DamageEffects def, float magnitude, in DamageContext context)
    {
        return magnitude;
    }

    void startDash()
    {
        if (isDashing || !isInAir)
            return;

        isDashing = true;
        currDash++;

        dashDir = GetDashDir();
        playerVel.y = 0f;

        StartCoroutine(DashCoroutine());
    
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

    IEnumerator DashCoroutine()
    {
        float dashDur = 0.3f;
        float dashSpeed = dashDist / dashDur;
        float startTime = Time.time;

        while (Time.time < startTime + dashDur)
        {
            controller.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }

        
        isDashing = false;

        currDash = Mathf.Clamp(currDash, 0, maxAirDash);
    }
}
