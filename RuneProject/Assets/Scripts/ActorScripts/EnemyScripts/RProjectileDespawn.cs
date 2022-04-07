using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RProjectileDespawn : MonoBehaviour
{
    [SerializeField] private bool activated = true;
    [SerializeField] private float despawnTime = 3;

    void Update()
    {
        if (activated)
        {
            despawnTime -= Time.deltaTime;

            if (despawnTime < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
