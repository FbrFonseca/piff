using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NPCMovement : MonoBehaviour
{
    public Transform target;
    public float speed = 2.0f;
    public float wanderRadius = 10.0f;
    public float waitTimeAtPoint = 2.0f;

    private Rigidbody2D rb;
    private Pathfinding pathfinding;
    private List<Node> path;
    private int targetIndex = 0;
    private float waitTimer = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        pathfinding = FindFirstObjectByType<Pathfinding>();
        
        if (target == null)
        {
            InvokeRepeating(nameof(ChooseRandomDestination), 0f, 3f);
        }
        else
        {
            InvokeRepeating(nameof(UpdatePath), 0f, 0.5f);
        }
    }

    void ChooseRandomDestination()
    {
        if (target != null)
        {
            return;
        }

        Vector2 randomDirection = Random.insideUnitCircle * wanderRadius;
        Vector3 randomPosition = transform.position + new Vector3(randomDirection.x, randomDirection.y, 0);

        if (pathfinding.IsWalkable(randomPosition))
        {
            path = pathfinding.FindPath(transform.position, randomPosition);
            targetIndex = 0;
        }
    }

    void UpdatePath()
    {
        if (target != null)
        {
            path = pathfinding.FindPath(transform.position, target.position);
            targetIndex = 0;
        }
    }

    void FixedUpdate()
    {
        if (path != null && targetIndex < path.Count)
        {
            Vector3 nextPosition = path[targetIndex].worldPosition;
            //Vector2 moveDirection = (nextPosition - transform.position).normalized;
            //rb.linearVelocity = moveDirection * speed;

            Vector2 newPosition = Vector2.MoveTowards(rb.position, nextPosition, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPosition);


            if (Vector2.Distance(transform.position, nextPosition) < 0.1f)
            {
                targetIndex++;
            }

        }
        else
        {
            rb.linearVelocity = Vector2.zero;

            if (target == null && waitTimer <= 0)
            {
                waitTimer = waitTimeAtPoint;
            }
            else if (waitTimer > 0)
            {
                waitTimer -= Time.fixedDeltaTime;
            }
        }
    }
}