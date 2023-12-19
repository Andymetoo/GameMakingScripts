using UnityEngine;
using UnityEngine.UI;

public class BedInteraction : MonoBehaviour
{
    public GameObject unmadeBed;
    public GameObject madeBed;
    public Slider progressBar; // Assign this in the inspector
    public Camera playerCamera; // Assign player's camera in the inspector
    private float progress = 0f;
    private float makeTime = 3f; // Time to make the bed
    private bool isMakingBed = false; // Flag to track if bed is being made
    private float interactionDistance = 3f; // Maximum distance to interact with the bed
    public Room room; // Assign this in the inspector

    void Start()
    {
        progressBar.gameObject.SetActive(false);
        progressBar.minValue = 0f;
        progressBar.maxValue = makeTime;
        progressBar.value = 0f;
    }

    void Update()
    {
        if (isMakingBed)
        {
            if (progress < makeTime && IsWithinInteractionRange() && IsActionButtonHeld())
            {
                progress += Time.deltaTime;
                progressBar.value = progress;
                //Debug.Log("Make Bed is triggering.");
            }
            else
            {
                StopMaking();
            }

            if (progress >= makeTime)
            {
                MakeBed();
            }
        }
    }
    

    public void StartMaking()
    {
        if (!unmadeBed.activeSelf || isMakingBed)
        {
            return;
        }

        progressBar.gameObject.SetActive(true);
        progress = 0f;
        isMakingBed = true;
    }

    public void StopMaking()
    {
        progressBar.gameObject.SetActive(false);
        progress = 0f;
        isMakingBed = false;
    }

    private void MakeBed()
    {
        unmadeBed.SetActive(false);
        madeBed.SetActive(true);
        progressBar.gameObject.SetActive(false);
        progress = 0f;
        isMakingBed = false;
        room.UpdateCompletionUI(); // Update room completion
    }

    private bool IsWithinInteractionRange()
    {
        return Vector3.Distance(playerCamera.transform.position, transform.position) <= interactionDistance;
    }

    private bool IsActionButtonHeld()
    {
        return Input.GetMouseButton(0) || Input.GetKey(KeyCode.E);
    }
}
