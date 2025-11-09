using TMPro;
using UnityEditor;
using UnityEngine;
using System.Collections;
public class room : MonoBehaviour
{
    public static room instance;

    [SerializeField] float eventTime;
    [SerializeField] GameObject hiddenDoor;
    [SerializeField] public GameObject mainDoor;
    [SerializeField] GameObject enemy;
    [SerializeField] int maxEnemies;
    [SerializeField] float spawnRate;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] public TMP_Text roomGoalLabel;
    [SerializeField] public TMP_Text doorStatusLabel; //door open ui


    public int roomGoalCount;
    float playerStartTime;
    float playerFinishTime;
    float playerTotalRoomTime;
    float spawnTimer;
    int enemyCount;
    int roomsComplete;
    int totalEnemiesSpawned;

    bool startSpawning;
    public bool roomActive;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.instance.updateGameGoal(1);        
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
        if(!mainDoor)
        {
            showDoorMessage();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            roomActive = true;
            startSpawning = true;
            doorState(true);
        }
    }

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        GameObject enemyClone = Instantiate(enemy, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
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
            Debug.Log("door open!");
        }
        if (playerFinishTime < eventTime)
        {
            hiddenDoor.SetActive(state);
            if(!hiddenDoor)
                StartCoroutine(showDoorMessage());
        }
    }    

    public void UpdateRoomGoal(int amount)
    {
        roomGoalCount += amount;
        
        if (roomGoalCount <= 0 && totalEnemiesSpawned >= maxEnemies)
        {
            doorState(false);
            roomActive = false;
            startSpawning = false;
            gameManager.instance.updateGameGoal(-1);
        }
        string v = roomGoalCount.ToString("F0");
        roomGoalLabel.text = v;  
    }
    IEnumerator showDoorMessage()
    {
        doorStatusLabel.gameObject.SetActive(true);
        doorStatusLabel.text = "Door Open!";
        yield return new WaitForSeconds(5f);
        doorStatusLabel.gameObject.SetActive(false);
       
    }
}
