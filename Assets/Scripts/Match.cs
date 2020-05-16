using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1)]//Matchは他から引用されるのでstartを先行処理させる。
public class Match : Photon.MonoBehaviour {

    const int HAND_NUM = 3;
    const int DECKCARD_NUM = 20;                  
    private Card[] enemyLibrary = new Card[DECKCARD_NUM];
    private Card[] enemyHand = new Card[HAND_NUM];
    private int enemyLibraryNum;
    private int wait;
    public PhotonView m_photonView;
    PuzzleSceneManager p1;
    CardData c1;

    void OnPhotonSerializeView(PhotonStream i_stream, PhotonMessageInfo i_info)
    {
        //マナ情報を随時更新するならここで（ObserveOptionの変更も必要）
    }

    public void DataChange()
    {
        this.name = "MatchManager";
        PuzzleSceneManager p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        for (int i = 0; i < DECKCARD_NUM; i++) { enemyLibrary[i]=p1.library[0, i];}
        enemyLibraryNum = p1.libraryNum[0];
        for (int i = 0; i < HAND_NUM; i++)
        {
            enemyHand[i] = p1.handCard[0,i];
        }
        wait = p1.waitCount[0];
        m_photonView.RPC("ChangeDataInput", PhotonTargets.Others, wait,enemyLibrary,enemyLibraryNum,enemyHand);
    }

    public void MatchEnd()
    {
        PhotonNetwork.Disconnect();
    }

    // Use this for initialization
    void Start()
    {
        p1 = GameObject.Find("PuzzleSceneManager").GetComponent<PuzzleSceneManager>();
        c1 = GameObject.Find("PuzzleSceneManager").GetComponent<CardData>();
        m_photonView = this.GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    [PunRPC]
    private void ChangeDataInput(int wait,Card[] data, int data3,Card[] data4,bool data5)
    {

        p1.waitCount[1]=wait;

        enemyLibrary = data;
        for (int i = 0; i < DECKCARD_NUM; i++)
        {
            p1.library[1, i] = enemyLibrary[i];
        }
        

        enemyLibraryNum = data3;
        p1.libraryNum[1] = enemyLibraryNum;

        enemyHand = data4;
    }
}