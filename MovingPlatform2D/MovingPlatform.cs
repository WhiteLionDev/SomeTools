using UnityEngine;
using System.Collections;
using System.Linq;

public class MovingPlatform : MonoBehaviour
{
	protected enum MoveType
	{
		MoveTowards,
		Lerp
	}

	protected enum PathType
	{
		ForwardAndBackward,
		ClosedCircuit,
		RebootAtEnd
	}

	[SerializeField]
	protected Transform[] wayPoints;
	[SerializeField]
	protected MoveType moveType = MoveType.MoveTowards;
	[SerializeField]
	protected PathType pathType = PathType.ForwardAndBackward;
	[SerializeField]
	protected float speed = 10;
	[SerializeField]
	protected float initialDelay = 0;
	[SerializeField]
	protected float loopDelay = 0;

	private bool move = false;
	private int index = 0;
	private int direction = 1;

	const float MAX_DISTANCE_TO_GOAL = 0.1f;

	void Awake()
	{
		if (wayPoints.Length == 0)
		{
			enabled = false;
			UnityEngine.Debug.LogWarning("Moving Platform Waypoints not found. Click for highlight the GameObject.", this);
		}
		else
		{
			foreach (Transform item in wayPoints)
			{
				if (item == null)
				{
					enabled = false;
					UnityEngine.Debug.LogWarning("There are missed Waypoints on the Moving Platform. Click for highlight the GameObject.", this);
				}
			}
		}

		if (initialDelay > 0)
			StartCoroutine(Wait(initialDelay));
		else
			move = true;
	}

	void Update()
	{
		if (!move)
			return;

		if (moveType == MoveType.MoveTowards)
			transform.position = Vector3.MoveTowards(transform.position, wayPoints[index].position, Time.deltaTime * speed);
		else if (moveType == MoveType.Lerp)
			transform.position = Vector3.Lerp(transform.position, wayPoints[index].position, Time.deltaTime * speed);

		var distanceSquared = (transform.position - wayPoints[index].position).sqrMagnitude;

		if (distanceSquared < MAX_DISTANCE_TO_GOAL * MAX_DISTANCE_TO_GOAL)
		{
			if ((direction > 0 && index >= wayPoints.Length - 1) || (direction < 0 && index <= 0))
			{
				switch (pathType)
				{
					case PathType.ForwardAndBackward:
						direction *= -1;
						break;
					case PathType.ClosedCircuit:
						if (direction > 0)
							index = -1;
						else
							index = wayPoints.Length;
						break;
					default:
						if (direction > 0)
							index = 0;
						else
							index = wayPoints.Length - 1;

						transform.position = wayPoints[index].transform.position;
						break;
				}

				if (loopDelay > 0)
				{
					move = false;
					StartCoroutine(Wait(loopDelay));
				}
			}
			index += direction;
		}
	}

	private IEnumerator Wait(float delay)
	{
		yield return new WaitForSeconds(delay);

		move = true;
	}

	public void OnDrawGizmos()
	{
		Gizmos.color = Color.magenta;

		if (wayPoints == null || wayPoints.Length < 2)
			return;

		var pathPoints = wayPoints.Where(t => t != null).ToList();

		if (pathPoints.Count < 2)
			return;

		for (var i = 1; i < pathPoints.Count; i++)
		{
			Gizmos.DrawLine(wayPoints[i - 1].position, wayPoints[i].position);
		}

		if (pathType == PathType.ClosedCircuit)
			Gizmos.DrawLine(wayPoints[wayPoints.Length - 1].position, wayPoints[0].position);

	}
}
