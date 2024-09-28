using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Avoider : MonoBehaviour
{
	public Transform targetTransform;
	public float moveSpeed;
	public float scareRange;
	public bool enableGizmos;

	float checkTimer;
	private NavMeshAgent navAgent;

	List<Vector3> validPoints = new();
	List<Vector3> invalidPoints = new();
	Vector3 targetPoint;

	bool isRunningAway = false;



	bool inRange => AvoideeInRange();

	private void Awake()
	{
		TryGetComponent(out navAgent);

		bool issue = false;
		if (navAgent is null)
		{
			Debug.LogWarning("Avoider needs Nav Mesh Agent.");
			issue = true;
		}
		if (targetTransform is null)
		{
			Debug.LogWarning("Avoider needs Target");
			issue = true;
		}
		if (!navAgent.isOnNavMesh)
		{
			Debug.LogWarning("Avoider has no NavMesh");
			issue = true;
		}

		if (issue) enabled = false;

		navAgent.speed = moveSpeed;
	}

	private void Update()
	{

		if (inRange && CheckLineOfSight(transform.position) && !isRunningAway)
		{
			validPoints.Clear();
			invalidPoints.Clear();
			//Call the sampler to get our samples
			PoissonDiscSampler sampler = new(scareRange, scareRange, 1f);

			int minDistance = 0;
			foreach (Vector2 refPoint in sampler.Samples())
			{
				validPoints.Add(new(transform.position.x + refPoint.x - (scareRange / 2), transform.position.y, transform.position.z + refPoint.y - (scareRange / 2)));
				if (Vector3.Distance(validPoints[^1], targetTransform.position) < 1f || CheckLineOfSight(validPoints[^1]))
				{
					invalidPoints.Add(validPoints[^1]);
					validPoints.Remove(validPoints[^1]);
				}
				else
				{
					if (DistanceCheck(validPoints[^1]) < DistanceCheck(validPoints[minDistance]))
						minDistance = validPoints.Count - 1;
				}
			}
			if (validPoints.Count > 0) targetPoint = validPoints[minDistance];
			navAgent.SetDestination(targetPoint);
			isRunningAway = true;
		}

		if (isRunningAway && navAgent.remainingDistance < 0.05f)
			isRunningAway = false;
	}

	bool AvoideeInRange() => Vector3.Distance(targetTransform.transform.position, transform.position) < scareRange;


	bool CheckLineOfSight(Vector3 source) =>
		Physics.Linecast(source, targetTransform.transform.position, out RaycastHit hitInfo)
		&& hitInfo.collider.gameObject == targetTransform.gameObject;

	float DistanceCheck(Vector2 input) => Vector3.Distance(Vector3.zero, new(input.x, transform.position.y, input.y));

	private void OnDrawGizmos()
	{
		foreach (Vector3 validPoint in validPoints)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawLine(transform.position, validPoint);
		}
		foreach (Vector3 invalidPoint in invalidPoints)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawLine(transform.position, invalidPoint);
		}
	}
}

/*
Sorting idea:

Get points from Poisson Disc, sort through them either using Linq or a primitive sorting algorithm.

(The Primitive Sorting Algorithm will loop through the whole list once, tracking the lowest number found, making a new list and feeding everything lower to the first spot on the list and everything higher to the last.)
(This is not a perfect sort, but should put most of the closest points in the first half of the list, at the very least.)

Then, check in order the new list whether it's in line of sight, hopefully having to check less points.

*/
