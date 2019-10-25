using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField]
    private GameObject mapBlock;
    [SerializeField]
    private GameObject edgeBlock;

    BlockController blockController;

    [SerializeField]
    private float blockSize = 1.0f;

    void Start()
    {
        blockController = GameObject.Find("BlockController").GetComponent<BlockController>();

        int numColumn = blockController.NumColumn;
        int numRow = (int)(blockController.NumRow / 2);

        for (int idxCol = 0; idxCol < numColumn; idxCol++)
        {
            for (int idxRow = 0; idxRow < numRow; idxRow++)
            {
                GameObject block = (idxCol == 0 || idxCol == numColumn - 1 || idxCol == 18) 
                    || (idxRow == numRow - 1) ? edgeBlock : mapBlock;

                Vector3 pos = new Vector3((blockController.NumRow + idxRow) * blockSize, idxCol * blockSize, 0f);
                Instantiate(block, pos, Quaternion.identity);
            }
        }
    }

    void Update()
    {

    }
}
