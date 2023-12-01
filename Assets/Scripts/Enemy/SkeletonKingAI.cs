using System.Collections.Generic;
using UnityEngine;

public class SkeletonKingAI : BossAI
{
    [SerializeField] private GameObject skeletonPrefab; // Skeleton prefab to be thrown
    [SerializeField] private GameObject boundary; // Reference to the boundary GameObject

    private List<GameObject> skeletons; // List of skeletons thrown by the Skeleton King
    private GameObject parentObject; // Parent GameObject for the skeletons
    private int throwForce = 100; // Force applied to the skeleton when thrown

    public float rangedAttackCooldown = 3f; // Cooldown between ranged attacks
    private float rangedAttackTimer = 3f; // Timer for ranged attacks

    public float meleeSpeedBuff = 1.2f; // Marginal speed buff in melee mode
    public float lowHealthSpeedBuff = 1.75f; // Bigger speed buff when below 15% health
    public float attackRange = 10f; // Range for ranged attacks
    public float meleeRange = 5f; // Range for switching to melee mode
    private float originalSpeed; // Store the original speed for later use

    //private bool isMeleeMode = false;

    public enum BossState
    {
        Ranged,
        Melee
    }

    public BossState bossState;

    protected override void Start()
    {
        base.Start();
        
        originalSpeed = speed;

        parentObject = new GameObject("Thrown Skeletons"); // Create a new GameObject to parent the skeletons
        skeletons = new List<GameObject>(); // Initialize the list of skeletons
    }

    protected override void Update()
    {
        base.Update();
        switch(bossState)
        {
            case BossState.Ranged:
                if(CanSeePlayer())
                {
                    RangedMode();
                    if(Vector2.Distance(transform.position, target.position) < meleeRange)
                    {
                        bossState = BossState.Melee;
                    }
                }
                rangedAttackTimer -= Time.deltaTime;
                break;
            case BossState.Melee:
                if(CanSeePlayer())
                {
                    MeleeMode();
                    RangedMode();
                    if(Vector2.Distance(transform.position, target.position) > meleeRange)
                    {
                        bossState = BossState.Ranged;
                        rb.velocity = Vector2.zero;
                    }
                }
                rangedAttackTimer -= Time.deltaTime;
                break;
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
/*
    private bool IsPlayerInBox()
    {
        // Check if the player is within the specified box
        float boxSize = 15f;

        // Use the boundary GameObject to check if the boss is within the specified boundary
        Collider2D boundaryCollider = boundary.GetComponent<Collider2D>();
        bool isInBoundary = Physics2D.OverlapBox(transform.position, new Vector2(boxSize, boxSize), 0f, 1 << boundaryCollider.gameObject.layer);

        return isInBoundary &&
               Mathf.Abs(transform.position.x - target.position.x) < boxSize * 0.5f &&
               Mathf.Abs(transform.position.y - target.position.y) < boxSize * 0.5f;
    }
    */

    private void RangedMode()
    {
        // Mirror the player's movements
        //Vector2 mirroredDirection = -(target.position - transform.position).normalized;
        //MoveTowardsTarget(mirroredDirection);

        // Check for ranged attack conditions and throw a skeleton if necessary
        if (rangedAttackTimer <= 0f)
        {
            ThrowSkeleton();

            rangedAttackTimer = rangedAttackCooldown; // Reset the ranged attack timer
        }

        // Check if the Skeleton King is cornered and switch to melee mode
        // CheckForCornered();
    }

    private void MeleeMode()
    {
        // Move towards the player in melee mode
        MoveTowardsTarget(target.position - transform.position);
    }

    /*
    private void CheckForCornered()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(2f, 2f), 0f);
        foreach (Collider2D collider in colliders)
        {
            if (collider.CompareTag("Wall") && (collider.CompareTag("Player") && Vector2.Distance(transform.position, target.position) < 6f))
            {
                isMeleeMode = true;
                return;
            }
        }
    }
    */

    private void ThrowSkeleton()
    {
        // Instantiate a new skeleton GameObject
        GameObject skeleton = Instantiate(skeletonPrefab, transform.position /*+ new Vector3 (2, 2, 0)*/, Quaternion.identity);
        parentObject.transform.position = transform.position;
        skeleton.transform.parent = parentObject.transform;
        skeletons.Add(skeleton);

        // Calculate the direction towards the player
        Vector2 direction = (target.position - transform.position).normalized;

        // Apply force to the skeleton to throw it towards the player
        Rigidbody2D skeletonRigidbody = skeleton.GetComponent<Rigidbody2D>();
        Collider2D skeletonCollider = skeleton.GetComponent<Collider2D>();
        Physics2D.IgnoreCollision(skeletonCollider, GetComponent<Collider2D>());
        skeletonRigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);
        
    }
}
