using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private GameObject mapBlock;
    [SerializeField]
    private GameObject edgeBlock;
    [SerializeField]
    private float blockFadeSpeed = 5.0f;
    [SerializeField]
    private float sceneFadeSpeed = 3.0f;

    private List<List<GameObject>> blockList;

    private float blockSize = 1.0f;

    [SerializeField]
    private Text linesLeftText;
    private int linesLeft;

    [SerializeField]
    private Text scoreText;
    private int score;

    [SerializeField]
    private Text comboText;
    private int combo;

    [SerializeField]
    private Image sceneFader;

    private int stage = 1;
    private bool pause;

    private Transform maps;
    private Transform blocks;

    private BlockController blockController;

    private const int numColumn = 24;
    private const int numRow = 12;
    private const int defaultLinesLeft = 0;
    private const int scorePoint = 100;

    private int curLineIdx;

    public List<List<GameObject>> BlockList
    {
        get { return blockList; }
    }

    public int NumColumn
    {
        get { return numColumn; }
    }

    public int NumRow
    {
        get { return numRow; }
    }

    public bool Pause
    {
        get { return pause; }
    }

    void Awake()
    {
        initialize();
        createMap();
    }

    private void initialize()
    {
        blockController = GameObject.Find("BlockController").GetComponent<BlockController>();
        blocks = GameObject.Find("Blocks").transform;
        maps = GameObject.Find("Maps").transform;

        blockList = new List<List<GameObject>>(numColumn);

        linesLeft = defaultLinesLeft + stage * 2;
        linesLeftText.text = linesLeft.ToString();

        score = 0;
        scoreText.text = score.ToString();

        combo = 0;
        comboText.text = combo.ToString();

        curLineIdx = 1;

        pause = false;
    }

    private void resetGame()
    {
        blockController.resetGame();

        linesLeft = defaultLinesLeft + stage * 2;
        linesLeftText.text = linesLeft.ToString();

        combo = 0;
        comboText.text = combo.ToString();

        curLineIdx = 1;

        int count = blocks.childCount;
        for (int idx = 0; idx < count; idx++) 
        {
            Destroy(blocks.GetChild(idx).gameObject);
        }
    }

    private void createMap()
    {
        for (int idxCol = 0; idxCol < numColumn; idxCol++)
        {
            blockList.Add(new List<GameObject>());

            for (int idxRow = 0; idxRow < numRow; idxRow++)
            {
                GameObject gameObject = null;

                if ((idxCol == 0) || (idxRow == 0) || (idxRow == numRow - 1))
                    gameObject = Instantiate(edgeBlock, new Vector3(idxRow, idxCol, -0.1f), Quaternion.identity, maps);
                else
                    Instantiate(mapBlock, new Vector3(idxRow, idxCol, 0), Quaternion.identity, maps);

                blockList[idxCol].Add(gameObject);
            }
        }

        for (int idxCol = 0; idxCol < numColumn; idxCol++)
        {
            for (int idxRow = 0; idxRow < numRow / 2; idxRow++)
            {
                GameObject block = (idxCol == 0 || idxCol == numColumn - 1 || idxCol == 18)
                    || (idxRow == numRow - 1) ? edgeBlock : mapBlock;

                Vector3 pos = new Vector3((numRow + idxRow) * blockSize, idxCol * blockSize, 0f);
                Instantiate(block, pos, Quaternion.identity, maps);
            }
        }
    }

    public void addTetromino()
    {
        Tetromino curTetromino = blockController.CurTetromino;

        while (curTetromino.transform.childCount != 0)
        {
            Vector3 pos = curTetromino.transform.GetChild(0).transform.position;

            int idxX = (int)Mathf.Round(pos.x);
            int idxY = (int)Mathf.Round(pos.y);

            blockList[idxY][idxX] = curTetromino.transform.GetChild(0).gameObject;

            curTetromino.transform.GetChild(0).parent = blocks;
        }
        Destroy(curTetromino.gameObject);

        if (checkCompleteLine())
        {
            StartCoroutine(removeCompleteLine());
        }

        combo = 0;
    }

    public void updateInfomation()
    {
        comboText.text = (++combo).ToString();

        score += combo * scorePoint;
        scoreText.text = score.ToString();

        linesLeftText.text = (--linesLeft).ToString();

        if (linesLeft == 0)
        {
            StopCoroutine(removeCompleteLine());
            StartCoroutine(victory());
        }
    }

    private void dropLines()
    {
        for (int idx = curLineIdx; idx < blockList.Count - 1; idx++)
        {
            if (blockList[idx].TrueForAll(x => x == null)) return;

            for (int idxRow = 1; idxRow < numRow - 1; idxRow++)
            {
                blockList[idx][idxRow] = blockList[idx + 1][idxRow];

                if (blockList[idx][idxRow] == null) continue;

                blockList[idx][idxRow].transform.position += Vector3.down;
            }
        }
    }

    private IEnumerator removeCompleteLine()
    {
        pause = true;
        List<GameObject> line = new List<GameObject>();

        for (int idx = 1; idx < numRow - 1; idx++)
        {
            line.Add(blockList[curLineIdx][idx]);
        }

        while (line.Exists(x => x.GetComponent<MeshRenderer>().material.color.a > 0.1f))
        {
            foreach (GameObject obj in line)
            {
                Color color = obj.GetComponent<MeshRenderer>().material.color;
                color.a = Mathf.Lerp(color.a, 0, blockFadeSpeed * Time.deltaTime);

                obj.GetComponent<MeshRenderer>().material.color = color;
            }

            yield return null;
        }

        foreach (GameObject obj in line)
        {
            Destroy(obj);
        }

        dropLines();
        updateInfomation();

        pause = false;

        if (checkCompleteLine()) StartCoroutine(removeCompleteLine());

        yield return null;
    }

    private bool checkCompleteLine()
    {
        for (int idx = curLineIdx; idx < blockList.Count - 1; idx++)
        {
            if (!blockList[idx].Exists(x => x == null))
            {
                curLineIdx = idx;

                return true;
            }
        }

        curLineIdx = 1;

        return false;
    }

    private IEnumerator victory()
    {
        pause = true;

        sceneFader.gameObject.SetActive(true);

        while (sceneFader.color.a < 0.95f)
        {
            Color color = sceneFader.color;
            color.a = Mathf.Lerp(color.a, 1.0f, sceneFadeSpeed * Time.deltaTime);

            sceneFader.color = color;

            yield return null;
        }

        stage++;

        resetGame();

        blockController.TetrominoDropDelay *= 0.9f;

        while (sceneFader.color.a > 0.05f)
        {
            Color color = sceneFader.color;
            color.a = Mathf.Lerp(color.a, 0f, sceneFadeSpeed * Time.deltaTime);

            sceneFader.color = color;

            yield return null;
        }

        sceneFader.gameObject.SetActive(false);

        pause = false;

        yield return null;
    }

    private IEnumerator defeat()
    {
        pause = true;

        sceneFader.gameObject.SetActive(true);

        while (sceneFader.color.a < 0.95f)
        {
            Color color = sceneFader.color;
            color.a = Mathf.Lerp(color.a, 1.0f, sceneFadeSpeed * Time.deltaTime);

            sceneFader.color = color;

            yield return null;
        }

        resetGame();

        while (sceneFader.color.a > 0.05f)
        {
            Color color = sceneFader.color;
            color.a = Mathf.Lerp(color.a, 0f, sceneFadeSpeed * Time.deltaTime);

            sceneFader.color = color;

            yield return null;
        }

        sceneFader.gameObject.SetActive(false);

        pause = false;

        yield return null;
    }
}
