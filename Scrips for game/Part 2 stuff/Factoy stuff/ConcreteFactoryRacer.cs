using UnityEngine;
using UnityEngine.AI;

namespace Scrips_for_game.Part_2_shit.Factoy_stuff
{
    public class ConcreteFactoryRacer: Abstact_Racer_Factory
    {
        public GameObject TopRacerPrefab;  
        public GameObject MidRacerPrefab;  
        public GameObject BottomRacerPrefab; // wanted to use different models, didnt get the time to do so 
        public override BaseAiclass CreateRacer1(Vector3 spawnPosition,Quaternion spawnRotation)
        {
            GameObject racerObject = GameObject.Instantiate(TopRacerPrefab, spawnPosition, spawnRotation);// just so it  faces forward.
            Top_racers racer = racerObject.AddComponent<Top_racers>();
            racer.agent = racerObject.GetComponent<NavMeshAgent>();
            racer.ChangeStats();
            return racer;
        }
        public override BaseAiclass CreateRacer2(Vector3 spawnPosition, Quaternion spawnRotation)
        {
            GameObject racerObject2 = GameObject.Instantiate(MidRacerPrefab, spawnPosition,spawnRotation);
            MidField racer = racerObject2.AddComponent<MidField>();
            racer.agent = racerObject2.GetComponent<NavMeshAgent>();
            racer.ChangeStats();
            return racer;
        }
        public override BaseAiclass CreateRacer3(Vector3 spawnPosition,Quaternion spawnRotation)
        {
            GameObject racerObject3 = GameObject.Instantiate(BottomRacerPrefab, spawnPosition, spawnRotation);
            BackMarkers racer = racerObject3.AddComponent<BackMarkers>();
            racer.agent = racerObject3.GetComponent<NavMeshAgent>();
            racer.ChangeStats();
            return racer;
        }
    }
}