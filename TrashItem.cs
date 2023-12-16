using UnityEngine;

public class TrashItem : MonoBehaviour
{
    [SerializeField] private int pointsValue = 10;

    void Start()
    {
        // Randomize rotation
        float randomYRotation = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);

        // Randomize scale
        Vector3 currentScale = transform.localScale;
        transform.localScale = new Vector3(
            currentScale.x * Random.Range(1f, 1.2f), // Randomize X scale
            currentScale.y * Random.Range(1f, 1.2f), // Randomize Y scale
            currentScale.z * Random.Range(1f, 1.2f)  // Randomize Z scale
        );
    }

    public void Dispose()
    {
        // Add points to the player's score
        //ScoreManager.AddPoints(pointsValue);

        // Play any disposal effects (sound, animation, etc.)
        PlayDisposalEffects();

        // Destroy or deactivate the trash object
        Destroy(gameObject);
    }

    private void PlayDisposalEffects()
    {
        // Implement any visual or audio effects for disposing of the trash
    }
}
