using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [Header("Ground")]
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject groundsParent;
    [SerializeField] private GameObject ground;

    [Space, Header("Obstacles")]
    [SerializeField] private Sprite[] sprites;

    private void Update()
    {
        Vector3 displacement = Vector3.left * Time.deltaTime * 10f;
        foreach (var ground in groundsParent.GetComponentsInChildren<Transform>())
        {
            if (ground != groundsParent.transform) ground.localPosition += displacement;
        }
    }

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
        if (collision.CompareTag("Ground")) 
        {
            Destroy(collision.gameObject);
        }
    }
}
