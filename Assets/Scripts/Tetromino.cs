using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Tetromino : MonoBehaviour
{
    [Serializable]
    private struct BlockIndex
    {
        public int[] rowIndex;
    }

    [SerializeField]
    private GameObject block;

    [SerializeField]
    private float blockSize = 1.0f;

    [SerializeField]
    BlockIndex[] blockIndex;

    BlockController blockController;

    void Start()
    {
        blockController = GameObject.Find("BlockController").GetComponent<BlockController>();

        for (int idxCol = 0; idxCol < blockIndex.Length; idxCol++)
        {
            for (int idxRow = 0; idxRow < blockIndex.Length; idxRow++)
            {
                if (blockIndex[idxCol].rowIndex[idxRow] == 1)
                {
                    Vector3 pos = new Vector3(idxRow * blockSize, -idxCol * blockSize, -0.1f);
                    Instantiate(block, pos, Quaternion.identity, transform);
                }
            }
        }

        locateSpawnPostion();
    }

    private void locateSpawnPostion()
    {
        transform.position = transform.position + blockController.BlockSpawnPoint;
    }

    public void move(Vector3 vector)
    {
        transform.position += vector;
    }


}
