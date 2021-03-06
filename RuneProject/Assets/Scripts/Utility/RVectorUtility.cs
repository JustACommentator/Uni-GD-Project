using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuneProject.UtilitySystem
{
    public class RVectorUtility
    {
        public static Vector3 ConvertKnockbackToWorldSpace(Vector3 playerPos, Vector3 enemyPos, Vector3 localKnockback)
        {
            playerPos.y = 0f;
            enemyPos.y = 0f;
            Vector3 dir = (enemyPos - playerPos).normalized;
            Vector3 rotatedDir = new Vector3(dir.z, 0f, -dir.x);
            Vector3 result = dir * localKnockback.x + Vector3.up * localKnockback.y + rotatedDir * localKnockback.z;

            return result;
        }
    }
}