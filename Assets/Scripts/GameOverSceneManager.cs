using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

public class GameOverSceneManager : MonoBehaviour {

    private int timeCount;                                           //シーン開始からの経過時間
    
    private GameObject objShicho;                                    //シチョウ（キャラクター）のオブジェクト
    private GameObject objText;                                      //説明テキストのオブジェクト
    private List<Sprite> shichoImage = new List<Sprite>();           //シチョウのスプライト

    // Use this for initialization
    void Start () {

        //BGM読み込みと再生
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("おもしろすぎてどっかん"));

        //効果音設定
        GetComponent<Utility>().SEAdd(gameObject);//効果音用のオーディオソースはシーン間引継ぎの必要がないのでシーンマネージャーに追加。

        //オブジェクト読み込み
        objShicho = GameObject.Find("Shicho").gameObject as GameObject;
        objText = GameObject.Find("ShichoText").gameObject as GameObject;

        //画像読み込み
        shichoImage.Add(Resources.Load<Sprite>("character51"));
        shichoImage.Add(Resources.Load<Sprite>("character52"));
        StartCoroutine(Guide());
    }
	
	// Update is called once per frame
	void Update () {
        timeCount++;
        objShicho.GetComponent<RectTransform>().localPosition = new Vector3(500, -40 - 10 * (Mathf.Cos((float)timeCount / 10)), 1);
    }

    public void PushTitleButton()
    {
        Utility u1 = GetComponent<Utility>();
        if (u1.pushObjectFlag == false)//押している間SkipButtonはずっと呼ばれるので、フラグが立った後はコルーチンを呼ばないようにする。（Onclickは押す→離すまでの一連の処理をトリガーとするので、押下のタイミングでは反応を返さない。故にGetCardPushやWaitPushとイベント判定タイミングを『押下時』で統一するためonclickは使わない。判定タイミングが異なるとWaitPushのpushObjectFlag判定が正しく行えない）
        {
            GameObject.Find("ButtonTitle").GetComponent<Animator>().enabled = true;
            GameObject.Find("ButtonTitle").GetComponent<Animator>().Play("Pressed", 0, 0);
            u1.StartCoroutine("LoadSceneCoroutine", "Title");
        }
        u1.pushObjectFlag = true;
    }


    private IEnumerator Guide()
    {
        Utility u1 = GetComponent<Utility>();
        if (PlayerPrefs.GetInt("scenarioCount", 0) == 20000)//シチョウ戦敗北
        {
            objShicho.GetComponent<Image>().sprite = shichoImage[1];
            objText.GetComponent<Text>().text = "ちょっとは頭が冷えたかな？";
            yield return StartCoroutine(u1.PushWait());
            objShicho.GetComponent<Image>().sprite = shichoImage[0];
            objText.GetComponent<Text>().text = "アタシに挑戦するなら何度でも受けて立つよ。ずっとここでゲームを楽しもう？";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "その方が絶対いいって。ね？";
            yield return StartCoroutine(u1.PushWait());
        }
        else//シチョウ戦以外
        {
            objShicho.GetComponent<Image>().sprite = shichoImage[1];
            objText.GetComponent<Text>().text = "システムキャラクター・シチョウちゃんのワンポイントアドバイス！";
            yield return StartCoroutine(u1.PushWait());
            objShicho.GetComponent<Image>().sprite = shichoImage[0];
            objText.GetComponent<Text>().text = "まず、相手の一番怖い呪文が何かを掴もう。その呪文の対抗策を考えるのがスタート。";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "本（デッキ）の相性によってはどんなにパズルゲームが上手くても勝てないことがあるからね。";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "あとはシュジンコウの配分かな。多重召喚を防ぐためにも５枚～１０枚くらいがやりやすいと思う。";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "シュジンコウ同士の必要魔力の種類ができるだけ被らないようにするのもコツだよ。";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "それでも勝てない時はどうするか。";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "相手の呪文の引きが悪い時を狙って、無抵抗の相手を嬲ればいいんじゃないかな！　何度でも挑戦できるんだし！";
            yield return StartCoroutine(u1.PushWait());
            objText.GetComponent<Text>().text = "それじゃ、次も頑張ってね。ついでに説明書（Guide）も見に来てね～！";
            yield return StartCoroutine(u1.PushWait());
        }
        u1.StartCoroutine("LoadSceneCoroutine", "Title");
    }

}