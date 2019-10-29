using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BlockController : MonoBehaviour
{
    [SerializeField]
    private Tetromino[] tetrominoes;

    [SerializeField]
    private Vector3 blockWaitPoint;
    private Vector3 blockSpawnPoint;

    private GameManager gameMgr;

    private Tetromino curTetromino;
    private Tetromino nextTetromino;
    private Tetromino previewTetromino;

    private List<List<GameObject>> blockList;

    private float timer = 0;
    private float tetrominoDropDelay = 0.5f;

    private int numColumn;
    private int numRow;

    private int tetrominoIdx;

    public Tetromino CurTetromino
    {
        get { return curTetromino; }
    }

    public Vector3 BlockSpawnPoint
    {
        get { return blockSpawnPoint; }
    }

    public Vector3 BlockWaitPoint
    {
        get { return blockWaitPoint; }
    }

    public float TetrominoDropDelay
    {
        get { return tetrominoDropDelay; }
        set { tetrominoDropDelay = value; }
    }

    private void Start()
    {
        initialize();
    }

    public void reset()
    {
        if (curTetromino != null)
            Destroy(curTetromino.gameObject);

        if (nextTetromino != null)
            Destroy(nextTetromino.gameObject);

        if (previewTetromino != null)
            Destroy(previewTetromino.gameObject);

        tetrominoIdx = Random.Range(0, tetrominoes.Length);
        nextTetromino = Instantiate(tetrominoes[tetrominoIdx]);
    }

    private void initialize()
    {
        gameMgr = GameObject.Find("GameManager").GetComponent<GameManager>();

        tetrominoIdx = Random.Range(0, tetrominoes.Length);
        nextTetromino = Instantiate(tetrominoes[tetrominoIdx]);

        blockList = gameMgr.BlockList;
        numColumn = gameMgr.NumColumn;
        numRow = gameMgr.NumRow;

        blockSpawnPoint = new Vector2((int)((numRow - 1) / 2), numColumn - 2);
    }

    void Update()
    {
        if (gameMgr.Pause) return;

        timer += Time.deltaTime;

        if (timer > tetrominoDropDelay)
        {
            if (curTetromino == null) spawnTetromino();
            else moveTetromino(Vector3.down);

            timer = 0;
        }

        detectInput();
    }

    private void detectInput()
    {
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
        curTetromino = nextTetromino;
        curTetromino.locateWithOffset(BlockSpawnPoint);

        previewTetromino = Instantiate(tetrominoes[tetrominoIdx]);
        previewTetromino.locate(curTetromino.transform.position);

        for (int idx = 0; idx < previewTetromino.transform.childCount; idx++)
        {
            GameObject obj = previewTetromino.transform.GetChild(idx).gameObject;

            Color color = obj.GetComponent<MeshRenderer>().material.color;
            color.a = 0.5f;

            obj.GetComponent<MeshRenderer>().material.color = color;
        }
        dropPrevTetrominoToFloor();

        tetrominoIdx = Random.Range(0, tetrominoes.Length);
        nextTetromino = Instantiate(tetrominoes[tetrominoIdx]);
    }

    private void rotateTetromino()
    {
        curTetromino.rotate(new Vector3(0, 0, 90));

        for (int idx = 0; idx < curTetromino.transform.childCount; idx++)
        {
            Vector3 pos = curTetromino.transform.GetChild(idx).transform.position;

            int idxX = (int)Mathf.Round(pos.x);
            int idxY = (int)Mathf.Round(pos.y);

            if ((idxX > numRow - 2 || idxX < 1) ||
                (idxY > numColumn - 1 || idxY < 1))
            {
                curTetromino.rotate(new Vector3(0, 0, -90));

                return;
            }

            if (blockList[idxY][idxX] != null)
            {
                curTetromino.rotate(new Vector3(0, 0, -90));

                return;
            }
        }

        previewTetromino.rotate(new Vector3(0, 0, 90));

        dropPrevTetrominoToFloor();
    }

    private bool checkCanMove(Tetromino tetromino, Vector3 vector)
    {
        for (int idx = 0; idx < tetromino.transform.childCount; idx++)
        {
            Vector3 pos = tetromino.transform.GetChild(idx).transform.position;

            int idxX = (int)Mathf.Round(pos.x);
            int idxY = (int)Mathf.Round(pos.y);

            if (idxY > numColumn - 1) continue;

            if ((vector == Vector3.left && blockList[idxY][idxX - 1] != null) ||
                (vector == Vector3.right && blockList[idxY][idxX + 1] != null) ||
                (vector == Vector3.down && blockList[idxY - 1][idxX] != null))
                return false;
        }

        return true;
    }

    private bool moveTetromino(Vector3 vector)
    {
        if (!checkCanMove(curTetromino, vector))
        {
            if (vector == Vector3.down)
            {
                gameMgr.addTetromino();

                curTetromino = null;

                Destroy(previewTetromino.gameObject);
            }
            return false;
        }

        curTetromino.move(vector);

        dropPrevTetrominoToFloor();

        return true;
    }

    private void dropPrevTetrominoToFloor()
    {
        previewTetromino.locate(curTetromino.transform.position);
        while (checkCanMove(previewTetromino, Vector3.down))
        {
            previewTetromino.move(Vector3.down);
        }
    }
}
