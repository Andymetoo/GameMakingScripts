using UnityEngine;

public class DirtSmudge : MonoBehaviour
{
    private Material smudgeMaterial;
    [SerializeField] private float fadeSpeed = 1f; // Base fade speed
    private float randomizedFadeSpeed; // Actual fade speed after randomization
    private bool isBeingSwept = false;

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
        Color color = smudgeMaterial.color;
        color.a -= randomizedFadeSpeed * Time.deltaTime; // Decrease alpha value
        smudgeMaterial.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject); // Destroy or deactivate the smudge when fully faded
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
