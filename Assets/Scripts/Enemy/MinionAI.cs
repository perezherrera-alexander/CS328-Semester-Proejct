using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinionAI : EnemyAI
{
    protected override void RotateToTarget()
    {
        if (target.CompareTag("Player"))
        {
            base.RotateToTarget();
        }
    }
}
