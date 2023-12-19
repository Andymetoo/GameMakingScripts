using UnityEngine;
using System.Collections.Generic;
using System.Linq; // Needed for LINQ queries

public class Room : MonoBehaviour {
    public List<Task> smudges;
    public List<Task> dirtPiles;
    public List<Task> plants;
    public List<Task> trash;
    public List<Task> bed;
    // Add more lists for other task types

    // Completion Slider
    public UnityEngine.UI.Slider completionSlider;

    public float difficultyLevel; // Set this in the inspector or via code

    void Start() {
        InitializeTasks();
    }

    void InitializeTasks() {
        Debug.Log($"Initializing tasks for difficulty level: {difficultyLevel}");
        ActivateRandomTasks(smudges, difficultyLevel);
        ActivateRandomTasks(dirtPiles, difficultyLevel);
        ActivateRandomTasks(plants, difficultyLevel);
        ActivateRandomTasks(trash, difficultyLevel);
        ActivateRandomTasks(bed, difficultyLevel);
        // Repeat for other task types
    }

    void ActivateRandomTasks(List<Task> tasks, float difficulty) {
        int tasksToActivate = CalculateNumberOfTasks(difficulty, tasks.Count);
        Debug.Log($"Activating {tasksToActivate} out of {tasks.Count} tasks.");
        HashSet<int> activatedTasks = new HashSet<int>();

        while (activatedTasks.Count < tasksToActivate) {
            int randomIndex = UnityEngine.Random.Range(0, tasks.Count);
            if (!activatedTasks.Contains(randomIndex)) {
                tasks[randomIndex].isActive = true;
                tasks[randomIndex].gameObject.SetActive(true); // Activate the task GameObject
                activatedTasks.Add(randomIndex);
            }
        }
    }

    int CalculateNumberOfTasks(float difficulty, int maxTasks) {
        float activationPercent = Mathf.Clamp(difficulty / 10.0f, 0.05f, 1.0f);
        int calculatedTasks = Mathf.RoundToInt(maxTasks * activationPercent);
        Debug.Log($"Calculated Tasks to activate: {calculatedTasks}, Activation Percent: {activationPercent}");
        return Mathf.Clamp(calculatedTasks, 1, maxTasks); 
    }

    public void UpdateCompletionUI() {
        int totalTasks = smudges.Count + dirtPiles.Count + plants.Count + trash.Count + bed.Count; // Include bed tasks
        int completedTasks = CountCompletedTasks();

        float completionPercentage = (float)completedTasks / totalTasks;

        // Update the UI element
        if (completionSlider != null) {
            completionSlider.value = completionPercentage;
        }
    }

    private int CountCompletedTasks() {
        int completedCount = 0;
        completedCount += smudges.Count(task => !task.isActive);
        completedCount += dirtPiles.Count(task => !task.isActive);
        completedCount += plants.Count(task => !task.isActive);
        completedCount += trash.Count(task => !task.isActive);
        completedCount += bed.Count(task => !task.isActive); // Include bed tasks
        return completedCount;
    }
}
