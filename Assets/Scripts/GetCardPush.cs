using UnityEngine;
using UnityEngine.UI;

public class GetCardPush : MonoBehaviour {

    public GameObject refObj;

    // Use this for initialization
    void Start () {
        refObj= GameObject.Find("StorySceneManager");
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //獲得カードがタップされた場合
    public void PushCard()
    {
        Scenario s1 = refObj.GetComponent<Scenario>();
        CardData c1 = refObj.GetComponent<CardData>();
        refObj.GetComponent<Utility>().pushObjectFlag = true;
        c1.CardList();//カードリストのロード 
        s1.objText.GetComponent<Text>().text = "<size=36>" + c1.cardExplain[s1.getCard[int.Parse(name.Substring(4))]] + "</size>";//テキストを説明文に変更
    }
    //獲得カードから指が離れた場合
    public void LeaveCard()
    {
        Scenario s1 = refObj.GetComponent<Scenario>();
        s1.objText.GetComponent<Text>().text = s1.getCardText;//テキストを戻す
        refObj.GetComponent<Utility>().pushObjectFlag = false;
    }


}
