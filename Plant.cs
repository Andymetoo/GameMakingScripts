using UnityEngine;
using System;

public class Plant : MonoBehaviour
{
    public GameObject deadPlant;
    public GameObject livingPlant;
    public float waterLevel = 0f;
    public float maxWaterLevel = 3f;
    private float lastWateredTimer = 2f; // Timer for how long the bar stays visible
    public event Action OnWateringBarDeactivationNeeded;
    public Room room; // Assign this in the inspector

    private bool isAlive = false;

    void Start()
    {
        // Initialize plant state
        UpdatePlantState();
    }

    void Update()
    {
        if (lastWateredTimer > 0)
        {
            lastWateredTimer -= Time.deltaTime;
            if (lastWateredTimer <= 0)
            {
                OnWateringBarDeactivationNeeded?.Invoke(); // Trigger the event
            }
        }
    }

    public void WaterPlant(float amount)
    {
        waterLevel += amount;
        waterLevel = Mathf.Clamp(waterLevel, 0, maxWaterLevel);
        lastWateredTimer = 2f; // Reset the timer

        if (waterLevel >= maxWaterLevel)
        {
            // Logic to switch to the living plant model
            Debug.Log("Plant is fully watered");
            WaterPlant();
        }

        room.UpdateCompletionUI(); // Update room completion when plant is watered
    }

    public float GetWaterLevelPercentage()
    {
        return waterLevel / maxWaterLevel;
    }

    public void WaterPlant()
    {
        isAlive = true;
        UpdatePlantState();
    }

    private void UpdatePlantState()
    {
        deadPlant.SetActive(!isAlive);
        livingPlant.SetActive(isAlive);
    }
}
