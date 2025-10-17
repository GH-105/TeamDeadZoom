using UnityEngine;

public class room : MonoBehaviour
{
    [SerializeField] float hiddenDoorTime;
    [SerializeField] GameObject hiddenDoor;
    [SerializeField] GameObject mainDoor;
    
    

    int roomGoalCount;
    float playerStartTime;
    float playerFinishTime;
    float playerTotalRoomTime;
    int roomsComplete;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameManager.instance.updateGameGoal(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateRoomGoal(int amount)
    {
       
        roomGoalCount += amount;
        if (roomGoalCount <= 0)
        {
            RecordPlayerTime();
            OpenMainDoor();
            gameManager.instance.updateGameGoal(-1); 

            if(playerFinishTime < hiddenDoorTime)
            {
                RevealHiddenDoor();
            }
        }
        Debug.Log($"Room completed in {playerFinishTime} seconds. Hidden door threshold: {hiddenDoorTime}");
    }

    public void RecordPlayerTime()
    {
        playerFinishTime = playerFinishTime.time - playerStartTime;
        playerTotalRoomTime += playerFinishTime;
    }

    private void OpenMainDoor()
    {
        if (mainDoor != null) Destroy(mainDoor);
    }

    private void RevealHiddenDoor()
    {
        if (hiddenDoor != null) Destroy(hiddenDoor);
    }
}
