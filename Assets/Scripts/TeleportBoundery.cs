using UnityEngine;

public class TeleportBoundery : MonoBehaviour
{
    [SerializeField] private Boundery boundery;

    private void FixedUpdate()
    {
        if (Mathf.Abs(transform.position.x) > boundery.XLimit)
        {
            transform.position = new Vector3(boundery.XLimit * (transform.position.x > 0 ? -1 : 1), transform.position.y, transform.position.z);
        } 

        if (Mathf.Abs(transform.position.y) > boundery.YLimit) 
        {
            transform.position = new Vector3(transform.position.x, boundery.YLimit * (transform.position.y > 0 ? -1 : 1), transform.position.z);
        }
    }
}
