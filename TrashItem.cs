using UnityEngine;

public class TrashItem : MonoBehaviour
{
    // Add any properties you need, like points value, type of trash, etc.
    [SerializeField] private int pointsValue = 10;

    // This method could be called when the trash is successfully disposed of.
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