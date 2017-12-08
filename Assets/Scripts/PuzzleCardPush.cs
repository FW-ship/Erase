using UnityEngine;
using UnityEngine.UI;

public class PuzzleCardPush : MonoBehaviour {
    const int HAND_NUM = 3;

    private GameObject refObj;
    private GameObject[,] objText=new GameObject[2,HAND_NUM];
    private GameObject[,] objImage = new GameObject[2, HAND_NUM];

    // Use this for initialization
    void Start () {
        refObj= GameObject.Find("PuzzleSceneManager");
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < HAND_NUM; j++)
            {
                objText[i, j] = GameObject.Find("ExplainText" + i.ToString() + j.ToString());
                objImage[i, j] = GameObject.Find("ExplainBack" + i.ToString() + j.ToString());
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //カードがタップされた場合
    public void PushCard()
    {
        objImage[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))].GetComponent<Image>().enabled = true;
        objText[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))].GetComponent<Text>().enabled = true;
        PuzzleSceneManager p1 = refObj.GetComponent<PuzzleSceneManager>();
        CardData c1 = refObj.GetComponent<CardData>();
        c1.CardList();//カードリストのロード
        string[] explainList = c1.cardExplain[p1.handCard[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))]].Split('\n');
        objText[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))].GetComponent<Text>().text =explainList[0] + "\n" + explainList[1] + "\n" + explainList[2] + "\n" + explainList[3];//テキストを代入
    }
    //獲得カードから指が離れた場合
    public void LeaveCard()
    {
        objImage[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))].GetComponent<Image>().enabled = false;
        objText[int.Parse(name.Substring(4)) / 10, int.Parse(name.Substring(5))].GetComponent<Text>().enabled = false;
    }
}
