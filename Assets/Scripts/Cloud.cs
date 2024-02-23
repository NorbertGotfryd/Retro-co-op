using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    public float moveSpeed;
    public Vector3 startPosition;
    public Vector3 endPosition;

    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosition, moveSpeed * Time.deltaTime);

        if (transform.position == endPosition)
            transform.position = startPosition;
    }
}
