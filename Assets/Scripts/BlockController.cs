using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private GameObject mapBlock;
    [SerializeField]
    private GameObject edgeBlock;

    [SerializeField]
    private Tetromino[] tetrominoes;


    private List<List<GameObject>> blockList;

    private const int numColumn = 21;
    private const int numRow = 12;

    private Vector3 blockSpawnPoint;
    private Tetromino curTetromino;

    private int curIdx;

    private float timer;
    private const float tetrominoDropDelay = 0.1f;

    public Vector3 BlockSpawnPoint
    {
        get { return blockSpawnPoint; }
    }

    void Awake()
    {
        curIdx = 1;
        timer = 0;

        blockList = new List<List<GameObject>>(numColumn);

        for (int idxCol = 0; idxCol < numColumn; idxCol++)
        {
            blockList.Add(new List<GameObject>());

            for (int idxRow = 0; idxRow < numRow; idxRow++)
            {
                GameObject gameObject = null;

                if ((idxCol == 0) || (idxRow == 0) || (idxRow == numRow - 1))
                {
                    gameObject = Instantiate(edgeBlock, new Vector3(idxRow, idxCol, 0), Quaternion.identity);
                }
                else
                {
                    Instantiate(mapBlock, new Vector3(idxRow, idxCol, 0), Quaternion.identity);
                }

                blockList[idxCol].Add(gameObject);
            }
        }

        blockSpawnPoint = new Vector2((int)((numRow - 1) / 2), numColumn + 1);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveTetromino(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveTetromino(Vector3.right);
        }


        timer += Time.deltaTime;

        if (timer > tetrominoDropDelay)
        {
            if (curTetromino == null) spawnTetromino();
            else moveTetromino(Vector3.down);

            timer = 0;
        }
    }

    private void spawnTetromino()
    {
        int max = tetrominoes.Length;
        int idx = Random.Range(0, max);

        curTetromino = Instantiate(tetrominoes[idx]);
    }

    private void moveTetromino(Vector3 vector)
    {
        if (vector == Vector3.left)
        {
            for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
            {
                Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

                int idxX = (int)pos.x;
                int idxY = (int)pos.y;

                if (blockList[idxY][idxX - 1] != null)
                {
                    return;
                }
            }
        }
        else if (vector == Vector3.right)
        {
            for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
            {
                Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

                int idxX = (int)pos.x;
                int idxY = (int)pos.y;

                if (blockList[idxY][idxX + 1] != null)
                {
                    return;
                }
            }
        }

        if (vector == Vector3.down)
        {
            for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
            {
                Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

                int idxX = (int)pos.x;
                int idxY = (int)pos.y;

                if (idxY > numColumn - 1) continue;

                if (blockList[idxY - 1][idxX] != null)
                {
                    addTetromino();

                    while (checkCompleteLine())
                    {
                        clearCompleteLine();
                    }

                    curTetromino = null;

                    return;
                }
            }
        }

        curTetromino.move(vector);
    }

    private void addTetromino()
    {
        for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
        {
            Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

            int idxX = (int)pos.x;
            int idxY = (int)pos.y;

            blockList[idxY][idxX] = curTetromino.transform.GetChild(idx).gameObject;
        }
    }

    private void clearCompleteLine()
    {
        blockList[curIdx].RemoveRange(1, numRow - 2);
    }

    private bool checkCompleteLine()
    {
        for (int idx = curIdx; idx < blockList.Count; idx++)
        {
            if (!blockList[idx].Exists(x => x == null))
            {
                curIdx = idx;

                return true;
            }
        }

        return false;
    }


}
