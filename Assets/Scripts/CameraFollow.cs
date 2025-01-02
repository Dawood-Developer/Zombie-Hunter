using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Range(0.01f, 0.1f)] [SerializeField] float followDelay;
     Transform objectToFollow;

    [SerializeField] float xOffset;
    [SerializeField] float yOffset;

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (objectToFollow == null)
            return;

        FollowThis(objectToFollow);
    }

    public void FollowThis(Transform otf)
    {
        Vector3 myPos = transform.position;

        Vector3 targetPos = new Vector3(otf.position.x, myPos.y, otf.position.z);

        Vector3 interpolatedPos = Vector3.Lerp(myPos, targetPos, followDelay);

        transform.position = GetClampedPosition(interpolatedPos);
    }

    private Vector3 GetClampedPosition(Vector3 originalPosition)
    {
        float clampedX = Mathf.Clamp(originalPosition.x, -xOffset, xOffset);
        float clampedZ = Mathf.Clamp(originalPosition.z, -yOffset, yOffset);
        return new Vector3(clampedX, originalPosition.y, clampedZ); 
    }

    public void Init()
    {
        objectToFollow = FindFirstObjectByType<PlayerMovement>().transform;
    }
}
