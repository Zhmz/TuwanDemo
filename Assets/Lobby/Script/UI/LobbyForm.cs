using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuwan;
using UnityEngine.UI;
using System.Linq;
using World;
using GameFramework.Event;
using Tuwan.Const;
using Tuwan.Proto;

namespace Lobby
{
    public enum ELobbyUserType
    {
        Audience,   //观众
        Guest,      //嘉宾
        Host,       //主持人
    }

    public class LobbyForm : UGuiForm
    {
        [Header("Common")]
        public Text LobbyNameText;
        public Text LobbyIdText;
        public Button LobbyBGMButton;
        public Button SettingsButton;
        public Button ExitButton;
        public Button StartDanceButton;
        public Button StopDanceButton;
        public Button ChatButton;
        public GameObject ChatNode;
        public Button GiftButton;
        public Button TestGiftButton;

        [Header("SVGA")]
        public GameObject SVGAGiftGO;

        [Header("Host")]
        public GameObject HostNode;
        public Button HostTurnOnMicButton;
        public Button HostTurnOffMicButton;
        public Button HostManageMicButton;

        [Header("Guest")]
        public GameObject GuestNode;
        public Button GuestTurnOnMicButton;
        public Button GuestTurnOffMicButton;
        public Button GuestExitMicButton;

        [Header("Audience")]
        public GameObject AudienceNode;
        public Button AudienceRequestMicButton;
        public Text RequestForbidCountDownText;

        //迪厅用户数据，后期转到model层
        public ELobbyUserType userType = ELobbyUserType.Audience;

        private bool isChatUIOpen = false;

        private List<UserInfoResponsedData> ApplyList = new List<UserInfoResponsedData>();

        //移动组件
        private PlayerMovement movement;
        public PlayerMovement Movement
        {
            get
            {
                if (movement == null)
                {
                    movement = FindObjectsOfType<PlayerMovement>().Where(c => c.IsOwner).FirstOrDefault();
                }
                return movement;
            }
        }

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            OnOpenLobbyForm(userData as string);
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            RegisterButtonListener();

            EventCenter.inst.AddEventListener<string>((int)UIEventTag.EVENT_UI_ENTER_LOBBY_FORM, OnOpenLobbyForm);
            RegisterButtonListener();
            RegisterSocketListener();
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            UnregisterButtonListener();
            EventCenter.inst.AddEventListener<string>((int)UIEventTag.EVENT_UI_ENTER_LOBBY_FORM, OnOpenLobbyForm);
            UnregisterButtonListener();
            UnRegisterSocketListener();
        }

        void RegisterSocketListener()
        {
            EventCenter.inst.AddSocketEventListener<int, string>((int)SocketIoType.APPLY_WHEAT, OnReceiveApplyWheat);
        }
        void UnRegisterSocketListener()
        {
            EventCenter.inst.RemoveSocketEventListener<int, string>((int)SocketIoType.APPLY_WHEAT, OnReceiveApplyWheat);
        }

        private void OnReceiveApplyWheat(int uid, string data)
        {
            if (LobbyData.inst.JudgeSelfHoster())
            {
                //主持人
                ApplyList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInfoResponsedData>>(data);
            }
            else
            {
                if (uid == Store.Config.UserInfo.uid)
                {
                    Debug.Log("申请成功");
                }
            }
        }
        void RegisterButtonListener()
        {
            LobbyBGMButton.onClick.AddListener(OnLobbyBGMButtonClick);
            SettingsButton.onClick.AddListener(OnSettingsButtonClick);
            ExitButton.onClick.AddListener(OnExitButtonClick);
            StartDanceButton.onClick.AddListener(OnStartDanceButtonClick);
            StopDanceButton.onClick.AddListener(OnStopDanceButtonClick);
            ChatButton.onClick.AddListener(OnChatButtonClick);
            GiftButton.onClick.AddListener(OnGiftButtonClick);
            TestGiftButton.onClick.AddListener(OnTestGiftButtonClick);

            HostTurnOnMicButton.onClick.AddListener(OnHostTurnOnMicButtonClick);
            HostTurnOffMicButton.onClick.AddListener(OnHostTurnOffMicButtonClick);
            HostManageMicButton.onClick.AddListener(OnHostManageMicButtonClick);

            GuestTurnOnMicButton.onClick.AddListener(OnGuestTurnOnMicButtonClick);
            GuestTurnOffMicButton.onClick.AddListener(OnGuestTurnOffMicButtonClick);
            GuestExitMicButton.onClick.AddListener(OnGuestExitMicButtonClick);

            AudienceRequestMicButton.onClick.AddListener(OnAudienceRequestMicButtonClick);
        }

        void UnregisterButtonListener()
        {
            LobbyBGMButton.onClick.RemoveListener(OnLobbyBGMButtonClick);
            SettingsButton.onClick.RemoveListener(OnSettingsButtonClick);
            ExitButton.onClick.RemoveListener(OnExitButtonClick);
            StartDanceButton.onClick.RemoveListener(OnStartDanceButtonClick);
            StopDanceButton.onClick.RemoveListener(OnStopDanceButtonClick);
            ChatButton.onClick.RemoveListener(OnChatButtonClick);
            GiftButton.onClick.RemoveListener(OnGiftButtonClick);
            TestGiftButton.onClick.RemoveListener(OnTestGiftButtonClick);

            HostTurnOnMicButton.onClick.RemoveListener(OnHostTurnOnMicButtonClick);
            HostTurnOffMicButton.onClick.RemoveListener(OnHostTurnOffMicButtonClick);
            HostManageMicButton.onClick.RemoveListener(OnHostManageMicButtonClick);

            GuestTurnOnMicButton.onClick.RemoveListener(OnGuestTurnOnMicButtonClick);
            GuestTurnOffMicButton.onClick.RemoveListener(OnGuestTurnOffMicButtonClick);
            GuestExitMicButton.onClick.RemoveListener(OnGuestExitMicButtonClick);

            AudienceRequestMicButton.onClick.RemoveListener(OnAudienceRequestMicButtonClick);
        }

