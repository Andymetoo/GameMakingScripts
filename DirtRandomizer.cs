using UnityEngine;

public class DirtRandomizer : MonoBehaviour
{
    [SerializeField] private Material[] dirtMaterials; // Array to store your different dirt materials

    void Start()
    {
        // Randomly select a material from the array
        Material selectedMaterial = dirtMaterials[Random.Range(0, dirtMaterials.Length)];
        
        // Apply the selected material to the Renderer component
        GetComponent<Renderer>().material = selectedMaterial;
    }
}