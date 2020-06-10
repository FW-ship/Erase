using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TutorialSceneManager : PuzzleSceneManager
{
    public GameObject objTextImage;                                         //テキスト表示欄の画像
    public GameObject objFace;
    public GameObject objName;
    public GameObject objNote;
    public string[] scenarioText;
    public int linenum = 0;
    

    // Start is called before the first frame update
    void Start()
    {
        statusEffect[0] = new List<StatusEffect>();
        statusEffect[1] = new List<StatusEffect>();
        c1 = GetComponent<CardData>();
        u1 = GetComponent<Utility>();
        waitCount[0] = 0;
        waitCount[1] = 0;
        ReadText(PlayerPrefs.GetString("ScenarioName"));
        MakeEnemy();
        StartCoroutine(MainGame());
        StartCoroutine(ScenarioPlay());
    }

    private void MakeEnemy()
    {
        //敵のデッキを作成。
        c1.deckCard[1, 0] = c1.card[int.Parse(scenarioText[2])].Clone();
        c1.deckCard[1, 1] = c1.card[int.Parse(scenarioText[3])].Clone();
        c1.deckCard[1, 2] = c1.card[int.Parse(scenarioText[4])].Clone();
        c1.deckCard[1, 3] = c1.card[int.Parse(scenarioText[5])].Clone();
        c1.deckCard[1, 4] = c1.card[int.Parse(scenarioText[6])].Clone();
        c1.deckCard[1, 5] = c1.card[int.Parse(scenarioText[7])].Clone();
        c1.deckCard[1, 6] = c1.card[int.Parse(scenarioText[8])].Clone();
        c1.deckCard[1, 7] = c1.card[int.Parse(scenarioText[9])].Clone();
        c1.deckCard[1, 8] = c1.card[int.Parse(scenarioText[10])].Clone();
        c1.deckCard[1, 9] = c1.card[int.Parse(scenarioText[11])].Clone();
        c1.deckCard[1, 10] = c1.card[int.Parse(scenarioText[12])].Clone();
        c1.deckCard[1, 11] = c1.card[int.Parse(scenarioText[13])].Clone();
        c1.deckCard[1, 12] = c1.card[int.Parse(scenarioText[14])].Clone();
        c1.deckCard[1, 13] = c1.card[int.Parse(scenarioText[15])].Clone();
        c1.deckCard[1, 14] = c1.card[int.Parse(scenarioText[16])].Clone();
        c1.deckCard[1, 15] = c1.card[int.Parse(scenarioText[17])].Clone();
        c1.deckCard[1, 16] = c1.card[int.Parse(scenarioText[18])].Clone();
        c1.deckCard[1, 17] = c1.card[int.Parse(scenarioText[19])].Clone();
        c1.deckCard[1, 18] = c1.card[int.Parse(scenarioText[20])].Clone();
        c1.deckCard[1, 19] = c1.card[int.Parse(scenarioText[21])].Clone();
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            PlayerPrefs.SetInt("enemyDeckCard" + i.ToString(), c1.deckCard[1, i].cardNum);//敵デッキのセーブ
        }
        //マナ獲得能力を設定。
        c1.enemyGetManaPace[0] = int.Parse(scenarioText[23]);
        c1.enemyGetManaPace[1] = int.Parse(scenarioText[24]);
        c1.enemyGetManaPace[2] = int.Parse(scenarioText[25]);
        c1.enemyGetManaPace[3] = int.Parse(scenarioText[26]);
        c1.enemyGetManaPace[4] = int.Parse(scenarioText[27]);
        for (int i = 0; i < BLOCKTYPE_NUM + 1; i++)
        {
            PlayerPrefs.SetInt("enemyGetManaPace" + i.ToString(), c1.enemyGetManaPace[i]);//敵マナ獲得ペースのセーブ
        }
    }

    private IEnumerator ScenarioPlay()
    {
        //シナリオ部  
        for (int i = 30; i < scenarioText.Length; i++)
        {
            //★未：wait系と分岐処理のテスト。ＥＮＤでシーンジャンプ。
            
            if (scenarioText[i].Length >= 10 && scenarioText[i].Substring(0, 10) == "ＷＡＩＴＥＲＡＳＥ：") { yield return StartCoroutine(Wait(0));i++; }
            else if (scenarioText[i].Length >= 9 && scenarioText[i].Substring(0, 9) == "ＷＡＩＴＴＵＲＮ：") { yield return StartCoroutine(Wait(1));i++; }
            else if (scenarioText[i].Length >= 9 && scenarioText[i].Substring(0, 9) == "ＷＡＩＴＬＡＳＴ：") { yield return StartCoroutine(Wait(2));i++; }
            else if (scenarioText[i].Length >= 6 && scenarioText[i].Substring(0, 6) == "ＩＦＮＯＴ：") { IFNOT(ref i); }
            else if (scenarioText[i].Length >= 5 && scenarioText[i].Substring(0, 5) == "ＢＡＣＫ：") { i -= 1 + int.Parse(scenarioText[i].Substring(5)); }
            else if (scenarioText[i].Length >= 4 && scenarioText[i].Substring(0, 4) == "ＰＵＴ：") { linenum = i; yield return StartCoroutine(Put());i = linenum; }
            else if (scenarioText[i].Length >= 5 && scenarioText[i].Substring(0, 5) == "ＮＯＴＥ：") { Note(i); }
            else if (scenarioText[i].Length >= 4 && scenarioText[i].Substring(0, 4) == "ＥＮＤ：") { string[] tmp = scenarioText[i].Substring(4).Split(',');if (win) { PlayerPrefs.SetString("ScenarioName", tmp[0]); } else { PlayerPrefs.SetString("ScenarioName", tmp[1]); } }
            else {
                if (scenarioText[i] == "" || scenarioText[i] == "ＥＮＤ：" || scenarioText[i] == "ＩＦＥＮＤ：") { continue; }
                Talk(ref i); yield return StartCoroutine(u1.PushWait());
            }
            Resources.UnloadUnusedAssets();
        }
    }

    private void Note(int i)
    {
        string[] xy=new string[2];
        objNote.SetActive(true);
        //位置設定
        xy=scenarioText[i].Substring(5).Split(',');
        objNote.GetComponent<RectTransform>().localPosition = new Vector2(int.Parse(xy[0]), int.Parse(xy[1]));
        StartCoroutine("MoveNote");
    }

    private IEnumerator MoveNote()
    {
        for(int j=0;j<10000;j++)
        {
            objNote.GetComponentsInChildren<RectTransform>()[1].localPosition = new Vector2((-1+Mathf.Sin(j/10))*20, 0);
            yield return null;
        }
    }

    private IEnumerator Wait(int mode)
    {
        objTextImage.SetActive(false);
        objName.SetActive(false);

        for (int j = 0; j < 5; j++) { maxmana[j] = 0; }
        if (mode == 0)
        {
            while (!blockfloat)//プレイヤーが何か消すまで
            {
                yield return null;
            }
            while (blockfloat)//落下＆連鎖処理が終わるまで
            {
                yield return null;
            }
        }
        if (mode == 1)
        {
            while (!turnProcess)//ターンエンド処理に入るまで
            {
                yield return null;
            }
            while (turnProcess)//ターンエンド処理が終わるまで
            {
                yield return null;
            }
        }
        if (mode == 2)
        {
            while (!winloseFlag)//勝敗決定まで
            {
                yield return null;
            }
        }
        objTextImage.SetActive(true);
        objName.SetActive(true);
    }

    private void IFNOT(ref int i)
    {
        bool jumpflag = true;
        string[] manas = new string[5];
        int[] mana=new int[5];
        //求められたマナを得られたかチェック。満たしていなければ一行下がる。満たしていれば最寄りのIFENDまでスキップ。
        manas=scenarioText[i].Substring(3).Split(',');
        for (int j = 1; j < BLOCKTYPE_NUM + 1; j++) { mana[j] = int.Parse(manas[j]); }
        for (int j = 1; j < BLOCKTYPE_NUM+1; j++) { if (mana[j] > maxmana[j]) { jumpflag = false; } }
        if (jumpflag) {
            while (scenarioText[i] != "ＩＦＥＮＤ：" && i< scenarioText.Length-1) { i++; }
        }
    }


    private IEnumerator Put()
    {
        int cnum = 0;
        for (int i = 0; i < HAND_NUM; i++)
        {
            linenum++;
            cnum = int.Parse(scenarioText[linenum]);
            handCard[0,i]=c1.card[cnum].Clone();
            if (cardImage[cnum] == null) { cardImage[cnum] = Resources.Load<Sprite>("card" + cnum.ToString()); }
            objCard[0, i].GetComponent<Image>().sprite = cardImage[cnum];
            for (int j = 0; j < BLOCKTYPE_NUM + 1; j++) { handCard[0, i].cardMana[j] = 0; }//カードに貯まったマナはリセット
            DrawCardCost(0, i);
            handCard[0, i].useCard = false;//新たに引いたので、使っていない状態になおす。
            objCard[0, i].GetComponent<Image>().enabled = true;//引き直したので、非表示になっていたカードを表示しなおす。
        }

        for (int j = 0; j < WORLD_HEIGHT; j++)
        {
            for (int i = 0; i < WORLD_WIDTH; i++)
            {
                block[i, j] = 0;
            }
        }
        yield return null;
        for (int j = WORLD_HEIGHT-1; j >=0; j--)
        {
            linenum++;  
            for (int i = 0; i < WORLD_WIDTH; i++)
            {
                block[i,j] = int.Parse(scenarioText[linenum].Substring(i, 1));
                seAudioSource[0].PlayOneShot(se[0]); yield return null;
                blockMoveTime[i, j] = 0;
            }
        }

    }



    private void ReadText(string fileName)
    {
        string text;
        text = (Resources.Load(fileName, typeof(TextAsset)) as TextAsset).text.Replace("\r", "");
        scenarioText = text.Split(char.Parse("\n"));
    }

    private void Talk(ref int i)
    {
        string drawText = "";
        string[] drawChara;
        drawChara = scenarioText[i].Split('：');
        if (drawChara.Length < 2) { return; }
        objName.GetComponentInChildren<Text>().text = drawChara[0];
        objFace.GetComponent<Image>().sprite = Resources.Load<Sprite>(drawChara[0] + drawChara[1]);
        i++;
        for (; i < scenarioText.Length; i++)
        {
            if (scenarioText[i] == "") { ScenarioDraw(drawText); return; }
            drawText += scenarioText[i] + "\n";
        }
        ScenarioDraw(drawText); return;
    }

    private void ScenarioDraw(string text)
    {
        objTextImage.GetComponentInChildren<Text>().text = "";//テキストの初期化
            objName.GetComponent<Image>().enabled = true;
            objFace.GetComponent<Image>().enabled = true;
            objName.GetComponentInChildren<Text>().enabled = true;
            objTextImage.GetComponent<Image>().enabled = true;
            objTextImage.GetComponentInChildren<Text>().enabled = true;
            objTextImage.GetComponentInChildren<Text>().text = text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public new void BlockPush(int x)
    {
        StopCoroutine("MoveNote");
        objNote.SetActive(false);
        if (blockfloat) { return; }
        deleteBlock[x / 10, x % 10] = true;
    }

    public new void turnEndButton()
    {
        StopCoroutine("MoveNote");
        objNote.SetActive(false);
        turnEndButtonPush = true;
    }

}
