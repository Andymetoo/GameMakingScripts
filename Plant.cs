using UnityEngine;

public class Plant : MonoBehaviour
{
    private bool isBeingWatered = false;
    private float waterAmount = 0f;
    private const float MAX_WATER = 1f; // Maximum water needed

    void Update()
    {
        if (isBeingWatered)
        {
            WaterPlant();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Water")) // Assuming your water particles have the tag "Water"
        {
            isBeingWatered = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isBeingWatered = false;
        }
    }

    void WaterPlant()
    {
        if (waterAmount < MAX_WATER)
        {
            waterAmount += Time.deltaTime; // Increase water amount over time
            // Update any UI element or visual feedback for watering progress
        }
        else
        {
            // Plant is fully watered - you can trigger any effect or change here
        }
    }
}
