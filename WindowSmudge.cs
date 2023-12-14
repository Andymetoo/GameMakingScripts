using UnityEngine;

public class WindowSmudge : MonoBehaviour
{
    private Material smudgeMaterial;
    [SerializeField] private float fadeSpeed = 1f; // Base fade speed
    private float randomizedFadeSpeed; // Actual fade speed after randomization
    private bool isBeingSwept = false;
    private bool isWet = false;
    private GameObject wetEffect; // Declare the wetEffect variable

    public bool IsWet
    {
        get { return isWet; }
    }
    

    void Start()
    {
        // Randomize Y rotation
        //float randomYRotation = Random.Range(0f, 360f);
        //transform.eulerAngles = new Vector3(transform.eulerAngles.x, randomYRotation, transform.eulerAngles.z);

        // Randomize X and Z scale
        float randomScaleFactor = Random.Range(0.5f, 1f);
        transform.localScale = new Vector3(transform.localScale.x * randomScaleFactor, transform.localScale.y, transform.localScale.z * randomScaleFactor);

        // Create a new instance of the material
        smudgeMaterial = new Material(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = smudgeMaterial;

        // Randomize fade speed
        randomizedFadeSpeed = fadeSpeed * Random.Range(0.5f, 1.5f);

        wetEffect = transform.Find("Wet").gameObject;
        if (wetEffect != null)
        {
            wetEffect.SetActive(false); // Initially deactivate the wet effect
        }
    }

    void Update()
    {
        if (Input.GetMouseButton(0) && isBeingSwept && ToolSwitcher.ActiveToolIndex == 2 && isWet)
        {
            FadeSmudge();
        }
    }

    private void FadeSmudge()
    {
        // Fade the main smudge
        Color smudgeColor = smudgeMaterial.color;
        smudgeColor.a -= randomizedFadeSpeed * Time.deltaTime; // Decrease alpha value
        smudgeMaterial.color = smudgeColor;

        // Fade the wet effect
        if (wetEffect != null)
        {
            Renderer wetRenderer = wetEffect.GetComponent<Renderer>();
            if (wetRenderer != null)
            {
                Material wetMaterial = wetRenderer.material;
                Color wetColor = wetMaterial.color;
                wetColor.a -= randomizedFadeSpeed * Time.deltaTime; // Decrease alpha value
                wetMaterial.color = wetColor;
            }
        }

        // Check if the smudge is fully faded
        if (smudgeColor.a <= 0)
        {
            Destroy(gameObject); // Destroy or deactivate the smudge when fully faded
            if (wetEffect != null)
            {
                Destroy(wetEffect); // Optionally, you can destroy the wet effect object as well
            }
        }
    }


        public void MakeWet()
    {
        isWet = true;
        if (wetEffect != null)
        {
            wetEffect.SetActive(true); // Activate the wet effect
        }
        Debug.Log("Smudge is now wet");
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
