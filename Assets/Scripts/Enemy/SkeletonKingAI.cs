using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class SkeletonKingAI : BossAI
{
    public enum BossState
    {
        Attacking,
        Dead
    }

    public enum AttackType
    {
        Melee,
        Ranged
    }

    [SerializeField] private GameObject skeletonPrefab;
    public BossState currentState = BossState.Attacking;
    public AttackType currentAttackType = AttackType.Melee;
    public float rangedAttackTimer = 5f;
    public float rangedAttackCooldown = 5f;
    public float throwForce = 10f;

    private List<GameObject> skeletons = new List<GameObject>();
    private GameObject parentObject;

    protected override void Start()
    {
        base.Start();

        parentObject = new GameObject("Thrown Skeletons");

        skeletons = new List<GameObject>();
    }

    protected override void Update()
    {
        base.Update();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (rangedAttackTimer > 0f)
        {
            rangedAttackTimer -= Time.fixedDeltaTime;
        }

        if (currentState != BossState.Attacking && IsPlayerTooClose())
        {
            BackUp();
        } 
        else if (currentState != BossState.Attacking && !IsPlayerTooClose())
        {
            RotateToTarget();
        } 
        else if (currentState == BossState.Attacking)
        {
            if (currentAttackType == AttackType.Melee)
            {
                // Implement your logic for melee attack here
                RotateToTarget();

                speed *= 1.25f; // Increase the speed of the boss
            }
            else if (currentAttackType == AttackType.Ranged)
            {
                // Implement your logic for ranged attack here
                FaceTarget();

                RangedAttack();
            }
        }

        DetermineAttackType();
    }

    private void RangedAttack()
    {
        if (rangedAttackTimer <= 0f) // Check if cooldown is over
        {
            // Instantiate a new skeleton GameObject
            GameObject skeleton = Instantiate(skeletonPrefab, transform.position, Quaternion.identity);
            parentObject.transform.position = transform.position;
            skeleton.transform.parent = parentObject.transform;
            skeletons.Add(skeleton);

            // Calculate the direction towards the player
            Vector2 direction = (target.position - transform.position).normalized;

            // Apply force to the skeleton to throw it towards the player
            Rigidbody2D skeletonRigidbody = skeleton.GetComponent<Rigidbody2D>();
            skeletonRigidbody.AddForce(direction * throwForce, ForceMode2D.Impulse);

            rangedAttackTimer = rangedAttackCooldown; // Start cooldown
        }
    }

    private void DetermineAttackType()
    {
        if (health <= 50)
        {
            currentAttackType = AttackType.Ranged;
        } 
        else
        {
            currentAttackType = AttackType.Melee;
        }
        
        if (IsPlayerTooClose())
        {
            currentAttackType = AttackType.Melee;
        }
        else
        {
            currentAttackType = AttackType.Ranged;
        }
    }

    private void FaceTarget() 
    {
        Vector2 targetDir = target.position - transform.position;
        float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg - 90f;
        Quaternion q = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = q; // Only update the rotation without interpolation
    }

    private bool IsPlayerTooClose()
    {
        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        float desiredDistance = 5f; // Set your desired distance here

        return distanceToPlayer < desiredDistance;
    }

    private void BackUp()
    {
        // Implement your logic for backing up here
        // For example, you can move the boss backwards
        transform.Translate(Vector3.back * Time.deltaTime * speed);
    }
}
