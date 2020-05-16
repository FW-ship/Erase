using UnityEngine;
using UnityEngine.UI;

public class CardPush : MonoBehaviour {

    public GameObject refObj;
    public GameObject refObj2;
    Drag d1;
    CardData c1;
    // Use this for initialization
    void Start () {
        refObj = GameObject.Find("card1");
        refObj2= GameObject.Find("MakeBookSceneManager");
        d1 = refObj.GetComponent<Drag>();
        c1 = refObj2.GetComponent<CardData>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PushCard()
    {
        int i;
        
        d1.objSelectCardExplain.SetActive(true);
        i = c1.deckCard[0,int.Parse(name.Substring(8))].cardNum;
        d1.objSelectCardExplain.GetComponentInChildren<Text>().text = c1.card[i].cardExplain;
    }

    public void LeaveCard()
    {
        d1.objSelectCardExplain.SetActive(false);
    }


}
