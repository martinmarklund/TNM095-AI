using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;

public class Worker : MonoBehaviour
{

    
    // Position related variables
    private Vector3 agentDestination;
    public Transform coffeeMachine;
	public Transform workstation;
    public Transform boss;

    // Agent related variables
    public NavMeshAgent agent;

    public float energy = 5.0f;
    public float occupation = 1.0f;

    private Task move;
    private LayerMask mask;

    private Transform[] coffeeMachines;
    private Transform[] workStations;    

    //**** TASKS ****//
    // Check if energy level is too low
    [Task]
    public bool isWorking;
    [Task]
    public bool isBossNear;

    [Task]
    public bool NeedsEnergy()
    {
        if (energy < 0)
        {
            return true;            
        }
        else
            return false;
    }
    
    // Check if agent has arrived at current goal
    [Task]
    public bool arrived;

    
    // Behaviour tree calls this function to decide destination
    [Task]
    void Move(string goal)
    {
        if (goal == "Coffee")
        {                                
            Move(coffeeMachines[FindClosest(coffeeMachines)]);
            isWorking = false;
        }
        else if (goal == "Workstation")
        {
            Move(workstation);
            isWorking = true;
        }


        move = Task.current;
    }    

    // Refill energy depending on type
    [Task]
    void RefillEnergy(string type)
    {
        if (type == "Coffee")
        {
            energy = 15.0f;
        }

        arrived = false;
        Task.current.Succeed();
    }


    // Start is called before the first frame update
    void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Move(workstation);
        mask = ~LayerMask.GetMask("Ignore Raycast");

        GameObject[] cM = GameObject.FindGameObjectsWithTag("CoffeMachine");
        coffeeMachines = new Transform[cM.Length];
        for (int i = 0; i < cM.Length; i++)
        {
            coffeeMachines[i] = cM[i].transform;
        }

        cM = GameObject.FindGameObjectsWithTag("Workstation");
        workStations = new Transform[cM.Length];
        for (int i = 0; i < cM.Length; i++)
        {
            workStations[i] = cM[i].transform;
        }
    }

    private void FixedUpdate()
    {
        IsBossNear();
    }

    // Update is called once per frame
    void Update()
	{
        // Check what the agent is doing
        occupation = Occupation();

        // Drain/Increase needs
        energy -= Time.deltaTime * occupation * 0.3f;

        IsAtGoal(agent.destination);

	}

    // True if agent is close enough to goal, otherwise false
	public void IsAtGoal(Vector3 goal)
	{
		if (Vector3.Distance(agent.transform.position, goal) < 2.0f)
		{
            arrived = true;
            move.Complete(true);
        }
        else
            arrived = false;
	}

    //Checks if the boss is near and see you
    public void IsBossNear()
    {
        //First check if the boss is within view istance
        if (Vector3.Distance(gameObject.transform.position, boss.position) < 20.0f) //TODO: hardcoded distance, should only check 2d?
        {
            isBossNear = false;
            //Cast a raycast and see what it hits with a layermask
            RaycastHit hit;
            if (Physics.Raycast(gameObject.transform.position, (boss.position - gameObject.transform.position), out hit, 20.0f, mask))
            {
                //If target hit is boss the boss is near and sees you
                if (hit.transform.tag == "Boss")
                {
                    isBossNear = true;

                }
                //If wall or door is hit the boss cant see you
                else if (hit.transform.tag == "Wall" || hit.transform.tag == "Door") //TODO if several floors check for roof/floor hit too
                {
                    isBossNear = false;
                }
                Debug.DrawRay(gameObject.transform.position, Vector3.Normalize(boss.position - gameObject.transform.position) * hit.distance, Color.yellow);
            }
        }
        else
        {
            isBossNear = false;
        }
    }

    public int FindClosest(Transform[] transArr)
    {
        agent.ResetPath();
        int chosen = 0;
        NavMeshPath path = new NavMeshPath();
        float minPathLength = 99999.0f;

        //Loop trough all pp
        for (int i = 0; i < transArr.Length; i++)
        {
            path.ClearCorners();
            float pathL = 0.0f;
            //Calculate the path            
            agent.CalculatePath(GetObjectFront(transArr[i]), path);            
            
            //If path is valid
            if (true || path.status == NavMeshPathStatus.PathComplete)
            {                
                //Calc the leght of the path            
                for (int j = 1; j < path.corners.Length; ++j)
                {
                    pathL += Vector3.Distance(path.corners[j - 1], path.corners[j]);
                }                
                //If this path is shorter than the current min set it as the current destiantion
                if (pathL < minPathLength)
                {                    
                    minPathLength = pathL;
                    chosen = i;
                }
            }
        }

        return chosen;
    }

    Vector3 GetObjectFront(Transform obj)
    {        
        return obj.position + obj.forward;
    }

    // Overloaded move that is used within script to assign destination
    void Move(Transform goal)
    {
        arrived = false;
        agent.destination = goal.position;
    }

    public float Occupation()
    {
        if (isWorking)
            return 1.3f;
        else if (!arrived)
            return 0.5f;
        else
            return 1.0f;
    }

    /* 
    void CheckNeeds()
         {
             if (energy < 5)
             {
                 Move(workstation);
             }

             if (energy < 0)
             {
                 Move(coffee);

                 if (isAtGoal(coffee))
                 {
                     energy = 5.0f;
                 }
             }
         }

     void NeedCoffee() {

     }
     */
}


