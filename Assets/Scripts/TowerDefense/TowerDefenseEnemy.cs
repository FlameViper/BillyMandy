using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerDefenseEnemy : Enemy {

    [SerializeField] protected float nextWaypointDistance = 0.5f;
    [SerializeField] protected float updatePathDelay = 0.5f;
    [SerializeField] protected float acceleration = 0.5f;
    [SerializeField] protected int countsAs = 1;
    protected Vector2 _currentVelocity = Vector2.zero;
    protected int currentWaypoint = 0;
    protected float timer;
    protected float baseMovmentSpeed;
    protected float _currentScaleX;
    protected bool isUpdatingPath = false;
    protected Seeker seeker;
    protected Path path;

    protected override void Awake() {
        
    }
    protected override void Start() {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        target = player;
        currentHealth = baseHealth;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        baseColor = spriteRenderer.color;
        baseMovmentSpeed = moveSpeed;
        seeker = GetComponent<Seeker>();
        _currentScaleX = transform.localScale.x;
        UpdatePath(Player.Instance.transform.position,transform.position);
    }
    protected override void Update() {
        if (isFrozen) {
            rb.velocity = Vector2.zero;
            _currentVelocity = Vector2.zero;
           // currentWaypoint = 0;

            return;
        }
        base.Update();
    }
    protected override void Movement() {
       
        if (target != null) {
            DoFrameUpdate();
        }
    }

    public void DoFrameUpdate() {

      
        Vector2 targetPosition = Player.Instance.transform.position;
      
        Vector2 aiPosition = transform.position;

       
        float distance = Vector2.Distance(targetPosition, aiPosition);

        if (distance < stopDistance) {
            rb.velocity = Vector2.zero;
        }

        if (!isUpdatingPath) {
            timer += Time.deltaTime;
            if (timer >= updatePathDelay) {

                UpdatePath(targetPosition, aiPosition);
                isUpdatingPath = true;
                timer = 0f;
            }
        }
        else {
            if (seeker.IsDone()) {
                isUpdatingPath = false;
            }
        }
        if (rb.velocity == Vector2.zero) {
            _currentVelocity = Vector2.zero;

        }
     
        FollowPath(targetPosition, aiPosition);

    }

    public void UpdatePath(Vector2 targetPosition, Vector2 aiPosition) {
        if (seeker.IsDone()) {

            Vector2 adjustedTargetPosition = new Vector2(targetPosition.x,  targetPosition.y);

            seeker.StartPath(transform.position, adjustedTargetPosition, OnPathComplete);


        }
    }
    public void FollowPath(Vector2 targetPosition, Vector2 aiPosition) {
       
        if (path == null) {

            return;

        }
        if (currentWaypoint >= path.vectorPath.Count) {
           
            return;

        }

        Vector2 direction = ((Vector2)path.vectorPath[currentWaypoint] - aiPosition);



        Vector2 force = direction * acceleration;
        // Apply acceleration
        _currentVelocity += force;

        // Limit velocity to the maximum speed
        _currentVelocity = Vector2.ClampMagnitude(_currentVelocity, moveSpeed);


        // Movement
        rb.velocity = new Vector2(_currentVelocity.x, _currentVelocity.y);
       


        float squaredDistance = Vector2.Dot(direction, direction);
        if (squaredDistance < nextWaypointDistance * nextWaypointDistance) {
            currentWaypoint++;
        }

        HandleFlip();


    }
    private void HandleFlip() {
  
        float flipDirectionX = Mathf.Sign(Player.Instance.transform.position.x - transform.position.x);
 
        if (flipDirectionX > 0.05f) {
            FlipCharacter(true);
        }
        else if (flipDirectionX < -0.05f) {
            FlipCharacter(false);
        }
        
    }
    public void FlipCharacter(bool flip) {

        float currentScaleY = transform.localScale.y;
        float currentScaleZ = transform.localScale.z;


        if (flip) {
            _currentScaleX = -Mathf.Abs(_currentScaleX);
        }
        else {
            _currentScaleX = Mathf.Abs(_currentScaleX);
        }
        transform.localScale = new Vector3(_currentScaleX, currentScaleY, currentScaleZ);
    }

    public void OnPathComplete(Path path) {
        if (!path.error) {
            //Release any previous paths
            if (this.path != null) {
                // Release the previous path
                this.path.Release(this);
            }
            this.path = path;

            path.Claim(this);

            currentWaypoint = 0;
        }

    }

    protected override void Die() {
        base.Die();
        TowerDefenseManager.Instance.UpdateEnemyCount(countsAs);
    }

}
