using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]//Matchは他から引用されるのでstartを先行処理させる。
public class Match : Photon.MonoBehaviour {

    const int BLOCKTYPE_NUM = 5;
    const int HAND_NUM = 3;
    const int DECKCARD_NUM = 20;
    public int[,] cardMana = new int[HAND_NUM,BLOCKTYPE_NUM+1];
    public int[] library = new int[DECKCARD_NUM];

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        //マナ情報を随時更新するならここで（ObserveOptionの変更も必要）
    }

    public void DataChange()
    {
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < DECKCARD_NUM; i++) { library[i]=p1.library[0, i, 0, 0];}
        this.photonView.RPC("ChangeDataInput", PhotonTargets.AllViaServer, library);
    }

    [PunRPC]
    private void ChangeDataInput(int[] data)
    {
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            p1.library[1, i, 0, 0] = data[i];
        }
        p1.waitFlag = false;
    }

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}
}
//RPCでデータそうしんができてない。なぜ？