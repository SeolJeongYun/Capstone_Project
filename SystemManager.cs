using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using BackEnd;
using LitJson;

public class SystemManager : MonoBehaviourPunCallbacks
{
    public InputField L_email;
    public InputField L_password;
    public InputField C_email;
    public InputField C_password;
    public InputField Nickname;
    public InputField ChatInput;
    public GameObject basePanel;
    public GameObject onlinePanel;
    public GameObject offlinePanel;
    public GameObject loginPanel;
    public GameObject signupPanel;
    public GameObject nicknamePanel;
    public GameObject loadingPanel;
    public GameObject StatusPanel;
    public GameObject chatPanel;
    public GameObject MenuBar;
    public Text NoticeText;
    public Text[] ChatText;
    public Button N_checkbtn;
    public Button startbtn;
    public Button LoginBtn;
    public Button SignUpBtn;
    public Button ManBtn;
    public Button WomenBtn;
    public Button sendBtn;
    int gender;
    int n_cp;
    int status;
    int menu_check;
    int back_check;
    int chat_check = 0;
    string UserNickName;
    string inDate = null;
    public PhotonView PV;
//-----------------------------------------------------------------------------------------
    
    void Awake()
    {
        Screen.SetResolution(960,540,false);
        PhotonNetwork.SendRate = 60;
        PhotonNetwork.SerializationRate = 30;
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    void Start()
    {
        var bro = Backend.Initialize(true);

        if(bro.IsSuccess())
        {
            Debug.Log("Success" + bro);
        }   
        else
        {
            Debug.LogError("error" + bro);
        }
    }

//-----------------------------------Photon Network----------------------------------------

    public void Connect() => PhotonNetwork.ConnectUsingSettings();

    public override void OnConnectedToMaster()
    {
        if(PhotonNetwork.IsConnected)
        {
            Debug.Log("서버 연결 성공");
            PhotonNetwork.LocalPlayer.NickName = Backend.UserNickName;
            PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 10}, null);
        }
        else 
        {
            Debug.Log("서버 재접속 중");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnJoinedRoom()
    {
        Transform[] points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        int idx = Random.Range(1, points.Length);
        var bro = Backend.GameData.Get("user_gender",new Where());
        var data = bro.GetReturnValuetoJSON();
        var rows = data["rows"];
        for(int i = 0; i < rows.Count; ++i)
        {
            var user = rows[i];
            inDate = user["inDate"]["S"].ToString();
            Debug.Log(inDate);
        }
        // select[]를 이용하여 리턴 시, owner_inDate와 score만 출력되도록 설정
        string[] select = {"gender"};
        // 테이블 내 해당 rowIndate를 지닌 row를 조회
        // select에 존재하는 컬럼만 리턴
        bro = Backend.GameData.GetMyData("user_gender", inDate , select);
        data = bro.GetReturnValuetoJSON()["row"];
        string gender = data["gender"]["N"].ToString();
        Debug.Log(gender.ToString());
        if(gender == "1")
        {
            PhotonNetwork.Instantiate("Man", points[idx].position, points[idx].rotation, 0);
        }
        else if(gender == "2")
        {
            PhotonNetwork.Instantiate("Women", points[idx].position, points[idx].rotation, 0);
        }
    }

    public void Disconnect()
    {
        if(PhotonNetwork.IsConnected)
        {
            Debug.Log("서버 끊음");
            PhotonNetwork.Disconnect();
        }
    }
//-----------------------------------Backend Network---------------------------------------

    public void SignUp()
    {
        var bro = Backend.BMember.CustomSignUp(C_email.text, C_password.text);
        if (bro.IsSuccess()) 
        {
            //Debug.Log("회원가입에 성공했습니다. : " + bro);
            NoticeText.text = "회원가입에 성공했습니다.";
            status = 1;
            StatusPanel.SetActive(true);
        } 
        else 
        {
            //Debug.LogError("회원가입에 실패했습니다. : " + bro);
            NoticeText.text = "다른 이메일을 입력해주세요.";
            status = 2;
            StatusPanel.SetActive(true);
        }
    }

    public void Login()
    {
        var bro = Backend.BMember.CustomLogin(L_email.text, L_password.text);
        if (bro.IsSuccess()) 
        {
            UserNickName = Backend.UserNickName;
            if(UserNickName.Length>0)
            {
                Connect();
                onlinePanel.SetActive(false);
                loginPanel.SetActive(false);
                loadingPanel.SetActive(true);
            }
            else
            {
                nicknamePanel.SetActive(true);
            }
        } 
        else
        {
            NoticeText.text = "이메일, 비밀번호를 다시 입력해주세요.";
            status = 2;
            StatusPanel.SetActive(true);
        }
    }

    public void GameStart()
    {
        onlinePanel.SetActive(false);
        loginPanel.SetActive(false);
        nicknamePanel.SetActive(false);
        loadingPanel.SetActive(true);
        Backend.BMember.CreateNickname(Nickname.text);
        Param param = new Param();
        param.Add("gender", gender);
        var bro = Backend.GameData.Insert("user_gender", param);
        if(bro.IsSuccess())
        {
            Connect();
            Debug.Log("Success");
        }
        else
        {
            Debug.LogError("Fail");
        }
    }

//------------------------------------DB System-------------------------------------------

    public void ManSelection()
    {
        if(WomenBtn.interactable == false)
        {
            ManBtn.interactable = false;
            WomenBtn.interactable = true;
            gender = 1;
        }
        else
        {
            ManBtn.interactable = false;
            gender = 1;
        }
    }

    public void WomenSelection()
    {
        if(ManBtn.interactable == false)
        {
            WomenBtn.interactable = false;
            ManBtn.interactable = true;
            gender = 2;
        }
        else
        {
            WomenBtn.interactable = false;
            gender = 2;
        }
    }
//------------------------------------Chat System-----------------------------------------

    public void Send()
    {
        PV.RPC("ChatRPC", RpcTarget.All, PhotonNetwork.NickName + " : " + ChatInput.text);
        ChatInput.text = "";
    }

    [PunRPC] // RPC는 플레이어가 속해있는 방 모든 인원에게 전달한다
    void ChatRPC(string msg)
    {
        bool isInput = false;
        for (int i = 0; i < ChatText.Length; i++)
            if (ChatText[i].text == "")
            {
                isInput = true;
                ChatText[i].text = msg;
                break;
            }
        if (!isInput) // 꽉차면 한칸씩 위로 올림
        {
            for (int i = 1; i < ChatText.Length; i++) ChatText[i - 1].text = ChatText[i].text;
            ChatText[ChatText.Length - 1].text = msg;
        }
    }

//------------------------------------Panel System-----------------------------------------
    public void OnlinePanel()
    {
        basePanel.SetActive(false);
        onlinePanel.SetActive(true);
    }

    public void OfflinePanel()
    {
        basePanel.SetActive(false);
        offlinePanel.SetActive(true);
    }

    public void LoginPanel()
    {
        loginPanel.SetActive(true);
        L_email.text = "";
        L_password.text = "";
    }

    public void SignUpPanel()
    {
        signupPanel.SetActive(true);
        C_email.text = "";
        C_password.text = "";
    }

    public void B_Back()
    {
        basePanel.SetActive(true);
        onlinePanel.SetActive(false);
        offlinePanel.SetActive(false);
    }

    public void O_Back()
    {
        onlinePanel.SetActive(true);
        loginPanel.SetActive(false);
        signupPanel.SetActive(false);
    }

    public void Quit()
    {
        Backend.BMember.Logout();
        Application.Quit();
    }

    public void NickNameCheck()
    {
        BackendReturnObject bro = Backend.BMember.CheckNicknameDuplication(Nickname.text);
        if(bro.IsSuccess())
        {
            NoticeText.text = "닉네임 사용 가능합니다.";
            status = 2;
            StatusPanel.SetActive(true);
            ManBtn.interactable = true;
            WomenBtn.interactable = true;
            n_cp = 1;
        }
        else
        {
            NoticeText.text = "다른 닉네임을 입력해주세요.";
            status = 2;
            StatusPanel.SetActive(true);
        }
    }

    public void Status()
    {
        switch(status)
        {
            case 1:
                StatusPanel.SetActive(false);
                signupPanel.SetActive(false);
                break;
            
            case 2:
                StatusPanel.SetActive(false);
                break;
        }
    }

    public void Menu()
    {
        if(menu_check == 0)
        {
            MenuBar.SetActive(true);
            menu_check = 1;
        }
        else
        {
            MenuBar.SetActive(false);
            menu_check = 0;
        }
    }

    public void ChatPanel()
    {
        switch(chat_check)
        {
            case 0:
                chatPanel.SetActive(true);
                chat_check = 1;
                break;
            
            case 1:
                chatPanel.SetActive(false);
                chat_check = 0;
                break;
        }
    }

    void Update()
    {
        if(Nickname.text.Length > 0)
        {
            N_checkbtn.interactable = true;
        }
        else
        {
            N_checkbtn.interactable = false;
            startbtn.interactable = false;
            n_cp = 0;
        }

        if(n_cp == 1)
        {
            if(ManBtn.interactable != true || WomenBtn.interactable != true)
            {
                startbtn.interactable = true;
            }
            else
            {
                startbtn.interactable = false;
            }
        }

        if(ChatInput.text.Length > 0)
        {
            sendBtn.interactable = true;
        }
        else
        {
            sendBtn.interactable = false;
        }
    }
}
