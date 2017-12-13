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
    //赤(Red)=1　青(Blue)=2　緑(Green)=3　黒(Black)=4　黄(Yellow)=5
    //１０の位（ブロックが宙に浮いているか否か）　※このシステムはブロック落下中も関係なく消去判定を行うので区別は必須。落ちている途中のブロックが隣り合ったからといって消えてはいけない
    //着地している（消去判定の対象となる）=0　　浮いている（消去判定の対象とならない）=1　　固定された（フィールドブロックに代入された）ばかりのアクティブブロック=2(固定されたばかりを2として独立させる理由は一瞬でも0にすると浮いていても消去判定の対象になってしまうこと、1にするとアクティブブロック固定の際に着地判定まで発生してしまうので不要な着地演出が行われてしまう)

    //定数の宣言
    const int MAXPLAYERLIFE = 20;                //プレイヤーの最大lifepoint
    const int MAXENEMYLIFE = 20;                 //敵の最大lifepoint
    const int WORLD_HEIGHT = 13;                 //フィールドの縦の長さ
    const int WORLD_WIDTH = 7;                   //フィールドの横の長さ
    const int WAIT_NEXT = 20;                    //ブロック固定後の硬直時間
    const int PACE_ACTIVE_BLOCK_MOVE = 100;      //アクティブブロックの落下ペース
    const int PACE_BLOCK_MOVE = 14;              //フィールドブロックの落下ペース
    const int BLOCKTYPE_NUM = 5;                 //ブロックの色の種類数
    const int BLOCK_SIZE = 60;                   //ブロックの大きさ（ピクセル）
    const int FIELD_LEFT = -199;                 //フィールドの左端（ピクセル）
    const int FIELD_TOP = 370;                   //フィールドの上端（ピクセル）
    const int TURN_TIME = 600;                   //１ターンのフレーム数
    const int DECKCARD_NUM = 20;                 //デッキの枚数
    const int HAND_NUM = 3;                      //手札の枚数
    const int SKILL_TYPE = 4;                    //カードのスキルタイプの数
    const int CARD_ALL = 100;                    //カードの全種類数
    const int SOUND_NUM = 16;                    //効果音の種類数
    const int SPELL_TIME = 60;                   //呪文カットインの演出時間（フレーム）
    const int ATTACK_TIME = 60;                  //シュジンコウの攻撃演出時間（フレーム）
    const int ROTATION_TIME = 6;                 //ブロックの回転にかかる時間（フレーム）
    const int FOLLOWER_BREAK_TIME = 60;          //シュジンコウの破壊演出時間（フレーム）
    const int CARD_WIDTH = 90;                   //カードの幅
    const int PLAYER_FOLLOWER_POSITION_X = -330; //プレイヤー側シュジンコウの基本位置X座標
    const int PLAYER_FOLLOWER_POSITION_Y = 40;   //プレイヤー側シュジンコウの基本位置Y座標
    const int ENEMY_FOLLOWER_POSITION_X = 300;   //敵側シュジンコウの基本位置X座標
    const int ENEMY_FOLLOWER_POSITION_Y = 40;    //敵側シュジンコウの基本位置Y座標
    const int MANA_POSITION_X = -500;            //ブロック消去演出でマナが集まる位置X座標
    const int MANA_POSITION_Y = 320;             //ブロック消去演出でマナが集まる位置Y座標
    const int PLAYER_LIFE_POSITION_X = -500;     //プレイヤー側ＬＰ表示のX座標
    const int PLAYER_LIFE_POSITION_Y = 80;       //プレイヤー側ＬＰ表示のY座標
    const int ENEMY_LIFE_POSITION_X = 650;       //敵側ＬＰ表示のX座標
    const int ENEMY_LIFE_POSITION_Y = 80;        //敵側ＬＰ表示のY座標
    const int PLAYER_SPELL_POSITION_X = -390;    //プレイヤー側詠唱演出の基本位置X座標
    const int PLAYER_SPELL_POSITION_Y = 70;      //プレイヤー側詠唱演出の基本位置Y座標
    const int ENEMY_SPELL_POSITION_X = 390;      //敵側詠唱演出の基本位置X座標
    const int ENEMY_SPELL_POSITION_Y = 70;       //敵側詠唱演出の基本位置Y座標
    const int PLAYER_TARAI_POSITION_X = 70;      //プレイヤー側タライ演出の基本位置X座標
    const int PLAYER_TARAI_POSITION_Y = 350;     //プレイヤー側タライ演出の基本位置Y座標
    const int ENEMY_TARAI_POSITION_X = -120;     //敵側タライ演出の基本位置X座標
    const int ENEMY_TARAI_POSITION_Y = 350;      //敵側タライ演出の基本位置Y座標
    const int PLAYER_CARD_X = -537;              //プレイヤー側手札０のX位置
    const int ENEMY_CARD_X = 294;                //敵側手札０のX位置
    const int CARD_Y = 320;                      //手札のY位置
    const int PLAYER_LIBRARY_X = -615;           //プレイヤー側ライブラリのX位置
    const int ENEMY_LIBRARY_X = 615;             //敵側ライブラリのX位置
    const int LIBRARY_Y = 320;                   //ライブラリのY位置
    const int SUMMON = 1;                        //第０種呪文と第２種呪文においてスキル種別（召喚）を表す。
    const int COUNTER = 2;                       //第２種呪文においてスキル種別（カウンター）を表す。
    const int DECK_EAT = 3;                      //第２種呪文においてスキル種別（デッキ破壊）を表す。
    const int HAND_CHANGE = 4;                   //第２種呪文においてスキル種別（手札交換）を表す。            
    const int OTHER = 10000;                     //第２種呪文において、スキル種別（その他）を表す。
    const int OWN = 1;                           //第１種呪文においてスキル種別（対象：自身のシュジンコウ）を現す。
    const int YOURS = 2;                         //第１種呪文においてスキル種別（対象：相手のシュジンコウ）を現す。

    //ボタン入力管理用定数
    //KeyInputでは以下に定義する定数とkeyとを＆演算子で比較して両者１ならＴＲＵＥを返す形で押されているかを判定する。
    const int INPUT_A = 1;                       //Aボタンの入力
    const int INPUT_B = 2;                       //Bボタンの入力
    const int INPUT_DOWN = 4;                    //下ボタンの入力
    const int INPUT_RIGHT = 8;                   //右ボタンの入力
    const int INPUT_LEFT = 16;                   //左ボタンの入力

    //変数の宣言
    private int activeType;                 //アクティブブロックの回転配置。0はactiveBlock[1]が上にある場合。1は右にある場合、2は下にある場合、3は左にある場合。
    private int beforeActiveType;           //アクティブブロックの回転配置が、回転前はどこだったか。回転描画の際に使用。
    private int autoRepeatKeyTime;          //押しっぱなしを連打扱いで入力処理するために、押しっぱなしの時間をカウントするための変数。アクティブブロックの横移動に使用。
    private int rotationEffectTime;         //回転演出の時間カウント変数。回転演出描画に使用。
    private int chainCountForDraw;          //チェイン演出用連鎖数（連鎖数リセット後も最後の連鎖の連鎖数はしばらく表示する必要があるため）（-1はブレイク状態を表す）
    private int blockAutoMoveTime;          //ブロックの自動落下のための時間カウント変数。
    private int downKeyBlankTime;           //ブロック高速落下の待機時間用変数。（下押しっぱなし時の高速落下を２フレームに１度にするための変数）。0が下キー押し直後、1が落下演出状態、2以降の数字が自然状態。
    private int blockMoveTime;              //フィールドブロックの落下時間カウント。一定周期で落下。
    private int activeX;                    //アクティブブロックの横位置
    private int activeY;                    //アクティブブロックの縦位置
    private int checkBlock;                 //（隣接するブロックが同色かを）探査する起点となるブロックの色
    private int blockNum;                   //（隣接するブロックが同色かの）探査で適合したブロックの個数
    private int key;                        //入力内容を保存する変数。※２進数で１の位がＡ、２の位がＢ、４の位が↓、８の位が→、１６の位が←として押されていたらその桁が１となるようにする。
    private int oldKey;                     //前フレームの入力内容を保存する変数。押しっぱなしの処理に用いる。
    private int chainEffectTime;            //chainの演出時間管理変数。
    private int timeCount;                  //パズルシーン開始からの経過時間。
    private bool downButtonFlag;            //下ボタンが押されているか否か
    private bool rightButtonFlag;           //右ボタンが押されているか否か
    private bool leftButtonFlag;            //左ボタンが押されているか否か
    private bool winloseFlag;               //勝ち負けが決まったか否か
    private bool cutInRunning;              //カットインが動いているか否か
    private bool stopFlag;                  //一時停止フラグ
    private string missSpellName;           //多重召喚失敗の際のテキスト
    private string phaseCount;              //フェイズの進行状況
    private int[] activeBlock = new int[2];                                     //アクティブブロックの色
    private int[] nextBlockOne = new int[2];                                    //ネクストブロックの色
    private int[] nextBlockTwo = new int[2];                                    //ネクネク（２ネクスト）ブロックの色
    private int[] playerEliminatBlockCount = new int[BLOCKTYPE_NUM + 1];        //消えたブロックの色と数をカウント※[]内の数が色を現す。０は使わないが１～５までを使うので要素数は（０を含め）６個。
    private int[,] clearBlock = new int[WORLD_WIDTH, WORLD_HEIGHT];             //フィールド探査時に二重探査を避けるための各座標のフラグ配列 
    private int[,] bufferBlock = new int[WORLD_WIDTH, WORLD_HEIGHT];            //一時的にブロックの情報を仮置きするための配列。消去判定の際にブロックの消去フラグを保存したり、連鎖判定用の落下後予測の際に落下後のブロック配置を代入する。 


    public int chainCount;                                                     //連鎖数
    public int nextTurnTime;                                                   //次のターンまでの残り時間
    public bool[] libraryOutFlag = new bool[2];                                //ライブラリアウトが発生したかの判定
    public bool[] phaseSkipFlag = new bool[5];                                 //フェイズを飛ばすかどうかのフラグ
    public bool[,] useCard = new bool[2, HAND_NUM];                            //手札のカードが使われたか否か。trueなら使用されたということでターン終了時に新たなカードに変える（カードを引く）。
    public int[] waitCount = new int[2];                                       //通信待機をここまで何回行ったか
    public int[] handFollower = new int[2];                                    //場に出ているシュジンコウ。０がプレイヤーで１がエネミー
    public int[] handFollowerForDraw = new int[2];                             //場に出ているシュジンコウの描写用変数。消滅演出の間にキャラクターを保持しておくため。
    public int[] libraryNum = new int[2];                                      //ライブラリの残り枚数
    public int[] followerDamage = new int[2];                                  //シュジンコウに与えられるダメージ。１ターンにDF以上のダメージが与えられると破壊される。
    public int[] lifePoint = new int[2];                                       //lifepoint。０がプレイヤーで１がエネミー。
    public int[] enemyGetManaPace = new int[BLOCKTYPE_NUM + 1];                //敵が各マナについて取得するペース
    public int[,] handCard = new int[2, HAND_NUM];                             //手札のカード
    public int[,] followerStatus = new int[2, 3];                              //シュジンコウのステータス。１次元目が所有者がプレイヤーか敵か。２次元目がステータス。（[,0]がAT、[,1]がDF、[,2]が特殊効果※未実装）
    public int[,] block = new int[WORLD_WIDTH, WORLD_HEIGHT];                  //フィールドの各座標に置かれているブロックの色   
    public int[,,] cardMana = new int[2, HAND_NUM, BLOCKTYPE_NUM + 1];         //cardManaはカードに貯まっているマナの状況を管理する。１次元が「プレイヤー(0)かエネミー(1)か」、２次元が「何番目のカードか(0,1,2)」、３次元が「何色のマナか(1,2,3,4,5)」を表す。[0,0,2]ならプレイヤー側の０番目のカードの青マナということ。
    public int[,,] cardCost = new int[2, HAND_NUM, BLOCKTYPE_NUM + 1];         //手札のコストを管理する配列。
    public int[,,] cardSkill = new int[2, HAND_NUM, SKILL_TYPE];               //カードのスキル内容。一次元目がプレイヤーか敵か、２次元目が手札の何枚目か、３次元目がスキルの種類。中の数字がスキル威力。
    /// <summary>
    /// library[プレイヤー(0)エネミー(1),デッキの何枚目か(0~19),カード番号(0)、必要マナ(1)、スキル種別(2),細別(マナなら色、スキル種別なら第何種か)]
    /// </summary>
    public int[,,,] library = new int[2, DECKCARD_NUM, 3, 10];                 //ライブラリ（使用中のデッキ）の状態を管理する配列。１次元が「プレイヤーかエネミーか」２次元が「デッキの何番目のカード」か３次元が「カード番号(0)、必要マナ(1)、スキル種別(2)」、４次元目が「細別(マナなら色、スキル種別なら第何種か)」

    private GameObject objMatch;                                                             //通信用ゲームオブジェクト
    private GameObject objBackButton;                                                        //ルームに戻るボタン
    private GameObject objEliminatBlockParent;                                               //消去演出用オブジェクトの親オブジェクト
    private GameObject objForNextTurnTime;                                                   //ターンの残り時間のオブジェクトを代入
    private GameObject objWinLose;                                                           //勝敗演出のオブジェクトを代入
    private GameObject objChainCount;                                                        //連鎖数表示のゲームオブジェクトを代入する
    private GameObject objField;                                                             //フィールドオブジェクト
    private GameObject objStopButton;                                                        //一時停止ボタンのゲームオブジェクト
    private Sprite[] blockImage = new Sprite[20];                                            //blockImageは各色のブロックの画像。（[]内は色と状態※block配列に入った数字と同じ）
    private Sprite[] stopImage = new Sprite[2];                                              //一時停止ボタン用画像
    private GameObject[] objLifeDamage = new GameObject[2];                                  //第三種（攻撃）呪文演出のオブジェクト
    private GameObject[] objActiveBlock = new GameObject[2];                                 //アクティブブロックのゲームオブジェクトを代入する配列
    private GameObject[] objNextBlock = new GameObject[2];                                   //ネクストブロックのゲームオブジェクトを代入する配列
    private GameObject[] objFollower = new GameObject[2];                                    //シュジンコウのゲームオブジェクトを代入する配列
    private GameObject[] objLibrary = new GameObject[2];                                     //ライブラリのゲームオブジェクトを代入する配列
    private GameObject[] objCutIn = new GameObject[2];                                       //カットインのオブジェクト
    private GameObject[] objFollowerDamage = new GameObject[2];                              //シュジンコウに与えられているダメージを表示するオブジェクト
    private GameObject[] objButton = new GameObject[5];                                      //操作ボタンのオブジェクト
    private GameObject[] objShock = new GameObject[2];                                       //呪文の失敗演出用の衝撃マークを表示するオブジェクト
    private GameObject[] objTarai = new GameObject[2];                                       //呪文の失敗演出用の金盥を表示するオブジェクト
    private GameObject[] objLifePoint = new GameObject[2];                                   //lifepointのゲームオブジェクトを代入する(0がＰＬ、1が敵）
    private GameObject[,] objCard = new GameObject[2, HAND_NUM];                             //objCardは手札のカードのゲームオブジェクト（描画されるブロック）を代入する配列。
    private GameObject[,] objFieldBlock = new GameObject[WORLD_WIDTH, WORLD_HEIGHT];         //objはフィールドブロックのゲームオブジェクト（描画されるブロック）を代入する配列。
    private GameObject[,] objFollowerStatus = new GameObject[2, 2];                          //シュジンコウのATDFのゲームオブジェクトを代入する配列
    private GameObject[,] objEliminatBlock = new GameObject[WORLD_WIDTH, WORLD_HEIGHT];      //各ブロックの消去時演出用オブジェクト
    private GameObject[,,] objCardMana = new GameObject[2, HAND_NUM, BLOCKTYPE_NUM + 1];     //カードの残り必要マナ表示

    public List<AudioSource> seAudioSource = new List<AudioSource>();                      //効果音のオーディオソース（音程変更等で効果音ごとに触るので各効果音ごとに取得）
    public List<AudioClip> se = new List<AudioClip>();                                     //効果音のオーディオクリップ
    private List<Sprite> cardImage = new List<Sprite>();                                   //カードの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）
    private List<Sprite> followerImage = new List<Sprite>();                               //シュジンコウの画像（配列は全種類分だが、実際にロードするのは使用する分のみ）

    private System.Random rnd = new System.Random();                                         //乱数を生成。


    // Use this for initialization
    void Start()
    {
        waitCount[0] = 0;
        waitCount[1] = 0;
        StartCoroutine(MainGame());
    }

    // Update is called once per frame
    void Update()
    {

    }

    //ゲーム本体
    private IEnumerator MainGame()
    {
        yield return StartCoroutine(StartSetting());
        while (true)
        {
            //フレームごとに①入力処理とその反応(KeyInput)→②時間経過による動きとその反応(TimeFunc)→③描画(ScreenDraw)の流れを行う。
            if (winloseFlag == false && stopFlag == false)//勝ち負け決定したら動かさない。一時停止中は動かさない。
            {
                KeyInput();
                TimeFunc();
                ScreenDraw();
            }
            yield return null;
        }
    }


    //開始処理のコルーチン
    private IEnumerator StartSetting()
    {
        int i, j, k, l;

        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0) { objMatch = PhotonNetwork.Instantiate("MatchManager", new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), 0); }
        //操作ボタンについて描画用オブジェクトを変数に代入。
        objButton[0] = GameObject.Find("AButton").gameObject as GameObject;
        objButton[1] = GameObject.Find("BButton").gameObject as GameObject;
        objButton[2] = GameObject.Find("DownButton").gameObject as GameObject;
        objButton[3] = GameObject.Find("RightButton").gameObject as GameObject;
        objButton[4] = GameObject.Find("LeftButton").gameObject as GameObject;

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

        //アクティブブロック＆ネクストブロックについて描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            //activeblock[i](iは数字)に対応するobjActiveBlock配列にゲームオブジェクトを代入。
            objActiveBlock[i] = GameObject.Find("activeblock" + i.ToString()).gameObject as GameObject;

            //nextblock[i](iは数字)に対応するobjNextBlock配列にゲームオブジェクトを代入。
            objNextBlock[i] = GameObject.Find("nextblock" + i.ToString()).gameObject as GameObject;
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
                for (k = 1; k < BLOCKTYPE_NUM + 1; k++)
                {
                    objCardMana[i, j, k] = GameObject.Find("cardmana" + i.ToString() + j.ToString() + k.ToString()).gameObject as GameObject;
                }
            }
        }


        //カットイン関連も描画用オブジェクトを変数に代入。
        for (i = 0; i < 2; i++)
        {
            objCutIn[i] = GameObject.Find("cutin" + i.ToString()).gameObject as GameObject;
            objTarai[i] = GameObject.Find("tarai" + i.ToString()).gameObject as GameObject;
            objShock[i] = GameObject.Find("shock" + i.ToString()).gameObject as GameObject;
        }

        //一時停止ボタン・ルームに戻るボタンのオブジェクト
        objStopButton = GameObject.Find("StopButton").gameObject as GameObject;
        objBackButton = GameObject.Find("BackButton").gameObject as GameObject;
        objBackButton.gameObject.SetActive(false);      //戻るボタンは一時停止中しか出てこない。
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0){ objStopButton.gameObject.SetActive(false); }

        //連鎖数表示について描画用オブジェクトを変数に代入。
        objChainCount = GameObject.Find("chaincount").gameObject as GameObject;

        //フィールド表示について描画用オブジェクトを変数に代入。
        objField = GameObject.Find("Field").gameObject as GameObject;

        //lifepoint表示について描画用オブジェクトを変数に代入
        for (i = 0; i < 2; i++)
        {
            objLifePoint[i] = GameObject.Find("lifepoint" + i.ToString()).gameObject as GameObject;
            objLifeDamage[i] = GameObject.Find("lifedamage" + i.ToString()).gameObject as GameObject;
        }

        //次ターン残り時間表示について描画用オブジェクトを変数に代入。
        objForNextTurnTime = GameObject.Find("turntime").gameObject as GameObject;

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
        blockImage[4] = Resources.Load<Sprite>("blockblack");
        blockImage[5] = Resources.Load<Sprite>("blockyellow");
        blockImage[11] = Resources.Load<Sprite>("blockredfloat");
        blockImage[12] = Resources.Load<Sprite>("blockbluefloat");
        blockImage[13] = Resources.Load<Sprite>("blockgreenfloat");
        blockImage[14] = Resources.Load<Sprite>("blockblackfloat");
        blockImage[15] = Resources.Load<Sprite>("blockyellowfloat");

        //一時停止ボタンの画像読み込み
        stopImage[0] = Resources.Load<Sprite>("stopbutton");
        stopImage[1] = Resources.Load<Sprite>("startbutton");

        //効果音読み込み
        for (i = 0; i < SOUND_NUM; i++)
        {
            seAudioSource.Add(gameObject.AddComponent<AudioSource>());
        }
        for (i = 0; i < seAudioSource.Count; i++)//リストの要素数の回数for文を回し、各要素にボリューム設定する。
        {
            seAudioSource[i].volume = PlayerPrefs.GetFloat("SEVolume", 0.8f);
        }
        se.Add(Resources.Load<AudioClip>("kako"));
        se.Add(Resources.Load<AudioClip>("fire"));
        se.Add(Resources.Load<AudioClip>("eliminat"));
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
        for (i = 0; i < 2; i++)
        {
            yield return StartCoroutine(LibraryMake(i));//ライブラリ作成
        }

        //カード画像読み込み
        for (i = 0; i < CARD_ALL + 1; i++)
        {
            cardImage.Add(null);//カード番号と同じ数だけ要素数を確保。（この手順がないと要素数が足りないとしてエラーが出る）
            followerImage.Add(null);
        }
        for (l = 0; l < 2; l++)
        {
            for (i = 1; i < CARD_ALL + 1; i++)
            {
                for (j = 0; j < DECKCARD_NUM; j++)
                {
                    if (library[l, j, 0, 0] == i && cardImage[i] == null)
                    {
                        cardImage[i] = Resources.Load<Sprite>("card" + i.ToString());
                        if (library[l, j, 2, 0] == SUMMON || library[l, j, 2, 2] == SUMMON)
                        {
                            followerImage[i] = Resources.Load<Sprite>("follower" + i.ToString());
                        }//召喚呪文ならフォロワー画像も
                    }
                }
            }
        }

        //カットイン読み込み（シチョウ戦のみ）
        if (PlayerPrefs.GetInt("scenarioCount", 0) == 20000) { objCutIn[1].GetComponent<Image>().sprite = Resources.Load<Sprite>("cutinShicho"); }

        //ゲームの初期化
        InitGame();
        yield return StartCoroutine(WaitMatchData(300));//スタートタイミングを合わせる同期
        GameObject.Find("NowLoading").GetComponent<Image>().enabled=false;
    }

    //パズル部の変数の初期化
    private void PuzzleVariableSetting()
    {
        int i, j, k, l;
        libraryOutFlag[0] = false;
        libraryOutFlag[1] = false;
        stopFlag = false;
        lifePoint[0] = MAXPLAYERLIFE;
        lifePoint[1] = MAXENEMYLIFE;
        activeType = 0;
        beforeActiveType = 0;
        autoRepeatKeyTime = 0;
        rotationEffectTime = 0;
        chainCount = 0;
        blockAutoMoveTime = 0;
        downKeyBlankTime = 2;//0が下キー押し直後、1が落下演出状態、2以降の数字が自然状態。
        blockMoveTime = 0;
        chainEffectTime = 30;//0~29はチェイン演出に使う。通常状態は30以上。
        key = 0;
        timeCount = 0;
        phaseCount = "";
        nextTurnTime = TURN_TIME;
        winloseFlag = false;

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
            handFollower[l] = 0;
            handFollowerForDraw[l] = 0;
            followerDamage[l] = 0;
        }

        for (i = 1; i < BLOCKTYPE_NUM + 1; i++) { playerEliminatBlockCount[i] = 0; }//iは色と対応させているのでi=0は使わない

        for (k = 0; k < 2; k++)
        {
            for (i = 0; i < HAND_NUM; i++)
            {
                for (j = 1; j < BLOCKTYPE_NUM + 1; j++) { cardMana[k, i, j] = 0; }//貯めたマナのリセット
            }
        }
        //相手のマナ獲得能力の代入。
        for (i = 1; i < BLOCKTYPE_NUM + 1; i++)
        {
            enemyGetManaPace[i] = GetComponent<CardData>().enemyGetManaPace[i];
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0) { enemyGetManaPace[i]=99999999; }//対人戦では獲得ペースを最大にして実質獲得しないようにする
        }
    }


    // ゲームの初期化
    private void InitGame()
    {
        int i, j;
        //関連変数の初期化。
        PuzzleVariableSetting();

        // アクティブブロックの位置をセット
        activeX = 2;//アクティブブロックの横軸位置
        activeY = 1;//アクティブブロックの縦軸位置

        // アクティブブロックを生成（内部では２手先まで決めるので３回）
        CreateNewActiveBlock();
        CreateNewActiveBlock();
        CreateNewActiveBlock();

        // フィールドの初期化（ブロックの消去）
        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                block[j, i] = 0;
            }
        }
        //手札を３枚引く
        for (i = 0; i < HAND_NUM; i++)
        {
            DrawCard(0, i);//プレイヤー
            DrawCard(1, i);//敵
        }
    }


    // 新しいブロックの生成
    private void CreateNewActiveBlock()
    {
        //ネクストをアクティブブロックに、ネクネクをネクストブロックに代入。
        activeBlock[0] = nextBlockOne[0];
        activeBlock[1] = nextBlockOne[1];

        nextBlockOne[0] = nextBlockTwo[0];
        nextBlockOne[1] = nextBlockTwo[1];

        // ネクネクにランダムにブロックをセット(Random.Rangeで設定する最大値はexclusive(その値を含まない)なので+1する)
        nextBlockTwo[0] = Random.Range(1, BLOCKTYPE_NUM + 1);
        nextBlockTwo[1] = Random.Range(1, BLOCKTYPE_NUM + 1);

        //新規のアクティブブロックを出すので回転配置は初期状態に。
        activeType = 0; beforeActiveType = 0;
    }



    // キー入力処理
    private void KeyInput()
    {
        // キー入力を得る
        if (downButtonFlag == true) { key += INPUT_DOWN; }
        if (rightButtonFlag == true) { key += INPUT_RIGHT; }
        if (leftButtonFlag == true) { key += INPUT_LEFT; }

        // キー入力に応じて処理をする
        if (blockAutoMoveTime > 0)
        {//ブロック固定後の硬直時間でない
         //入力されたキーとそれに対する反応（中のif文は壁際処理（壁やブロックに当たって移動できない場合）の判定）

            //下キー
            if (((key & INPUT_DOWN) != 0) && (downKeyBlankTime > 1))//keyは＆演算子を用いたビット演算でキーが入力されているかチェックする。（されていれば０でなく、されていなければ０）
            {
                MoveActiveBlock(0, 1);
                downKeyBlankTime = 0;
            }

            //左キー
            if ((((key & ~oldKey) & INPUT_LEFT) != 0) && (activeX > 0))
            {
                if ((activeX > 1) || ((activeX == 1) && (activeType != 3)))
                {
                    if (block[activeX - 1, activeY] == 0)
                    {
                        if ((activeType != 3) || (block[activeX - 2, activeY] == 0))
                        {
                            if (activeType != 0 || block[activeX - 1, activeY - 1] == 0)
                            {
                                if (activeType != 2 || block[activeX - 1, activeY + 1] == 0)
                                {
                                    MoveActiveBlock(-1, 0);
                                    autoRepeatKeyTime = 0;
                                    seAudioSource[3].PlayOneShot(se[3]);
                                };
                            };
                        };
                    };
                };
            }

            //右キー
            if ((((key & ~oldKey) & INPUT_RIGHT) != 0) && (activeX < WORLD_WIDTH - 1))
            {
                if ((activeX < WORLD_WIDTH - 2) || ((activeX == WORLD_WIDTH - 2) && (activeType != 1)))
                {
                    if (block[activeX + 1, activeY] == 0)
                    {
                        if ((activeType != 1) || (block[activeX + 2, activeY] == 0))
                        {
                            if (activeType != 0 || block[activeX + 1, activeY - 1] == 0)
                            {
                                if (activeType != 2 || block[activeX + 1, activeY + 1] == 0)
                                {
                                    MoveActiveBlock(1, 0);
                                    autoRepeatKeyTime = 0;
                                    seAudioSource[3].PlayOneShot(se[3]);
                                };
                            };
                        };
                    };
                };
            }

            //左キー押しっぱなし
            if (((key & INPUT_LEFT) != 0) && (activeX > 0) && (autoRepeatKeyTime > 16))
            {
                if ((activeX > 1) || ((activeX == 1) && (activeType != 3)))
                {
                    if (block[activeX - 1, activeY] == 0)
                    {
                        if ((activeType != 3) || (block[activeX - 2, activeY] == 0))
                        {
                            if (activeType != 0 || block[activeX - 1, activeY - 1] == 0)
                            {
                                if (activeType != 2 || block[activeX - 1, activeY + 1] == 0)
                                {
                                    MoveActiveBlock(-1, 0);
                                    autoRepeatKeyTime = 15;
                                    seAudioSource[3].PlayOneShot(se[3]);
                                };
                            };
                        };
                    };
                };
            }

            //右キー押しっぱなし
            if (((key & INPUT_RIGHT) != 0) && (activeX < WORLD_WIDTH - 1) && (autoRepeatKeyTime > 16))
            {
                if ((activeX < WORLD_WIDTH - 2) || ((activeX == WORLD_WIDTH - 2) && (activeType != 1)))
                {
                    if (block[activeX + 1, activeY] == 0)
                    {
                        if ((activeType != 1) || (block[activeX + 2, activeY] == 0))
                        {
                            if (activeType != 0 || block[activeX + 1, activeY - 1] == 0)
                            {
                                if (activeType != 2 || block[activeX + 1, activeY + 1] == 0)
                                {
                                    MoveActiveBlock(1, 0);
                                    autoRepeatKeyTime = 15;
                                    seAudioSource[3].PlayOneShot(se[3]);
                                };
                            };
                        };
                    };
                };
            }

            //Aキー
            if (((key & ~oldKey) & INPUT_A) != 0)
            {
                rotationEffectTime = 1;//回転描画スタート。（この変数は１以上の場合のみ描画に反映され、毎フレームカウントが進んでいく）
                if ((activeType != 0) || (activeX != WORLD_WIDTH - 1))
                {
                    if ((activeType != 2) || (activeX != 0))
                    {
                        if ((activeType != 0) || (block[activeX + 1, activeY] == 0))
                        {
                            if ((activeType != 2) || (block[activeX - 1, activeY] == 0))
                            {
                                if ((activeType != 3) || (block[activeX, activeY - 1] == 0))
                                {
                                    if ((activeType != 1) || ((activeY < WORLD_HEIGHT - 1) && ((activeY < WORLD_HEIGHT - 2) && block[activeX, activeY + 1] == 0)))
                                    {
                                        activeType += 1;//障害物がなければ、そのまま回転
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                    else
                                    {//activeTypeが1で下に障害物がある場合
                                        if (block[activeX, activeY - 1] == 0 && activeY > 1)//上に障害物がなければ
                                        {
                                            activeType += 1;
                                            activeY--;
                                            seAudioSource[4].PlayOneShot(se[4]);
                                        }
                                        else
                                        {//上に障害物があるなら
                                            if (activeX > 0 && block[activeX - 1, activeY] == 0)//対側に障害物がなければ
                                            {
                                                activeType += 2;
                                                seAudioSource[4].PlayOneShot(se[4]);
                                            }
                                        }
                                    }
                                }
                                else
                                {//activeTypeが3で上に障害物がある場合
                                    if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)//下に障害物がなければ
                                    {
                                        activeType += 1;
                                        activeY++;
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                    else
                                    {//下に障害物があるなら
                                        if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//対側に障害物がなければ
                                        {
                                            activeType += 2;
                                            seAudioSource[4].PlayOneShot(se[4]);
                                        }
                                    }
                                }
                            }
                            else
                            {//activeTypeが2で左に障害物がある場合
                                if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//右に障害物がなければ
                                {
                                    activeType += 1;
                                    activeX++;
                                    seAudioSource[4].PlayOneShot(se[4]);
                                }
                                else
                                {//右に障害物があるなら
                                    if (block[activeX, activeY - 1] == 0)//上に障害物がなければ
                                    {
                                        activeType += 2;
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                }
                            }
                        }
                        else
                        {//activeTypeが0で右に障害物がある場合
                            if (activeX > 0 && block[activeX - 1, activeY] == 0)//左に障害物がなければ
                            {
                                activeType += 1;
                                activeX--;
                                seAudioSource[4].PlayOneShot(se[4]);
                            }
                            else
                            {//左に障害物があるなら
                                if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)
                                {
                                    activeType += 2;
                                    seAudioSource[4].PlayOneShot(se[4]);
                                }
                            }
                        }
                    }
                    else
                    {//activeTypeが2で左に壁があるなら
                        if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//右に障害物がなければ
                        {
                            activeType += 1;
                            activeX++;
                            seAudioSource[4].PlayOneShot(se[4]);
                        }
                        else
                        {//右に障害物があるなら
                            if (block[activeX, activeY - 1] == 0)//上に障害物がなければ
                            {
                                activeType += 2;
                                seAudioSource[4].PlayOneShot(se[4]);
                            }
                        }
                    }
                }
                else
                {//activeTypeが0で右に壁があるなら
                    if (activeX > 0 && block[activeX - 1, activeY] == 0)//左に障害物がなければ
                    {
                        activeType += 1;
                        activeX--;
                        seAudioSource[4].PlayOneShot(se[4]);
                    }
                    else
                    {//左に障害物があるなら
                        if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)//下に障害物がなければ
                        {
                            activeType += 2;
                            seAudioSource[4].PlayOneShot(se[4]);
                        }
                    }
                }
            }

            //Bキー					
            if (((key & ~oldKey) & INPUT_B) != 0)
            {
                rotationEffectTime = 1;//回転描画スタート。（この変数は１以上の場合のみ描画に反映され、毎フレームカウントが進んでいく）
                if ((activeType != 2) || (activeX != WORLD_WIDTH - 1))
                {
                    if ((activeType != 0) || (activeX != 0))
                    {
                        if ((activeType != 2) || (block[activeX + 1, activeY] == 0))
                        {
                            if ((activeType != 0) || (block[activeX - 1, activeY] == 0))
                            {
                                if ((activeType != 1) || (block[activeX, activeY - 1] == 0))
                                {
                                    if ((activeType != 3) || ((activeY < WORLD_HEIGHT - 1) && ((activeY < WORLD_HEIGHT - 2) && block[activeX, activeY + 1] == 0)))
                                    {
                                        activeType -= 1;//障害物がなければそのまま回転
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                    else
                                    {//activeTypeが3で下に障害物があるなら
                                        if (block[activeX, activeY - 1] == 0 && activeY > 1)//上に障害物がなければ
                                        {
                                            activeType -= 1;
                                            activeY--;
                                            seAudioSource[4].PlayOneShot(se[4]);
                                        }
                                        else
                                        {//上に障害物があるなら
                                            if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//対側に障害物がなければ
                                            {
                                                activeType += 2;
                                                seAudioSource[4].PlayOneShot(se[4]);
                                            }
                                        }
                                    }
                                }
                                else
                                {//activeTypeが1で上に障害物があるなら
                                    if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)//下に障害物がなければ
                                    {
                                        activeType -= 1;
                                        activeY++;
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                    else
                                    {//下に障害物があるなら
                                        if (activeX > 0 && block[activeX - 1, activeY] == 0)//対側に障害物がなければ
                                        {
                                            activeType += 2;
                                            seAudioSource[4].PlayOneShot(se[4]);
                                        }
                                    }
                                }
                            }
                            else
                            {//activeTypeが0で左に障害物があるなら
                                if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//右に障害物がなければ
                                {
                                    activeType -= 1;
                                    activeX++;
                                    seAudioSource[4].PlayOneShot(se[4]);
                                }
                                else
                                {//右に障害物があるなら
                                    if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)//下に障害物がなければ
                                    {
                                        activeType += 2;
                                        seAudioSource[4].PlayOneShot(se[4]);
                                    }
                                }
                            }
                        }
                        else
                        {//activeTypeが2で右に障害物があるなら
                            if (activeX > 0 && block[activeX - 1, activeY] == 0)//左に障害物がなければ
                            {
                                activeType -= 1;
                                activeX--;
                                seAudioSource[4].PlayOneShot(se[4]);
                            }
                            else
                            {//左に障害物があるなら
                                if (block[activeX, activeY - 1] == 0)//上に障害物がなければ
                                {
                                    activeType += 2;
                                    seAudioSource[4].PlayOneShot(se[4]);
                                }
                            }
                        }
                    }
                    else
                    {//activeTypeが0で左に壁があるなら
                        if (activeX < WORLD_WIDTH - 1 && block[activeX + 1, activeY] == 0)//右に障害物がなければ
                        {
                            activeType -= 1;
                            activeX++;
                            seAudioSource[4].PlayOneShot(se[4]);
                        }
                        else
                        {//右に障害物があるなら
                            if (activeY < WORLD_HEIGHT - 1 && block[activeX, activeY + 1] == 0)//下に障害物がなければ
                            {
                                activeType += 2;
                                seAudioSource[4].PlayOneShot(se[4]);
                            }
                        }
                    }
                }
                else
                {//activeTypeが2で右に壁があるなら
                    if (activeX > 0 && block[activeX - 1, activeY] == 0)//左に障害物がなければ
                    {
                        activeType -= 1;
                        activeX--;
                        seAudioSource[4].PlayOneShot(se[4]);
                    }
                    else
                    {//左に障害物があるなら
                        if (block[activeX, activeY - 1] == 0)//上に障害物がなければ
                        {
                            activeType += 2;
                            seAudioSource[4].PlayOneShot(se[4]);
                        }
                    }
                }
            }

            //activeTypeは0,1,2,3で管理する。3の次は0に戻る。
            if (activeType > 3) { activeType -= 4; }
            if (activeType < 0) { activeType += 4; }
        }
        //keyの値を使い終わったのでoldKeyに代入した後初期化。oldKeyは次のフレームでkeyと比較して押しっぱなし判定に使う。
        oldKey = key;
        key = 0;
        // 終了
    }



    // 時間カウント関係処理
    private void TimeFunc()
    {
        int i, j, k, l;
        if (blockMoveTime % PACE_BLOCK_MOVE == 0)
        {
            MoveBlock();
        }

        blockAutoMoveTime++;
        autoRepeatKeyTime++;
        downKeyBlankTime++;
        blockMoveTime++;
        chainEffectTime++;
        timeCount++;
        nextTurnTime--;

        LibraryOutCheck();//ライブラリアウトが起きたかの判定。ブレイクタイミングでの負けを即座に判定するために毎フレーム判定（判定自体も軽い）

        if (nextTurnTime == 0)
        {
            StartCoroutine("TurnFunc");
        }

        if (rotationEffectTime > 0) { rotationEffectTime++; }//回転アニメーションを進める
        if (rotationEffectTime > ROTATION_TIME)
        {
            rotationEffectTime = 0;
            beforeActiveType = activeType;
        }//回転アニメーションが完了したのでbeforeActiveTypeを更新し、次の回転時に現配置をベースに回転アニメーションできるようにする。

        if (blockAutoMoveTime == 1)
        {
            //blockAutoMoveTimeが1になる、つまりブロック固定後の硬直が切れたらアクティブブロックを新しくする。
            CreateNewActiveBlock();
            //硬直解除によりアクティブブロックを表示
            for (i = 0; i < 2; i++) { objActiveBlock[i].GetComponent<Image>().enabled = true; }
        }
        // 一定時間が経過していたらブロックを下に落とす
        if (blockAutoMoveTime > 0 && blockAutoMoveTime % PACE_ACTIVE_BLOCK_MOVE == 0) { MoveActiveBlock(0, 1); }

        //敵のマナ獲得
        for (j = 1; j < BLOCKTYPE_NUM + 1; j++)
        {
            //同色マナはどのカードにも同じ個数入るので、入る個数をまず決定。
            k = rnd.Next(5) * rnd.Next(4);
            l = rnd.Next(100) + 500;
            for (i = 0; i < HAND_NUM; i++)
            {
                if (timeCount % (enemyGetManaPace[j] * l / 600) == 0) { cardMana[1, i, j] += k; }
            }
        }

    }


    // アクティブブロックの移動
    private void MoveActiveBlock(int MoveX, int MoveY)
    {
        int NewX, newY;

        // 移動後の座標をセットする
        NewX = MoveX + activeX;
        newY = MoveY + activeY;

        // ブロックの固定	
        if (MoveY != 0)
        {
            // 画面の一番下のブロック位置まで来ていたらブロックを固定させる
            if ((newY >= WORLD_HEIGHT) | ((newY == WORLD_HEIGHT - 1) & (activeType == 2)))
            {
                LockActiveBlock(activeX, activeY);
                // 移動を無効にする
                MoveY = 0;
            }
            else
            {
                // activeBlock[0]がフィールド上のブロックに当たっていないか調べる
                if ((CheckHitActiveBlock(NewX, newY) == -1) && (activeY > 0) && ((newY < WORLD_HEIGHT) & ((newY != WORLD_HEIGHT - 1) | (activeType != 2))))
                {
                    // あたっていたらブロックを固定する
                    LockActiveBlock(activeX, activeY);
                    // 移動を無効にする
                    MoveY = 0;
                }
                //activeBlock[1]がフィールド上のブロックに当たっていないか調べる
                if ((CheckHitActiveBlock(NewX, newY) == 0) && ((newY < WORLD_HEIGHT) & ((newY != WORLD_HEIGHT - 1) | (activeType != 2))))
                {
                    if (activeType == 0)
                    {
                        if ((CheckHitActiveBlock(NewX, newY - 1) == -1) & (activeY > 0))
                        {
                            // あたっていたらブロックを固定する
                            LockActiveBlock(activeX, activeY);
                            // 移動を無効にする
                            MoveY = 0;
                        };
                    }
                    if (activeType == 1)
                    {
                        if (CheckHitActiveBlock(NewX + 1, newY) == -1)
                        {
                            // あたっていたらブロックを固定する
                            LockActiveBlock(activeX, activeY);
                            // 移動を無効にする
                            MoveY = 0;
                        };
                    }
                    if (activeType == 2)
                    {
                        if (CheckHitActiveBlock(NewX, newY + 1) == -1)
                        {
                            // あたっていたらブロックを固定する
                            LockActiveBlock(activeX, activeY);
                            // 移動を無効にする
                            MoveY = 0;
                        };
                    }
                    if (activeType == 3)
                    {
                        if (CheckHitActiveBlock(NewX - 1, newY) == -1)
                        {
                            // あたっていたらブロックを固定する
                            LockActiveBlock(activeX, activeY);
                            // 移動を無効にする
                            MoveY = 0;
                        };
                    };
                };
            };
        }
        // 座標を移動する
        activeX += MoveX;
        activeY += MoveY;
        // 終了
    }



    // アクティブブロックがフィールド上のブロックに当たっていないか調べる
    private int CheckHitActiveBlock(int x, int y)
    {
        //当たっていたら-1を返す
        if (block[x, y] != 0) { return -1; }
        // 当たっていない場合0を返す
        return 0;
    }


    // アクティブブロックを固定する
    // 及び次のブロックを出す
    // もし次のブロックを出すスペースがなかったらブレイク発生
    private void LockActiveBlock(int x, int y)
    {
        int i;
        //固定後の硬直中なら処理せずリターン。
        if (blockAutoMoveTime < 1) { return; }

        //activeBlock[0]の位置は引数通りなのでそのままブロックの固定（block配列への代入）を行う。※+20はアクティブから固定されたばかりというフラグ。
        block[x, y] = activeBlock[0] + 20;

        //activeBlock[1]については、activeType（アクティブブロックの回転状況）による場合分けをしたうえでブロックの固定（block配列への代入）を行う。
        if (activeType == 0) { block[x, y - 1] = activeBlock[1] + 20; }
        if (activeType == 1) { block[x + 1, y] = activeBlock[1] + 20; }
        if (activeType == 2) { block[x, y + 1] = activeBlock[1] + 20; }
        if (activeType == 3) { block[x - 1, y] = activeBlock[1] + 20; }

        for (i = 0; i < 2; i++) { objActiveBlock[i].GetComponent<Image>().enabled = false; }//硬直中はアクティブブロックがない。

        //ブロック固定後の硬直時間を設定する。（blockAutoMoveTimeが１以上になるまでは硬直状態であるため、blockAutoMoveTimeをマイナスの値に設定する。）
        blockAutoMoveTime = -WAIT_NEXT;
        //新たなアクティブブロックの位置を設定
        activeX = 2; activeY = 1;
        ChainCheck();
    }


    private int CheckEliminatBlock()
    {
        int i, j, l, m;  // 汎用変数
        int eliminatFlag = 0;//消えるブロックが一つでもあるか否か
                             // 一時使用用ブロックデータを初期化
        InitBufferBlock();

        // 各ブロックが消えるか調べる
        for (i = 1; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                // もしブロックがない場合は次に移る
                if (block[j, i] == 0 || block[j, i] > 9) { continue; }

                // ブロックが消えるかどうか調べて調査結果をバッファに保存する
                //CheckEliminatBlockToOneの下準備
                checkBlock = block[j, i];//ブロックの色を代入
                blockNum = 1;//blockNum（くっついている同色の個数）の初期化（自身を数えて1を代入）
                             //clearBlock（CheckEliminatBlockToOneの二重探査防止配列）のリセット
                for (l = 0; l < WORLD_HEIGHT; l++)
                {
                    for (m = 0; m < WORLD_WIDTH; m++)
                    {
                        clearBlock[m, l] = 0;
                    }
                }
                //CheckEliminatBlockToOneの実行
                bufferBlock[j, i] = CheckEliminatBlockToOne(j, i);
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
                    //消えたブロックの色と数をカウント※playerEliminatBlockCountは[]内の数が色を現す
                    playerEliminatBlockCount[block[j, i]] += 1;
                    //消去演出
                    StartCoroutine(EliminatBlockEffect(j, i, block[j, i]));
                    //ブロックの消去
                    block[j, i] = 0;
                }
            }
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
            seAudioSource[2].PlayOneShot(se[2]);
        }//一つでも消えていればchainCountを増やす。

        //カードにマナを補充。
        for (i = 0; i < HAND_NUM; i++)
        {//playerCardmanaはプレイヤー(cardManaの１次元が「プレイヤー(0)」である)のカードに貯まっているマナの状況を管理する。２次元(i)が「何番目のカードか(0,1,2)」、３次元(j)が「何色のマナか」を表す。[0,0,2]なら自分の０番目のカードの青マナということ。
            for (j = 1; j < BLOCKTYPE_NUM + 1; j++) { cardMana[0, i, j] += playerEliminatBlockCount[j] * chainCount; }
        }

        for (i = 1; i < BLOCKTYPE_NUM + 1; i++) { playerEliminatBlockCount[i] = 0; }//iは色と対応させているのでi=0は使わない

        ChainCheck();

        //ブレイク処理
        BreakField();

        if (eliminatFlag == 0) { return 0; }//消えてなかったら０を返す
        return 1;//一つでも消えていたら１を返す
    }

    //ブレイク（窒息）処理
    private void BreakField()
    {
        int i, j;
        if (block[2, 1] != 0 && block[2, 1] < 10)
        {
            chainCount = 0;
            chainEffectTime = 30;
            //ブロック固定後の硬直時間を追加。
            blockAutoMoveTime -= 10;
            //ブレイク演出
            StartCoroutine(ShakeField());
            for (i = 0; i < HAND_NUM; i++)
            {
                StartCoroutine(ShakeCard(0, i));
            }
            //手札全ロス
            for (i = 0; i < HAND_NUM; i++)
            {
                if (useCard[0, i] == false)//使用確定カードは捨てない。
                {
                    DrawCard(0, i);
                    for (j = 1; j < BLOCKTYPE_NUM + 1; j++) { cardMana[0, i, j] = 0; }//カードに貯まったマナもリセット
                }
            }

            for (i = 0; i < WORLD_HEIGHT; i++)
            {
                for (j = 0; j < WORLD_WIDTH; j++)
                {
                    block[j, i] = 0;
                }
            }
            chainCountForDraw = -1;//-1はブレイク状態を表す。
            chainEffectTime = 0;
        }
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

    // 特定ブロックが消えるか探索（※自己参照する構造なので注意）
    private int CheckEliminatBlockToOne(int x, int y)
    {

        // チェックするブロックの位置を保存。（既に探査したものを２重探査しないためのチェック済みマーク代わりの変数）
        clearBlock[x, y] = 1;

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //右が同色ならそれを引数に自己参照。
        if ((x < WORLD_WIDTH - 1) && (block[x + 1, y] == checkBlock) && (clearBlock[x + 1, y] == 0)) { blockNum += 1; CheckEliminatBlockToOne(x + 1, y); }

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //左が同色ならそれを引数に自己参照。
        if ((x > 0) && (block[x - 1, y] == checkBlock) && (clearBlock[x - 1, y] == 0)) { blockNum += 1; CheckEliminatBlockToOne(x - 1, y); }

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。	
        if (blockNum > 3) { return 1; }
        //下が同色ならそれを引数に自己参照。
        if ((y < WORLD_HEIGHT - 1) && (block[x, y + 1] == checkBlock) && (clearBlock[x, y + 1] == 0)) { blockNum += 1; CheckEliminatBlockToOne(x, y + 1); }

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。	
        if (blockNum > 3) { return 1; }
        //上が同色ならそれを引数に自己参照。
        if ((y > 1) && (block[x, y - 1] == checkBlock) && (clearBlock[x, y - 1] == 0)) { blockNum += 1; CheckEliminatBlockToOne(x, y - 1); }

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。	
        if (blockNum > 3) { return 1; }

        // ここまで来ていたら消えない
        return 0;
    }

    //フィールドブロックの落下関数
    private void MoveBlock()
    {
        int newY, j, i;
        int lockBlock = 0;//ブロックが固定されたか否か

        for (i = WORLD_HEIGHT - 1; i > -1; i--)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if (block[j, i] > 9)//ブロックが10以上の（つまり浮いている）時
                {
                    // 移動後の座標をセットする
                    newY = i + 1;
                    if ((j != activeX || newY != activeY) && ((activeType == 0 && (j != activeX || newY != activeY - 1)) || (activeType == 1 && (j != activeX + 1 || newY != activeY)) || (activeType == 2 && (j != activeX || newY != activeY + 1)) || (activeType == 3 && (j != activeX - 1 || newY != activeY))))
                    {//移動後の位置がアクティブブロックと重ならない時
                        if (newY >= WORLD_HEIGHT)//ブロックがフィールド下端に達しているか調べる
                        {
                            block[j, i] = block[j, i] % 10;
                            lockBlock = 1;
                        }
                        else
                        {
                            // 移動後のブロックが画面上のブロックに当たっていないか調べる
                            if (block[j, newY] != 0)
                            {
                                // あたっていたらブロックを固定する
                                block[j, i] = block[j, i] % 10;
                                lockBlock = 1;
                            }
                            else
                            {// あたっていなければ座標を移動する
                                block[j, newY] = block[j, i];
                                block[j, i] = 0;
                            }
                        }
                    };
                };
            };
        }
        if (lockBlock == 1) { CheckEliminatBlock(); seAudioSource[0].PlayOneShot(se[0]); };//一つでも新たにブロックを固定したならば、ブロックが消えるかチェック。
        // 終了
        return;
    }



    //アクティブブロック固定、連鎖時のブロック状態変更、連鎖判定。
    private void ChainCheck()
    {
        int i;
        int j;
        int k;
        InitBufferBlock();
        // 空きを詰める
        for (i = WORLD_HEIGHT - 1; i > -1; i--)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if (block[j, i] != 0)
                {
                    for (k = i + 1; (k < WORLD_HEIGHT) && (block[j, k] == 0 || block[j, k] > 9) && bufferBlock[j, k] == 0; k++) {; }//ブロックが落ちた時に固定される高さを探査
                    k--;

                    if (k != i)//ブロックに落ちる余地がある場合
                    {
                        bufferBlock[j, k] = block[j, i];//落ちた場合に固定される場所のバッファに代入。
                        if (block[j, i] > 9) { bufferBlock[j, k] = block[j, i] % 10; }//落ち切った想定でのシミュレーションなので、バッファからは浮きフラグ(+10)及び固定されたばかりフラグ(+20)を消去
                        if (block[j, i] < 10) { block[j, i] += 10; }//固定フラグがたっているブロックだったならば+10して浮きフラグに設定。
                        if (block[j, i] > 20) { block[j, i] -= 10; }//固定されたばかりフラグならば、-10して浮きフラグに設定。
                    }
                    else
                    {
                        if (block[j, i] > 9) { bufferBlock[j, i] = block[j, i] % 10; }//ブロックがフラグ的には浮いているが位置は下端に達しているなら、バッファからは浮きor固定されたばかりフラグを消去してその位置にそのまま代入。
                    }
                }
            }
        }

        if (chainCount > 0 && FallCheckEliminatBlock() == 0)
        {
            chainCount = 0;
        }//連鎖カウントのリセットを判定
    }


    //ブロック落下後消去予測(要は落下後のシミュレーションの状態にして(block配列の代わりにbufferBlock配列を使用して)回すCheckEliminatBlock関数もどき)
    private int FallCheckEliminatBlock()
    {
        int i, j, l, m; // 汎用変数 

        // 各ブロックが消えるか調べる
        for (i = 1; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if ((block[j, i] == 0 || block[j, i] > 9) && bufferBlock[j, i] == 0) continue;//落下後（bufferBlock）も現在（block）もブロックがないなら飛ばして次へ。

                // ブロックが消えるかどうか調べて調査結果をバッファに保存する
                blockNum = 1;//隣接同色の数をカウント（自身を数えてまず１つ）
                if (bufferBlock[j, i] == 0) { checkBlock = block[j, i]; } else { checkBlock = bufferBlock[j, i]; }//現在ブロックがあれば、それ(block)の色をcheckBlockに代入。落下後にブロックが来る予定なら、それ(bufferBlock)の色をcheckBlockに代入。
                                                                                                                  //clearBlock(二重探査防止用配列)の初期化
                for (l = 0; l < WORLD_HEIGHT; l++)
                {
                    for (m = 0; m < WORLD_WIDTH; m++)
                    {
                        clearBlock[m, l] = 0;
                    }
                }
                bufferBlock[j, i] = FallCheckEliminatBlockToOne(j, i) * 100;//消えるならBufferBlockに100を足す。
            }
        }

        // 消えると判断されたブロックを確認
        for (i = 0; i < WORLD_HEIGHT; i++)
        {
            for (j = 0; j < WORLD_WIDTH; j++)
            {
                if (bufferBlock[j, i] > 99)
                {
                    return 1;//消えるブロックが一つでもあるなら1を返す	
                }
            }
        }
        return 0;//なければ0を返す
    }


    //各ブロックの落下後消去予測。落下後をbufferBlock配列で管理しているため、CheckEliminatBlockToOneとは別関数。（※自己参照する関数なので注意）
    private int FallCheckEliminatBlockToOne(int x, int y)
    {

        // チェックするブロックの位置を保存。（既に探査したものを２重探査しないためのチェック済みマーク代わりの変数）
        clearBlock[x, y] = 1;

        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //右が同色ならそれを引数に自己参照。
        if ((x < WORLD_WIDTH - 1) && (block[x + 1, y] == checkBlock || bufferBlock[x + 1, y] == checkBlock) && (clearBlock[x + 1, y] == 0)) { blockNum += 1; FallCheckEliminatBlockToOne(x + 1, y); }
        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //左が同色ならそれを引数に自己参照。
        if ((x > 0) && (block[x - 1, y] == checkBlock || bufferBlock[x - 1, y] == checkBlock) && (clearBlock[x - 1, y] == 0)) { blockNum += 1; FallCheckEliminatBlockToOne(x - 1, y); }
        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //下が同色ならそれを引数に自己参照。
        if ((y < WORLD_HEIGHT - 1) && (block[x, y + 1] == checkBlock || bufferBlock[x, y + 1] == checkBlock) && (clearBlock[x, y + 1] == 0)) { blockNum += 1; FallCheckEliminatBlockToOne(x, y + 1); }
        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }
        //上が同色ならそれを引数に自己参照。
        if ((y > 1) && (block[x, y - 1] == checkBlock || bufferBlock[x, y - 1] == checkBlock) && (clearBlock[x, y - 1] == 0)) { blockNum += 1; FallCheckEliminatBlockToOne(x, y - 1); }
        //同色の隣接ブロックに対して自己参照をするとblockNumは1増えるので、3つ（自分含めて4つ）以上の同色ブロックがつながっていれば消えるのが確定（1を返す）。
        if (blockNum > 3) { return 1; }


        // ここまで来ていたら消えない
        return 0;
    }




    //画面描画のための画像関連処理（ターン処理に伴うものを除く）
    private void ScreenDraw()
    {
        int i, j, k, l;
        int x1 = 0;
        int x2 = 0;
        int y1 = 0;
        int y2 = 0;
        if (downKeyBlankTime == 1) { k = BLOCK_SIZE / 2; } else { k = 0; }//高速落下のブランクフレーム中、アクティブブロックの位置をブロック半個下に表示させる。
        //activetypeとbeforeActiveTypeから、activeblock[1]の位置決定に用いる変数を設定。（activetypeによる位置の違い及び回転中の位置の変化に対応（回転の何フレーム目かはrotationEffectTimeにて管理している））
        if (activeType == 0) { x1 = 0; y1 = -1; }
        if (activeType == 1) { x1 = 1; y1 = 0; }
        if (activeType == 2) { x1 = 0; y1 = 1; }
        if (activeType == 3) { x1 = -1; y1 = 0; }
        if (beforeActiveType == 0) { x2 = 0; y2 = -1; }
        if (beforeActiveType == 1) { x2 = 1; y2 = 0; }
        if (beforeActiveType == 2) { x2 = 0; y2 = 1; }
        if (beforeActiveType == 3) { x2 = -1; y2 = 0; }

        //★フィールドブロックの描画★
        //block[i,j]が０ならimage[i,j]を非表示にする
        for (i = 0; i < WORLD_WIDTH; i++)
        {
            for (j = 0; j < WORLD_HEIGHT; j++)
            {
                if (block[i, j] == 0)
                {
                    objFieldBlock[i, j].GetComponent<Image>().enabled = false;
                }
                else
                {//block[i,j]が０でないなら（0の時にこの代入をするとblockImage[0]が未定義なのでおかしくなる）
                 //block[i,j]が０でないならimage[i,j]を表示する
                    objFieldBlock[i, j].GetComponent<Image>().enabled = true;

                    //block[i,j]に対応するblockImageの画像を選ぶ。
                    if (block[i, j] > 20)
                    {
                        objFieldBlock[i, j].GetComponent<Image>().sprite = blockImage[block[i, j] - 20];//固定直後のブロックは通常ブロックのような見た目。
                    }
                    else
                    {
                        objFieldBlock[i, j].GetComponent<Image>().sprite = blockImage[block[i, j]];//20以下のブロックについてはそのまま描画。
                    }
                }
            }
        }
        //★アクティブブロックの描画★
        //アクティブブロックの画像選択
        for (i = 0; i < 2; i++)
        {
            objActiveBlock[i].GetComponent<Image>().sprite = blockImage[activeBlock[i]];
        }
        //アクティブブロックはフィールドブロックと差別化するため点滅する。
        for (i = 0; i < 2; i++)
        {
            objActiveBlock[i].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f - 0.15f * (Mathf.Cos((float)timeCount / 10)));
        }

        //アクティブブロックの描画位置設定
        objActiveBlock[0].GetComponent<RectTransform>().localPosition = new Vector3(activeX * BLOCK_SIZE + FIELD_LEFT, FIELD_TOP - activeY * BLOCK_SIZE + k, 0);
        //アクティブブロック[1]はx1,y1(activetypeにより決定される),x2,y2(beforeActiveTypeによって決定される),rotationEffectTimeの３変数を用いた式によってactiveblock[0]のＸ，Ｙ座標との位置関係を現せる。
        objActiveBlock[1].GetComponent<RectTransform>().localPosition = new Vector3(activeX * BLOCK_SIZE + FIELD_LEFT + ((ROTATION_TIME - rotationEffectTime) * x2 + rotationEffectTime * x1) * BLOCK_SIZE / ROTATION_TIME, FIELD_TOP - activeY * BLOCK_SIZE + k - ((ROTATION_TIME - rotationEffectTime) * y2 + rotationEffectTime * y1) * BLOCK_SIZE / ROTATION_TIME, 0);

        //ネクストブロックの画像選択
        for (i = 0; i < 2; i++)
        {
            objNextBlock[i].GetComponent<Image>().sprite = blockImage[nextBlockOne[i]];
        }

        //★ステータスの描画★
        //連鎖数表示
        if (chainCountForDraw != 0 && chainEffectTime < 30)
        {
            objChainCount.GetComponent<Text>().enabled = true;
            if (chainCountForDraw > 0)
            {

                objChainCount.GetComponent<Text>().text = chainCountForDraw.ToString() + "Chain";
            }
            else
            {
                objChainCount.GetComponent<Text>().text = "Break!!!\n(Card&Block Lost)";
            }
            objChainCount.GetComponent<RectTransform>().localPosition = new Vector3(chainEffectTime, 0, 0);
        }
        else
        {
            objChainCount.GetComponent<Text>().enabled = false;
        }
        //ＬＰ表示
        if (lifePoint[0] >= 15) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=blue>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[0] >= 10 && lifePoint[0] < 15) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=orange>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[0] < 10) { objLifePoint[0].GetComponent<Text>().text = "LP: <color=red>" + lifePoint[0].ToString() + "</color>"; }
        if (lifePoint[1] >= 15) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=blue>" + lifePoint[1].ToString() + "</color>"; }
        if (lifePoint[1] >= 10 && lifePoint[1] < 15) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=orange>" + lifePoint[1].ToString() + "</color>"; }
        if (lifePoint[1] < 10) { objLifePoint[1].GetComponent<Text>().text = "LP: <color=red>" + lifePoint[1].ToString() + "</color>"; }
        //ターン残り時間表示
        if (nextTurnTime < 0)
        {
            objForNextTurnTime.GetComponent<Text>().text = "<color=white>" + phaseCount + "</color>";
        }
        else
        {
            objForNextTurnTime.GetComponent<Text>().text = "<color=white>" + nextTurnTime.ToString() + "F for next turn</color>";
        }

        //★カードの描画★
        for (i = 0; i < 2; i++)
        {
            //手札の描画
            for (j = 0; j < HAND_NUM; j++)
            {
                objCard[i, j].GetComponent<Image>().sprite = cardImage[handCard[i, j]];
            }
            //残り必要マナの描画
            for (j = 0; j < HAND_NUM; j++)
            {
                l = 0;
                for (k = 1; k < BLOCKTYPE_NUM + 1; k++)
                {
                    if (cardCost[i, j, k] > cardMana[i, j, k] && useCard[i, j] == false)//魔力が足りず、使用確定カードでもないなら残り必要マナを表示する
                    {
                        objCardMana[i, j, k].gameObject.SetActive(true);
                        objCardMana[i, j, k].GetComponent<RectTransform>().sizeDelta = new Vector2(CARD_WIDTH * (cardCost[i, j, k] - cardMana[i, j, k]) / cardCost[i, j, k], 20);
                        objCardMana[i, j, k].GetComponentInChildren<Text>().text = "<color=white>" + (cardCost[i, j, k] - cardMana[i, j, k]).ToString() + "</color>";
                    }
                    else
                    {
                        objCardMana[i, j, k].gameObject.SetActive(false);
                        l++;
                    }
                }
                if (l == BLOCKTYPE_NUM) { objCard[i, j].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.7f - 0.3f * (Mathf.Cos((float)timeCount / 10))); } else { objCard[i, j].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f); }//魔力が足りていたらカードは点滅する。
                if (useCard[i, j] == true) { objCard[i, j].GetComponent<Image>().color = new Color(0.5f, 0.5f, 0.5f, 1.0f); }//使用確定カードなら暗くなる。
            }

            //ライブラリの描画
            objLibrary[i].GetComponentInChildren<Text>().text = libraryNum[i].ToString();
        }

        //★シュジンコウの描画★
        for (i = 0; i < 2; i++)
        {
            if (handFollowerForDraw[i] == 0)
            {
                objFollower[i].gameObject.SetActive(false);
                for (j = 0; j < 2; j++)
                {
                    objFollowerStatus[i, j].gameObject.SetActive(false);
                }
                objFollowerDamage[i].gameObject.SetActive(false);
            }
            else
            {
                objFollower[i].gameObject.SetActive(true);
                for (j = 0; j < 2; j++)
                {
                    objFollowerStatus[i, j].gameObject.SetActive(true);
                    if (j == 0) { objFollowerStatus[i, j].GetComponent<Text>().text = "<color=red>" + followerStatus[i, j].ToString() + "</color>"; }//ATの表示
                    if (j == 1) { objFollowerStatus[i, j].GetComponent<Text>().text = "<color=blue>" + followerStatus[i, j].ToString() + "</color>"; }//DFの表示
                }
                objFollower[i].GetComponent<Image>().sprite = followerImage[handFollowerForDraw[i]];
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

    }



    public void PushAButton()
    {
        key += INPUT_A;//Aボタン入力フラグ(押しっぱなしに対応しないのでkeyに直接代入）
    }

    public void PushBButton()
    {
        key += INPUT_B;//Bボタン入力フラグ（押しっぱなしに対応しないのでkeyに直接代入）
    }

    public void PushDownButton()
    {
        downButtonFlag = true;
    }

    public void PushRightButton()
    {
        rightButtonFlag = true;
    }

    public void PushLeftButton()
    {
        leftButtonFlag = true;
    }

    public void LeaveDownButton()
    {
        downButtonFlag = false;
    }

    public void LeaveRightButton()
    {
        rightButtonFlag = false;
    }
    public void LeaveLeftButton()
    {
        leftButtonFlag = false;
    }

    //ライブラリのシャッフル（playerはプレイヤー（０）か敵（１）か）
    public void Shuffle(int player)
    {

        int i, j;
        int[,] tmp = new int[3, 10];
        int n = DECKCARD_NUM;
        while (n > 1)
        {
            n--;
            int k = rnd.Next(n + 1);
            for (i = 0; i < 3; i++)
            {
                for (j = 0; j < 10; j++)
                {
                    tmp[i, j] = library[player, k, i, j];
                    library[player, k, i, j] = library[player, n, i, j];
                    library[player, n, i, j] = tmp[i, j];
                }
            }
        }

    }
    //ライブラリからカードを引く（playerがプレイヤー(0)か敵(1)か、handが手札の何枚目か）
    public void DrawCard(int player, int hand)
    {
        int i, k;
        //通信対戦で相手側のドローなら相手側で処理してくれるのでスキップ（それ以外の場合は処理）
        if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay == 0 || player == 0)
        {
            for (i = 0; i < DECKCARD_NUM; i++)
            {
                if (library[player, i, 0, 0] != 0)
                {//ライブラリの上から順にカードがある（０でない）ところまで探していく。
                    handCard[player, hand] = library[player, i, 0, 0];//カードの種類を代入
                    for (k = 1; k < BLOCKTYPE_NUM + 1; k++)
                    {
                        cardCost[player, hand, k] = library[player, i, 1, k];//カードのコストを代入
                    }
                    for (k = 0; k < SKILL_TYPE; k++)
                    {
                        cardSkill[player, hand, k] = library[player, i, 2, k];//カードの効果を代入
                    }
                    library[player, i, 0, 0] = 0;//カードを引いたのでライブラリから消す。
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

    //勝利
    private IEnumerator Win(string Case)
    {
        if (winloseFlag == false)//まだ勝ち負けが決まっていないなら（シーン移動が完了するまで並列でゲームは動き続けるので、winやlose関数が多重起動しないように）
        {
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                objMatch.GetComponent<Match>().MatchEnd();
            }
            seAudioSource[9].PlayOneShot(se[9]);
            objWinLose.gameObject.SetActive(true);
            objWinLose.GetComponent<Text>().text = "<color=red><size=240>YOU WIN</size><size=144>\n" + Case + "</size></color>";
            winloseFlag = true;
            yield return new WaitForSeconds(3.0f);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
            }
            else
            {
                PlayerPrefs.SetInt("scenarioCount", PlayerPrefs.GetInt("scenarioCount", 0) + 1);//シナリオを進める。
                yield return GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "StoryScene");//ストーリーシーンへ
            }
        }
    }


    //敗北
    private IEnumerator Lose(string Case)
    {
        if (winloseFlag == false)//まだ勝ち負けが決まっていないなら（シーン移動が完了するまで並列でゲームは動き続けるので、winやlose関数が多重起動しないように）
        {
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                objMatch.GetComponent<Match>().MatchEnd();
            }
            seAudioSource[9].PlayOneShot(se[9]);
            objWinLose.gameObject.SetActive(true);
            objWinLose.GetComponent<Text>().text = "<color=blue><size=240>YOU LOSE</size><size=144>\n" + Case + "</size></color>";
            winloseFlag = true;
            yield return new WaitForSeconds(3.0f);
            if (GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0)
            {
                GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
            }
            else
            {
                yield return GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "GameOverScene");
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
            for (j = 1; j < BLOCKTYPE_NUM + 1; j++)
            {
                if (cardMana[player, i, j] < cardCost[player, i, j]) { k = 1; break; }//マナが足りていないのが判明した時点でそのカードについては処理終了。       
            }
            if (k == 0)                    //ここまでk=0のままならマナが全て足りているので使用確定。
            {
                useCard[player, i] = true;//カードの使用確定
            }
        }
    }

    //２種呪文の処理関数
    private IEnumerator Spell2Func(int skill)
    {
        int i, l;
        for (l = 0; l < 2; l++)
        {
            for (i = 0; i < HAND_NUM; i++)
            {
                if (useCard[l, i] == true && cardSkill[l, i, 2] == skill)                    //使用確定カードで２種カウンターなら発動。
                {
                    objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                    seAudioSource[11].PlayOneShot(se[11]);
                    if (skill != SUMMON) { yield return StartCoroutine(SpellEffect(l, i)); }//呪文演出(SUMMONは内蔵してるので飛ばす）
                    OtherSpell(l, handCard[l, i]);//呪文効果
                    while (cutInRunning == true) { yield return null; }//カットインが終わるまで待つ(OtherSpellは演出内蔵のものがあるので要カットイン待機)
                }
            }
        }
    }

    //ターン処理関数
    private IEnumerator TurnFunc()
    {
        int i, l;
        //第２種（特殊効果）呪文フェイズ→第１種（強化）呪文フェイズ→第３種（ダメージ）呪文フェイズ→シュジンコウ攻撃フェイズ→第０種（召喚呪文）フェイズの順で処理される。
        //使用カードの確定（自身）※送信処理前に確定させないと、同期待ち中に得たマナで使用されるカードが増えうる。
        CardUse(0);
        //通信対戦時の同期待ち（cardManaデータを一致させる）
        phaseCount = "通信待機フェイズ";
        yield return StartCoroutine(WaitMatchData(300));//300フレームまで同期遅れを許容
        //使用カードの確定（相手）
        CardUse(1);

        phaseCount = "特殊呪文フェイズ";
        //第２種呪文フェイズ
        //２種呪文妨害（２種呪文を妨害する２種呪文(COUNTER)は相互作用を発生させるので、COUNTER同士では影響を与えない効果に＋他呪文と隔離。この種別だけはフェイズスキップも無視する）
        yield return StartCoroutine(Spell2Func(COUNTER));

        if (phaseSkipFlag[0] == false)
        {
            //２種召喚（召喚は他呪文との相互作用（ダメージ処理や強化等）が発生するので先行処理する）
            yield return StartCoroutine(Spell2Func(SUMMON));
            //デッキ削り（デッキに作用する効果は相互作用を起こすので効果ごとに処理を分ける）
            yield return StartCoroutine(Spell2Func(DECK_EAT));
            //手札入れ替え
            yield return StartCoroutine(Spell2Func(HAND_CHANGE));
            //その他の２種呪文
            yield return StartCoroutine(Spell2Func(OTHER));
            yield return StartCoroutine(FollowerBreak());
            LifePointCheck();
        }
        if (phaseSkipFlag[1] == false)
        {
            phaseCount = "強化呪文フェイズ";
            //第１種呪文フェイズ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (useCard[l, i] == true && cardSkill[l, i, 1] != 0)                    //使用確定カードで第一種呪文なら発動。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        if ((cardSkill[l, i, 1] == OWN && handFollower[l] == 0) || (cardSkill[l, i, 1] == YOURS && ((handFollower[1] == 0 && l == 0) || (handFollower[0] == 0 && l == 1)))) { StartCoroutine("SpellMiss", l); }//シュジンコウがいなければ詠唱失敗演出を詠唱演出に重ねる。
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                        while (cutInRunning == true) { yield return null; }//カットインが終わるまで待つ
                        if (cardSkill[l, i, 1] == OWN && handFollower[l] > 0) { seAudioSource[5].PlayOneShot(se[5]); Buff(l, handCard[l, i]); }//自分のシュジンコウへの呪文効果（シュジンコウがいる場合のみ）
                        if (cardSkill[l, i, 1] == YOURS && ((handFollower[1] > 0 && l == 0) || (handFollower[0] > 0 && l == 1)))//相手のシュジンコウへの呪文効果
                        {
                            seAudioSource[6].PlayOneShot(se[6]);
                            if (l == 0) { Buff(1, handCard[l, i]); }
                            if (l == 1) { Buff(0, handCard[l, i]); }
                        }//呪文効果（シュジンコウがいる場合のみ）
                    }
                }
            }
            yield return StartCoroutine(FollowerBreak());
            LifePointCheck();
        }
        if (phaseSkipFlag[2] == false)
        {
            phaseCount = "攻撃呪文フェイズ";
            //第３種呪文フェイズ
            for (l = 0; l < 2; l++)
            {
                for (i = 0; i < HAND_NUM; i++)
                {
                    if (useCard[l, i] == true && cardSkill[l, i, 3] != 0)                    //使用確定カードで三種呪文なら発動。
                    {
                        objCard[l, i].GetComponent<Image>().enabled = false;//使用したら非表示
                        yield return StartCoroutine(SpellEffect(l, i));//呪文演出
                        while (cutInRunning == true) { yield return null; }//カットインが終わるまで待つ
                        seAudioSource[1].PlayOneShot(se[1]);
                        if (l == 0) { StartCoroutine(LifeDamage(1)); yield return StartCoroutine(Damage(1, cardSkill[l, i, 3])); }//呪文効果
                        if (l == 1) { StartCoroutine(LifeDamage(0)); yield return StartCoroutine(Damage(0, cardSkill[l, i, 3])); }//Damage()の第一引数はダメージを「受ける」キャラクターなのでlとDamageの第一引数は逆になる
                    }
                }
            }
            yield return StartCoroutine(FollowerBreak());
            LifePointCheck();
        }
        if (phaseSkipFlag[3] == false)
        {
            phaseCount = "戦闘フェイズ";
            //戦闘フェイズ
            yield return StartCoroutine("FollowerAttack");
            yield return StartCoroutine(FollowerBreak());
            LifePointCheck();
        }
        for (l = 0; l < 2; l++)
        {
            followerDamage[l] = 0;//シュジンコウダメージのリセット
        }
        if (phaseSkipFlag[4] == false)
        {
            phaseCount = "召喚呪文フェイズ";
            //第０種呪文フェイズ
            for (l = 0; l < 2; l++)
            {
                yield return StartCoroutine(SummonCheck(l, 0));
            }
            yield return StartCoroutine(FollowerBreak());
            LifePointCheck();
        }
        TurnEnd();//ターン終了処理
        //通信対戦時の同期待ち（cardManaデータを一致させる）
        phaseCount = "通信待機フェイズ";
        yield return StartCoroutine(WaitMatchData(300));//300フレームまで同期遅れを許容
    }

    private void TurnEnd()
    {
        int l, i, j;

        //フェイズスキップフラグの初期化
        for (i = 0; i < 5; i++)
        {
            phaseSkipFlag[i] = false;
        }

        //使ったカード(useCardがtrueになっているカード)は新しく引き直す。
        for (l = 0; l < 2; l++)
        {
            for (i = 0; i < HAND_NUM; i++)
            {
                if (useCard[l, i] == true)
                {
                    DrawCard(l, i);
                    for (j = 1; j < BLOCKTYPE_NUM + 1; j++) { cardMana[l, i, j] = 0; }//カードに貯まったマナもリセット
                    useCard[l, i] = false;//新たに引いたので、使っていない状態になおす。
                    objCard[l, i].GetComponent<Image>().enabled = true;//引き直したので、非表示になっていたカードを表示しなおす。
                }
            }
        }

        phaseCount = "";
        nextTurnTime = TURN_TIME;//次のターンまでの時間を再設定。
    }


    //シュジンコウ召喚のための多重召喚チェック
    public IEnumerator SummonCheck(int player, int spellType)
    {
        int i, m;
        m = 0;
        missSpellName = "";//多重召喚テキストの初期化

        for (i = 0; i < HAND_NUM; i++)
        {
            if (useCard[player, i] == true && cardSkill[player, i, spellType] == SUMMON)//使用確定カードで、呼び出されている種別の召喚呪文（SUMMON）なら発動。
            {
                objCard[player, i].GetComponent<Image>().enabled = false;//使用したカードは非表示に
                m++;
                MakeMissSpellName(handCard[player, i]);//呪文失敗時に使うテキスト生成
            }
        }

        if (m > 1 || (handFollower[player] != 0 && m == 1))//２体以上を同時に召喚しようとする、もしくは既にシュジンコウがいるところに召喚した
        {
            StartCoroutine("SpellMiss", player); //詠唱失敗演出。
            yield return StartCoroutine(SpellEffect(player, 3));//呪文演出(第二引数の３は多重召喚失敗用の専用引数)
        }

        if (m == 1 && handFollower[player] == 0)//シュジンコウを出しておらず、今回召喚するのが１体だけ
        {
            for (i = 0; i < HAND_NUM; i++)//どのカードが発動したのかの再確認
            {
                if (useCard[player, i] == true && cardSkill[player, i, spellType] == SUMMON)
                {
                    yield return StartCoroutine(SpellEffect(player, i));//呪文演出
                    while (cutInRunning == true) { yield return null; }//カットインが終わるまで待つ
                    Summon(player, handCard[player, i]);
                }
            }
        }
    }

    //シュジンコウ召喚（playerがプレイヤー(0)か敵(1)か、summonNumが召喚するシュジンコウ番号）
    private void Summon(int player, int summonNum)
    {
        CardData c1 = GetComponent<CardData>();
        c1.CardList();
        followerStatus[player, 0] = c1.followerStatus[summonNum, 0];
        followerStatus[player, 1] = c1.followerStatus[summonNum, 1];
        followerStatus[player, 2] = c1.followerStatus[summonNum, 2];
        handFollower[player] = summonNum;
        handFollowerForDraw[player] = handFollower[player];
        seAudioSource[10].PlayOneShot(se[10]);
    }

    //ダメージ処理関数（playerがダメージを「受けた」のがプレイヤーか敵か、damageがダメージ量）
    public IEnumerator Damage(int player, int damage)
    {
        if (handFollower[player] == 0)//シュジンコウがいないとき
        {
            lifePoint[player] -= damage;
            if (damage != 0) { yield return StartCoroutine("DamageEffect", player); }
        }
        else
        {
            followerDamage[player] += damage;
            if (damage != 0) { yield return StartCoroutine("FollowerDamageEffect", player); }
        }
    }

    //シュジンコウ強化
    public void Buff(int player, int card)
    {
        int i;
        CardData c1 = GetComponent<CardData>();
        c1.CardList();
        for (i = 0; i < 2; i++)
        {
            followerStatus[player, i] += c1.followerStatus[card, i];
        }
        if (followerStatus[player, 0] < 0) { followerStatus[player, 0] = 0; }
    }

    //第２種呪文効果
    public void OtherSpell(int player, int card)
    {
        CardData c1 = GetComponent<CardData>();
        c1.CardList();//カードリスト呼び出し
        c1.cardSkill2Use[card](player);//第二種呪文の入ったデリゲート配列を呼び出す
    }

    //第３種呪文演出(playerは呪文のダメージを受ける側)
    public IEnumerator LifeDamage(int player)
    {
        //playerの炎上演出をオンに
        objLifeDamage[player].GetComponent<Image>().enabled = true; objLifeDamage[player].GetComponent<Animator>().enabled = true;
        for (int i = 0; i < 30; i++) { yield return null; }//演出フレームが終わるまで待つ。
        //演出をオフに戻す
        objLifeDamage[player].GetComponent<Image>().enabled = false; objLifeDamage[player].GetComponent<Animator>().enabled = false;
    }

    //シュジンコウ破壊チェック。シュジンコウの受けたダメージがDFを越えていたら破壊される。
    public IEnumerator FollowerBreak()
    {
        int l, i;
        bool[] followerBreakFlag = { false, false };

        for (l = 0; l < 2; l++)
        {
            if (handFollower[l] != 0 && followerStatus[l, 1] <= followerDamage[l])
            {
                followerBreakFlag[l] = true;
                handFollower[l] = 0;
                StartCoroutine("FollowerBreakEffect", l);//シュジンコウ破壊演出。
            }
        }
        //破壊演出がある場合、終わるまで待つ
        if (followerBreakFlag[0] == true || followerBreakFlag[1] == true) { for (i = 0; i < FOLLOWER_BREAK_TIME; i++) { yield return null; } }

        for (l = 0; l < 2; l++)
        {
            if (followerBreakFlag[l] == true && handFollower[l] == 0)//演出中に新たなシュジンコウが出ていたらステータス初期化は行わない。（ただし現状の仕様では存在しないケース）
            {
                for (i = 0; i < 3; i++)
                {
                    followerStatus[l, i] = 0;
                }
            }
        }
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
        int i, j;
        CardData c1 = GetComponent<CardData>();
        c1.CardList();
        if (player == 0)
        {
            c1.LoadDeckList(0);
        }
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
                library[player, i, 0, 0] = c1.deckCard[player, i];//カードの種類
            }
        }
        //カード番号が決まったなら、コストや効果をそれに合わせて代入(ライブラリが作成された段階でカードのデータも代入しておくことで、サーチカード等も実装しやすい）
        for (i = 0; i < DECKCARD_NUM; i++)
        {
            for (j = 1; j < BLOCKTYPE_NUM + 1; j++)
            {
                library[player, i, 1, j] = c1.cardCost[library[player, i, 0, 0], j];
            }//カードのコスト
            for (j = 0; j < SKILL_TYPE; j++)
            {
                library[player, i, 2, j] = c1.cardSkill[library[player, i, 0, 0], j];
            }//カードの効果
        }
        libraryNum[player] = DECKCARD_NUM;
        //シャッフル
        if ((player == 1 && GameObject.Find("BGMManager").GetComponent<BGMManager>().multiPlay != 0) == false)
        { Shuffle(player); }//通信対戦の相手から受信したライブラリはシャッフルしない。（相手方でシャッフル済）
    }

    //呪文詠唱時演出
    //キャラクターカットイン（呪文名表示）
    public IEnumerator SpellEffect(int player, int hand)//playerがＰＬエネミー、handが手札の何枚目か（多重召喚失敗の場合は特別に引数3とする）。
    {
        int i;
        cutInRunning = true;
        CardData c1 = GetComponent<CardData>();
        c1.CardList();
        objCutIn[player].GetComponent<Image>().enabled = true;

        if (player == 0) { objCutIn[player].GetComponentInChildren<Text>().text += "<color=red>"; }
        if (player == 1) { objCutIn[player].GetComponentInChildren<Text>().text += "<color=blue>"; }
        if (hand < 3)
        {
            objCutIn[player].GetComponentInChildren<Text>().text += c1.cardName[handCard[player, hand]];
            string[] explainList = c1.cardExplain[handCard[player, hand]].Split('\n');
            objCutIn[player].GetComponentInChildren<Text>().text += "\n<size=24>";
            objCutIn[player].GetComponentInChildren<Text>().text += explainList[2];
            objCutIn[player].GetComponentInChildren<Text>().text += "</size>";
        }//通常handは３未満。
        if (hand == 3) { objCutIn[player].GetComponentInChildren<Text>().text += missSpellName + "？\n(多重召喚)"; }//多重召喚失敗時のhandが3。その専用処理。
        objCutIn[player].GetComponentInChildren<Text>().text += "</color>";

        for (i = 0; i < 10; i++)
        {
            if (player == 0) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_SPELL_POSITION_X - 500 + 50 * i, PLAYER_SPELL_POSITION_Y, 0); }
            if (player == 1) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_SPELL_POSITION_X + 500 - 50 * i, ENEMY_SPELL_POSITION_Y, 0); }
            yield return null;
        }
        if (player == 0) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_SPELL_POSITION_X, PLAYER_SPELL_POSITION_Y, 0); }//出てきたら位置を固定
        if (player == 1) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_SPELL_POSITION_X, ENEMY_SPELL_POSITION_Y, 0); }
        for (i = 0; i < SPELL_TIME - 10; i++)//そのまま演出の残り時間を待つ。（SpellMissと同時並行でコルーチンを回している場合は、ここのループの間にSpellMiss側でobjcutinに変更を加える）
        {
            yield return null;
        }
        if (player == 0) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_SPELL_POSITION_X - 500, PLAYER_SPELL_POSITION_Y, 0); }
        if (player == 1) { objCutIn[player].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_SPELL_POSITION_X + 500, ENEMY_SPELL_POSITION_Y, 0); }
        objCutIn[player].GetComponentInChildren<Text>().text = "";
        objCutIn[player].GetComponent<Image>().enabled = false;
        cutInRunning = false;
    }

    //missSpellName（多重召喚失敗用テキスト）の生成
    public void MakeMissSpellName(int card)
    {
        CardData c1 = GetComponent<CardData>();
        c1.CardList();
        //カード名をmissSpellNameに追加する
        missSpellName += c1.cardName[card];
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

    //シュジンコウ破壊演出
    //点滅後に消える。
    public IEnumerator FollowerBreakEffect(int player)
    {
        int i;
        for (i = 0; i < FOLLOWER_BREAK_TIME; i++)
        {
            if (handFollower[player] == 0) { objFollower[player].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 0.5f - 0.5f * (Mathf.Cos((float)timeCount / 10))); } else { yield break; }//演出中に新たなシュジンコウを出したらコルーチン終了する
            yield return null;
        }
        seAudioSource[15].PlayOneShot(se[15]);
        objFollower[player].GetComponent<Image>().color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        if (handFollower[player] == 0)//新たなシュジンコウが出ていなければ（ただし現状の仕様では存在しないケース）
        {
            handFollowerForDraw[player] = 0;//ここで画像も完全消滅。
        }
        yield return null;
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

    //シュジンコウの攻撃演出
    public IEnumerator FollowerAttack()
    {
        int i;

        if (handFollower[0] != 0 && handFollower[1] != 0)//両方シュジンコウがいる場合の演出
        {
            for (i = 0; i < 10; i++)
            {
                objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X + 32 * i, PLAYER_FOLLOWER_POSITION_Y, 0);//画面の真ん中(-15)でぶつかる
                objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X - 32 * i, ENEMY_FOLLOWER_POSITION_Y, 0);
                yield return null;
            }
            seAudioSource[7].PlayOneShot(se[7]);
            StartCoroutine(Damage(1, followerStatus[0, 0]));//Damageの第一引数はダメージを「受ける」キャラクターなのでシュジンコウの持ち主とは逆。
            yield return StartCoroutine(Damage(0, followerStatus[1, 0]));//ダメージ演出を同時にやるなら、yield return StartCoroutine（今のコルーチンの進行を止めて参照コルーチンの終了を待つ）は後ろの一つだけでいい。
            for (i = ATTACK_TIME / 2 - 10; i > 0; i--)
            {
                objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X + 32 * (10 / (ATTACK_TIME / 2 - 10)) * i, PLAYER_FOLLOWER_POSITION_Y, 0);//前for文の逆動作を残りの時間で実行。
                objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X - 32 * (10 / (ATTACK_TIME / 2 - 10)) * i, ENEMY_FOLLOWER_POSITION_Y, 0);
                yield return null;
            }
            objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X, PLAYER_FOLLOWER_POSITION_Y, 0);//元の位置へ
            objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X, ENEMY_FOLLOWER_POSITION_Y, 0);
        }

        if (handFollower[0] != 0 && handFollower[1] == 0)//プレイヤーにだけいる場合の演出
        {
            for (i = 0; i < 10; i++)
            {
                objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X + 86 * i, PLAYER_FOLLOWER_POSITION_Y, 0);//敵のＬＰ表示(530)にぶつかる
                yield return null;
            }
            seAudioSource[8].PlayOneShot(se[8]);
            yield return StartCoroutine(Damage(1, followerStatus[0, 0]));//Damageの第一引数はダメージを「受ける」キャラクターなのでシュジンコウの持ち主とは逆。
            for (i = ATTACK_TIME / 2 - 10; i > 0; i--)
            {
                objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X + 86 * (10 / (ATTACK_TIME / 2 - 10)) * i, PLAYER_FOLLOWER_POSITION_Y, 0);//前for文の逆動作を残りの時間で実行。
                yield return null;
            }
            objFollower[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_FOLLOWER_POSITION_X, PLAYER_FOLLOWER_POSITION_Y, 0);//元の位置へ
        }

        if (handFollower[0] == 0 && handFollower[1] != 0)//敵にだけいる場合の演出
        {
            for (i = 0; i < 10; i++)
            {
                objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X - 83 * i, ENEMY_FOLLOWER_POSITION_Y, 0);//プレイヤーのＬＰ表示(-530)にぶつかる
                yield return null;
            }
            seAudioSource[8].PlayOneShot(se[8]);
            yield return StartCoroutine(Damage(0, followerStatus[1, 0]));//Damageの第一引数はダメージを「受ける」キャラクターなのでシュジンコウの持ち主とは逆。
            for (i = ATTACK_TIME / 2 - 10; i > 0; i--)
            {
                objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X - 83 * (10 / (ATTACK_TIME / 2 - 10)) * i, ENEMY_FOLLOWER_POSITION_Y, 0);//前for文の逆動作を残りの時間で実行。
                yield return null;
            }
            objFollower[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_FOLLOWER_POSITION_X, ENEMY_FOLLOWER_POSITION_Y, 0);//元の位置へ
        }
    }

    //ブロックの消去演出
    public IEnumerator EliminatBlockEffect(int x, int y, int color)
    {
        int i;
        //消えたブロックが何点のマナになったかテキスト表示(この時点ではchainCount増加はまだなので+1しておく)
        if (color == 1) { objEliminatBlock[x, y].GetComponent<Text>().text = "<color=red>" + (chainCount + 1).ToString() + "</color>"; }//color==1(赤)
        if (color == 2) { objEliminatBlock[x, y].GetComponent<Text>().text = "<color=blue>" + (chainCount + 1).ToString() + "</color>"; }//color==2(青)
        if (color == 3) { objEliminatBlock[x, y].GetComponent<Text>().text = "<color=green>" + (chainCount + 1).ToString() + "</color>"; }//color==3(緑)
        if (color == 4) { objEliminatBlock[x, y].GetComponent<Text>().text = "<color=black>" + (chainCount + 1).ToString() + "</color>"; }//color==4(黒)
        if (color == 5) { objEliminatBlock[x, y].GetComponent<Text>().text = "<color=yellow>" + (chainCount + 1).ToString() + "</color>"; }//color==5(黄)

        for (i = 0; i < 30; i++)
        {//消去演出オブジェクトをプレイヤー表示へと集める
            objEliminatBlock[x, y].GetComponent<RectTransform>().localPosition = new Vector3(((x * BLOCK_SIZE + FIELD_LEFT) * (30 - i) + MANA_POSITION_X * i) / 30, ((FIELD_TOP - y * BLOCK_SIZE) * (30 - i) + MANA_POSITION_Y * i) / 30, 0);//プレイヤー表示(MANA_POSITION)にぶつかる
            yield return null;
        }

        //消去演出オブジェクトを消す。
        objEliminatBlock[x, y].GetComponent<Text>().text = "";
    }

    //呪文の立ち消え演出（対象がいない、多重召喚等で呪文に失敗した場合）
    public IEnumerator SpellMiss(int player)
    {
        int i;
        for (i = 0; i < SPELL_TIME - 30; i++)//カットイン消滅残り30Fまで（SPELL_TIME-30）は動かない。
        {
            yield return null;
        }
        objTarai[player].GetComponent<Image>().enabled = true;
        for (i = 0; i < 10; i++)//10Fでタライが落ちる(350→150)
        {
            if (player == 0) { objTarai[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_TARAI_POSITION_X, PLAYER_TARAI_POSITION_Y - 20 * i, 0); }
            if (player == 1) { objTarai[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_TARAI_POSITION_X, ENEMY_TARAI_POSITION_Y - 20 * i, 0); }
            yield return null;
        }
        objShock[player].GetComponent<Image>().enabled = true;
        objCutIn[player].GetComponent<Image>().enabled = true;
        seAudioSource[12].PlayOneShot(se[12]);
        for (i = 0; i < 20; i++)//ぶつかったらタライは浮き上がり、カットインは回転しつつ下がる。
        {
            if (player == 0)
            {
                objTarai[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_TARAI_POSITION_X - i * 2, PLAYER_TARAI_POSITION_Y - 200 + 3 * i, 0);
                objTarai[0].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, 2));
                objCutIn[0].GetComponent<RectTransform>().localPosition = new Vector3(PLAYER_SPELL_POSITION_X, PLAYER_SPELL_POSITION_Y - i * 5, 0);
                objCutIn[0].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, -1));
            }
            if (player == 1)
            {
                objTarai[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_TARAI_POSITION_X + i * 2, ENEMY_TARAI_POSITION_Y - 200 + 3 * i, 0);
                objTarai[1].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, -2));
                objCutIn[1].GetComponent<RectTransform>().localPosition = new Vector3(ENEMY_SPELL_POSITION_X, ENEMY_SPELL_POSITION_Y - i * 5, 0);
                objCutIn[1].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, 1));
            }
            yield return null;
        }
        if (player == 0)
        {
            objTarai[0].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, -2 * 20));
            objCutIn[0].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, 1 * 20));
        }
        if (player == 1)
        {
            objTarai[1].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, 2 * 20));
            objCutIn[1].GetComponent<RectTransform>().transform.Rotate(new Vector3(0, 0, -1 * 20));
        }
        objCutIn[player].GetComponent<Image>().enabled = false;
        objTarai[player].GetComponent<Image>().enabled = false;
        objShock[player].GetComponent<Image>().enabled = false;
    }

    //ブレイク処理の演出
    public IEnumerator ShakeField()
    {
        while (blockAutoMoveTime < 0)
        {
            objField.GetComponent<RectTransform>().localPosition = new Vector3(5 * (Mathf.Cos((float)timeCount / 1)), 0, 0);
            yield return null;
        }
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

    //一時停止ボタンの挙動
    public void StopButton()
    {
        if (stopFlag == false)
        {
            stopFlag = true;
            objStopButton.GetComponent<Image>().sprite = stopImage[1];
            objBackButton.gameObject.SetActive(true);      //戻るボタンは一時停止中しか出てこない。
        }
        else
        {
            stopFlag =false;
            objStopButton.GetComponent<Image>().sprite = stopImage[0];
            objBackButton.gameObject.SetActive(false);      //戻るボタンは一時停止中しか出てこない。
            key = 0;
        }
    }

    //戻るボタンの挙動
    public void BackButton()
    {
            GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
    }

}