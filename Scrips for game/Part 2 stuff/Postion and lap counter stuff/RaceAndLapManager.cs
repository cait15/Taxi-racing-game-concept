using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class RaceAndLapManager : MonoBehaviour
{ // i changed the script name because it became more of a race manager
    
    [Header("Lap and Race SHit")]
    public int currentWaypoint; // for the player, the reason why i did this is because the player object is the one that actually controlls what lap the race is
    public int totalWaypoints;// for the player
    public TextMeshProUGUI Lapcounter; 
    public int lap = 1;
    public PostionManager PostionManager;
    private string No1;
    public GameObject LapPosition;
    public int leaderboardWaypointIndex = 0;
    private bool hasJustCompletedLap = false;
    public delegate void LoseGameCondition();
    public static event LoseGameCondition OnLoseGameCondition;
    public Transform currentGraphWayPointTracker; 
    private bool isLevel3;
    public delegate void WinGameCondition();
    public static event WinGameCondition OnWinGameCondition;
    private void Awake()
    {
        isLevel3 = SceneManager.GetActiveScene().name.Contains("3");// this is a neat little trick i learnt, if it has the number 3 in the name, it is level3
    }
    private void Start()
    {
        Lapcounter.text = lap.ToString() + " /3";
        Debug.Log("Just checking");
        if (isLevel3)
        {
            currentGraphWayPointTracker = GraphManager.instance.GetFirstWayPoint();
        }
    }
    private void Update()
    {
        if (leaderboardWaypointIndex >= 7)
        {
            leaderboardWaypointIndex = 0;
        }
    }
    private void OnTriggerEnter(Collider other)
    {  if (!other.CompareTag("Waypoint")) return;
        SFXManager.instance.PlayRequestedSound("CheckPoint", isLoop: false);
        if (!isLevel3)
        { // if it aint level 3, this will use the linked list
            if (other.transform == LinkedListManager.instance.GetWaypoint(currentWaypoint))
            {
                currentWaypoint = (currentWaypoint + 1) % LinkedListManager.instance.ListSize;
                totalWaypoints++;

                if (currentWaypoint == 0 && totalWaypoints > 1)
                {
                    lap++;
                    Lapcounter.text = lap.ToString() + " /3";
                    Debug.Log("Lap completed");
                    SFXManager.instance.PlayRequestedSound("LapCompleted", isLoop: false);
                }
                RaceEndConditionBs();
            }
        }
        else
        { // using graph stuffs
            if (other.transform == currentGraphWayPointTracker && totalWaypoints == 0) // this is just for the first waypoint
            {
                totalWaypoints++;
                if (leaderboardWaypointIndex < IgnoreBranchesManager.instance.idealGraphPath.Count &&
                    other.transform == IgnoreBranchesManager.instance.idealGraphPath[leaderboardWaypointIndex])
                {
                  leaderboardWaypointIndex++; // adds to this, this is only if the player crosses a waypoint that isnt branching hence the ideal path thats uses for the leaderboard
                    Debug.Log($"Leading waypoint: {leaderboardWaypointIndex}");
                }
            }
            
            if (GraphManager.instance.IsConnectedTo(currentGraphWayPointTracker, other.transform)) // checking if the waypoint thats crossed  is a waypoint connected. this is done as we dont know what branch the player is.for eg if the currentwa
            {
                currentGraphWayPointTracker = other.transform;
                totalWaypoints++;
                Debug.Log($"Total Waypoints: {totalWaypoints}");
                if (leaderboardWaypointIndex < IgnoreBranchesManager.instance.idealGraphPath.Count &&
                    other.transform == IgnoreBranchesManager.instance.idealGraphPath[leaderboardWaypointIndex])
                {
                    leaderboardWaypointIndex++;
                    Debug.Log($"Leading waypoint: {leaderboardWaypointIndex}");
                }
                Debug.Log($"Leading waypoint: {leaderboardWaypointIndex}");
            }

       
            if (GraphLapCompletedBs(currentGraphWayPointTracker))
            {
                if (!hasJustCompletedLap)
                {
                    lap++;
                    SFXManager.instance.PlayRequestedSound("LapCompleted", isLoop: false);
                    hasJustCompletedLap = true;
                    Debug.Log($"Lap completed: {lap}");
                    Lapcounter.text = lap.ToString() + " /3";
                }
            }
            else
            {
                if (currentGraphWayPointTracker != GraphManager.instance.GetFirstWayPoint())
                {
                    hasJustCompletedLap = false;
                }
            }

            RaceEndConditionBs();
        }
    }
    private void RaceEndConditionBs()
    {// ai i put it in a method just to make it cleaner, this just checks who if the player wins or loses
        if (lap > 3)
        {
            No1 = PostionManager.FirstPlace;
            LapPosition.SetActive(false);

            if (No1 == "Player")
            {
                OnWinGameCondition?.Invoke(); 
            }
            else
            {
                OnLoseGameCondition?.Invoke(); 
            }
        }
    }
    private bool GraphLapCompletedBs(Transform waypoint)
    {
        // works the same way as the linked list, once it crosses over the first waypoint again, it will increase the lap
        return waypoint == GraphManager.instance.GetFirstWayPoint() && totalWaypoints > 1;
    }
}


    

