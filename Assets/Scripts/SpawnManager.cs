using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private ListBoidVariable boids;
    [SerializeField] private GameObject boidPrefab;
    [SerializeField] private int boidCount;

    private void Start()
    {
        if (boids.boidMovements.Count > 0)
        {
            boids.boidMovements.Clear();
        }  

        while (boidCount-- > 0)
        {
            float direction = Random.Range(0, 360f);

            Vector3 position = new Vector2(Random.Range(-10f, 10f), Random.Range(-10f, 10f));
            GameObject boid = Instantiate(boidPrefab, position,Quaternion.Euler(Vector3.forward * direction) * boidPrefab.transform.localRotation, transform);

            boids.boidMovements.Add(boid.GetComponent<BoidMovement>());
        }
    }
}
