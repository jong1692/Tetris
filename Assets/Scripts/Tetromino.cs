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
    private Vector3 locateOffset;

    [SerializeField]
    private Blocks[] blocks;

    private BlockController blockController;

    void Awake()
    {
        blockController = GameObject.Find("BlockController").GetComponent<BlockController>();

        for (int idx = 0; idx < blocks.Length; idx++)
        {
            Vector3 pos = new Vector3(blocks[idx].positon.x, blocks[idx].positon.y, -0.1f);
            Instantiate(block, pos, Quaternion.identity, transform);
        }

        move(blockController.BlockWaitPoint);
    }

    public void locateWithOffset(Vector3 vector)
    {
        transform.position = vector + locateOffset;
    }

    public void locate(Vector3 vector)
    {
        transform.position = vector;
    }

    public void move(Vector3 vector)
    {
        transform.position += vector;
    }

    public void rotate(Vector3 vector)
    {
        transform.Rotate(vector);
    }


}
