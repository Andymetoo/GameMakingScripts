using UnityEngine;

public class DirtSmudge : MonoBehaviour
{
    private Material smudgeMaterial;
    [SerializeField] private float fadeSpeed = 1f; // Base fade speed
    [SerializeField] private float shrinkSpeed = 0.1f; // Adjust the speed of shrinking

    private float randomizedFadeSpeed; // Actual fade speed after randomization
    private bool isBeingSwept = false;
    private Task taskComponent; // Reference to the Task component

    void Start()
    {
        // Randomize Y rotation
        float randomYRotation = Random.Range(0f, 360f);
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, randomYRotation, transform.eulerAngles.z);

        // Randomize X and Z scale
        float randomScaleFactor = Random.Range(0.5f, 1f);
        transform.localScale = new Vector3(transform.localScale.x * randomScaleFactor, transform.localScale.y, transform.localScale.z * randomScaleFactor);

        // Create a new instance of the material
        smudgeMaterial = new Material(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = smudgeMaterial;

        // Randomize fade speed
        randomizedFadeSpeed = fadeSpeed * Random.Range(0.5f, 1.5f);
        taskComponent = GetComponentInParent<Task>(); // Adjust based on your hierarchy

    }

    void Update()
    {
        if (IsActionButtonHeld() && isBeingSwept && ToolSwitcher.ActiveToolIndex == 0)
        {
            FadeSmudge();
        }
    }

    private bool IsActionButtonHeld()
    {
        return Input.GetMouseButton(0) || Input.GetKey(KeyCode.E);
    }

    private void FadeSmudge()
    {
        transform.localScale -= new Vector3(shrinkSpeed, 0, shrinkSpeed) * Time.deltaTime;

        // Check if the smudge has shrunk enough to be considered gone
        if (transform.localScale.x <= 0.05f && transform.localScale.z <= 0.05f)
        {
            if (taskComponent != null)
            {
                taskComponent.isActive = false; // Update task status

                Room room = taskComponent.GetComponentInParent<Room>(); // Get the Room component
                if (room != null)
                {
                    room.UpdateCompletionUI(); // Update room completion
                }
            }

            Destroy(gameObject);
        }
    }

    public void StartSweeping()
    {
        isBeingSwept = true;
    }

    public void StopSweeping()
    {
        isBeingSwept = false;
    }
}