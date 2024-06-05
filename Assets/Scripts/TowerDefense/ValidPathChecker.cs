using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathChecker : MonoBehaviour {
    [SerializeField] private float movementSpeed = 30f;
    [SerializeField] protected float nextWaypointDistance = 0.5f;
    protected int currentWaypoint = 0;
    private Seeker seeker;
    private Path path;
    private Rigidbody2D rb;
    private bool pathChecked;
    private Vector2 lastPosition;
    private int failCount;
    private float checkInterval = 1f;
    private float checkTimer;

    private void Start() {
        seeker = GetComponent<Seeker>();
        rb = GetComponent<Rigidbody2D>();
        UpdatePath(Player.Instance.transform.position);
        pathChecked = false;
        lastPosition = rb.position;
        checkTimer = checkInterval;
    }

    private void Update() {
        checkTimer -= Time.deltaTime;
        if (checkTimer <= 0f) {
            CheckIfStuck();
            checkTimer = checkInterval;
        }

        FollowPath();
    }

    private void CheckIfStuck() {
        Vector2 currentPosition = rb.position;
        float distanceMoved = Vector2.Distance(currentPosition, lastPosition);

        if (distanceMoved < 0.5f) {
            failCount++;
            if (failCount >= 5 && !pathChecked) {
                pathChecked = true;
                TowerDefenseManager.Instance.UpdateValidPath(false);
                Destroy(gameObject);
            }
        }
        else {
            failCount = 0; // Reset fail count if it has moved
        }

        lastPosition = currentPosition;
    }

    private void FollowPath() {
        if (path == null) return;

        if (currentWaypoint >= path.vectorPath.Count) return;

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - rb.position).normalized;
        rb.velocity = direction * movementSpeed;

        float distanceToWaypoint = Vector2.Distance(rb.position, path.vectorPath[currentWaypoint]);
        if (distanceToWaypoint < nextWaypointDistance) {
            currentWaypoint++;
        }

        // Check if reached the final destination
        if (Vector2.Distance(rb.position, Player.Instance.transform.position)<0.5f) {
            rb.velocity = Vector2.zero;
            if (!pathChecked) {
                pathChecked = true;
                TowerDefenseManager.Instance.UpdateValidPath(true);
                Destroy(gameObject);
            }
        }
    }

    public void UpdatePath(Vector2 targetPosition) {
        if (seeker.IsDone()) {
            seeker.StartPath(transform.position, targetPosition, OnPathComplete);
        }
    }

    public void OnPathComplete(Path p) {
        if (!p.error) {
            // Release the previous path
            if (path != null) path.Release(this);

            path = p;
            path.Claim(this);
            currentWaypoint = 0;
        }
    }
    //private void Start() {
    //    seeker = GetComponent<Seeker>();
    //    rb = GetComponent<Rigidbody2D>();
    //    UpdatePath(Player.Instance.transform.position, transform.position);
    //    pathChecked = false;
    //}



    //private void Update() {
    //    Vector2 targetPosition = Player.Instance.transform.position;

    //    Vector2 aiPosition = transform.position;
    //    float distance = Vector2.Distance(targetPosition, aiPosition);
    //    if(lastDistance == distance) {
    //        failCount++;
    //        if (failCount >= 5 && !pathChecked) {           
    //            pathChecked = true;
    //            TowerDefenseManager.Instance.UpdateValidPath(false);
    //            Destroy(gameObject);
    //        }
    //    }
    //    lastDistance = distance;
    //    if (distance < 0.5f) {
    //        rb.velocity = Vector2.zero;
    //        if (!pathChecked) {
    //            pathChecked = true;
    //            TowerDefenseManager.Instance.UpdateValidPath(true);
    //            Destroy(gameObject);
    //        }
    //    }
    //    FollowPath(targetPosition,aiPosition);
    //}
    //public void FollowPath(Vector2 targetPosition, Vector2 aiPosition) {
    //    if (path == null) {

    //        return;

    //    }
    //    if (currentWaypoint >= path.vectorPath.Count) {

    //        return;

    //    }

    //    Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - aiPosition);

    //    rb.velocity = new Vector2(movmentSpeed, movmentSpeed);



    //    float squaredDistance = Vector2.Dot(direction, direction);
    //    if (squaredDistance < nextWaypointDistance * nextWaypointDistance) {
    //        currentWaypoint++;
    //    }


    //}

    //public void UpdatePath(Vector2 targetPosition, Vector2 aiPosition) {
    //    if (seeker.IsDone()) {

    //        Vector2 adjustedTargetPosition = new Vector2(targetPosition.x, targetPosition.y);

    //        seeker.StartPath(transform.position, adjustedTargetPosition, OnPathComplete);


    //    }
    //}

    //public void OnPathComplete(Path path) {
    //    if (!path.error) {
    //        //Release any previous paths
    //        if (this.path != null) {
    //            // Release the previous path
    //            this.path.Release(this);
    //        }
    //        this.path = path;

    //        path.Claim(this);

    //        currentWaypoint = 0;
    //    }

    //}
}
