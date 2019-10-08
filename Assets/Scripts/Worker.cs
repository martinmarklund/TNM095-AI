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
    public Transform toilet;
    public Transform sink;

    // Agent related variables
    public NavMeshAgent agent;
    //public float energy = 5.0f;
    public float occupation = 1.0f;
    //public float bladder = 1.0f;
    private Task move;
    private LayerMask mask;

    NeedsCompontent need;

    //**** TASKS ****//
    // Check if energy level is too low
    [Task]
    public bool isWorking;
    [Task]
    public bool isBossNear;

    [Task]
    public bool NeedsEnergy()
    {
        if (need.energyLevel < 0)
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

        switch(goal){
            case "Coffee":
                Move(coffeeMachine);
                isWorking = false;
                break;
            case "Workstation":
                Move(workstation);
                isWorking = true;
                break;
            case "Toilet":
                Move(toilet);
                isWorking = false;
                break;
            case "Sink":
                Move(sink);
                isWorking = false;
                break;
        }

        move = Task.current;
    }

    // Refill energy depending on type
    [Task]
    void RefillEnergy(string type)
    {
        if (type == "Coffee")
        {
            need.energyLevel += 15.0f;
        }

        arrived = false;
        Task.current.Succeed();
    }

    /*--- Bathroom check ----*/
    //If bladder is low enough, move to the toilet. Otherwise the task will fail
    [Task]
    public bool NeedBathroom() {
        if (need.bladderLevel < 0.2f) { return true; }
        else { return false; }
    }
   /* public void NeedBathroom()
    {

        if (need.bladderLevel > 0.3f)
        {
            Task.current.Fail();
        }

        else
        {
            Move("Toilet");
            Task.current.Succeed();
        }
    }*/

    //Use the bathroom; this will obv affect the bladder
    [Task]
    public void UseBathroom()
    {
     
            need.bladderLevel = 1.0f;
            Task.current.Succeed();

    }

    /*--- TASKS END ----*/


    private void Awake()
    {
        need = GetComponent<NeedsCompontent>();
       
    }

    // Start is called before the first frame update
    void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Move(workstation);
        mask = ~LayerMask.GetMask("Ignore Raycast");
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
        //energy -= Time.deltaTime * occupation * 0.3f;
        //bladder -= Time.deltaTime * 0.02f;

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


