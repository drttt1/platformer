using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    private void Awake()
    {
        if (!player)
            player = FindObjectOfType<PlayerController>().transform;
    }

    private void Update()
    {
        if (player == null) return;

        transform.position = new Vector3(player.position.x, player.position.y, -10f) + offset;
    }
}
