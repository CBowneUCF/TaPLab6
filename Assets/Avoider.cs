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
