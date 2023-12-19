using UnityEngine;
using UnityEngine.UI;
using System;

public class Plant : MonoBehaviour {
    public GameObject deadPlant;
    public GameObject livingPlant;
    public float waterLevel = 0f;
    public float maxWaterLevel = 3f;
    private float lastWateredTimer = 2f; // Timer for how long the bar stays visible
    public event Action OnWateringBarDeactivationNeeded;
    public Room room; // Assign this in the inspector
    public Slider plantWateringSlider; // Assign in inspector

    private bool isAlive = false;

    void Start() {
        UpdatePlantState();
    }

    void Update() {
        if (lastWateredTimer > 0) {
            lastWateredTimer -= Time.deltaTime;
            if (lastWateredTimer <= 0) {
                OnWateringBarDeactivationNeeded?.Invoke();
            }
        }
    }

    public void WaterPlant(float amount) {
        waterLevel += amount;
        waterLevel = Mathf.Clamp(waterLevel, 0, maxWaterLevel);
        lastWateredTimer = 2f; // Reset the timer

        if (waterLevel >= maxWaterLevel) {
            WaterPlant();
        }

        // Update the completion UI and watering slider
        if (room != null) {
            room.UpdateCompletionUI();
        }
        if (plantWateringSlider != null) {
            plantWateringSlider.gameObject.SetActive(true);
            plantWateringSlider.value = GetWaterLevelPercentage();
        }
    }

    public float GetWaterLevelPercentage() {
        return waterLevel / maxWaterLevel;
    }

    public void WaterPlant() {
        isAlive = true;
        UpdatePlantState();
    }

    private void UpdatePlantState() {
        deadPlant.SetActive(!isAlive);
        livingPlant.SetActive(isAlive);
    }
}
