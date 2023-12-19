using UnityEngine;

public enum TaskType { Smudge, DirtPile, Plant, Trash, Bed /*, other types */ }

public class Task : MonoBehaviour {
    public TaskType taskType;
    public bool isActive = false;

    // For Plant and Bed tasks, assign these in the inspector
    public GameObject activeModel; // E.g., wilted plant, disheveled bed
    public GameObject inactiveModel; // E.g., healthy plant, made bed

    void Start() {
        Debug.Log($"Initializing task: {taskType}, isActive: {isActive}");
        SetTaskState(isActive);
    }

    public void SetTaskState(bool active) {
        Debug.Log($"Setting task state for {taskType}, isActive: {active}");
        isActive = active;
        switch (taskType) {
            case TaskType.Plant:
                SetPlantAppearance(active);
                break;
            case TaskType.Bed:
                SetBedAppearance(active);
                break;
            // Add cases for other task types that require a visual change
            default:
                gameObject.SetActive(active); // For tasks like smudges, dirt piles, trash
                break;
        }
    }

    void SetPlantAppearance(bool needsWatering) {
        if (activeModel != null && inactiveModel != null) {
            activeModel.SetActive(needsWatering);
            inactiveModel.SetActive(!needsWatering);
        }
    }

    void SetBedAppearance(bool needsMaking) {
        if (activeModel != null && inactiveModel != null) {
            activeModel.SetActive(needsMaking);
            inactiveModel.SetActive(!needsMaking);
        }
    }
}
