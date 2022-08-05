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

        public static float GetFlatDistance(Vector3 x, Vector3 y)
        {
            x.y = 0f;
            y.y = 0f;

            return Vector3.Distance(x, y);
        }

        public static Quaternion GetFlatLookAt(Transform origin, Vector3 target)
        {
            target.y = origin.position.y;
            Vector3 dir = (target - origin.position).normalized;
            return Quaternion.LookRotation(dir);
        }
    }
}