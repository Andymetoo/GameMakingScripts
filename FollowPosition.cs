using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform target;

    void Update()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}

