using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Panda;

public class Worker : MonoBehaviour
{

    
    // Position related variables
    //private Vector3 agentDestination;
    //public Transform coffeeMachine;
	private Transform workstation;
    private Transform boss;
    public Transform toilet;
    public Transform sink;

    private GameObject[] coffeeMachines;
    private GameObject[] workStations;
    private GameObject[] toiletPosistions;
    private GameObject targetObject;

    // Agent related variables
    public NavMeshAgent agent;
    //public float energy = 5.0f;
    public float occupation = 1.0f;
    //public float bladder = 1.0f;
    private Task move;
    private LayerMask mask;

    NeedsCompontent need;
    public int queueNumber;

    // Thought related variables
    private Transform thoughtPivot;
    private RawImage thoughtBubble;
    public Texture[] thoughts = new Texture[3]; // Coffee, Work, Toilet


    //**** TASKS ****//
    // Check if energy level is too low
    [Task]
    public bool isWorking;
    [Task]
    public bool isBossNear;
    [Task]
    public bool queueing;

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

    [Task]
    public bool IsOccupied()
    {       
        return targetObject.GetComponent<DestinationProperties>().isFull();
    }

    [Task]
    public bool IsQueueFull()
    {
        return targetObject.GetComponent<DestinationProperties>().isQueueFull();
    }

    [Task]
    public bool IsFirstinLine()
    {
        if (queueNumber == 0)
            return true;
        else
            return false;
    }

    // Check if agent has arrived at current goal
    [Task]
    public bool arrived;

    //toilet.GetComponent<DestinationProperties>().inUse;
    
    // Behaviour tree calls this function to decide destination
    [Task]
    void Move(string goal)
    {
        switch(goal){
            case "Coffee":
                targetObject = coffeeMachines[FindClosest(coffeeMachines)];
                Move(targetObject.transform);
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

        UpdateThought(goal);
        move = Task.current;
    } 
    
    [Task]
    void Queue()
    {
        queueing = true;
        queueNumber = targetObject.GetComponent<DestinationProperties>().GetFirstFreeQueueArea();
        Move(targetObject.GetComponent<DestinationProperties>().queueAreas[queueNumber].areaTransform);
        Task.current.Succeed();
    }

    [Task]
    void IsNextFree()
    {
        if (targetObject.GetComponent<DestinationProperties>().GetFirstFreeQueueArea() == queueNumber - 1)
        {
            if(queueNumber > 0)
            {
                queueNumber--;
                Move(targetObject.GetComponent<DestinationProperties>().queueAreas[queueNumber].areaTransform);
                Task.current.Succeed();
            }
            else
                Task.current.Fail();
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void Stopqueueing()
    {
        queueing = false;
        Task.current.Succeed();
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
        queueing = false;
        //queueNumber = -1;
        agent = GetComponent<NavMeshAgent>();
		
        mask = ~LayerMask.GetMask("Ignore Raycast");

        thoughtPivot = gameObject.transform.GetChild(0);
        thoughtBubble = gameObject.transform.GetChild(1).GetChild(0).gameObject.GetComponent<RawImage>(); 
        boss = GameObject.FindGameObjectsWithTag("Boss")[0].transform;

        GameObject[] gameObjs = GameObject.FindGameObjectsWithTag("CoffeMachine");
        coffeeMachines = new GameObject[gameObjs.Length];
        for (int i = 0; i < gameObjs.Length; i++)
        {
            coffeeMachines[i] = gameObjs[i];
        }

        gameObjs = GameObject.FindGameObjectsWithTag("Workstation");
        workStations = new GameObject[gameObjs.Length];
        for (int i = 0; i < gameObjs.Length; i++)
        {
            workStations[i] = gameObjs[i];
        }

        gameObjs = GameObject.FindGameObjectsWithTag("Toilet");
        toiletPosistions = new GameObject[gameObjs.Length];
        for (int i = 0; i < gameObjs.Length; i++)
        {
            toiletPosistions[i] = gameObjs[i];
        }

        workstation = workStations[0].transform;//workStations[(int)(Random.value * (workStations.Length))].transform;
        Move(workstation);
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
        UpdateThoughtPosition();

	}

    // True if agent is close enough to goal, otherwise false
	public void IsAtGoal(Vector3 goal)
	{
		if (!arrived && Vector3.Distance(agent.transform.position, goal) < 2.0f)
		{
            arrived = true;
            move.Complete(true);
        }
//        else
//            arrived = false;
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

    public int FindClosest(GameObject[] gameArr)
    {
        agent.ResetPath();
        int chosen = 0;
        NavMeshPath path = new NavMeshPath();
        float minPathLength = 99999.0f;

        //Loop trough all objects
        for (int i = 0; i < gameArr.Length; i++)
        {
            path.ClearCorners();
            float pathL = 0.0f;
            //Calculate the path            
            agent.CalculatePath(GetObjectFront(gameArr[i].transform), path);            
            
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

    [Task]
    void MoveToUse()
    {
        arrived = false;        
        int firstFree = targetObject.GetComponent<DestinationProperties>().GetFirstFreeUseArea();
        Debug.Log(firstFree);
        if(firstFree != -1)
        {
            move = Task.current;
            agent.destination = targetObject.GetComponent<DestinationProperties>().useAreas[firstFree].areaTransform.position;
        }        
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

    /// <summary>
    /// Updates the position of the thought bubble so that it sticks to pivot point
    /// </summary>
    void UpdateThoughtPosition()
    {
        Vector3 bubblePosition = Camera.main.WorldToScreenPoint(thoughtPivot.position);
        thoughtBubble.transform.position = bubblePosition;
    }

    /// <summary>
    /// Checks the current task of the agent and updates thought texture accordingly
    /// </summary>
    void UpdateThought(string goal)
    {
        switch (goal)
        {
            case "Coffee":
                thoughtBubble.texture = thoughts[0];
                break;
            case "Workstation":
                thoughtBubble.texture = thoughts[1];
                break;
            case "Toilet":
                thoughtBubble.texture = thoughts[2];
                break;
            case "Sink":    // Can be changed to individual bubble for "Sink"
                thoughtBubble.texture = thoughts[2];
                break;
        }

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


