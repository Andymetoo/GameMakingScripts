using UnityEngine;
using UnityEngine.UI;

public class FurnitureInteraction : MonoBehaviour
{
    public GameObject brokenFurniture;
    public GameObject fixedFurniture;
    public float fixTime = 3f; // Time to fix the furniture
    private bool isFixingFurniture = false; // Flag to track if furniture is being fixed
    private float progress = 0f;
    public Room room; // Assign this in the inspector
    public Slider progressBar; // Assign a progress bar (optional)

    void Start()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
            progressBar.minValue = 0f;
            progressBar.maxValue = fixTime;
            progressBar.value = 0f;
        }
    }

    void Update()
    {
        if (isFixingFurniture)
        {
            if (progress < fixTime && IsActionButtonHeld())
            {
                progress += Time.deltaTime;
                if (progressBar != null)
                {
                    progressBar.value = progress;
                }
            }
            else
            {
                StopFixing();
            }

            if (progress >= fixTime)
            {
                FixFurniture();
            }
        }
    }

    // Public method to start fixing
    public void StartFixing()
    {
        if (isFixingFurniture || fixedFurniture.activeSelf)
        {
            return;
        }

        isFixingFurniture = true;
        progress = 0f;

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(true);
        }
    }

    public void StopFixing()
    {
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
        progress = 0f;
        isFixingFurniture = false;
    }

    public void FixFurniture()
    {
        brokenFurniture.SetActive(false);
        fixedFurniture.SetActive(true);

        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }

        room.UpdateCompletionUI(); // Update room completion
        isFixingFurniture = false;
    }

    private bool IsActionButtonHeld()
    {
        return Input.GetMouseButton(0) || Input.GetKey(KeyCode.E);
    }
}
