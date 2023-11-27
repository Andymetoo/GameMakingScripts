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
        Color color = smudgeMaterial.color;
        color.a -= randomizedFadeSpeed * Time.deltaTime; // Decrease alpha value
        smudgeMaterial.color = color;

        if (color.a <= 0)
        {
            Destroy(gameObject); // Destroy or deactivate the smudge when fully faded
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
