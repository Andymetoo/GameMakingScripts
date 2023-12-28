using UnityEngine;

public enum TaskType { Smudge, DirtPile, Plant, Trash, Bed /*, other types */ }

public class Task : MonoBehaviour {
    public TaskType taskType;
    public bool isActive = false;

    // For Bed tasks, assign these in the inspector
    public GameObject[] unmadeModels; // Array for multiple unmade bed models
    public GameObject madeModel; // Single made bed model

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
        // Plant appearance logic...
    }

    void SetBedAppearance(bool needsMaking) {
        if (unmadeModels != null && madeModel != null) {
            // Deactivate all unmade models first
            foreach (var unmadeModel in unmadeModels) {
                unmadeModel.SetActive(false);
            }

            // Activate one random unmade model or the made model
            if (needsMaking) {
                int randomIndex = Random.Range(0, unmadeModels.Length);
                unmadeModels[randomIndex].SetActive(true);
            } else {
                madeModel.SetActive(true);
            }
        }
    }
}