        public void InitLobbyForm()
        {
            AudienceNode.SetActive(userType == ELobbyUserType.Audience);
            GuestNode.SetActive(userType == ELobbyUserType.Guest);
            HostNode.SetActive(userType == ELobbyUserType.Host);

            ChatNode.SetActive(isChatUIOpen);



            UpdateDanceButtonState();
            UpdateBgmButtonState();
        }
        void UpdateBgmButtonState()
        {
            LobbyBGMButton.gameObject.SetActive(LobbyData.inst.JudgeSelfHoster());
        }
        private void InitRoomInfo()
        {
            LobbyNameText.text = "谁是卧底|007";
            LobbyNameText.text = "ID:" + Store.Config.Cid;
        }

        void UpdateDanceButtonState()
        {
            if (Movement == null)
            {
                StartDanceButton.gameObject.SetActive(true);
                StopDanceButton.gameObject.SetActive(false);
                return;
            }
            StartDanceButton.gameObject.SetActive(!Movement.isDancing);
            StopDanceButton.gameObject.SetActive(Movement.isDancing);
        }

        #region ButtonCallback
        public void OnLobbyBGMButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.LobbyBGMForm);
        }

        public void OnSettingsButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.LobbySettingsForm);
        }


        public void OnExitButtonClick()
        {

        }

        public void OnStartDanceButtonClick()
        {
            Movement.isDancing = true;
            UpdateDanceButtonState();
        }

        public void OnStopDanceButtonClick()
        {
            Movement.isDancing = false;
            UpdateDanceButtonState();
        }

        public void OnChatButtonClick()
        {
            isChatUIOpen = !isChatUIOpen;
            ChatNode.SetActive(isChatUIOpen);
        }

        public void OnGiftButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.GiftForm);
        }

        public void OnTestGiftButtonClick()
        {
            Player currentPlayer = GameObject.FindObjectsOfType<Player>().Where(c => c.IsOwner).FirstOrDefault();

            GameObject[] hosts = GameObject.FindGameObjectsWithTag("Host");

            GameObject host = hosts[Random.Range(0, hosts.Length)];

            //Vector3 position = currentPlayer.gameObject.transform.position;

            //position.y = position.y + 2;

            //普通创建
            //GameObject svgaGiftObj = GameObject.Instantiate(svgaGift, position, currentPlayer.gameObject.transform.rotation);

            List<Transform> receiverList = new List<Transform>();
            receiverList.Add(host.transform);
            Transform sender = currentPlayer.gameObject.transform;
            string giftUrl = "https://img3.tuwandata.com/uploads/play/1916771563790770.svga";
            PlaySvga playSvga = GameObject.FindFirstObjectByType<PlaySvga>();
            GameObject poolParent = GameObject.Find("PoolParent");
            SVGAUtils.SendGiftAnim(giftUrl, playSvga, sender, receiverList, SVGAGiftGO, poolParent);
        }

        public void OnHostTurnOnMicButtonClick()
        {

        }

        public void OnHostTurnOffMicButtonClick()
        {

        }

        public void OnHostManageMicButtonClick()
        {
            GameEntry.UI.OpenUIForm(UIFormId.RequestMicForm);
        }

        public void OnGuestTurnOnMicButtonClick()
        {

        }

        public void OnGuestTurnOffMicButtonClick()
        {

        }

        public void OnGuestExitMicButtonClick()
        {

        }

        public void OnAudienceRequestMicButtonClick()
        {
            //在禁止时间内
            if (isInRequestForbidding)
            {
                GameEntry.UI.OpenDialog(new DialogParams()
                {
                    Mode = 1,
                    Title = "提示",
                    Message = "请勿频繁申请上麦。"
                });
            }
            else
            {
                isInRequestForbidding = true;
                SetCountDown();
            }
        }

        #endregion


        #region Request Forbid
        private bool isInRequestForbidding = false;
        private int timer = 0;
        private int countDownTime = 30;
        Coroutine countDownCorou = null;

        void SetCountDown()
        {
            timer = countDownTime;
            countDownCorou = StartCoroutine(RequestForbidCountDownCoroutine());
        }

        void UpdateRequestForbidCountDownText()
        {
            if (isInRequestForbidding)
            {
                RequestForbidCountDownText.text = string.Format("{0}s", timer);
            }
            else
            {
                RequestForbidCountDownText.text = string.Empty;
            }
        }

        IEnumerator RequestForbidCountDownCoroutine()
        {
            while (timer >= 0)
            {
                UpdateRequestForbidCountDownText();
                yield return new WaitForSeconds(1);
                timer -= 1;
            }

            timer = 0;
            isInRequestForbidding = false;

            UpdateRequestForbidCountDownText();

            if (countDownCorou != null)
            {
                StopCoroutine(countDownCorou);
                countDownCorou = null;
            }
        }
        #endregion


        protected void OnOpenLobbyForm(string curSceneName)
        {
            if (curSceneName == "WorldOther")
            {
                HostNode.gameObject.SetActive(false);
                GuestNode.gameObject.SetActive(false);
                AudienceNode.gameObject.SetActive(false);
            }
            else if (curSceneName == "LobbyOther")
            {
                InitLobbyForm();
                InitRoomInfo();
            }
        }


        private void OnDestroy()
        {
            if (countDownCorou != null)
            {
                StopCoroutine(countDownCorou);
            }
        }
    }
}