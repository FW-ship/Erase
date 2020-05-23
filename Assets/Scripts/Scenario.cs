using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Scenario : MonoBehaviour {

    const int MAX_CHARACTER = 5;                                            //画面に表示するキャラクターの最大人数
    const int DECKCARD_NUM = 20;                                            //デッキのカード枚数
    const int BLOCKTYPE_NUM = 5;                                            //ブロックの色の種類数
    const int GETCARD_NUM = 3;                                              //1パックで獲得できるカードの数
    const int GETCARD_FROM_NUM = 1000;                                      //1パックでランダムチョイスされる母数
    const int CHARACTER0_X = -400;                                          //キャラクター０オブジェクトのＸ座標
    const int CHARACTER1_X = 400;                                           //キャラクター１オブジェクトのＸ座標
    const int CHARACTER_Y = 160;                                            //キャラクターのＹ座標（番号に関わらず一定）
    const int CHARACTER_NUM = 100;                                          //キャラクター立ち絵の数
    const int BACK_IMAGE_NUM = 20;                                          //背景の数
    const int SCENARIO_NUM = 3;                                             //シナリオの数

    private int packNum;                                              //選択したパック番号
    private int coin;                                                 //現在の所持コイン数
    private int coinForDraw;                                          //表示用の所持コイン数（増加エフェクト用）
    private int scenarioCount;                                        //現在のシナリオ位置
    private int maxScenarioCount;                                     //現在のシナリオ進行度
    private int timeCount;                                            //シーンが始まってからの時間
    private string nextScene;                                         //次にジャンプするシーン名
    private bool shichoFlag;                                          //シチョウ（キャラクター）が登場しているか
    public string getCardText;                                        //獲得カードテキスト
    public int[] getCard = new int[GETCARD_NUM];                      //カード獲得で得られる3枚
    private int[] layerDepth=new int[2];                                 //
    public string[] scenarioText;
    private string[] charaname = new string[10];//各オブジェクトに入っているキャラの対応表。

    private List<int> mainStory = new List<int>();                    //パックから黒いカードを入手したフラグ(0が未入手、1が入手したばかり（イベント未読）、2が入手かつイベント既読)

    //public GameObject objText;                                               //シナリオテキスト(カード獲得時のpushがあるのでpublic)
    private GameObject objSkipButton;                                        //スキップボタンオブジェクト
    private GameObject objFilm;                                              //回想演出のオブジェクト
    private GameObject objCoin;                                              //コイン表示のオブジェクト
    private GameObject objBookHolder;                                        //本（パック）選択ボタンのオブジェクト
    private GameObject objNowLoading;                                        //ナウローディング表示
    public GameObject objTextImage;                                         //テキスト表示欄の画像
    private GameObject objBackText;                                          //背景に文字を直書きする際のテキスト
    private GameObject[] objCharacter = new GameObject[MAX_CHARACTER];       //キャラクター表示
    private GameObject objCanvas;                                            //キャンバス
    private GameObject[] objGetCard = new GameObject[GETCARD_NUM + 1];       //カード獲得演出用オブジェクト([GETCARD_NUM]が親オブジェクト)
    public GameObject[] objBG = new GameObject[2];
    public GameObject objPlate;
    public GameObject objTime;
    public GameObject objFace;
    public GameObject objName;
    public GameObject objBlack;
    private Component[] SEComponent=new Component[5];
    private AudioClip bgm;                       //BGMのオーディオクリップ
    private List<Sprite> characterImage = new List<Sprite>();                //キャラクター立ち絵
    private List<Sprite> backImage = new List<Sprite>();                     //背景
    CardData c1;


    // Use this for initialization
    void Start() {
        int i;
        //オブジェクトの読み込み
        objBackText = GameObject.Find("backtext").gameObject as GameObject;
        objCanvas = GameObject.Find("canvas0").gameObject as GameObject;
        objNowLoading = GameObject.Find("NowLoading").gameObject as GameObject;
        objFilm = GameObject.Find("film").gameObject as GameObject;
        objSkipButton= GameObject.Find("SkipButton").gameObject as GameObject;
        objNowLoading.GetComponent<Image>().enabled = true;
        for (i = 0; i < MAX_CHARACTER; i++)
        {
            objCharacter[i] = GameObject.Find("character" + i.ToString()).gameObject as GameObject;
        }

        objGetCard[GETCARD_NUM] = GameObject.Find("packopen").gameObject as GameObject;
        for (i = 0; i < GETCARD_NUM; i++)
        {
            objGetCard[i] = GameObject.Find("card" + i.ToString()).gameObject as GameObject;
        }
        objGetCard[GETCARD_NUM].gameObject.SetActive(false);//カード獲得画面は普段のシナリオ中では見えない


        //キャラクター画像の読み込み
        for (i = 0; i < CHARACTER_NUM + 1; i++)
        {
            characterImage.Add(Resources.Load<Sprite>("character" + i.ToString()));
        }


        //シナリオ進行状況の読み込み
        for (i = 0; i < SCENARIO_NUM + 1; i++)
        {
            mainStory.Add(PlayerPrefs.GetInt("mainStory" + i.ToString(), 0));//メインストーリーのどれを見たかのロード
        }
        scenarioCount = PlayerPrefs.GetInt("scenarioCount", 0);//これからプレイするシナリオ番号のロード
        maxScenarioCount = PlayerPrefs.GetInt("maxScenarioCount", 0);//シナリオをどこまで進めたかのロード
        coin=PlayerPrefs.GetInt("coin", 0);
        coinForDraw = coin;
        objCoin= GameObject.Find("ButtonCoin").gameObject as GameObject;
        objCoin.GetComponentInChildren<Text>().text ="Coin:" + coin.ToString();
        objCoin.SetActive(false);
        objBookHolder = GameObject.Find("BookHolder").gameObject as GameObject;
        objBookHolder.SetActive(false);
        //シナリオコルーチンの実行
        ReadText(PlayerPrefs.GetString("ScenarioName"));
        c1 = GetComponent<CardData>();
        StartCoroutine(ScenarioPlay());
    }
    

    // Update is called once per frame
    void Update() {
        timeCount++;
        if (coinForDraw<coin && timeCount%3==0) { coinForDraw++; objCoin.GetComponentInChildren<Text>().text = "Coin:" + coinForDraw.ToString(); }
        if (coinForDraw > coin && timeCount % 3 == 0) { coinForDraw--; objCoin.GetComponentInChildren<Text>().text = "Coin:" + coinForDraw.ToString(); }
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
        Utility u1 = GetComponent<Utility>();
        string[] text;
        //システム処理
        //スキップ可能になる(yield returnを返す)前にシステム上の処理を終える      
        MakeEnemy();
        nextScene = "PuzzleScene";//次に行くシーン名
        objNowLoading.GetComponent<Image>().enabled = false;
        layerDepth[0] = 50;
        layerDepth[1] = 20;
        //シナリオ部  
        for (int i = 30; i < scenarioText.Length; i++)
        {
            if (scenarioText[i].Length>=4 && scenarioText[i].Substring(0, 4) == "ＢＧＭ：") { bgm = Resources.Load<AudioClip>(scenarioText[i].Substring(4)); u1.BGMPlay(bgm); }
            else if (scenarioText[i].Length >= 7 && scenarioText[i].Substring(0, 7) == "ＢＧＭＯＶＥ：") { StartCoroutine(BGMove(int.Parse(scenarioText[i].Substring(7)))); }
            else if (scenarioText[i].Length >= 3 && scenarioText[i].Substring(0, 3) == "ＢＧ：") { yield return StartCoroutine(ChangeBG(scenarioText[i].Substring(3))); }
            else if (scenarioText[i].Length >= 5 && scenarioText[i].Substring(0, 5) == "ＦＩＬＭ：") { if (scenarioText[i].Substring(5) == "ＯＦＦ") { objFilm.GetComponent<SpriteRenderer>().enabled = false; objFilm.GetComponent<Animator>().enabled = false; } else { objFilm.GetComponent<SpriteRenderer>().enabled = true; objFilm.GetComponent<Animator>().enabled = true; } }
            else if (scenarioText[i].Length >= 3 && scenarioText[i].Substring(0, 3) == "ＳＥ：") { StorySE(scenarioText[i].Substring(3),u1); }
            else if (scenarioText[i].Length >= 5 && scenarioText[i].Substring(0, 5) == "ＷＡＩＴ：") { yield return new WaitForSeconds(float.Parse(scenarioText[i].Substring(5))); }
            else if (scenarioText[i].Length >= 8 && scenarioText[i].Substring(0, 8) == "ＢＧＭＳＴＯＰ：") { u1.BGMStop(); }
            else if (scenarioText[i].Length >= 8 && scenarioText[i].Substring(0, 8) == "ＳＥＴＴＩＭＥ：") { if (scenarioText[i].Substring(8) == "ＮＩＧＨＴ") { SetTime(1); } else { SetTime(0); } }
            else if (scenarioText[i].Length >= 6 && scenarioText[i].Substring(0, 6) == "ＳＨＡＫＥ：") { StartCoroutine("ShakeScreen"); }
            else if (scenarioText[i].Length >= 8 && scenarioText[i].Substring(0, 8) == "ＢＧＭＫＥＥＰ：") { GameObject.Find("BGMManager").GetComponent<BGMManager>().bgmChange(false, 1); }//パズルシーンもBGMはそのまま
            else if (scenarioText[i].Length >= 7 && scenarioText[i].Substring(0, 7) == "ＢＧＴＥＸＴ：") { i++; BGText(ref i); yield return StartCoroutine(u1.PushWait()); }
            else if (scenarioText[i].Length >= 6 && scenarioText[i].Substring(0, 6) == "ＣＨＡＲＡ：") { text=scenarioText[i].Substring(6).Split(','); StartCoroutine(CharacterAct(text)); }
            else if (scenarioText[i].Length >= 5 && scenarioText[i].Substring(0, 5) == "ＺＯＯＭ：") { text = scenarioText[i].Substring(5).Split(','); StartCoroutine(BGZoom(int.Parse(text[0]), int.Parse(text[1]), int.Parse(text[2]))); }
            else { if (scenarioText[i] == "" || scenarioText[i] == "ＥＮＤ：") {continue;} Talk(ref i); yield return StartCoroutine(u1.PushWait()); }
            Resources.UnloadUnusedAssets();
        }

              
        //ゲームパートへ
        
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    //戦闘報酬
    private IEnumerator BattleEnd()
    {
        Utility u1 = GetComponent<Utility>();
        //システム処理
        if (scenarioCount > maxScenarioCount)
        {
            maxScenarioCount = scenarioCount;
            PlayerPrefs.SetInt("maxScenarioCount", maxScenarioCount);
        }//今までで一番シナリオが進んでいたら進行度(maxScenarioCount)も上げる。
        nextScene = "SelectScene";//次に行くシーン名
        yield return StartCoroutine("GetPack", 1);//パック１を獲得。
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
    }

    private IEnumerator EndRoll()
    {
        Utility u1 = GetComponent<Utility>();
        yield return StartCoroutine(ScenarioDraw("", ""));//背景以外のオブジェクトを全て消し、背景を空に。
        objBG[0].GetComponent<RectTransform>().sizeDelta = new Vector2(1280,4000);
        for (int i = 0; i <= 1500; i++)
        {
            objBG[0].GetComponent<RectTransform>().localPosition = new Vector3(0, 1500 - i * 2);
            if (i == 200)
            {
                StartCoroutine(ScenarioDraw("", ""));
            }

            yield return null;

            objBackText.GetComponent<Text>().fontSize = 36;
            objBackText.GetComponent<RectTransform>().localPosition = new Vector2(300, i * 2 - 1500);
            if (i == 300) { StartCoroutine(ScenarioDraw("", "\n\n\n\nScenario:#6")); }
            if (i == 480) { StartCoroutine(ScenarioDraw("", "\n\n\n\n\n\nProgram:#6")); }
            if(i==660) { StartCoroutine(ScenarioDraw("", "\n\n\n\n\n\n\n\nMainCharacters-Illustration:#6")); }
            if(i==840) { StartCoroutine(ScenarioDraw("", "\n\n\n[SpecialThanks]\n\nIllustration-materials:イラストAC\n 　　                       いらすとや\nPhoto-materials:写真AC\nMusic-materials:ハヤシユウ\nSE-materials:効果音ラボ")); }
            if(i==1200) { StartCoroutine(ScenarioDraw("", "\n\n\n\n[制作]\n\nBrainmixer")); }
            if(i==1380) { StartCoroutine(ScenarioDraw("", "\n\n\n\n\n\nThank you for playing.")); }
            objBackText.GetComponent<Text>().color = new Color(0.15f, 0.15f, 0.15f, (float)((i - 300)%180) / 50);
        }
        yield return StartCoroutine(u1.PushWait());
    }

    private IEnumerator ScenarioText100000()
    {
        objCoin.SetActive(true);
        objBookHolder.SetActive(true);
        objSkipButton.GetComponentInChildren<Text>().text="Back";
        Utility u1 = GetComponent<Utility>();
        //システム処理
        nextScene = "SelectScene"; //次に行くシーン名  
        bgm=Resources.Load<AudioClip>("おもしろすぎてどっかん");
        Resources.UnloadUnusedAssets();
        u1.BGMPlay(bgm);
        objNowLoading.GetComponent<Image>().enabled = false;
        yield return StartCoroutine(ScenarioDraw("【シチョウ】\n――ここでは、コインをパックと交換してあげるよ。\nOver（3枚以上）になったカードはコインに変換されるから、パズルが苦手でも、地道にやれば高難度ステージのカードが手に入るね。", ""));
        yield return StartCoroutine(u1.SelectWait());
        //パックを選択させる。
        yield return StartCoroutine("GetPack", packNum);//選択したパックを獲得。
        //シーン選択パートへ
        u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        yield return null;
    }

    public void PushPack(int num)
    {
        Utility u1 = GetComponent<Utility>();
        if (num == 1)
        {
            if (coin>=10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", ""));
            }
        }
        if (num == 2)
        {
            if (coin >= 10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", ""));
            }
        }
        if (num == 3)
        {
            if (coin >= 10)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 10;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", ""));
            }
        }
        if (num == 4)
        {
            if (coin >= 20)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 20;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", ""));
            }
        }
        if (num == 5)
        {
            if (coin >= 20)
            {
                u1.SEPlay(gameObject, Resources.Load<AudioClip>("clearing1"));
                objBookHolder.SetActive(false);
                nextScene = "StoryScene"; //次に行くシーン名 
                coin -= 20;
                packNum = num;
                u1.selectFlag = true;
            }
            else
            {
                StartCoroutine(ScenarioDraw("【シチョウ】\nコインが足りないね。アタシはまけてあげるほど優しくないよ？", ""));
            }
        }
        Resources.UnloadUnusedAssets();
    }

    //描画内容関数。text：シナリオテキスト、back:背景の種類、backtext:背景に直書きする文字
    private IEnumerator ScenarioDraw(string text, string backtext)
    {
        objTextImage.GetComponentInChildren<Text>().text = "";//テキストの初期化
        if (text == "")
        {
            objName.GetComponent<Image>().enabled = false;
            objFace.GetComponent<Image>().enabled = false;
            objName.GetComponentInChildren<Text>().enabled = false;
            objTextImage.GetComponent<Image>().enabled = false;
        }
        else
        {
            objName.GetComponent<Image>().enabled = true;
            objFace.GetComponent<Image>().enabled = true;
            objName.GetComponentInChildren<Text>().enabled = true;
            objTextImage.GetComponent<Image>().enabled = true;
        }
        if (backtext == "")
        {
            objBackText.GetComponent<Text>().enabled = false;
        }
        else
        {
            objBackText.GetComponent<Text>().enabled = true;
            objBackText.GetComponent<Text>().text = backtext;
        }
        if (text == "")
        {
            objTextImage.GetComponentInChildren<Text>().enabled = false;
        }
        else
        {
            objTextImage.GetComponentInChildren<Text>().enabled = true;
            objTextImage.GetComponentInChildren<Text>().text = text;
        }
        yield return null;
    }

    //キャラクターの小ジャンプ
    private IEnumerator CharacterJump(int chara)
    {
            for (int i = 0; i < 7; i++)
            {
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x, objCharacter[chara].GetComponent<RectTransform>().localPosition.y + 2, 1);
                yield return null;
            }
            for (int i = 7; i > 0; i--)
            {
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x, objCharacter[chara].GetComponent<RectTransform>().localPosition.y - 2, 1);
                yield return null;
            }
    }


    //画面振動する関数
    private IEnumerator ShakeScreen()
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0 - 5 + 10 * (i % 2));
            yield return null;
        }
        objCanvas.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
    }

    //パック獲得関数
    private IEnumerator GetPack(int packNum)
    {
        Utility u1 = GetComponent<Utility>();
        const int COMMON = 100;//Cは10%(100/1000)
        const int UNCOMMON = 50;//UCは5%(50/1000)
        const int RARE = 10;//Rは1%(10/1000)
        int i, j;
        int choice;
        int[] packCard = new int[GETCARD_FROM_NUM];//ランダムチョイスの母集団のカード番号配列
        int[] packCardRarity = new int[GETCARD_FROM_NUM];//ランダムチョイスの母集団のレアリティ配列
        int[] getCardRarity = new int[GETCARD_NUM];//得られる3枚のレアリティ
        Sprite[] cardImage = new Sprite[GETCARD_FROM_NUM];
        bgm=Resources.Load<AudioClip>("わくわくショッピング");
        objCoin.SetActive(true);
        u1.BGMPlay(bgm);
        objNowLoading.GetComponent<Image>().enabled = false;
        objSkipButton.SetActive(false);
        yield return StartCoroutine(ScenarioDraw("獲得カード", ""));
        objGetCard[GETCARD_NUM].gameObject.SetActive(true);
        
        yield return StartCoroutine(u1.PushWait());
        for (i = 0; i < GETCARD_NUM; i++)
        {
            objGetCard[i].GetComponent<Image>().raycastTarget = true;//表向きカードは説明表示あり
        }
        u1.SEPlay(gameObject,Resources.Load<AudioClip>("up"));
        Resources.UnloadUnusedAssets();
        //パック内容
        if (packNum == 1)//パック１（桃太郎）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 1; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 2; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 3; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < COMMON + j; i++) { packCard[i] = 5; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 7; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 8; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 9; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 15; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 16; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 21; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < RARE + j; i++) { packCard[i] = 22; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 23; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 24; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 25; packCardRarity[i] = RARE; }//R 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 26; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 27; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < RARE + j; i++) { packCard[i] = 28; packCardRarity[i] = RARE; }//R     
        }

        if (packNum == 2)//パック２（赤ずきん）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 32; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 34; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 30; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < COMMON + j; i++) { packCard[i] = 36; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 37; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 38; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 31; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 39; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 33; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 40; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < RARE + j; i++) { packCard[i] = 29; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 41; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 42; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 44; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 35; packCardRarity[i] = RARE; }//R
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 43; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 45; packCardRarity[i] = UNCOMMON; }//UC     
        }
        if (packNum == 3)//パック３（幸福な王子）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 54; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 55; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 56; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 57; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 58; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 59; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 60; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 46; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 47; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 48; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 49; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 50; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 51; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 52; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 53; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 61; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 62; packCardRarity[i] = RARE; }//R
        }
        if (packNum == 4)//パック４（エンディングおまけ）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 46; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 47; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 48; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 49; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 35; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 33; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 30; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 31; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 26; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 27; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 3; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 9; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 11; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 12; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 50; packCardRarity[i] = UNCOMMON; }//UC
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 6; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 10; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 14; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 4; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 13; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 17; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 18; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 20; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 19; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 53; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 61; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 51; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 52; packCardRarity[i] = RARE; }//R
        }
        if (packNum == 5)//パック５（アリス）
        {
            //for文の使い方がトリッキーなので注意。カウント変数であるiを初期化せず、代わりにfor文脱出条件に関する変数jをiで初期化することで、連続して配列に代入できるようにしている。
            i = 0;
            for (j = i; i < COMMON + j; i++) { packCard[i] = 71; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 72; packCardRarity[i] = COMMON; }//C
            for (j = i; i < COMMON + j; i++) { packCard[i] = 73; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 74; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 75; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 76; packCardRarity[i] = COMMON; }//C 
            for (j = i; i < COMMON + j; i++) { packCard[i] = 77; packCardRarity[i] = COMMON; }//C
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 10; packCardRarity[i] = UNCOMMON; }//UC  
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 66; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 68; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 69; packCardRarity[i] = UNCOMMON; }//UC                       
            for (j = i; i < UNCOMMON + j; i++) { packCard[i] = 70; packCardRarity[i] = UNCOMMON; }//UC 
            for (j = i; i < RARE + j; i++) { packCard[i] = 63; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 64; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 65; packCardRarity[i] = RARE; }//R
            for (j = i; i < RARE + j; i++) { packCard[i] = 29; packCardRarity[i] = RARE; }//R 
            for (j = i; i < RARE + j; i++) { packCard[i] = 67; packCardRarity[i] = RARE; }//R
        }

        getCardText = "";//獲得カードテキストの初期化
        for (i = 0; i < GETCARD_NUM; i++)
        {
            choice = Random.Range(0, GETCARD_FROM_NUM);
            getCard[i] = packCard[choice];//獲得するカードをランダムで決定
            cardImage[i] = Resources.Load<Sprite>("card" + getCard[i].ToString());//獲得したカードの画像をロード
            Resources.UnloadUnusedAssets();
            objGetCard[i].GetComponent<Image>().sprite = cardImage[i];//カード画像を表示
            getCardText += c1.card[getCard[i]].cardName;//カード名を表示
            //空白を入れて幅を合わせる
            for (j = 0; j < 10 - c1.card[getCard[i]].cardName.Length; j++) { getCardText += "　"; }
            //カードのレアリティを表示
            if (packCardRarity[choice] == COMMON) { getCardText += "<b><color=black>Ｃ　</color></b>";getCardRarity[i] = COMMON; }
            if (packCardRarity[choice] == UNCOMMON) { getCardText += "<b><color=#a06000ff>ＵＣ</color></b>";getCardRarity[i] = UNCOMMON; }
            if (packCardRarity[choice] == RARE) { getCardText += "<b><color=#ff5000ff>Ｒ　</color></b>";getCardRarity[i] = RARE; }
            //新しいカードならNew!表示、既に3枚持っているならOver表示
            if (c1.card[getCard[i]].haveCard == 0) { getCardText += "　<color=red>New!</color>"; }
            if (c1.card[getCard[i]].haveCard >= 3) { getCardText += "　<color=blue>Over...</color>"; }
            if (c1.card[getCard[i]].haveCard < 3)
            {
                c1.card[getCard[i]].haveCard++;
            }//獲得したカードについて手持ちが3枚以下なら所持カードに追加。
            else
            {
                if (packCardRarity[choice] == RARE) { coin += 10; getCardText += "→<color=olive>+10 Coin</color>"; }
                if (packCardRarity[choice] == UNCOMMON) { coin += 2; getCardText += "→<color=olive>+2 Coin</color>"; }
                if (packCardRarity[choice] == COMMON) { coin++; getCardText += "→<color=olive>+1 Coin</color>"; }

            }//手持ちが3枚以上ならコインに変換。
            getCardText += "\n";
            PlayerPrefs.SetInt("coin", coin);
            PlayerPrefs.SetInt("haveCard" + getCard[i].ToString(), c1.card[getCard[i]].haveCard);//セーブ
            if (getCard[i] == 15 && mainStory[1] == 0) { mainStory[1] = 1; PlayerPrefs.SetInt("mainStory1", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            if (getCard[i] == 45 && mainStory[2] == 0) { mainStory[2] = 1; PlayerPrefs.SetInt("mainStory2", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            if (getCard[i] == 62 && mainStory[3] == 0) { mainStory[3] = 1; PlayerPrefs.SetInt("mainStory3", 1); }//黒のカードを引いたらメインストーリーイベントのフラグをたてる。
            PlayerPrefs.Save();//カード取得のデータを確実に残すためセーブデータを書き込み。
        }
        StartCoroutine(getRareCard(getCardRarity));//レアカード判定に使う配列getCardRarityを引数の形でコルーチンに渡す
        objTextImage.GetComponentInChildren<Text>().text=getCardText;//テキストをオブジェクトに表示
        yield return StartCoroutine(u1.PushWait());
        objGetCard[GETCARD_NUM].gameObject.SetActive(false);
        coinForDraw = coin;
        objCoin.SetActive(false);
        objSkipButton.SetActive(true);
        u1.BGMStop();//カード獲得画面のbgmは消しておく。（そのまま裏シナリオに入る場合があるのでややこしくなるのを避ける）
        yield return null;
    }

    //レアカードの点滅演出
    private IEnumerator getRareCard(int[] getCardRarity)
    {
        const int RARE = 10;//Rは1%(10/1000)
        int i;
        while (true)
        {
            for (i = 0; i < GETCARD_NUM; i++)
            {
                if (getCardRarity[i] == RARE)
                {
                    objGetCard[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f - 0.2f * (Mathf.Cos((float)timeCount / 10)));
                }
            }
            yield return null;
        }
    }

    private void SkipButton()
    {
        Utility u1 = GetComponent<Utility>();
        if (u1.pushObjectFlag == false)//押している間SkipButtonはずっと呼ばれるので、フラグが立った後はコルーチンを呼ばないようにする。（Onclickは押す→離すまでの一連の処理をトリガーとするので、押下のタイミングでは反応を返さない。故にGetCardPushやWaitPushとイベント判定タイミングを『押下時』で統一するためonclickは使わない。判定タイミングが異なるとWaitPushのpushObjectFlag判定が正しく行えない）
        {
            objFilm.GetComponent<SpriteRenderer>().enabled = false;
            objFilm.GetComponent<Animator>().enabled = false;
            //次のパートへ

            objSkipButton.GetComponent<Animator>().enabled = true;
            objSkipButton.GetComponent<Animator>().Play("Pressed",0,0);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().b1.bgmChangeFlag == false) { u1.BGMPlay(bgm); };
            u1.StartCoroutine("LoadSceneCoroutine", nextScene);
        }
        u1.pushObjectFlag = true;
    }

    private IEnumerator BGMove(int XMove,int YMove=0,int time=300)
    {
        for (int j=0;j<time;j++)
        {
            for (int i = 0; i < 2; i++)
            {
                objBG[i].GetComponent<RectTransform>().localPosition = new Vector2((float)XMove / layerDepth[i]/ time + objBG[i].GetComponent<RectTransform>().localPosition.x, (float)YMove / layerDepth[i]/time + objBG[i].GetComponent<RectTransform>().localPosition.y);
            }
            yield return null;
        }
    }

    //シナリオテキストを読み込む。
    private void ReadText(string fileName)
    {
        string text;
        text = (Resources.Load(fileName, typeof(TextAsset)) as TextAsset).text.Replace("\r","");
        scenarioText = text.Split(char.Parse("\n"));
    }

    private IEnumerator ChangeBG(string BGName)
    {
        int i;
        if (objBlack.GetComponent<Image>().color.a < 1)
        {
            for (i = 0; i <= 30; i++)
            {
                objBlack.GetComponent<Image>().color = new Color(0, 0, 0, (float)i/30);
                yield return null;
            }
        }
        objBlack.GetComponent<Image>().color = new Color(0, 0, 0, 1);
        objBG[0].GetComponent<Image>().sprite = Resources.Load<Sprite>(BGName + "0");
        objBG[1].GetComponent<Image>().sprite = Resources.Load<Sprite>(BGName + "1");
        objPlate.GetComponent<Image>().sprite = Resources.Load<Sprite>(BGName + "plate");
        for (i = 0; i <= 30; i++)
        {
            objBlack.GetComponent<Image>().color = new Color(0, 0, 0, (float)(30-i) / 30);
            yield return null;
        }
        objBlack.GetComponent<Image>().color = new Color(0, 0, 0, 0);
    }
    private void Talk(ref int i)
    {
        string drawText="";
        string[] drawChara;
        drawChara=scenarioText[i].Split('：');
        if (drawChara.Length < 2) { return; }
        objName.GetComponentInChildren<Text>().text = drawChara[0]; 
        objFace.GetComponent<Image>().sprite= Resources.Load<Sprite>(drawChara[0] + drawChara[1]);
        i++;
        for (;i<scenarioText.Length;i++)
        {
            if (scenarioText[i] == "") { StartCoroutine(ScenarioDraw(drawText, "")); return; }
            drawText += scenarioText[i] + "\n";
        }
        StartCoroutine(ScenarioDraw(drawText, "")); return;
    }

    private void BGText(ref int i)
    {
        string drawText = "";
        for (; i < scenarioText.Length; i++)
        {
            if (scenarioText[i] == "") { StartCoroutine(ScenarioDraw("",drawText)); return; }
            drawText += scenarioText[i] + "\n";
        }
        StartCoroutine(ScenarioDraw("", drawText)); return;
    }
    
    private IEnumerator CharacterAct(string[] act)
    {
        int i;
        int namecount=0;
        for (i=0;i<10;i++)
        {
            if (charaname[i] == act[0]) { break;}
            if ( !string.IsNullOrEmpty(charaname[i])) { namecount++; }
        }
        if (i == 10) { charaname[namecount] = act[0];i = namecount;}
        objCharacter[i].GetComponent<Image>().enabled = true;
        objCharacter[i].GetComponent<Image>().sprite = null;//unity不具合回避
        objCharacter[i].GetComponent<Image>().sprite= Resources.Load<Sprite>(act[0] + act[1]);
        if (act[1] == "be")
        {
            objCharacter[i].GetComponent<RectTransform>().localPosition = new Vector3(int.Parse(act[2]), objCharacter[i].GetComponent<RectTransform>().localPosition.y, 1);
        }
        if (act[1] == "jump")
        {
            yield return StartCoroutine(CharacterJump(i));
        }
        if (act[1] == "walk")
        {
            yield return StartCoroutine(CharacterWalk(i, int.Parse(act[2])));
        }
        if (act[1] == "dash")
        {
            yield return StartCoroutine(CharacterDash(i, int.Parse(act[2])));
        }
        if (act[1] == "delete")
        {
            objCharacter[i].GetComponent<Image>().enabled = false;
        }
    }

    private IEnumerator BGZoom(int x,int zoom,int time)//★未実装。x位置を中心にtimeフレームでズーム。zoomの値は％表記
    {
        yield return null;
    }

    private IEnumerator CharacterWalk(int chara,int length)
    {
        if (length > 0)
        {
            objCharacter[chara].GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            for (int i = 0; i < length; i++)
            {
                if (i % 50 == 0)
                {
                    objCharacter[i].GetComponent<Image>().sprite = null;//unity不具合回避

                    if ((i / 50) % 2 == 0)
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＷＡＬＫ");
                    }
                    else
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＷＡＬＫ２");
                    }
                }
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x + 1, objCharacter[chara].GetComponent<RectTransform>().localPosition.y, 1);
                yield return null;
            }
        }
        if (length < 0)
        {
            objCharacter[chara].GetComponent<RectTransform>().localScale = new Vector2(-1, 1);
            for (int i = 0; i < -length; i++)
            {
                    if (i % 50 == 0)
                    {
                    objCharacter[i].GetComponent<Image>().sprite = null;//unity不具合回避
                    if ((i / 50) % 2 == 0)
                        {
                            objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＷＡＬＫ");
                        }
                        else
                        {
                                objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＷＡＬＫ２");
                        }
                    }
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x - 1, objCharacter[chara].GetComponent<RectTransform>().localPosition.y, 1);
                yield return null;
            }
        }

    }

    private IEnumerator CharacterDash(int chara, int length)
    {
        if (length > 0)
        {
            objCharacter[chara].GetComponent<RectTransform>().localScale = new Vector2(1, 1);
            for (int i = 0; i < length; i++)
            {
                if (i % 50 == 0)
                {
                    objCharacter[i].GetComponent<Image>().sprite = null;//unity不具合回避
                    if ((i / 50) % 2 == 0)
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＤＡＳＨ");
                    }
                    else
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＤＡＳＨ２");
                    }
                }
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x + 2, objCharacter[chara].GetComponent<RectTransform>().localPosition.y, 1);
                yield return null;
            }
        }
        if (length < 0)
        {
            objCharacter[chara].GetComponent<RectTransform>().localScale = new Vector2(-1,1);
            for (int i = 0; i < -length; i++)
            {
                if (i % 50 == 0)
                {
                    objCharacter[i].GetComponent<Image>().sprite = null;//unity不具合回避
                    if ((i / 50) % 2 == 0)
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＤＡＳＨ");
                    }
                    else
                    {
                        objCharacter[i].GetComponent<Image>().sprite = Resources.Load<Sprite>(chara + "ＤＡＳＨ２");
                    }
                }
                objCharacter[chara].GetComponent<RectTransform>().localPosition = new Vector3(objCharacter[chara].GetComponent<RectTransform>().localPosition.x - 2, objCharacter[chara].GetComponent<RectTransform>().localPosition.y, 1);
                yield return null;
            }
        }

    }

    private void SetTime(int time)
    {
    if (time == 1)
    {
            objTime.GetComponent<Image>().sprite = null;//unity不具合回避
            objTime.GetComponent<Image>().sprite = Resources.Load<Sprite>("night");
            objTime.GetComponentInChildren<Text>().text = "Night";
            objBG[0].GetComponent<Image>().color = new Color(0.2f,0.2f,0.5f);
            objBG[1].GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.5f);
        }
    else {
            objTime.GetComponent<Image>().sprite = null;//unity不具合回避
            objTime.GetComponent<Image>().sprite = Resources.Load<Sprite>("day");
            objTime.GetComponentInChildren<Text>().text = "Day";
            objBG[0].GetComponent<Image>().color = new Color(1,1,1);
            objBG[1].GetComponent<Image>().color = new Color(1,1,1);
        }
    }

    private void StorySE(string SE,Utility u1)
    {
        int i;
        AudioSource[] sources=GetComponents<AudioSource>();
        for (i = 0; i < 5; i++)
        {
            if (!sources[i].isPlaying) {sources[i].clip = Resources.Load<AudioClip>(SE); sources[i].Play(); return; }
        }
        if (i== 5) { i= 0; sources[i].clip = Resources.Load<AudioClip>(SE); sources[i].Play(); }
    }


}




