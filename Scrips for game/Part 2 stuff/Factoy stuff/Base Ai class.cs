using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public abstract class BaseAiclass : MonoBehaviour
{
   // its soo much cleaner now YAY
   public NavMeshAgent agent;
    public int currentWaypoint = 0;
    public int totalWaypoints;
    public int leaderboardWaypointIndex = 0;
    [SerializeField] private Transform modelTransform;
    public Transform currentTargetGraphWaypoint{ get; private set; }
    private bool Level3;
    public abstract void ChangeStats();
    private void Awake()
    {
        Level3 = SceneManager.GetActiveScene().name.Contains("3");
    }

    protected virtual void Start()
    {
        agent = GetComponent<NavMeshAgent>(); 
        if (transform.childCount > 0)
        {
            modelTransform = transform.GetChild(0);
        }
        if (Level3)
        {
            currentTargetGraphWaypoint = GraphManager.instance.GetFirstWayPoint();// uses the first vertex and sets that as its target
        }
    }
    protected virtual void Update()
    {
      HandleSlopeRotation();// just to make the ai handle slopes a bit better( more for the models)
    }

    private void HandleSlopeRotation()
    {// essentially takes the raycast and from that it will fix the model, works similar to our car raycasting
        RaycastHit hit;
        float rayLength = 3f;
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;
        if (Physics.Raycast(rayOrigin, Vector3.down, out hit, rayLength))
        {
            Vector3 groundNormal = hit.normal;
            Vector3 projectedForward = Vector3.ProjectOnPlane(transform.forward, groundNormal).normalized; // gets the forward, will hug the terrain instead of running into it as it did in the past
            Quaternion targetRotation = Quaternion.LookRotation(projectedForward, groundNormal);
            Quaternion modelFix = Quaternion.Euler(0f, -90f, 0f);
            Quaternion finalRotation = targetRotation * modelFix;
            modelTransform.rotation = Quaternion.Slerp(modelTransform.rotation, finalRotation, Time.deltaTime * 5f);
        }
    }
    
   public void SetNextDestination()
    {
        if (!Level3)
        {
            
            Transform targetDestination = LinkedListManager.instance.GetWaypoint(currentWaypoint); // helper method comes in clutch, it will send the index in which the ai needs to go to currently
            if (targetDestination != null)
            {
                agent.SetDestination(targetDestination.position);
                Vector3 direction = agent.steeringTarget - transform.position;
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);// if that method returns something it will set the destination
            }
        }
    }
    public void SetNextDestinaionGraph()
    {
        if (Level3)
        {
           
            if (currentTargetGraphWaypoint != null) // setting the next destination target, this is usually called after we got the next waypoint
            {
                agent.SetDestination(currentTargetGraphWaypoint.position); 
                Vector3 direction = agent.steeringTarget - transform.position; // this is purely for orientating the model
                Quaternion lookRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotation, agent.angularSpeed * Time.deltaTime);// setting the next destination
            }
        }
    }
    private void GetNextGraphWaypoint()
    {
        // this is for the randomization bullshit, there is probably a better way to do this but it will randomly select an index of the connected waypoints, so if
        // a waypoint has just one , it will obv pick that one but if it has two it will randomly pick between the two
        List<Transform> nextWaypoints = GraphManager.instance.GetCurrentConnectedWaypoints(currentTargetGraphWaypoint);
        if (nextWaypoints != null && nextWaypoints.Count > 0)
        {
            int rand = Random.Range(0, nextWaypoints.Count);
            Transform selectedGraphWaypoint = nextWaypoints[rand];
            currentTargetGraphWaypoint = selectedGraphWaypoint;
        }
    }
   private  void IncrementWaypoint()
    {
        currentWaypoint = (currentWaypoint + 1) % LinkedListManager.instance.ListSize;// oh this one im proud of, because the ai needs to wrap back to the first waypoint, what this is doing is once it currentwaypoint reaches the max of the linked list size it will return 0 because im using mod
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!Level3)
        {
            if (other.CompareTag("Waypoint") &&
                other.transform ==
                LinkedListManager.instance
                    .GetWaypoint(currentWaypoint)) // if it is crossing the waypoint, cant cheat yk
            {
                totalWaypoints++;
                IncrementWaypoint();
                SetNextDestination();
            }
        }

        if (Level3)
        {// level 3 trigger 
            if (other.CompareTag("Waypoint") && other.transform == currentTargetGraphWaypoint)
            {
                totalWaypoints++;
                GetNextGraphWaypoint();
                SetNextDestinaionGraph();
                if (leaderboardWaypointIndex < IgnoreBranchesManager.instance.idealGraphPath.Count && other.transform ==
                    IgnoreBranchesManager.instance.idealGraphPath[leaderboardWaypointIndex])
                {
                    leaderboardWaypointIndex = (leaderboardWaypointIndex + 1) % IgnoreBranchesManager.instance.idealGraphPath.Count; // for the leaderboard stuff
                }
            }
        }
    }

}
