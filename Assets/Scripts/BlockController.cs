using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private GameObject mapBlock;
    [SerializeField]
    private GameObject edgeBlock;

    [SerializeField]
    private Tetromino[] tetrominoes;


    private List<List<GameObject>> blockList;

    private const int numColumn = 24;
    private const int numRow = 12;

    [SerializeField]
    private Vector3 blockWaitPoint;
    private Vector3 blockSpawnPoint;

    private Tetromino curTetromino;
    private Tetromino nextTetromino;

    private int curIdx;

    private float timer;
    private const float tetrominoDropDelay = 0.5f;

    public Vector3 BlockSpawnPoint
    {
        get { return blockSpawnPoint; }
    }

    public Vector3 BlockWaitPoint
    {
        get { return blockWaitPoint; }
    }

    public int NumColumn
    {
        get { return numColumn; }
    }

    public int NumRow
    {
        get { return numRow; }
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
                    gameObject = Instantiate(edgeBlock, new Vector3(idxRow, idxCol, -0.1f), Quaternion.identity);
                }
                else
                {
                    Instantiate(mapBlock, new Vector3(idxRow, idxCol, 0), Quaternion.identity);
                }

                blockList[idxCol].Add(gameObject);
            }
        }

        blockSpawnPoint = new Vector2((int)((numRow - 1) / 2), numColumn - 2);
    }

    private void Start()
    {
        int max = tetrominoes.Length;
        int idx = Random.Range(0, max);

        if (nextTetromino == null) nextTetromino = Instantiate(tetrominoes[idx]);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer > tetrominoDropDelay)
        {
            if (curTetromino == null) spawnTetromino();
            else moveTetromino(Vector3.down);

            timer = 0;
        }

        if (curTetromino == null) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveTetromino(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveTetromino(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            moveTetromino(Vector3.down);

            timer = 0;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            rotateTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            bool result;
            do
            {
                result = moveTetromino(Vector3.down);
            } while (result);
        }
    }

    private void spawnTetromino()
    {
        int max = tetrominoes.Length;
        int idx = Random.Range(0, max);

        curTetromino = nextTetromino;
        curTetromino.locate(BlockSpawnPoint);

        nextTetromino = Instantiate(tetrominoes[idx]);
    }

    private void rotateTetromino()
    {
        if (!curTetromino.CanRotate) return;

        curTetromino.transform.Rotate(new Vector3(0, 0, 90));
    }

    private bool checkCanMove(Vector3 vector)
    {
        for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
        {
            Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

            int idxX = (int)Mathf.Round(pos.x);
            int idxY = (int)Mathf.Round(pos.y);

            if (idxY > numColumn - 1) continue;

            if ((vector == Vector3.left && blockList[idxY][idxX - 1] != null) ||
                (vector == Vector3.right && blockList[idxY][idxX + 1] != null) ||
                (vector == Vector3.down && blockList[idxY - 1][idxX] != null))
            {
                return false;
            }
        }

        return true;
    }

    private bool moveTetromino(Vector3 vector)
    {
        if (!checkCanMove(vector))
        {
            if (vector == Vector3.down)
            {
                addTetromino();

                while (checkCompleteLine())
                {
                    clearCompleteLine();
                    dropLines();
                }

                curTetromino = null;
            }

            return false;
        }

        curTetromino.move(vector);

        return true;
    }

    private void addTetromino()
    {
        for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
        {
            Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

            int idxX = (int)Mathf.Round(pos.x);
            int idxY = (int)Mathf.Round(pos.y);

            blockList[idxY][idxX] = curTetromino.transform.GetChild(idx).gameObject;
        }
    }

    private void dropLines()
    {
        for (int idx = curIdx; idx < blockList.Count - 1; idx++)
        {
            if (blockList[idx + 1].TrueForAll(x => x == null)) return;

            for (int idxRow = 1; idxRow < numRow - 1; idxRow++)
            {
                blockList[idx][idxRow] = blockList[idx + 1][idxRow];

                if (blockList[idx][idxRow] == null) continue;

                blockList[idx][idxRow].transform.position += Vector3.down;
            }
        }
    }

    private void clearCompleteLine()
    {
        for (int idx = 1; idx < blockList[curIdx].Count - 1; idx++)
        {
            GameObject obj = blockList[curIdx][idx];

            blockList[curIdx][idx] = null;

            Destroy(obj);
        }
    }

    private bool checkCompleteLine()
    {
        for (int idx = curIdx; idx < blockList.Count - 1; idx++)
        {
            if (!blockList[idx].Exists(x => x == null))
            {
                curIdx = idx;

                return true;
            }
        }

        curIdx = 1;

        return false;
    }


}
