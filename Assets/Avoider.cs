using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Avoider : MonoBehaviour
{
    public Transform targetTransform;
    public float scareRange;
    public float poissonCheckRate;
    public bool enableGizmos;

    float checkTimer;
    private NavMeshAgent navAgent;

    List<Vector3> validPoints = new();

    bool isRunningAway = false;



    bool inRange => AvoideeInRange();

    private void Awake()
    {
        TryGetComponent(out navAgent);
        
    }

    private void Update()
    {
        if (inRange)
        {
            if (CheckLineOfSight(transform.position) && !isRunningAway)
            {
                validPoints.Clear();
                //Call the sampler to get our samples
                var sampler = new PoissonDiscSampler(scareRange, scareRange, 0.5f);
                foreach (var point in sampler.Samples())
                {
                    Vector3 newPoint = new Vector3(point.x, 0, point.y);
                    if (CheckLineOfSight(newPoint))
                    {

                    }
                }
                isRunningAway = true;
            }
        }

        if (isRunningAway)
        {
            //Run to destination
        }
    }

    bool AvoideeInRange()
    {
        return (Vector3.Distance(targetTransform.transform.position, transform.position) < scareRange); 
    }


    bool CheckLineOfSight(Vector3 source)
    {
        RaycastHit hitInfo;
        if (Physics.Linecast(source, targetTransform.transform.position, out hitInfo))
        {
            if (hitInfo.collider.gameObject == targetTransform.gameObject)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }

    }






    private void OnDrawGizmos()
    {
        
    }
}

/*
Sorting idea:

Get points from Poisson Disc, sort through them either using Linq or a primitive sorting algorithm.

(The Primitive Sorting Algorithm will loop through the whole list once, tracking the lowest number found, making a new list and feeding everything lower to the first spot on the list and everything higher to the last.)
(This is not a perfect sort, but should put most of the closest points in the first half of the list, at the very least.)

Then, check in order the new list whether it's in line of sight, hopefully having to check less points.

*/
