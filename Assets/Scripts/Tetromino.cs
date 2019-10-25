using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tetromino : MonoBehaviour
{
    [Serializable]
    private struct Blocks
    {
        public Vector2 positon;
    }

    [SerializeField]
    private GameObject block;

    [SerializeField]
    private float blockSize = 1.0f;

    [SerializeField]
    private bool canRotate = true;

    [SerializeField]
    private Blocks[] blocks;

    private BlockController blockController;

    public bool CanRotate
    {
        get { return canRotate; }
    }

    void Start()
    {
        blockController = GameObject.Find("BlockController").GetComponent<BlockController>();

        for (int idx = 0; idx < blocks.Length; idx++)
        {
            Vector3 pos = new Vector3(blocks[idx].positon.x * blockSize, blocks[idx].positon.y * blockSize, -0.1f);
            Instantiate(block, pos, Quaternion.identity, transform);
        }

        move(blockController.BlockWaitPoint);
    }

    public void locate(Vector3 vector)
    {
        transform.position = vector * blockSize;
    }

    public void move(Vector3 vector)
    {
        transform.position += vector * blockSize;
    }


}
