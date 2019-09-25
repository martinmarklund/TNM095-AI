using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Worker : MonoBehaviour
{
	public Transform coffee;
	public Transform workstation;
	public NavMeshAgent agent;
	Vector3 agentDestination;
	public float energy = 5.0f;

	// Start is called before the first frame update
	void Start()
	{
		agent = GetComponent<NavMeshAgent>();
		Move(workstation);
	}

	// Update is called once per frame
	void Update()
	{
		energy -= 0.3f * Time.deltaTime;
		updateNeeds();

	}

	public bool isAtGoal(Transform goal)
	{
		if (Vector3.Distance(agent.transform.position, goal.position) < 1.0f)
		{
			return true;
		}
		return false;
	}

    

    void updateNeeds()
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

	void needCoffee() {

	}
	void Move(Transform goal)
	{
		agent.destination = goal.position;
	}

}


