using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor;

public class PuzzleSceneManager : MonoBehaviour
{


    //★基本事項★
    //ブロックの変数内数字について※10進法の各位に意味を持たせて処理している。
    //１の位（ブロックの色）
    //赤(Red)=1　青(Blue)=2　緑(Green)=3　黄(Yellow)=4
    //１０の位（ブロックが宙に浮いているか否か。）　

    //定数の宣言
    const int MAXPLAYERLIFE = 20;                //プレイヤーの最大lifepoint
    const int MAXENEMYLIFE = 20;                 //敵の最大lifepoint
    public const int WORLD_HEIGHT = 5;                 //フィールドの縦の長さ
    public const int WORLD_WIDTH = 5;                   //フィールドの横の長さ
    public const int BLOCKTYPE_NUM = 4;                 //ブロックの色の種類数
    const int BLOCK_SIZE = 130;                   //ブロックの大きさ（ピクセル）
    const int FIELD_LEFT = -315;                 //フィールドの左端（ピクセル）
    const int FIELD_TOP = 315;                   //フィールドの上端（ピクセル）
    public const int DECKCARD_NUM = 20;                 //デッキの枚数
    public const int HAND_NUM = 3;                      //手札の枚数
    const int SOUND_NUM = 16;                    //効果音の種類数
    const int SPELL_TIME = 60;                   //呪文カットインの演出時間（フレーム）
    const int ATTACK_TIME = 60;                  //シュジンコウの攻撃演出時間（フレーム）
    const int CARD_WIDTH = 90;                   //カードの幅
    const int PLAYER_FOLLOWER_POSITION_X = -485; //プレイヤー側シュジンコウの基本位置X座標
    const int PLAYER_FOLLOWER_POSITION_Y = -130;   //プレイヤー側シュジンコウの基本位置Y座標
    const int ENEMY_FOLLOWER_POSITION_X = 485;   //敵側シュジンコウの基本位置X座標
    const int ENEMY_FOLLOWER_POSITION_Y = -130;    //敵側シュジンコウの基本位置Y座標
    const int MANA_POSITION_X = -485;            //ブロック消去演出でマナが集まる位置X座標
    const int MANA_POSITION_Y = -160;             //ブロック消去演出でマナが集まる位置Y座標
    const int PLAYER_LIFE_POSITION_X = -480;     //プレイヤー側ＬＰ表示のX座標
    const int PLAYER_LIFE_POSITION_Y = 70;       //プレイヤー側ＬＰ表示のY座標
    const int ENEMY_LIFE_POSITION_X = 480;       //敵側ＬＰ表示のX座標
    const int ENEMY_LIFE_POSITION_Y = 70;        //敵側ＬＰ表示のY座標
    const int PLAYER_CARD_X = -537;              //プレイヤー側手札０のX位置
    const int ENEMY_CARD_X = 294;                //敵側手札０のX位置
    const int CARD_Y = -290;                      //手札のY位置
    const int PLAYER_LIBRARY_X = -380;           //プレイヤー側ライブラリのX位置
    const int ENEMY_LIBRARY_X = 380;             //敵側ライブラリのX位置
    const int LIBRARY_Y = -80;                   //ライブラリのY位置
    const int SUMMON = 1;                        //第０種呪文と第２種呪文においてスキル種別（召喚）を表す。
    const int COUNTER = 2;                       //第２種呪文においてスキル種別（カウンター）を表す。
    const int DECK_EAT = 3;                      //第２種呪文においてスキル種別（デッキ破壊）を表す。
    const int HAND_CHANGE = 4;                   //第２種呪文においてスキル種別（手札交換）を表す。            
    const int OTHER = 10000;                     //第２種呪文において、スキル種別（その他）を表す。
    const int OWN = 1;                           //第１種呪文においてスキル種別（対象：自身のシュジンコウ）を現す。
    const int YOURS = 2;                         //第１種呪文においてスキル種別（対象：相手のシュジンコウ）を現す。
    const int PACE_BLOCK_MOVE = 15;              //フィールドブロックの落下ペース

    //変数の宣言
     private int chainCountForDraw;          //チェイン演出用連鎖数（連鎖数リセット後も最後の連鎖の連鎖数はしばらく表示する必要があるため）（-1はブレイク状態を表す）
    public int[,] blockMoveTime=new int[WORLD_WIDTH, WORLD_HEIGHT];              //フィールドブロックの落下時間カウント。一定周期で落下。
    private int chainEffectTime;            //chainの演出時間管理変数。
    public bool winloseFlag;               //勝ち負けが決まったか否か
    private int[] playerEliminatBlockCount = new int[BLOCKTYPE_NUM + 1];        //消えたブロックの色と数をカウント※[]内の数が色を現す。０は使わないが１～５までを使うので要素数は（０を含め）６個。
    private int[,] bufferBlock = new int[WORLD_WIDTH, WORLD_HEIGHT];            //一時的にブロックの情報を仮置きするための配列。消去判定の際にブロックの消去フラグを保存したり、連鎖判定用の落下後予測の際に落下後のブロック配置を代入する。 
    private int timeCount;
    public bool turnEndButtonPush;
    public bool turnProcess;
    public bool pauseFlag=false;
    private int nowPageStatusEffect=1;
    private int changeStatusEffect = 0;
    private int statusEffectViewPlayer = 0;
    public bool[] libraryOutFlag = new bool[2];                                //ライブラリアウトが発生したかの判定
    public int chainCount;                                                     //連鎖数
    private bool[,] lastbrightblock = new bool[WORLD_WIDTH, WORLD_HEIGHT];
    private int[,] brokenblock = new int[WORLD_WIDTH, WORLD_HEIGHT];            //消去演出中か否か、演出中なら何コマ目(+2)か。
    public bool[] phaseSkipFlag = new bool[5];                                 //フェイズを飛ばすかどうかのフラグ
    public int[] waitCount = new int[2];                                       //通信待機をここまで何回行ったか
    public int[] libraryNum = new int[2];                                      //ライブラリの残り枚数
    public int[] followerDamage = new int[2];                                  //シュジンコウに与えられるダメージ。
    public int[] lifePoint = new int[2];                                       //lifepoint。０がプレイヤーで１がエネミー。
    public int[] enemyGetManaPace = new int[BLOCKTYPE_NUM + 1];                //敵が各マナについて取得するペース
    public Card[,] handCard = new Card[2, HAND_NUM];                           //手札のカード
    public int[,] followerStatus = new int[2, 3];                              //シュジンコウのステータス。１次元目が所有者がプレイヤーか敵か。２次元目がステータス。（[,0]がAT、[,1]がDF、[,2]が特殊効果※未実装）
    public int[,] block = new int[WORLD_WIDTH, WORLD_HEIGHT];                  //フィールドの各座標に置かれているブロックの色   
    public bool[,]deleteBlock = new bool[WORLD_WIDTH, WORLD_HEIGHT];
    public Card[,] library = new Card[2,DECKCARD_NUM];
    public bool blockfloat=false;
    private bool deleteeffect = false;
    public bool win = false;
    private GameObject objMatch;                                                             //通信用ゲームオブジェクト
    private GameObject objEliminatBlockParent;                                               //消去演出用オブジェクトの親オブジェクト
    private GameObject objForNextTurnTime;                                                   //ターンの残り時間のオブジェクトを代入
    private GameObject objWinLose;                                                           //勝敗演出のオブジェクトを代入
    private GameObject objChainCount;                                                        //連鎖数表示のゲームオブジェクトを代入する
    private GameObject objField;                                                             //フィールドオブジェクト
    private GameObject objBlockField;                                                        //ブロックを置く部分
    private Sprite[] blockImage = new Sprite[20];                                            //blockImageは各色のブロックの画像。（[]内は色と状態※block配列に入った数字と同じ）
    public Sprite[] mana = new Sprite[20];
    private GameObject[] objLifeDamage = new GameObject[2];                                  //第三種（攻撃）呪文演出のオブジェクト
    private GameObject[] objFollower = new GameObject[2];                                    //シュジンコウのゲームオブジェクトを代入する配列
    private GameObject[] objLibrary = new GameObject[2];                                     //ライブラリのゲームオブジェクトを代入する配列
    private GameObject[] objFollowerDamage = new GameObject[2];                              //シュジンコウに与えられているダメージを表示するオブジェクト
    private GameObject[] objLifePoint = new GameObject[2];                                   //lifepointのゲームオブジェクトを代入する(0がＰＬ、1が敵）
    private GameObject[,] objFieldBlock = new GameObject[WORLD_WIDTH, WORLD_HEIGHT];         //objはフィールドブロックのゲームオブジェクト（描画されるブロック）を代入する配列。
    private GameObject[,] objFollowerStatus = new GameObject[2, 2];                          //シュジンコウのATDFのゲームオブジェクトを代入する配列
    private GameObject[,] objEliminatBlock = new GameObject[WORLD_WIDTH, WORLD_HEIGHT];      //各ブロックの消去時演出用オブジェクト
    private GameObject[,,] objCardMana = new GameObject[2, HAND_NUM, 6];     //カードの残り必要マナ表示

