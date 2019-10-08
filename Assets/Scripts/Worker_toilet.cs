using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Panda;
using UnityEngine.UI;
public class Worker_toilet : MonoBehaviour
{


    // Position related variables
    private Vector3 agentDestination;
    public Transform coffeeMachine;
    public Transform workstation;
    public Transform toilet;
    public Transform sink;

    // Agent related variables
    public NavMeshAgent agent;
    public float energy = 5.0f;
    public float occupation = 1.0f;
    public float bladder = 1.0f;
    public Task move;

    // Thought related variables
    public Transform thoughtPivot;
    public RawImage thoughtBubble;
    public Texture[] thoughts = new Texture[3]; // Coffee, Work, Toilet
    


    //**** TASKS ****//
    // Check if energy level is too low
    [Task]
    public bool isWorking;

    [Task]
    public bool NeedsEnergy()
    {
        if (energy < 0)
        {
            Task.current.Succeed();
            return true;
        }
        else
            return false;
    }

    [Task]
    public void NeedBathroom() {

        if (bladder > 0.3f)
        {
            Task.current.Fail();
        }

        else {
            Move("Toilet");
            Task.current.Succeed();
        }
    }
    //Use the bathroom, affect the bladder
    [Task]
    public void UseBathroom() {

        if (arrived == true) {
            bladder = 1.0f;
            Task.current.Succeed();
        }
       
    }

    // Check if agent has arrived at current goal
    [Task]
    public bool arrived;

    // Behaviour tree calls this function to decide destination
    [Task]
    void Move(string goal)
    {

        //Byta ut mot en switch-case??? billigare
        if (goal == "Coffee")
        {
            Move(coffeeMachine);
            isWorking = false;
        }
        else if (goal == "Workstation")
        {
            Move(workstation);
            isWorking = true;
        }

        else if (goal == "Toilet")
        {
            Move(toilet);
            isWorking = false;
        }

        else if (goal == "Sink") {
            Move(sink);
            isWorking = false;
        }

        UpdateThought(goal);

        move = Task.current;
    }

    // Refill energy depending on type
    [Task]
    void RefillEnergy(string type)
    {
        if (type == "Coffee")
        {
            energy = 15.0f;
            bladder -= 0.5f;
        }

        arrived = false;
        Task.current.Succeed();
    }


    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        Move(workstation);
    }

    // Update is called once per frame
    void Update()
    {
        // Check what the agent is doing
        occupation = Occupation();

        // Drain/Increase needs
        energy -= Time.deltaTime * occupation * 0.3f;
        bladder -= Time.deltaTime * 0.02f;

        IsAtGoal(agent.destination);

        // Update thought bubbles
        UpdateThoughtPosition();
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

    /// <summary>
    /// Updates the position of the thought bubble so that it sticks to pivot point
    /// </summary>
     void UpdateThoughtPosition() {
         Vector3 bubblePosition = Camera.main.WorldToScreenPoint(thoughtPivot.position);
         thoughtBubble.transform.position = bubblePosition;
     }

    /// <summary>
    /// Checks the current task of the agent and updates thought texture accordingly
    /// </summary>
     void UpdateThought(string goal) {
         switch(goal)
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
}


