using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RPlayerInventory : MonoBehaviour
{
    [SerializeField] private int initialCrystals = 0;
    
    private int currentCrystals = 0;

    private void Start()
    {
        currentCrystals = initialCrystals;
    }

    private void RemoveCrystals(int amount) {
        currentCrystals = currentCrystals - Mathf.Clamp(amount, -currentCrystals, 0);
    }

    private void AddCrystals(int amount) {
        currentCrystals += amount;
    }
}