    public GameObject objStatusViewCancelButton;
    public GameObject objStatusEffect;
    public GameObject[,] objCard = new GameObject[2, HAND_NUM];                             //objCardは手札のカードのゲームオブジェクト（描画されるブロック）を代入する配列。
    public Sprite[] brokenBlockSprite=new Sprite[20];
    public GameObject objTurnEndButton;
    public GameObject objSpellback;
    public List<AudioSource> seAudioSource = new List<AudioSource>();                      //効果音のオーディオソース（音程変更等で効果音ごとに触るので各効果音ごとに取得）
    public List<AudioClip> se = new List<AudioClip>();                                     //効果音のオーディオクリップ
    public List<Sprite> cardImage = new List<Sprite>();                                   //カードの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）
    private List<Sprite> followerImage = new List<Sprite>();                               //シュジンコウの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）
    private Sprite[] spellback = new Sprite[20];
    private System.Random rnd = new System.Random();                                         //乱数を生成。
    public CardData c1;
    public Utility u1;
    public List<StatusEffect>[] statusEffect = new List<StatusEffect>[2];

    public int[] maxmana = new int[5];
    public bool cardErase = false;

    // Use this for initialization
    void Start()
    {
        statusEffect[0] = new List<StatusEffect>();
        statusEffect[1] = new List<StatusEffect>();
        c1 = GetComponent<CardData>();
        u1 = GetComponent<Utility>();
        waitCount[0] = 0;
        waitCount[1] = 0;
        StartCoroutine(MainGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //ゲーム本体
    public IEnumerator MainGame()
    {
        yield return StartCoroutine(StartSetting());
        while (true)
        {
            //フレームごとに①入力処理とその反応(InputDelete)→②時間経過による動きとその反応(TimeFunc)→③描画(ScreenDraw)の流れを行う。
            if (!winloseFlag)//勝ち負け決定したら動かさない。
            {
                timeCount++;
                TimeFunc();
                DeleteBlock();
                ScreenDraw();
            }
            yield return null;
        }
    }

    private void DeleteBlock()
    {
        for (int i = 0; i < WORLD_WIDTH; i++)
        {
            for (int j = 0; j < WORLD_HEIGHT; j++)
            {
                if (deleteBlock[i, j] == true)
                {
                    StartCoroutine(BlockDeleteWait(i,j));
                }
                deleteBlock[i,j] = false;
            }
        }
    }

    private IEnumerator BlockDeleteWait(int i,int j)
    {
        deleteeffect = true;
        lastbrightblock[i, j] = true;
        if (chainCount > 0)
        {
            for (int k = 0; k < 25; k++)
            {
                yield return null;
            }
        }
        brokenblock[i, j] = 1;
        objFieldBlock[i, j].GetComponent<Image>().sprite = null;//unity不具合回避
        objFieldBlock[i, j].GetComponent<Image>().sprite = brokenBlockSprite[0];
        if (block[i, j] == 1) { objFieldBlock[i, j].GetComponent<Image>().color = new Color(1, 0, 0); }
        if (block[i, j] == 2) { objFieldBlock[i, j].GetComponent<Image>().color = new Color(0, 0, 1); }
        if (block[i, j] == 3) { objFieldBlock[i, j].GetComponent<Image>().color = new Color(0, 1, 0); }
        if (block[i, j] == 4) { objFieldBlock[i, j].GetComponent<Image>().color = new Color(1, 1, 0); }
        block[i, j] = 0;
        seAudioSource[2].PlayOneShot(se[2]);
        for (int k = j - 1; k >= 0; k--) { if (block[i, k] != 0) { if (block[i, k] < 10) { blockMoveTime[i, k] = 0; } block[i, k] = block[i, k] % 10 + 10; } }
        objTurnEndButton.SetActive(false);
        blockfloat = true;
        lastbrightblock[i, j] = false;
        deleteeffect = false;
    }

    //開始処理のコルーチン
    private IEnumerator StartSetting()
    {
        int i, j, k, l;

        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0) { objMatch = PhotonNetwork.Instantiate("MatchManager", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0); }


        //消去ブロックの演出オブジェクトを代入＆コピー
        objEliminatBlock[0, 0] = GameObject.Find("eliminatblock0").gameObject as GameObject;
        objEliminatBlockParent = GameObject.Find("eliminatblock").gameObject as GameObject;
        for (i = 0; i < WORLD_WIDTH; i++)
        {
            for (j = 0; j < WORLD_HEIGHT; j++)
            {
                if (i != 0 || j != 0) { objEliminatBlock[i, j] = Instantiate(objEliminatBlock[0, 0], objEliminatBlockParent.transform); }//演出用オブジェクトをコピー。[0,0]はコピー元なのでif文でコピー処理から外す。
            }
        }
        //フィールドブロックについて描画用オブジェクトを変数に代入。
        for (i = 0; i < WORLD_WIDTH; i++)
        {
            for (j = 0; j < WORLD_HEIGHT; j++)
            {//blockij(iとjは数字)に対応するobj配列にゲームオブジェクトを代入。
                objFieldBlock[i, j] = GameObject.Find("block" + i.ToString() + j.ToString()).gameObject as GameObject;
            }
        }

        //勝敗演出について描画用オブジェクトを変数に代入。
        objWinLose = GameObject.Find("winlose").gameObject as GameObject;

        //シュジンコウについて描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            objFollower[i] = GameObject.Find("follower" + i.ToString()).gameObject as GameObject;
            objFollowerDamage[i] = GameObject.Find("followerdamage" + i.ToString()).gameObject as GameObject;
        }

        //カードについて描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            for (j = 0; j < HAND_NUM; j++)
            {
                objCard[i, j] = GameObject.Find("card" + i.ToString() + j.ToString()).gameObject as GameObject;
            }
        }

