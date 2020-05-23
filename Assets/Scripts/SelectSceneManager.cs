using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectSceneManager : MonoBehaviour {

    const int CHAPTER_NUM = 6;                                        //名も無い本（回想）の章の数
    const int BOOK_NUM = 3;                                           //通常の本の数

    private bool chapterSelectFlag;                                 //章選択表示をしているかのフラグ
    
    private GameObject[] objBook = new GameObject[BOOK_NUM];                  //本のオブジェクト
    private GameObject[] objBlackBookChapter = new GameObject[CHAPTER_NUM];   //名も無い本の章選択オブジェクト

    // Use this for initialization
    void Start() {

        //BGM読み込みと再生
        GetComponent<Utility>().BGMPlay(Resources.Load<AudioClip>("森林ループ"));


    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushMakeBookButton()
    {
        SceneManager.LoadScene("MakeBookScene");
    }

    public void PushBlueBookButton()
    {
        PlayerPrefs.SetString("ScenarioName", "テスト用文章");
        SceneManager.LoadScene( "StoryScene");
    }
    



}