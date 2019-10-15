using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using Panda;
using System;

public class Worker : MonoBehaviour
<<<<<<< HEAD
{
    // Position related variables
    //private Vector3 agentDestination;
    //public Transform coffeeMachine;
    public Transform workstation;
=======
{
    // Position related variables
    //private Vector3 agentDestination;
    //public Transform coffeeMachine;
	private Transform workstation;
>>>>>>> cb8168c9277e80d00268981d7e00880a5b6f4a80
    private Transform boss;
    //public Transform toilet;
    //public Transform sink;

<<<<<<< HEAD
    public float totalWork = 1.0f;
=======
    private int totalWork = 1;
    
    private GameObject[] coffeeMachines;
    private GameObject[] workStations;
    private GameObject[] toiletPosistions;
    private GameObject[] sinks;
    private GameObject targetObject;
>>>>>>> cb8168c9277e80d00268981d7e00880a5b6f4a80


    // Agent related variables
    public NavMeshAgent agent;
    public float occupation = 1.0f;
    private Task move;
    private LayerMask mask;
    public bool isWorking;


    NeedsCompontent need;
    public int queueNumber;
    public int useAreaNumber;

    public int workBench;
    public bool haveAssignedWorkstation;

    // Thought related variables
    private Transform thoughtPivot;
    private RawImage thoughtBubble;
    public Texture[] thoughts = new Texture[3]; // Coffee, Work, Toilet


    //**** TASKS ****//
    // Check if energy level is too low
    private bool isUsing;
    public bool needsNewTarget;
    [Task]
    public bool IsWorking(string goal) {
        switch (goal) {
            case "Coffee":
                return false;
            case "Workstation":
                return true;
            case "Toilet":
                return false;
            case "Sink":
                return false;
        }

        return true;
    }


    [Task]
    public bool isBossNear;
    [Task]
    public bool queueing;

    [Task]
    public bool GoodMatch() {
        return true;
    }

    [Task]
    public bool NeedsEnergy()
    {
        if (need.energyLevel < 20.0f)
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


    

    [Task]
    void Queue()
    {
<<<<<<< HEAD
        switch(goal){
            case "Coffee":
                Move(coffeeMachines[FindClosest(coffeeMachines)]);
                isWorking = IsWorking(goal);
                break;
            case "Workstation":
                Move(workstation);
                isWorking = IsWorking(goal);
                break;
            case "Toilet":
                Move(toilet);
                isWorking = IsWorking(goal);
                break;
            case "Sink":
                Move(sink);
                isWorking = IsWorking(goal);
                break;
=======
        queueing = true;
        queueNumber = targetObject.GetComponent<DestinationProperties>().GetFirstFreeQueueArea();
        targetObject.GetComponent<DestinationProperties>().BookQueueAreaState(queueNumber);
        Move(targetObject.GetComponent<DestinationProperties>().queueAreas[queueNumber].areaTransform);
        Task.current.Succeed();
    }

    [Task]
    void IsNextFree()
    {
        //if (targetObject.GetComponent<DestinationProperties>().GetFirstFreeQueueArea() == queueNumber - 1)
        if (!targetObject.GetComponent<DestinationProperties>().queueAreas[queueNumber - 1].inUse)
        {
            if(queueNumber > 0)
            {
                queueNumber--;                
                targetObject.GetComponent<DestinationProperties>().BookQueueAreaState(queueNumber);
                targetObject.GetComponent<DestinationProperties>().ReleaseQueueAreaState(queueNumber + 1);
                Move(targetObject.GetComponent<DestinationProperties>().queueAreas[queueNumber].areaTransform);
                Task.current.Succeed();
            }
            else
                Task.current.Fail();
>>>>>>> cb8168c9277e80d00268981d7e00880a5b6f4a80
        }
        else
        {
            Task.current.Fail();
        }
    }

    [Task]
    void Stopqueueing()
    {
        if (queueing)
        {
            queueing = false;
            targetObject.GetComponent<DestinationProperties>().ReleaseQueueAreaState(queueNumber);
        }        
        Task.current.Succeed();
    }

    // Refill energy depending on type
    [Task]
    void RefillEnergy(string type)
    {
        if (type == "Coffee")
        {
            need.energyLevel += 75.0f;
            need.bladderLevel -= 0.1f;
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

    [Task]
    public bool NotClean()
    {
        if (need.hygieneLevel < 8.0f) { return true; }
        else { return false; }
    }

    //Use the bathroom; this will obv affect the bladder
    [Task]
    public void Bathroom()
    {
        if (NotClean()) { need.hygieneLevel = 10.0f; }
        need.bladderLevel = 1.0f;
    }

    [Task]
    public void WorkEfficiency(int i)
    {

        if (i == 1) {

            Debug.Log("energy:" + need.energyLevel);

            if (need.energyLevel < 100 && need.energyLevel > 50)
            {
          
                DoWork(5);
            }
            else if (need.energyLevel < 50 && need.energyLevel > 20)
            {
                DoWork(4);
            }

        }    

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
        haveAssignedWorkstation = false;
        isWorking = false;
        arrived = true;
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


        gameObjs = GameObject.FindGameObjectsWithTag("Sink");
        sinks = new GameObject[gameObjs.Length];
        for (int i = 0; i < gameObjs.Length; i++)
        {
            sinks[i] = gameObjs[i];
        }

        workBench = (int)(UnityEngine.Random.value * (workStations.Length));
        //workstation = workStations[(int)(Random.value * (workStations.Length))].transform;
        //workstation = workStations[0].transform;
        //targetObject = workstation.gameObject;
        //Move(workstation);
        //Move("Workstation");        
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

    [Task]
    public void NeedsNewTarget()
    {
        Debug.Log("need new");
        needsNewTarget = true;
        Task.current.Succeed();
    }

    [Task]
    public void FindNewTarget()
    {
        if (needsNewTarget)
        {
            Debug.Log("trying to find new");
            FindNextClosest();
            Move(targetObject.transform);
            move = Task.current;
            needsNewTarget = false;
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
    
    public int FindNextClosest()
    {        
        GameObject[] gameArr = null;
        agent.ResetPath();
        int chosen = 0;
        NavMeshPath path = new NavMeshPath();
        float minPathLength = 99999.0f;

        switch (targetObject.tag)
        {
            case "CoffeMachine":
                gameArr = coffeeMachines;
                break;
            case "Workstation":
                gameArr = workStations;
                chosen = (int)(UnityEngine.Random.value * (workStations.Length));
                minPathLength = -1;
                //workStations[(int))].transform;
                break;
            case "Toilet": //TODO not implemented yet
                gameArr = toiletPosistions;
                break;
            case "Sink": //TODO not implemented yet
                gameArr = sinks;
                break;
        }            

        //Loop trough all objects
        for (int i = 0; i < gameArr.Length; i++)
        {
            if (gameArr[i].transform != targetObject.transform)
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
        }
        targetObject = gameArr[chosen];
        return chosen;
    }

    Vector3 GetObjectFront(Transform obj)
    {
        return obj.position + obj.forward;
    }

    [Task]
    public void AssignWorkStation()
    {
        haveAssignedWorkstation = true;
        workstation = targetObject.transform;
        Task.current.Succeed();
    }

    // Overloaded move that is used within script to assign destination
    void Move(Transform goal)
    {
        arrived = false;
        agent.destination = goal.position;
    }

    // Behaviour tree calls this function to decide destination
    [Task]
    void Move(string goal)
    {
        if (isUsing)
        {           
            targetObject.GetComponent<DestinationProperties>().ReleaseUseAreaState(useAreaNumber);
            isUsing = false;
        }
        else if (queueing)
        {            
            targetObject.GetComponent<DestinationProperties>().ReleaseQueueAreaState(queueNumber);
            queueing = false;
        }

        switch (goal)
        {
            case "Coffee":
                targetObject = coffeeMachines[FindClosest(coffeeMachines)];
                Move(targetObject.transform);
                isWorking = false;
                break;
            case "Workstation":
                if (haveAssignedWorkstation)
                {
                    targetObject = workstation.gameObject;
                }
                else
                {
                    targetObject = workStations[workBench];
                }
                //targetObject = workStations[(int)(Random.value * (workStations.Length))];
                Move(targetObject.transform);
                isWorking = true;
                break;
            case "Toilet":
                targetObject = toiletPosistions[FindClosest(toiletPosistions)];
                Move(targetObject.transform);
                isWorking = false;
                break;
            case "Sink":
                targetObject = sinks[FindClosest(sinks)];
                Move(targetObject.transform);
                isWorking = false;
                break;
        }

        UpdateThought(goal);
        move = Task.current;
    }

    [Task]
    void MoveToUse()
    {
        if (!isUsing)
        {
            arrived = false;
            int firstFree = targetObject.GetComponent<DestinationProperties>().GetFirstFreeUseArea();

            //Debug.Log(firstFree);
            if (firstFree != -1)
            {
                useAreaNumber = firstFree;
                isUsing = true;
                targetObject.GetComponent<DestinationProperties>().BookUseAreaState(useAreaNumber);
                move = Task.current;
                agent.destination = targetObject.GetComponent<DestinationProperties>().useAreas[useAreaNumber].areaTransform.position;
            }
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
    

    void DoWork(float i) {

        totalWork += i*0.1f;
        Debug.Log("Total work: " + totalWork);

    }
    

}
