using System.Collections;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject groundsParent;
    [SerializeField] private GameObject ground;

    [Space, Header("Obstacles")]
    [SerializeField] private Sprite[] obstacleSprites;
    [Tooltip("The time range possible between spawned obstacles. <min, max>")]
    [SerializeField] private Vector2 timeBetweenObstacles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground")) 
        {
            var newGround = Instantiate(ground, spawnPoint.position, Quaternion.identity);
            newGround.transform.SetParent(groundsParent.transform);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") || collision.CompareTag("Respawn")) 
        {
            Destroy(collision.gameObject);
        }
    }

    public void Move(Vector3 displacement)
    {
        foreach (var ground in groundsParent.GetComponentsInChildren<Transform>())
        {
            if (ground != groundsParent.transform) ground.localPosition += displacement;
        }
    }

    public void SpawnRandomObstacle()
    {
        var newObstacle = new GameObject("Obstacle");
        newObstacle.transform.position = spawnPoint.position;
        newObstacle.transform.rotation = Quaternion.identity;

        newObstacle.tag = "Respawn";
        newObstacle.transform.SetParent(groundsParent.transform);

        int r = UnityEngine.Random.Range(0, obstacleSprites.Length);
        newObstacle.AddComponent<SpriteRenderer>().sprite = obstacleSprites[r];
        newObstacle.AddComponent<CapsuleCollider2D>();
    }

    public IEnumerator SpawningObstacles(float delay = 0f) 
    {
        yield return new WaitForSeconds(delay);

        while (GameManager.Instance.State == GameState.Playing)
        {
            SpawnRandomObstacle();

            float r = UnityEngine.Random.Range(timeBetweenObstacles.x, timeBetweenObstacles.y);
            yield return new WaitForSeconds(r);
        }
    }
}
