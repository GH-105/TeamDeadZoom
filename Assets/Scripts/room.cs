using UnityEditor;
using UnityEngine;

public class room : MonoBehaviour
{
    [SerializeField] float eventTime;
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
            Destroy(mainDoor);
            gameManager.instance.updateGameGoal(-1); 
            if(roomGoalCount <= 0 && playerFinishTime < eventTime)
            {
                Destroy(hiddenDoor);
            }
            
        }
    }
}