        //カードの必要マナ表示のバーと数字についても描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            for (j = 0; j < HAND_NUM; j++)
            {
                for (k = 0; k < 6; k++)
                {
                    objCardMana[i, j, k] = GameObject.Find("cardmana" + i.ToString() + j.ToString() + (k+1).ToString()).gameObject as GameObject;
                }
            }
        }

        //連鎖数表示について描画用オブジェクトを変数に代入。
        objChainCount = GameObject.Find("chaincount").gameObject as GameObject;

        //フィールド表示について描画用オブジェクトを変数に代入。
        objField = GameObject.Find("Field").gameObject as GameObject;
        objBlockField= GameObject.Find("fieldback").gameObject as GameObject;

        //lifepoint表示について描画用オブジェクトを変数に代入
        for (i = 0; i < 2; i++)
        {
            objLifePoint[i] = GameObject.Find("lifepoint" + i.ToString()).gameObject as GameObject;
            objLifeDamage[i] = GameObject.Find("lifedamage" + i.ToString()).gameObject as GameObject;
        }

        //ライブラリ残り枚数表示について描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            objLibrary[i] = GameObject.Find("library" + i.ToString()).gameObject as GameObject;
        }

        //シュジンコウのステータスについて描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)//プレイヤーと敵
        {
            for (j = 0; j < 2; j++)//ATとDFで２つ
            {
                objFollowerStatus[i, j] = GameObject.Find("followerstatus" + i.ToString() + j.ToString()).gameObject as GameObject;
            }
        }

        //ブロック画像の読み込み
        blockImage[1] = Resources.Load<Sprite>("blockred");
        blockImage[2] = Resources.Load<Sprite>("blockblue");
        blockImage[3] = Resources.Load<Sprite>("blockgreen");
        blockImage[4] = Resources.Load<Sprite>("blockyellow");
        blockImage[11] = Resources.Load<Sprite>("blockred");
        blockImage[12] = Resources.Load<Sprite>("blockblue");
        blockImage[13] = Resources.Load<Sprite>("blockgreen");
        blockImage[14] = Resources.Load<Sprite>("blockyellow");

        for (i = 0; i < 20; i++)
        {
            if (i < 10) { brokenBlockSprite[i] = Resources.Load<Sprite>("broken_000" + i.ToString()); }
            else { brokenBlockSprite[i] = Resources.Load<Sprite>("broken_00" + i.ToString()); }
        }

        //マナ画像の読み込み
        mana[0]= Resources.Load<Sprite>("mana0");
        mana[1] = Resources.Load<Sprite>("mana1");
        mana[2] = Resources.Load<Sprite>("mana2");
        mana[3] = Resources.Load<Sprite>("mana3");
        mana[4] = Resources.Load<Sprite>("mana4");
        mana[11] = Resources.Load<Sprite>("mana11");
        mana[12] = Resources.Load<Sprite>("mana12");
        mana[13] = Resources.Load<Sprite>("mana13");
        mana[14] = Resources.Load<Sprite>("mana14");
        mana[15] = Resources.Load<Sprite>("mana15");

        //効果音読み込み
        for (i = 0; i < SOUND_NUM; i++)
        {
            seAudioSource.Add(gameObject.AddComponent<AudioSource>());
        }
        for (i = 0; i < seAudioSource.Count; i++)//リストの要素数の回数for文を回し、各要素にボリューム設定する。
        {
            seAudioSource[i].volume = PlayerPrefs.GetFloat("SEVolume", 0.8f);
        }
        se.Add(Resources.Load<AudioClip>("stone-break1"));
        se.Add(Resources.Load<AudioClip>("fire"));
        se.Add(Resources.Load<AudioClip>("glass-crack1"));
        se.Add(Resources.Load<AudioClip>("move"));
        se.Add(Resources.Load<AudioClip>("roll"));
        se.Add(Resources.Load<AudioClip>("up"));
        se.Add(Resources.Load<AudioClip>("down"));
        se.Add(Resources.Load<AudioClip>("attackdouble"));
        se.Add(Resources.Load<AudioClip>("attacksingle"));
        se.Add(Resources.Load<AudioClip>("winlose"));
        se.Add(Resources.Load<AudioClip>("summon"));
        se.Add(Resources.Load<AudioClip>("spell2"));
        se.Add(Resources.Load<AudioClip>("spellmiss"));
        se.Add(Resources.Load<AudioClip>("cure"));
        se.Add(Resources.Load<AudioClip>("slip"));
        se.Add(Resources.Load<AudioClip>("followerbreak"));

        //BGM読み込みと再生
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().b1.bgmChangeFlag == true)//trueの時のみ変更
        {
            GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("一気に進もう"));
        }
        else
        {
            GameObject.Find("BGMManager").GetComponent<BGMManager>().b1.bgmChangeFlag = true;//falseなら音楽は変えずにtrueに戻し、次回からまた変更されるようにする
        }
        //通信対戦の場合、データ送信前に相手側が確実にシーン移動できるように5秒待つ。
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0){ yield return new WaitForSeconds(5.0f); }
        for (i = 0; i < 2; i++)
        {
            yield return StartCoroutine(LibraryMake(i));//ライブラリ作成
        }

        //カード画像読み込み
        for (i = 0; i < c1.card.Count; i++)
        {
            cardImage.Add(null);//カード番号と同じ数だけ要素数を確保。（この手順がないと要素数が足りないとしてエラーが出る）
            followerImage.Add(null);
        }
        for (l = 0; l < 2; l++)
        {
            for (i = 1; i < c1.card.Count; i++)
            {
                for (j = 0; j < DECKCARD_NUM; j++)
                {
                    if (library[l, j].cardNum == i && cardImage[i] == null)
                    {
                        cardImage[i] = Resources.Load<Sprite>("card" + i.ToString());
                    }
                }
            }
        }
        for (int x = 0; x < 20; x++)
        {
            if (x < 10) { spellback[x]=Resources.Load<Sprite>("spellback_010" + x.ToString()); }
            else{ spellback[x]=Resources.Load<Sprite>("spellback_01" + x.ToString()); }
        }
        objFollower[0].GetComponent<Image>().sprite = null;//unity不具合回避
        objFollower[1].GetComponent<Image>().sprite = null;//unity不具合回避
        objFollower[0].GetComponent<Image>().sprite= Resources.Load<Sprite>("character50");
        objFollower[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("follower14");

        //ゲームの初期化
        InitGame();
        yield return StartCoroutine(WaitMatchData(300));//スタートタイミングを合わせる同期
        GameObject.Find("NowLoading").GetComponent<Image>().enabled=false;
    }

    //パズル部の変数の初期化
    private void PuzzleVariableSetting()
    {
        int i,l;
        lifePoint[0] = MAXPLAYERLIFE;
        lifePoint[1] = MAXENEMYLIFE;
        chainCount = 0;
        chainEffectTime = 90;//0~89はチェイン演出に使う。通常状態は90以上。
        timeCount = 0;
        winloseFlag = false;
        turnEndButtonPush = false;
        turnProcess = false;
        cardErase = false;
        objWinLose.gameObject.SetActive(false);

        //フェイズスキップフラグの初期化
        for (i = 0; i < 5; i++)
        {
            phaseSkipFlag[i] = false;
        }
        //シュジンコウの初期化
        for (l = 0; l < 2; l++)
        {
            for (i = 0; i < 3; i++)
            {
                followerStatus[l, i] = 0;
            }
            followerDamage[l] = 0;
        }

        for (i = 0; i < BLOCKTYPE_NUM + 1; i++) { playerEliminatBlockCount[i] = 0; }//iは色と対応させているのでi=0は使わない

        //相手のマナ獲得能力の代入。
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            enemyGetManaPace[i] = GetComponent<CardData>().enemyGetManaPace[i];
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0) { enemyGetManaPace[i]=0; }//対人戦では獲得ペースを最大にして実質獲得しないようにする
        }


    }


    // ゲームの初期化
    private void InitGame()
    {
        int i, j;
        //関連変数の初期化。
        PuzzleVariableSetting();

        // フィールドの初期化（ブロックの消去）
        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                block[j, i] = 0;
                blockMoveTime[i, j] = 0;
            }
        }
        //手札を３枚引く
        for (i = 0; i < HAND_NUM; i++)
        {
            DrawCard(0, i);//プレイヤー
            DrawCard(1, i);//敵
        }

        //ブロックを生成
        CreateNewBlock(true);
    }


    // 新しいブロックの生成
    private void CreateNewBlock(bool waitmode)
    {
        if (!winloseFlag) { StartCoroutine(FillBlank(waitmode)); }
    }

    private IEnumerator FillBlank(bool waitmode)
    {
        for (int i = WORLD_HEIGHT-1; i >= 0; i--)
        {
            for (int j = 0; j < WORLD_WIDTH; j++)
            {
                if (block[j,i]==0) { block[j, i] = Random.Range(1, BLOCKTYPE_NUM + 1); while(CheckEliminatBlock(true)){ block[j, i] = Random.Range(1, BLOCKTYPE_NUM + 1); } }
                if (waitmode) { seAudioSource[0].PlayOneShot(se[0]); yield return null; }
                blockMoveTime[i, j]=0;
            }
        }
    }


    // 時間カウント関係処理
    private void TimeFunc()
    {
        int i, j,k;
        if (!deleteeffect && !turnProcess) { MoveBlock(); }
        chainEffectTime++;

        LibraryOutCheck();//ライブラリアウトが起きたかの判定。ブレイクタイミングでの負けを即座に判定するために毎フレーム判定（判定自体も軽い）

        if (turnEndButtonPush && turnProcess==false)
        {
            //敵のマナ獲得
            for (j = 1; j < BLOCKTYPE_NUM + 1; j++)
            {
                //同色マナはどのカードにも同じ個数入るので、入る個数をまず決定。
                k=(int)(enemyGetManaPace[j]*(float)rnd.Next(0,20)/10);
                for (i = 0; i < HAND_NUM; i++)
                {
                    handCard[1,i].cardMana[j] += k;
                    if (handCard[1, i].cardMana[j] > handCard[1, i].cardCost[j]) { handCard[1, i].cardMana[0] += handCard[1, i].cardMana[j] - handCard[1, i].cardCost[j]; handCard[1, i].cardMana[j] = handCard[1, i].cardCost[j]; }
                }
            }

            StartCoroutine(TurnFunc());
        }

    }

    public void turnEndButton()
    {
        turnEndButtonPush = true;
    }

    private bool CheckEliminatBlock(bool checkonly)
    {
        int i, j;  // 汎用変数
        int eliminatFlag = 0;//消えるブロックが一つでもあるか否か

        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if (checkonly==false && block[j, i] > 10) { return false; }
            }
        }

        InitBufferBlock();
        // 各ブロックが消えるか調べる
        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                // ブロックが消えるかどうか調べて調査結果をバッファに保存する
                bufferBlock[j, i] = CheckEliminatBlockToOne(j, i,block);
            }
        }

        // 消えると判断されたブロックを消す	
        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if (bufferBlock[j, i] != 0)
                {
                    eliminatFlag = 1;
                    //消去
                    if (checkonly)
                    {

                    }
                    else
                    {
                        //消えたブロックの色と数をカウント※playerEliminatBlockCountは[]内の数が色を現す
                        playerEliminatBlockCount[block[j, i]] += 1;
                        playerEliminatBlockCount[0] += 1;
                        StartCoroutine(EliminatBlockEffect(j, i, block[j, i])); deleteBlock[j, i] = true; }
                    
                }
            }
        }
        if (checkonly)
        {
            if (eliminatFlag == 0) { return false; }//消えてなかったら０を返す
            return true;//一つでも消えていたら１を返す
        }
        if (eliminatFlag == 1)
        {
            chainCount++;
            chainEffectTime = 0;
            chainCountForDraw = chainCount;
            if (chainCountForDraw % 4 == 0)//音程がファ、ドなら
            {
                seAudioSource[2].pitch *= 1.059463f;//ピッチを半音階上げる。
            }
            else//それ以外なら
            {
                seAudioSource[2].pitch *= 1.122462f;//ピッチを１音階上げる。
            }
            if (chainCountForDraw == 1) { seAudioSource[2].pitch = 1.0f; }//連鎖数が１なら基本ピッチに戻す（ピッチの初期化）。（ピッチを上げるコードの前に書くと、１連鎖目からピッチが上がってしまう。それは動作として望ましくないのでこの位置）
        }//一つでも消えていればchainCountを増やす。

        //カードにマナを補充。
        for (i = 0; i < HAND_NUM; i++)
        {//playerCardmanaはプレイヤー(cardManaの１次元が「プレイヤー(0)」である)のカードに貯まっているマナの状況を管理する。２次元(i)が「何番目のカードか(0,1,2)」、３次元(j)が「何色のマナか」を表す。[0,0,2]なら自分の０番目のカードの青マナということ。
            for (j = 1; j < BLOCKTYPE_NUM + 1; j++) {
                int l = 1;
                for (int k = 0; k < chainCount; k++) { l *= chainCount; }
                handCard[0,i].cardMana[j] += playerEliminatBlockCount[j] * l;
                if (handCard[0, i].cardMana[j] > handCard[0, i].cardCost[j]) { handCard[0, i].cardMana[0] += handCard[0, i].cardMana[j] - handCard[0, i].cardCost[j]; handCard[0, i].cardMana[j] = handCard[0, i].cardCost[j]; }
            }
        }
        for (j = 1; j < BLOCKTYPE_NUM + 1; j++)
            {
                int l = 1;
                for (int k = 0; k < chainCount; k++) { l *= chainCount; }
                maxmana[j] += playerEliminatBlockCount[j] * l;
            }


        for (i = 0; i < BLOCKTYPE_NUM + 1; i++) { playerEliminatBlockCount[i] = 0; }
        if (eliminatFlag == 0) { return false; }//消えてなかったら０を返す
        return true;//一つでも消えていたら１を返す
    }


    // 特定ブロックが消えるか探索
    int CheckEliminatBlockToOne(int x, int y,int[,] Block)
    {
        int CheckBlock;
        int i;
        int BlockNum;

        if (Block[x, y] == 0 || Block[x,y]>10) { return 0; }
        // チェックするブロックの種類を保存
        CheckBlock = Block[x,y];


        // 左右にどれだけつながっているか調べる
        for (i = 0; x + i >= 0 && Block[x + i,y] == CheckBlock; i--) { }
        i++;
        for (BlockNum = 0; x + i < WORLD_WIDTH && Block[x + i,y] == CheckBlock; BlockNum++, i++) { }

        // ３つ以上つながっていたらここで終了
        if (BlockNum >= 3) return 1;


        // 上下にどれだけつながっているか調べる
        for (i = 0; y + i >= 0 && Block[x,y + i] == CheckBlock; i--) { }
        i++;
        for (BlockNum = 0; y + i < WORLD_HEIGHT && Block[x,y + i] == CheckBlock; BlockNum++, i++) { }

        // ３つ以上つながっていたらここで終了
        if (BlockNum >= 3) return 1;


        // ここまで来ていたら消えない
        return 0;
    }


    // 一時使用用ブロックデータの初期化
    private void InitBufferBlock()
    {
        int i, j;

        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                bufferBlock[j, i] = 0;
            }
        }
    }
    

    //フィールドブロックの落下関数
    private void MoveBlock()
    {
        int newY, j, i;
        int floatBlock=0;//浮いているブロック
        int lockBlock = 0;//固定されたブロック

            for (i = WORLD_HEIGHT - 1; i > -1; i--)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                    if (block[j, i] > 9)//ブロックが10以上の（つまり浮いている）時
                    {
                    blockMoveTime[j, i]++;
                    floatBlock++;
                    if (blockMoveTime[j, i] % PACE_BLOCK_MOVE == 0)
                    {
                        // 移動後の座標をセットする
                        newY = i + 1;
                        if (newY >= WORLD_HEIGHT)//移動後のブロックがフィールド下端に達しているか調べる
                        {
                            block[j, newY] = block[j, i] % 10;
                            block[j, i] = 0;
                            blockMoveTime[j, i] = 0;
                            lockBlock++;
                        }
                        else
                        {
                            // 移動後のブロックが画面上のブロックとかぶらないか調べる
                            if (block[j, newY] != 0)
                            {
                                if (block[j, newY] > 10) {
                                //浮いてるブロックとかぶる時は、移動させずにblockMoveTimeを相手に合わせる
                                blockMoveTime[j, i] = blockMoveTime[j, newY];
                                }
                                else
                                {
                                    // あたっていたらブロックを固定する
                                    block[j, newY] = block[j, i] % 10;
                                    block[j, i] = 0;
                                    blockMoveTime[j, i] = 0;
                                    lockBlock++;
                                }
                            }
                            else
                            {// あたっていなければ座標を移動する
                                block[j, newY] = block[j, i];
                                blockMoveTime[j, newY] = 0;
                                block[j, i] = 0;
                                deleteBlock[j, newY] = deleteBlock[j,i];//追加
                                deleteBlock[j, i] = false;//追加
                                blockMoveTime[j, i] = 0;
                                //移動後に動く余地がなければ即座に固定。
                                if (newY + 1 >= WORLD_HEIGHT)//移動後のブロックがフィールド下端に達しているか調べる
                                {
                                    block[j, newY] = block[j, newY] % 10;
                                    lockBlock++;
                                }
                                else
                                {
                                    // 移動後のブロックが画面上の固定ブロックに接していないか調べる
                                    if (block[j, newY + 1] != 0 && block[j, newY + 1] < 10)
                                    {
                                        // あたっていたらブロックを固定する
                                        block[j, newY] = block[j, newY] % 10;
                                        lockBlock++;
                                    }
                                }

                            }
                        }
                    }
                };
            };
        }
        if (lockBlock > 0) { seAudioSource[0].PlayOneShot(se[0]); };
        if (lockBlock == floatBlock) {   if (!CheckEliminatBlock(false)) { chainCount = 0;objTurnEndButton.SetActive(true); blockfloat = false; }}
        // 終了
        return;
    }





    //画面描画のための画像関連処理（ターン処理に伴うものを除く）
    private void ScreenDraw()
    {
        int i, j, k, l,m,manacalc,manacalccount,nowmanacalc;
        float enoughdark,enoughbright;
        //フィールドブロックの描画
        //block[i,j]が０ならimage[i,j]を非表示にする
        for (i = 0; i < WORLD_WIDTH; i++)
        {
            for (j = 0; j < WORLD_HEIGHT; j++)
            {
                if (block[i, j] == 0)
                {
                    if (brokenblock[i,j]>0)
                    {
                        brokenblock[i, j]++;
                        if (brokenblock[i, j] < 22) { objFieldBlock[i, j].GetComponent<Image>().sprite = null;//unity不具合回避
                            objFieldBlock[i, j].GetComponent<Image>().sprite = brokenBlockSprite[brokenblock[i, j] - 2]; }
                        else{
                            objFieldBlock[i, j].GetComponent<Image>().enabled = false; brokenblock[i, j] = 0;
                        }
                    }
                    else
                    {
                        objFieldBlock[i, j].GetComponent<Image>().enabled = false;
                    }
                }
                else
                {//block[i,j]が０でないなら（0の時にこの代入をするとblockImage[0]が未定義なのでおかしくなる）
                 //block[i,j]が０でないならimage[i,j]を表示する
                    objFieldBlock[i, j].GetComponent<Image>().enabled = true;
                    objFieldBlock[i, j].GetComponent<Image>().sprite = null;//unity不具合回避
                    objFieldBlock[i, j].GetComponent<Image>().sprite = blockImage[block[i, j]];
                    if (deleteeffect) {
                        if (lastbrightblock[i,j]) { objFieldBlock[i, j].GetComponent<Image>().color = new Color(0.9f+Mathf.Sin((float)timeCount / 3)*0.1f, 0.9f+ Mathf.Sin((float)timeCount / 3) * 0.1f, 0.9f+ Mathf.Sin((float)timeCount / 3) * 0.1f); }
                        else {
                            objFieldBlock[i, j].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f);
                        }
                    }
                    else
                    {
                        objFieldBlock[i, j].GetComponent<Image>().color = new Color(1, 1, 1);
                    }
                    if (block[i,j]>10) { objFieldBlock[i, j].GetComponent<RectTransform>().localPosition= new Vector3(130*i-260, 260-130*j-(blockMoveTime[i,j]%PACE_BLOCK_MOVE)*BLOCK_SIZE/PACE_BLOCK_MOVE, 0); }
                    else { objFieldBlock[i, j].GetComponent<RectTransform>().localPosition = new Vector3(130 * i - 260, 260-130 * j, 0); }
                }
            }
        }

        //連鎖中はブロックフィールドを暗くする
        if (blockfloat)
        {
            objBlockField.GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f);
        }
        else
        {
            objBlockField.GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }

        //ステータスの描画
        //連鎖数表示
        if (chainCountForDraw != 0 && chainEffectTime < 90)
        {

            objChainCount.GetComponent<Image>().enabled = true;
            objChainCount.GetComponent<Image>().sprite = null;
            if (chainCountForDraw==1) { objChainCount.GetComponent<Image>().sprite = mana[11]; }
            if (chainCountForDraw == 2) { objChainCount.GetComponent<Image>().sprite = mana[12]; }
            if (chainCountForDraw == 3) { objChainCount.GetComponent<Image>().sprite = mana[13]; }
            if (chainCountForDraw == 4) { objChainCount.GetComponent<Image>().sprite = mana[14]; }
            if (chainCountForDraw >= 5) { objChainCount.GetComponent<Image>().sprite = mana[15]; }
            objChainCount.GetComponent<Image>().color = new Color(0.95f,0.95f,0.95f,0.5f);
            objChainCount.GetComponentInChildren<Text>().enabled = true;
            objChainCount.GetComponentInChildren<Text>().text = "<color=red>" + chainCountForDraw.ToString() + "</color>" + "Chain";
            objChainCount.GetComponent<RectTransform>().localPosition = new Vector3(chainEffectTime, 0, 0);
        }
        else
        {
            objChainCount.GetComponentInChildren<Text>().enabled = false;
            objChainCount.GetComponent<Image>().enabled = false;
        }
        //ＬＰ表示
        if (lifePoint[0] >= 15) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=blue>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[0] >= 10 && lifePoint[0] < 15) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=orange>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[0] < 10) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=red>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[1] >= 15) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=blue>" + lifePoint[1].ToString() + "</color>"; }
        if (lifePoint[1] >= 10 && lifePoint[1] < 15) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=orange>" + lifePoint[1].ToString() + "</color>"; }
        if (lifePoint[1] < 10) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=red>" + lifePoint[1].ToString() + "</color>"; }

        //カードの描画
        for (i = 0; i < 2; i++)
        {
            //手札の描画
            for (j = 0; j < HAND_NUM; j++)
            {
                l = 0;
                m = 0;
                for (k = 0; k < BLOCKTYPE_NUM + 1; k++)
                {
                    
                    manacalc = handCard[i,j].cardCost[k];nowmanacalc = handCard[i,j].cardMana[k];
                    manacalccount = 4;

                        while (manacalccount >= 0 && m<6)
                        {
                            if (manacalc - (int)Mathf.Pow(3, manacalccount) >= 0)
                            {
                                manacalc -= (int)Mathf.Pow(3, manacalccount);
                                if (nowmanacalc - (int)Mathf.Pow(3, manacalccount) >= 0) { nowmanacalc -= (int)Mathf.Pow(3, manacalccount); enoughdark = 0;enoughbright = Mathf.Sin((float)timeCount/10)*0.7f; } else { enoughbright = 0; enoughdark = 0.68f; }
                                if (k == 0) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(0.7f-enoughdark+enoughbright/3,0.7f-enoughdark + enoughbright/3, 0.7f-enoughdark + enoughbright/3, 1); }
                                if (k == 1) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(1.0f-enoughdark, enoughbright, enoughbright, 1); }
                                if (k == 2) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(enoughbright, enoughbright, 1.0f-enoughdark, 1); }
                                if (k == 3) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(enoughbright, 1.0f-enoughdark, enoughbright, 1); }
                                if (k == 4) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(1.0f-enoughdark, 1.0f-enoughdark, enoughbright, 1); }                
                                m++;
                            }
                            else { manacalccount--; }
                        }
                    if (handCard[i,j].cardCost[k] > handCard[i,j].cardMana[k])
                    {
                    }
                    else
                    {
                        l++;
                    }
                }
                if (l == BLOCKTYPE_NUM+1) { objCard[i, j].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f - 0.3f * (Mathf.Cos((float)timeCount / 10))); } else { objCard[i, j].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f); }//魔力が足りていたらカードは点滅する。
                if (handCard[i,j].useCard == true) { objCard[i, j].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f); }//使用確定カードなら暗くなる。
            }
            //ライブラリの描画
            objLibrary[i].GetComponentInChildren<Text>().text = libraryNum[i].ToString();
        }

        //シュジンコウの描画
        for (i = 0; i < 2; i++)
        {
                objFollower[i].gameObject.SetActive(true);
                for (j = 0; j < 2; j++)
                {
                    objFollowerStatus[i, j].gameObject.SetActive(true);
                    if (j == 0) { objFollowerStatus[i, j].GetComponent<Text>().text = "<color=red>" + followerStatus[i, j].ToString() + "</color>"; }//ATの表示
                    if (j == 1) { objFollowerStatus[i, j].GetComponent<Text>().text = "<color=blue>" + followerStatus[i, j].ToString() + "</color>"; }//DFの表示
                }
                if (followerDamage[i] == 0)
                {
                    objFollowerDamage[i].gameObject.SetActive(false);
                }
                else
                {
                    objFollowerDamage[i].gameObject.SetActive(true);
                    objFollowerDamage[i].GetComponent<Text>().text = "<color=red>" + followerDamage[i].ToString() + "</color><size=48>damage</size>";
                }
        }
    }

    

    //ライブラリのシャッフル（playerはプレイヤー（０）か敵（１）か）
    public void Shuffle(int player)
    {

        int i, j;
        Card tmp;
        int n = DECKCARD_NUM;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    tmp = library[player, k];
                    library[player, k] = library[player, n];
                    library[player, n] = tmp;
                }
            }
        }

    }
    //ライブラリからカードを引く（playerがプレイヤー(0)か敵(1)か、handが手札の何枚目か）
    public void DrawCard(int player, int hand)
    {
        int i;
        //通信対戦で相手側のドローなら相手側で処理してくれるのでスキップ（それ以外の場合は処理）
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay == 0 || player == 0)
        {
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                if (library[player, i] != null)
                {//ライブラリの上から順にカードがある（０でない）ところまで探していく。
                    handCard[player, hand] = library[player, i];//カードの種類を代入
                    DrawCardCost(player,hand);
                    library[player, i] = null;//カードを引いたのでライブラリから消す。
                    libraryNum[player]--;//ライブラリの残り枚数を1減らす。
                    if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
                    {
                        Match m1 = objMatch.GetComponent<Match>();
                        m1.DataChange();
                        //ドローは同期タイミング関係なく一方的に送信するのでwaitFlagは要らない。また、あるとbreak時に待ちぼうけになる（自分側でしかbreakの発生を認識できないため）。
                    }
                    return;//カードを代入したらそこで終わり。
                }
            }
            //ライブラリが切れていたら（ここまでreturnしてないということは引けるカードがないということ）クリアやゲームオーバー
            //ここではlibraryOutFlagだけを変え、勝敗コルーチンへの移動はフェイズ終了時にまとめてやる（状況を変える判定はフェイズ終了時にまとめる。通信対戦の問題と、同フェイズ中の処理順問題を避けるため）
            if (player == 0)//プレイヤーのライブラリ切れならゲームオーバー
            {
                libraryOutFlag[0] = true;
                if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
                {
                    Match m1 = objMatch.GetComponent<Match>();
                    m1.DataChange();
                    //ドローは同期タイミング関係なく一方的に送信するのでwaitFlagは要らない。また、あるとbreak時に待ちぼうけになる（自分側でしかbreakの発生を認識できないため）。
                }
            }
            if (player == 1)
            {
                libraryOutFlag[1] = true;
            }
        }
    }

    private void LibraryOutCheck()
    {
        if (libraryOutFlag[0] == true)//プレイヤーのライブラリ切れならゲームオーバー
        {
            StartCoroutine(Lose("-LibraryOut-"));
            return;
        }

        if (libraryOutFlag[1] == true)//敵のライブラリ切れならクリア
        {
            StartCoroutine(Win("-LibraryOut-"));
            return;
        }
    }


    //引いたカードのコスト表示
    public void DrawCardCost(int i,int j)
    {
        int m = 0;
        int manacalc,nowmanacalc,manacalccount;
        objCard[i, j].GetComponent<Image>().sprite = null;//unity不具合回避
        objCard[i, j].GetComponent<Image>().sprite = cardImage[handCard[i, j].cardNum];
        for (int k = 0; k < 6; k++) { objCardMana[i, j, k].SetActive(false); }
        for (int k = 0; k < BLOCKTYPE_NUM + 1; k++)
        {
            manacalc = handCard[i,j].cardCost[k]; nowmanacalc = handCard[i,j].cardMana[k];
            manacalccount = 4;
            while (manacalccount >= 0 && m < 6)
            {
                if (manacalc - (int)Mathf.Pow(3, manacalccount) >= 0)
                {
                    objCardMana[i, j, m].SetActive(true);
                    objCardMana[i, j, m].GetComponent<Image>().sprite = null;//unity不具合回避
                    objCardMana[i, j, m].GetComponent<Image>().sprite = mana[manacalccount];
                    manacalc -= (int)Mathf.Pow(3, manacalccount);
                    
                    if (k == 0) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(0.8f, 0.8f, 0.8f, 1); }
                    if (k == 1) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(1.0f, 0, 0, 1); }
                    if (k == 2) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(0, 0, 1.0f, 1); }
                    if (k == 3) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(0, 1.0f, 0, 1); }
                    if (k == 4) { objCardMana[i, j, m].GetComponent<Image>().color = new Color(1.0f, 1.0f, 0, 1); }
                    m++;
                }
                else { manacalccount--; }
            }
        }
    }






    //勝利
    private IEnumerator Win(string Case)
    {
        if (winloseFlag == false)//まだ勝ち負けが決まっていないなら（シーン移動が完了するまで並列でゲームは動き続けるので、winやlose関数が多重起動しないように）
        {
            win = true;
            yield return null;
            seAudioSource[9].PlayOneShot(se[9]);
            objWinLose.gameObject.SetActive(true);
            objWinLose.GetComponent<Text>().text = "<color=red><size=240>YOU WIN</size><size=144>\n" + Case + "</size></color>";
            winloseFlag = true;
            yield return new WaitForSeconds(3.0f);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                objMatch.GetComponent<Match>().MatchEnd();
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
            }
            else
            {
                yield return GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");//ストーリーシーンへ
            }
        }
    }


    //敗北
    private IEnumerator Lose(string Case)
    {
        if (winloseFlag == false)//まだ勝ち負けが決まっていないなら（シーン移動が完了するまで並列でゲームは動き続けるので、winやlose関数が多重起動しないように）
        {
            win = false;
            yield return null;
            seAudioSource[9].PlayOneShot(se[9]);
            objWinLose.gameObject.SetActive(true);
            objWinLose.GetComponent<Text>().text = "<color=blue><size=240>YOU LOSE</size><size=144>\n" + Case + "</size></color>";
            winloseFlag = true;
            yield return new WaitForSeconds(3.0f);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                objMatch.GetComponent<Match>().MatchEnd();
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
            }
            else
            {
                yield return GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");//ストーリーシーンへ
            }
        }
    }

    //ターン処理時の使用カード確定関数
    private void CardUse(int player)
    {
        int i, j, k;
        for (i = 0; i < HAND_NUM; i++)
        {
            k = 0;
            for (j = 0; j < BLOCKTYPE_NUM + 1; j++)
            {
                if (handCard[player,i].cardMana[j] < handCard[player,i].cardCost[j]) { k = 1; break; }//マナが足りていないのが判明した時点でそのカードについては処理終了。       
            }
            if (k == 0)                    //ここまでk=0のままならマナが全て足りているので使用確定。
            {
                handCard[player,i].useCard = true;//カードの使用確定
            }
        }
    }

    //２種呪文の処理関数
    private IEnumerator Spell2Func()
    {
        int i, l;
        for (l = 0; l < 2; l++)
        {
            for (i = 0; i < HAND_NUM; i++)
            {
                if (objCard[l, i].GetComponent<Image>().enabled && handCard[l,i].useCard == true && handCard[l,i].cardSkillDelegate!=null)                    //使用確定カードで２種なら発動。
                {
                    objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                    seAudioSource[11].PlayOneShot(se[11]);
                    yield return StartCoroutine(SpellEffect(l, i)); //呪文演出
                    OtherSpell(l, handCard[l, i].cardNum);//呪文効果
                }
            }
        }
    }

    private IEnumerator SpellbackDraw(int player)
    {
        for (int j = 0; j < 5; j++)
        {
            yield return null;
        }
        objSpellback.SetActive(true);
        if(player==0)objSpellback.GetComponent<Image>().color = new Color(0.4f,0.4f,1,1);
        if (player == 1) objSpellback.GetComponent<Image>().color = new Color(1, 0.4f, 0.4f, 1);
        for (int i = 0; i < 20; i++)
            {
                objSpellback.GetComponent<Image>().sprite = null;
                objSpellback.GetComponent<Image>().sprite = spellback[i];
                for (int j = 0; j < 5; j++) { yield return null;
                }
            }
        objSpellback.SetActive(false);
        for (int j = 0; j < 5; j++)
        {
            yield return null;
        }
    }


    //ターン処理関数
    private IEnumerator TurnFunc()
    {
        int i, l;
        objStatusEffect.SetActive(true);
        objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(-1280, 0);
        turnEndButtonPush = false;
        turnProcess = true;
        //第２種（特殊効果）呪文フェイズ→第１種（強化）呪文フェイズ→第３種（ダメージ）呪文フェイズ→シュジンコウ攻撃フェイズ→第０種（召喚呪文）フェイズの順で処理される。
        //使用カードの確定（自身）※送信処理前に確定させないと、同期待ち中に得たマナで使用されるカードが増えうる。
        while (cardErase == true) { yield return null; }
        CardUse(0);
        //通信対戦時の同期待ち（cardManaデータを一致させる）
        yield return StartCoroutine(WaitMatchData(300));//300フレームまで同期遅れを許容
        //使用カードの確定（相手）
        CardUse(1);
        //第２種呪文フェイズ
        //呪文妨害（呪文を妨害する呪文(COUNTER)は相互作用を発生させるので、COUNTER同士では影響を与えない効果に＋他呪文と隔離。この種別だけはフェイズスキップも無視する）


        if (phaseSkipFlag[0] == false)
        {
            yield return StartCoroutine(Spell2Func());
            LifePointCheck();
        }
        else
        {
            //フェイズスキップ時はスペルミス演出とカード消去のみ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (objCard[l, i].GetComponent<Image>().enabled && handCard[l,i].useCard == true && handCard[l, i].cardSkillDelegate != null)                    //使用確定カードで第二種呪文なら演出。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        StartCoroutine(SpellMiss(l));//詠唱失敗演出を詠唱演出に重ねる。
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                    }
                }
            }
        }

        if (phaseSkipFlag[1] == false)
        {
            //第１種呪文フェイズ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (objCard[l, i].GetComponent<Image>().enabled && handCard[l,i].useCard == true && (handCard[l, i].buff[0,0]!=0 || handCard[l, i].buff[0, 1] != 0 || handCard[l, i].buff[1, 0] != 0 || handCard[l, i].buff[1, 1] != 0))                    //使用確定カードで第一種呪文なら発動。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                        seAudioSource[6].PlayOneShot(se[6]);
                        if (l == 0)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    followerStatus[j, k] += handCard[l, i].buff[j, k];
                                }
                            }
                        }
                        if (l == 1)
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                for (int k = 0; k < 2; k++)
                                {
                                    if (j == 0) { followerStatus[1, k] += handCard[l, i].buff[1, k]; }
                                    if (j == 1) { followerStatus[0, k] += handCard[l, i].buff[0, k]; }//敵が打った時は敵味方が逆。
                                }
                            }
                        }
                    }
                }
                LifePointCheck();
            }
        }
        else
        {
            //フェイズスキップ時はスペルミス演出とカード消去のみ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (objCard[l, i].GetComponent<Image>().enabled && handCard[l, i].useCard == true && (handCard[l, i].buff[0, 0] != 0 || handCard[l, i].buff[0, 1] != 0 || handCard[l, i].buff[1, 0] != 0 || handCard[l, i].buff[1, 1] != 0))                    //使用確定カードで第一種呪文なら演出。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        StartCoroutine(SpellMiss(l));//詠唱失敗演出を詠唱演出に重ねる。
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                    }
                }
            }
        }

        if (phaseSkipFlag[2] == false)
        {
            //第３種呪文フェイズ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (objCard[l, i].GetComponent<Image>().enabled && handCard[l, i].useCard == true && handCard[l,i].damage>0)                    //使用確定カードで三種呪文なら発動。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                        seAudioSource[1].PlayOneShot(se[1]);
                        if (l == 0) { StartCoroutine(LifeDamage(1)); yield return StartCoroutine(Damage(1, handCard[l,i].damage)); }//呪文効果
                        if (l == 1) { StartCoroutine(LifeDamage(0)); yield return StartCoroutine(Damage(0, handCard[l, i].damage)); }//Damage()の第一引数はダメージを「受ける」キャラクターなのでlとDamageの第一引数は逆になる
                    }
                }
                LifePointCheck();
            }
        }
        else
        {
            //フェイズスキップ時はスペルミス演出とカード消去のみ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (objCard[l, i].GetComponent<Image>().enabled && handCard[l, i].useCard == true && handCard[l, i].damage > 0)                    //使用確定カードで第三種呪文なら演出。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        StartCoroutine(SpellMiss(l));//詠唱失敗演出を詠唱演出に重ねる。
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                   }
                }
            }
        }

        //状態異常処理
        for (l = 0; l < 2; l++)
        {
            List<int> tmp = new List<int>();
            bool tmpflag=false;
            for (int x = 0; x<statusEffect[l].Count; x++)
            {
                tmpflag = true;
                objStatusEffect.SetActive(true);
                if (statusEffect[l][x].restTurn > 0) { statusEffect[l][x].statusEffectDelegate(l); statusEffect[l][x].restTurn--; LifePointCheck(); } else { statusEffect[l][x].statusEndEffectDelegate(l); tmp.Add(x); LifePointCheck(); }
                yield return StartCoroutine(StatusEffectDraw(l, x));
            }
            for (int x = 0; x < tmp.Count; x++) { statusEffect[l].RemoveAt(tmp[x]); }
            if (tmpflag && l==0) { for (int x = 0; x < 50; x++) { yield return null; } }
        }
        for (l = 0; l < 2; l++)
        {
            followerDamage[l] = 0;//シュジンコウダメージのリセット
        }

        
        TurnEnd();//ターン終了処理
        //通信対戦時の同期待ち（cardManaデータを一致させる）
        yield return StartCoroutine(WaitMatchData(300));//300フレームまで同期遅れを許容
        for (int j = 0; j < WORLD_HEIGHT * WORLD_WIDTH; j++) { yield return null; }
        turnProcess = false;
        objStatusEffect.SetActive(false);
    }

    private void TurnEnd()
    {
        int l, i, j;

        //フェイズスキップフラグの初期化
        for (i = 0; i < 5; i++)
        {
            phaseSkipFlag[i] = false;
        }

        //使ったカードと消したカードは新しく引き直す。
        for (l = 0; l < 2; l++)
        {
            for (i = 0; i < HAND_NUM; i++)
            {
                if (objCard[l, i].GetComponent<Image>().enabled == false)//使ったカードも消したカードもこの時点まで来ると表示が消えている
                {
                    DrawCard(l, i);
                    for (j = 0; j < BLOCKTYPE_NUM + 1; j++) { handCard[l,i].cardMana[j] = 0; }//カードに貯まったマナはリセット
                    handCard[l,i].useCard = false;//新たに引いたので、使っていない状態になおす。
                    objCard[l, i].GetComponent<Image>().enabled = true;//引き直したので、非表示になっていたカードを表示しなおす。
                }
            }
        }
        CreateNewBlock(true);//空白部分にブロックを生成。
    }

    //ダメージ処理関数（playerがダメージを「受けた」のがプレイヤーか敵か、damageがダメージ量）
    public IEnumerator Damage(int player, int damage)
    {
            if (damage != 0) { yield return StartCoroutine("DamageEffect", player); }
            followerDamage[player] += damage; lifePoint[player] -= damage;
            if (damage != 0) { yield return StartCoroutine("FollowerDamageEffect", player); }
    }

    //第２種呪文効果
    public void OtherSpell(int player, int cardnum)
    {
        c1.card[cardnum].cardSkillDelegate(player);//特殊効果の入ったデリゲート配列を呼び出す
    }


    //呪文ダメージ演出(playerは呪文のダメージを受ける側)
    public IEnumerator LifeDamage(int player)
    {
        //playerの炎上演出をオンに
            objLifeDamage[player].GetComponent<Image>().enabled = true; objLifeDamage[player].GetComponent<Animator>().enabled = true;
            for (int i = 0; i < 30; i++) { yield return null; }//演出フレームが終わるまで待つ。
                                                               //演出をオフに戻す
            objLifeDamage[player].GetComponent<Image>().enabled = false; objLifeDamage[player].GetComponent<Animator>().enabled = false;
    }



    //ライフポイントチェック（０以下ならクリアやゲームオーバー）
    public void LifePointCheck()
    {
        if (lifePoint[0] <= 0)//プレイヤーのlifepointが０以下ならゲームオーバー
        {
            StartCoroutine(Lose("-LostLifePoint-"));
        }
        if (lifePoint[1] <= 0)//敵のlifepointが０以下ならクリア
        {
            StartCoroutine(Win("-LostLifePoint-"));
        }
    }

    //デッキをライブラリ（ゲームで使用するカード配列）に落とし込み、ライブラリをシャッフルする。（ライブラリの初期化）
    public IEnumerator LibraryMake(int player)
    {
        int i;
        if (player == 1) { c1.EnemyDeckList(); }
        //デッキをライブラリに代入
        if (player == 1 && GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
        {
            yield return StartCoroutine(WaitMatchData(300));//データ同期待ち
        }
        else
        {
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                library[player, i] = c1.deckCard[player, i].Clone();//カードの種類
            }
            Shuffle(player);//シャッフル
            yield return StartCoroutine(WaitMatchData(600));//データ同期待ち
        }
        libraryNum[player] = DECKCARD_NUM;
    }

    //呪文詠唱時演出
    public IEnumerator SpellEffect(int player, int hand)//playerがＰＬエネミー、handが手札の何枚目か。
    {
        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = null;//unity不具合回避
        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = cardImage[handCard[player, hand].cardNum];
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = null;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = objFollower[player].GetComponent<Image>().sprite;
        objStatusEffect.GetComponentsInChildren<Text>()[0].text = handCard[player, hand].cardExplain;
        objStatusEffect.GetComponentsInChildren<Text>()[1].text = "";
        objStatusEffect.GetComponentsInChildren<Text>()[2].text = "";
        StartCoroutine(SpellbackDraw(player));
        for (int i = 0; i < 5; i++)
        {
            if (player == 0) { objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(i * 1280 / 5 - 1280, 0); }
            if (player == 1) { objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(-i * 1280 / 5 + 1280, 0); }
            yield return null;
        }
        objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        seAudioSource[6].PlayOneShot(se[6]);
        for (int i = 0; i < 100; i++) {
            //呪文詠唱演出（詠唱アニメーション＋効果音）
            objStatusEffect.GetComponentsInChildren<RectTransform>()[1].sizeDelta = new Vector2(135+135*Mathf.Sin(i/3),360);
            yield return null;
        }
        objStatusEffect.GetComponentsInChildren<RectTransform>()[1].sizeDelta = new Vector2(270, 360);
        for (int i = 0; i < 5; i++)
        {
            if (player == 0) { objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(i * 1280 / 5, 0); }
            if (player == 1) { objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(-i * 1280 / 5, 0); }
            yield return null;
        }
        objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(1280, 0);
    }

    //シュジンコウダメージ演出
    //シュジンコウを揺らす。
    public IEnumerator FollowerDamageEffect(int player)
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            if (player == 0) { objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X - 5 + (10 * (i % 2)), PLAYER_FOLLOWER_POSITION_Y, 0); }
            if (player == 1) { objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X - 5 + (10 * (i % 2)), ENEMY_FOLLOWER_POSITION_Y, 0); }
            yield return null;
        }
    }

    //プレイヤー(player==0)orエネミー(player==1)ダメージ演出
    //ＬＰ表示欄を揺らす
    public IEnumerator DamageEffect(int player)
    {
        int i;
        for (i = 0; i < 30; i++)
        {
            if (player == 0) { objLifePoint[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_LIFE_POSITION_X - 5 + (10 * (i % 2)), PLAYER_LIFE_POSITION_Y, 0); }
            if (player == 1) { objLifePoint[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_LIFE_POSITION_X - 5 + (10 * (i % 2)), ENEMY_LIFE_POSITION_Y, 0); }
            yield return null;
        }
        if (player == 0) { objLifePoint[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_LIFE_POSITION_X, PLAYER_LIFE_POSITION_Y, 0); }
        if (player == 1) { objLifePoint[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_LIFE_POSITION_X, ENEMY_LIFE_POSITION_Y, 0); }
    }

    //ブロックの消去演出
    public IEnumerator EliminatBlockEffect(int x, int y, int color)
    {
        int i;
        objEliminatBlock[x, y].GetComponent<Image>().enabled = true;
        objEliminatBlock[x, y].GetComponent<Image>().sprite = null;
        //消えたブロックが何点のマナになったか表示(この時点ではchainCount増加はまだなので+1しておく)
        if (chainCount == 0) { objEliminatBlock[x, y].GetComponent<Image>().sprite = mana[0]; }
        if (chainCount == 1) { objEliminatBlock[x, y].GetComponent<Image>().sprite = mana[1]; }
        if (chainCount == 2) { objEliminatBlock[x, y].GetComponent<Image>().sprite = mana[2]; }
        if (chainCount == 3) { objEliminatBlock[x, y].GetComponent<Image>().sprite = mana[3]; }
        if (chainCount >= 4) { objEliminatBlock[x, y].GetComponent<Image>().sprite = mana[4]; }//4以上の時は過多過ぎて全部埋まるので4で統一でＯＫ

        if (color == 1) { objEliminatBlock[x, y].GetComponent<Image>().color = new Color(1,0,0); }//color==1
        if (color == 2) { objEliminatBlock[x, y].GetComponent<Image>().color = new Color(0, 0, 1); }//color==2
        if (color == 3) { objEliminatBlock[x, y].GetComponent<Image>().color = new Color(0, 1, 0); }//color==3
        if (color == 4) { objEliminatBlock[x, y].GetComponent<Image>().color = new Color(1, 1, 0); }//color==4

        for (i = 0; i < 30; i++)
        {//消去演出オブジェクトをプレイヤー表示へと集める
            objEliminatBlock[x, y].GetComponent<RectTransform>().localPosition = new Vector3(((x * BLOCK_SIZE + FIELD_LEFT) * (30 - i) + MANA_POSITION_X * i) / 30, ((FIELD_TOP - y * BLOCK_SIZE) * (30 - i) + MANA_POSITION_Y * i) / 30, 0);//プレイヤー表示(MANA_POSITION)にぶつかる
            yield return null;
        }

        //消去演出オブジェクトを消す。
        objEliminatBlock[x, y].GetComponent<Image>().enabled = false;
    }



    public IEnumerator ShakeDeck(int player)
    {
        for (int i = 0; i < 30; i++)
        {
            if (player == 0) { objLibrary[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_LIBRARY_X + 3 * (Mathf.Cos((float)timeCount / 1)), LIBRARY_Y); }
            if (player == 1) { objLibrary[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_LIBRARY_X + 3 * (Mathf.Cos((float)timeCount / 1)), LIBRARY_Y); }
            yield return null;
        }
        if (player == 0) { objLibrary[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_LIBRARY_X, LIBRARY_Y); }
        if (player == 1) { objLibrary[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_LIBRARY_X, LIBRARY_Y); }
    }

    public IEnumerator ShakeCard(int player, int hand)
    {
        for (int i = 0; i < 30; i++)
        {
            if (player == 0) { objCard[player, hand].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_CARD_X + (CARD_WIDTH + 30) * hand + 3 * (Mathf.Cos((float)timeCount / 1)), CARD_Y); }
            if (player == 1) { objCard[player, hand].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_CARD_X + (CARD_WIDTH + 30) * hand + 3 * (Mathf.Cos((float)timeCount / 1)), CARD_Y); }
            yield return null;
        }
        if (player == 0) { objCard[player, hand].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_CARD_X + (CARD_WIDTH + 30) * hand, CARD_Y); }
        if (player == 1) { objCard[player, hand].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_CARD_X + (CARD_WIDTH + 30) * hand, CARD_Y); }
    }

    //通信対戦用同期待ちコルーチン
    private IEnumerator WaitMatchData(int flame)
    {
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
        {
            Match m1 = objMatch.GetComponent<Match>();
            waitCount[0]++;
            m1.DataChange();
            for (int i = 0; i < flame; i++)
            {
                if (waitCount[0] == waitCount[1])
                {
                    yield break;
                }
                yield return null;
            }
            //待ってもレスポンスがなければ勝利扱いで終了。
            StartCoroutine(Win("-Disconnect-"));
        }
    }

    //呪文の立ち消え演出（対象がいない、多重召喚等で呪文に失敗した場合）
    public IEnumerator SpellMiss(int player)
    {
        int i;
        for (i = 0; i < SPELL_TIME - 30; i++)//カットイン消滅残り30Fまで（SPELL_TIME-30）は動かない。
        {
            yield return null;
        }
        //演出を入れる。
    }

    public void BlockPush(int x)
    {
        if (blockfloat) { return; }
            deleteBlock[x / 10, x % 10] = true;
    }

    public void CharacterPush(int x)
    {
        pauseFlag = true;
        if (statusEffect[x].Count == 0) { StartCoroutine(NoStatusEffectView(x)); return; }
        u1.flick = 0;
        StartCoroutine(StatusEffectView(x));
    }
    public IEnumerator NoStatusEffectView(int player)
    {
        objStatusEffect.SetActive(true);
        objStatusViewCancelButton.SetActive(true);
        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = null;
        objStatusEffect.GetComponentsInChildren<Image>()[1].color=new Color(1,1,1,0);
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = null;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = objFollower[player].GetComponent<Image>().sprite;
        objStatusEffect.GetComponentsInChildren<Text>()[0].text = "<size=48>状態異常はありません</size>";
        objStatusEffect.GetComponentsInChildren<Text>()[1].text = "";
        objStatusEffect.GetComponentsInChildren<Text>()[2].text = "0/0";
        while (pauseFlag)
        {
            yield return null;
        }
        objStatusEffect.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);

        objStatusEffect.SetActive(false);
        objStatusViewCancelButton.SetActive(false);
    }

    public IEnumerator StatusEffectView(int player)
    {
        int effectnum = 0;
        string explain;
        Sprite cardsprite;
        int restturn;
        int pagenum;
        int drawPage = 1;
        nowPageStatusEffect = 1;
        changeStatusEffect = 0;
        explain = statusEffect[player][effectnum].effectExplain;
        cardsprite = cardImage[statusEffect[player][effectnum].cardNum];
        restturn = statusEffect[player][effectnum].restTurn;
        pagenum = statusEffect[player].Count;
        statusEffectViewPlayer = player;
        //薄暗いRaycastオブジェクトで画面を覆ってゲーム画面に干渉できないようにする。（状態異常イメージの親がコレ。親が移動処理ベース）
        objStatusEffect.SetActive(true);
        objStatusViewCancelButton.SetActive(true);

        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = null;//unity不具合回避
        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = cardsprite;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = null;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = objFollower[player].GetComponent<Image>().sprite;
        objStatusEffect.GetComponentsInChildren<Text>()[0].text = explain;
        objStatusEffect.GetComponentsInChildren<Text>()[1].text = restturn.ToString();
        objStatusEffect.GetComponentsInChildren<Text>()[2].text = nowPageStatusEffect.ToString() + "/" + pagenum.ToString();
        while (pauseFlag){
            if (u1.flick==1 && nowPageStatusEffect < statusEffect[statusEffectViewPlayer].Count) { nowPageStatusEffect++; changeStatusEffect = -1; } else if (u1.flick==-1 && nowPageStatusEffect > 1) { nowPageStatusEffect--; changeStatusEffect = 1; }
            if (changeStatusEffect == 5 || changeStatusEffect == -5) {
                effectnum = nowPageStatusEffect - 1;
                explain = statusEffect[player][effectnum].effectExplain;
                cardsprite = cardImage[statusEffect[player][effectnum].cardNum];
                restturn = statusEffect[player][effectnum].restTurn;
                objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = null;//unity不具合回避
                objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = cardsprite;
                objStatusEffect.GetComponentsInChildren<Text>()[0].text = explain;
                objStatusEffect.GetComponentsInChildren<Text>()[1].text = restturn.ToString();
                objStatusEffect.GetComponentsInChildren<Text>()[2].text = nowPageStatusEffect.ToString() + "/" + pagenum.ToString();
                if (changeStatusEffect == 5) { changeStatusEffect = -5; } else { changeStatusEffect = 5; }
            }
            if (changeStatusEffect == 0) { drawPage = nowPageStatusEffect; }
            if (nowPageStatusEffect - drawPage > 0) { changeStatusEffect--; } else if(nowPageStatusEffect - drawPage < 0) { changeStatusEffect++; }
            objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(changeStatusEffect * 1280 / 5,0);
            yield return null;
        }
        objStatusEffect.SetActive(false);
        objStatusViewCancelButton.SetActive(false);
    }

    public void CharacterBackPush()
    {
        pauseFlag = false;
    }

    private IEnumerator StatusEffectDraw(int player,int effectnum)
    {
        string explain;
        Sprite cardsprite;
        int restturn;
            explain=statusEffect[player][effectnum].effectExplain;
            cardsprite = cardImage[statusEffect[player][effectnum].cardNum];
            restturn = statusEffect[player][effectnum].restTurn;

        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = null;//unity不具合回避
        objStatusEffect.GetComponentsInChildren<Image>()[1].sprite = cardsprite;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = null;
        objStatusEffect.GetComponentsInChildren<Image>()[2].sprite = objFollower[player].GetComponent<Image>().sprite;
        objStatusEffect.GetComponentsInChildren<Text>()[0].text = explain;
        objStatusEffect.GetComponentsInChildren<Text>()[1].text = restturn.ToString();
        objStatusEffect.GetComponentsInChildren<Text>()[2].text = (effectnum+1).ToString() + "/" + statusEffect[player].Count.ToString();
        //5fで入場
        for (int i = 0; i < 5; i++)
        {
            objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(1280 - 128*2 * i, 0);
            yield return null;
        }
        //50f中央表示
        for (int i = 0; i < 50; i++)
        {
            objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
            if (restturn == 0) {
                //カード画像が点滅しながら消える。
                float sin=Mathf.Sin((float)i/10);
                objStatusEffect.GetComponentsInChildren<Image>()[1].color = new Color(1,1,1,sin*(50-i)/50);
            }
            yield return null;
        }
        //5fで退場
        for (int i = 0; i < 5; i++)
        {
            objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(-128*2 * i, 0);
            yield return null;
        }
        objStatusEffect.GetComponent<RectTransform>().localPosition = new Vector2(-1280, 0);
        objStatusEffect.GetComponentsInChildren<Image>()[1].color = new Color(1, 1, 1, 1);
    }




}