using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class room : MonoBehaviour
{
    public static room instance;

    [SerializeField] float eventTime;
    [SerializeField] GameObject hiddenDoor;
    [SerializeField] public GameObject mainDoor;
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [SerializeField] float spawnRate;
    [SerializeField] List<Transform> spawnPos;
    [SerializeField] public TMP_Text roomGoalLabel;
    [SerializeField] public TMP_Text doorStatusLabel; //door open ui
    
    public int roomGoalCount;
    float playerFinishTime;
    float spawnTimer;
    int enemyCount;
    int totalEnemiesSpawned;

    bool startSpawning;
    public bool roomActive;
    bool doorOpened = false;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
              if (gameManager.instance.bossRoom == this)
        {
            gameManager.instance.updateGameGoal(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(startSpawning)
        {
            spawnTimer += Time.deltaTime;

            if(enemyCount < maxEnemies && spawnTimer >= spawnRate)
            {
                if (totalEnemiesSpawned < maxEnemies)
                    spawn();
                else
                    startSpawning = false;
            }
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && doorOpened == false)
        {
            roomActive = true;
            startSpawning = true;
            doorState(true);
        }
    }

    void spawn()
    {
        int listPos = Random.Range(0, spawnPos.Count);

        GameObject enemyClone = Instantiate(enemy, spawnPos[listPos].position, spawnPos[listPos].rotation);
        enemyClone.GetComponent<EnemyAI>().thisRoom = this;
        enemyCount++;
        totalEnemiesSpawned++;
        spawnTimer = 0;
    }

   public void doorState(bool state)
    {
        mainDoor.SetActive(state);

        if (!state)
        {
            StartCoroutine(showDoorMessage());
        }
        if (playerFinishTime < eventTime)
        {
            
            if(hiddenDoor != null)
            {
                hiddenDoor.SetActive(state);
                StartCoroutine(showDoorMessage());
            }    
                
        }
    }    

    public void UpdateRoomGoal(int amount)
    {
        roomGoalCount = Mathf.Max(0, roomGoalCount + amount);
        
        if (!doorOpened && roomGoalCount == 0 && totalEnemiesSpawned >= maxEnemies)
        {
            doorState(false);
            doorOpened = true;

            roomActive = false;
            startSpawning = false;

            if (gameManager.instance.bossRoom == this)
            {
                Debug.Log("Boss Room Complete");
                gameManager.instance.updateGameGoal(-1);
            }
        }
            roomGoalLabel.text = roomGoalCount.ToString();
    }
    IEnumerator showDoorMessage()
    {
        doorStatusLabel.gameObject.SetActive(true);
        doorStatusLabel.text = "Door Open!";
        yield return new WaitForSeconds(1f);
        doorStatusLabel.gameObject.SetActive(false);
       
    }
}
