﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-10)]//utilityは他から引用されるのでstartを先行処理させる。
public class Utility : MonoBehaviour {
    private GameObject objBGM;                                  //BGMのオブジェクト
    private bool fadeFlag;                                      //フェードイン・フェードアウト中か否か
    public bool pushObjectFlag;                                 //ボタンオブジェクトのタップ(true)か画面自体（ストーリー進行）のタップ(false)かの判定
    public bool selectFlag;                                     //選択待ち中、どれかが選択されたか否かの判定
    private float touchStartPos;
    private float touchEndPos;
    public int flick;

    // Use this for initialization
    void Start () {
        objBGM = GameObject.Find("BGMManager").gameObject as GameObject;
        pushObjectFlag = false;
    }

	
	// Update is called once per frame
	void Update () {
        Flick();
	}


    public IEnumerator LoadSceneCoroutine(string scene)
    {
        GetComponent<Utility>().SEAdd(gameObject);
        SEPlay(gameObject, Resources.Load<AudioClip>("pera"));
        yield return new WaitForSeconds(0.2f);//操作感のために僅かにウェイトを持たせる。
        GameObject.Find("NowLoading").GetComponent<Image>().enabled = true;
        SceneManager.LoadScene(scene);
        yield return null;
    }

    public void BGMVolume(float volume)
    {
        PlayerPrefs.SetFloat("BGMVolume", volume);
        objBGM.GetComponent<AudioSource>().volume = volume;
    }

    public void SEVolume(float volume)
    {
        PlayerPrefs.SetFloat("SEVolume", volume);
    }

    public void BGMStop()
    {
        objBGM.GetComponent<AudioSource>().Stop();
    }

    public IEnumerator BGMFadeOut(int time)
    {
        while (fadeFlag == true) { yield return null; }//他でフェイドインフェイドアウト中なら待つ。
        fadeFlag = true;
        for (int i = 0; i < time; i++)
        {
            objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SEVolume",0.8f) * (1.0f-(float)i/time);
            yield return null;
        }
        objBGM.GetComponent<AudioSource>().volume = 0f;//最終的には０に。（for文をi<=timeにするとtime=0で０除算が発生しうる構造になるので、最後のvol=0のみfor文から隔離）
        fadeFlag = false;
    }

    public IEnumerator BGMFadeIn(int time)
    {
        while (fadeFlag == true) { yield return null; }//他でフェイドインフェイドアウト中なら待つ。
        fadeFlag = true;
        for (int i = 0; i < time; i++)
        {
            objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SEVolume", 0.8f) * ((float)i / time);
            yield return null;
        }
        objBGM.GetComponent<AudioSource>().volume = PlayerPrefs.GetFloat("SEVolume", 0.8f);//最終的には０に。（for文をi<=timeにするとtime=0で０除算が発生しうる構造になるので、最後のvol=SEVolumeのみfor文から隔離）
        fadeFlag = false;
    }

    public void BGMPlay(AudioClip bgm)
    {
        objBGM.GetComponent<AudioSource>().clip = bgm;
        objBGM.GetComponent<AudioSource>().Play();
    }

    //SEはシーンごとにオブジェクト作成するので（コンポーネントが複数個必要になる場合があるため）、AddComponentと同時にvolume操作を行う専用関数を作ってAddComponentの代わりに使用し、SEvolumeを反映させる。
    public void SEAdd(GameObject obj)
    {
        obj.AddComponent<AudioSource>();
        AudioSource[] se = obj.GetComponents<AudioSource>();
        for (int i = 0; i < se.Length; i++)
        {
            se[i].volume = PlayerPrefs.GetFloat("SEVolume", 0.8f);//addcomponentが複数あると重複処理することになるが、１関数にまとめた方が分かりやすい
        }
    }

    public void SEPlay(GameObject obj ,AudioClip se)
    {
        obj.GetComponent<AudioSource>().PlayOneShot(se);
    }

    //画面が押されたかチェックするコルーチン
    public IEnumerator PushWait()
    {
        while (true)//ブレークするまでループを続ける。
        {
            if (Input.GetMouseButtonDown(0) == true)
            {
                yield return null;//本体に処理を返して他のオブジェクトのイベントトリガーを確認。
                if (pushObjectFlag == false)//フラグが立っていたらオブジェクト処理のためのタップだったと判定。
                {
                    yield break;//falseならコルーチン脱出
                }
                else
                {
                    yield return null;//trueならコルーチン継続
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    public IEnumerator SelectWait()
    {
        selectFlag = false;
        while (true)//ループを続ける。
        {
            yield return null;
            if (selectFlag == true) { break; }
        }
    }

    //URLへの遷移と、その前の演出等を見せるための待機をセットにしたコルーチン
    public IEnumerator GoToURL(string URL,float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        Application.OpenURL(URL);
    }

    void Flick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            touchStartPos = Input.mousePosition.x;
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            touchEndPos = Input.mousePosition.x;
            flick = GetDirection();
        }
        else {
            flick = 0;
        }
    }
    int GetDirection()
    {
        float directionX = touchEndPos - touchStartPos;


        if (30 < directionX)
        {
            //右向きにフリック
            return -1;
        }
        else if (-30 > directionX)
        {
            //左向きにフリック
            return 1;
        }
        return -100;//ただのタップ
    }


}
