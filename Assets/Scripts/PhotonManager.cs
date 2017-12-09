using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonManager : MonoBehaviour {

    const int ROOM_NUM = 5;             //最大部屋数

    private string[] roomName = new string[ROOM_NUM];   //各部屋の名前
    private GameObject[] objRoom = new GameObject[ROOM_NUM];//部屋ボタンのオブジェクト
    private GameObject objBackLobbyButton;
    private GameObject objMakeRoomButton;

	// Use this for initialization
	void Start () {
        GameObject.Find("Placeholder").GetComponent<Text>().text= PlayerPrefs.GetString("userName", "ナナシ");
        for (int i = 0; i < ROOM_NUM; i++){ objRoom[i]=GameObject.Find("Room" + i.ToString());GameObject.Find("Room" + i.ToString()).SetActive(false);}//変数に入れてから非アクティブ化
        objBackLobbyButton=GameObject.Find("ButtonBackLobby");
        objBackLobbyButton.SetActive(false);
        objMakeRoomButton= GameObject.Find("ButtonMakeRoom");
        PhotonNetwork.ConnectUsingSettings("ver1.1");    //ロビー入室
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    //ロビーに入ると※現時点では中身なし。必要に応じて追加
    void OnJoinedLobby()
    {
    }

    //ルーム作成
    public void CreateRoom()
    {
        string userName = PlayerPrefs.GetString("userName", "ナナシ");
        PhotonNetwork.autoCleanUpPlayerObjects = false;
        //カスタムプロパティ
        ExitGames.Client.Photon.Hashtable customProp = new ExitGames.Client.Photon.Hashtable();
        customProp.Add("userName", userName); //ユーザ名
        PhotonNetwork.SetPlayerCustomProperties(customProp);

        RoomOptions roomOptions = new RoomOptions();
        roomOptions.customRoomProperties = customProp;
        //ロビーで見えるルーム情報としてカスタムプロパティのuserNameを使いますよという宣言
        roomOptions.customRoomPropertiesForLobby = new string[] { "userName" };
        roomOptions.maxPlayers = 2; //部屋の最大人数
        roomOptions.isOpen = true; //入室許可する
        roomOptions.isVisible = true; //ロビーから見えるようにする
        //userNameが名前のルームがなければ作って入室、あれば普通に入室する。
        PhotonNetwork.JoinOrCreateRoom(userName, roomOptions, null);
        //ルームに入ったらロビー関連ボタンは消去
        for (int i = 0; i < ROOM_NUM; i++) { objRoom[i].SetActive(false); }
        objMakeRoomButton.SetActive(false);
        GameObject.Find("TitleText").GetComponent<Text>().text = "タイセンシャ\n　　　サンセンマチ";
        objBackLobbyButton.SetActive(true);
    }

    //ルーム一覧が取れると
    void OnReceivedRoomListUpdate()
    {
        //ルーム一覧を取る
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        if (rooms.Length == 0)
        {
            //部屋がない時の特別処理は現状では何もナシ。
        }
        else
        {
            GameObject.Find("TitleText").GetComponent<Text>().text = "";
            //それぞれのルームのボタン出現
            for (int i = 0; i < rooms.Length; i++)
            {
                if (rooms[i].playerCount == 2) {continue;}//既に２人入っている部屋は出現しない
                objRoom[i].SetActive(true);
                objRoom[i].GetComponentInChildren<Text>().text = "対戦相手：\n" + (string)rooms[i].customProperties["userName"];
                roomName[i] = (string)rooms[i].customProperties["userName"];
            }
        }
    }

    //押したボタンに応じた部屋に入る
    public void PushJoinRoomButton(int roomNum)
    {
        PhotonNetwork.JoinRoom(roomName[roomNum]);
    }

    //部屋作成に失敗
    void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        GameObject.Find("TitleText").GetComponent<Text>().text = "ルームが\n　　　つくれません。";
    }

    //部屋に入れなかった
    void OnPhotonJoinRoomFailed(object[] codeAndMsg)
    {
        GameObject.Find("TitleText").GetComponent<Text>().text = "ルームに\n　　　はいれません。";
    }

    //部屋からロビーに戻る
    public void PushBackLobbyBotton()
    {
        objBackLobbyButton.SetActive(false);
        objMakeRoomButton.SetActive(true);
        PhotonNetwork.LeaveRoom();
    }

    public void ChangeName()
    {
        PlayerPrefs.SetString("userName", GameObject.Find("TextInput").GetComponent<Text>().text);
        GameObject.Find("Name").GetComponent<Text>().text = "Name:";
    }

    //部屋に入ると※未実装
    void OnJoinedRoom()
    {
        Debug.Log("PhotonManager OnJoinedRoom");
    }

    //オフラインゲームに戻る
    public void PushExitButton()
    {
        PhotonNetwork.Disconnect();
        GetComponent<Utility>().StartCoroutine("LoadSceneCoroutine", "SelectScene");
    }

}
//通信対戦部分の実装。