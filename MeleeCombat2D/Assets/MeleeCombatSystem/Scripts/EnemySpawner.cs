using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            Enemy.Create(playerTransform.position + GetRandomDir() * Random.Range(50f, 100f));
        }
    }

    private Vector3 GetRandomDir()
    {
        return new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

}
