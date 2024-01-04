
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Managing;
using FishNet.Transporting;
using FishNet.Transporting.Multipass;
using FishNet.Transporting.Tugboat;
using GameFramework.Event;
using TMPro;
using Tuwan;
using Tuwan.Const;
using Tuwan.Proto;

//using Unity.Netcode;
//using Unity.Netcode.Transports.UTP;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;
using World;

//轨迹类型
public enum ETraceType
{
    Line,//直线
    Arc,//弧形
    Bezier,//贝塞尔曲线
}

public class NetworkInit : MonoBehaviour
{
    private TuwanNetworkInit networkInit;

    public GameObject svgaGift;
    GameObject poolParent;

    public ETraceType traceType = ETraceType.Line;
    private float speed = 10;
    private float curveHeight = 3;

    public Slider speedSlider;
    public Text speedValueText;
    public Slider curveHeightSlider;
    public Text curveHeightVaueText;

    public Dropdown traceDropDown;


    //麦位
    public GameObject HostSlot;
    public ParticleSystem HostParticle;
    public GameObject GuestSlotParent;
    public GameObject[] GuestSlotList;
    public ParticleSystem[] GuestParticleList;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        speedSlider.onValueChanged.AddListener(OnSpeedSliderValueChange);
        curveHeightSlider.onValueChanged.AddListener(OnParabolicHeightSliderValueChange);
        traceDropDown.onValueChanged.AddListener(OnTraceTypeChange);
        EventCenter.inst.AddSocketEventListener<int, string>((int)SocketIoType.CHAT_TEXT, OnReceiveGift);
        EventCenter.inst.AddEventListener<int>((int)UIEventTag.EVENT_UI_TUWAN_PLAYER_INFO_SYNCED, OnTuwanUIdSynced);
        EventCenter.inst.AddEventListener((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY, OnFromWorldEnterLobby);
    }
    private void OnReceiveGift(int type, string data)
    {
        if (type == (int)SocketDataType.CHAT_SHOW_GIFT)
        {
            ShowGiftResponse giftInfo = Newtonsoft.Json.JsonConvert.DeserializeObject<ShowGiftResponse>(data);
            string svgaPath = TuwanUtils.ReplacePath(giftInfo.pathsvga);
            List<int> toUids = new List<int>();
            for (var i = 0; i < giftInfo.touserinfo.Count; i++)
            {
                toUids.Add(int.Parse(giftInfo.touserinfo[i].userid));
            }
            SendGift(svgaPath, giftInfo.from, toUids);
        }
    }
    private void OnDisable()
    {
        speedSlider.onValueChanged.RemoveListener(OnSpeedSliderValueChange);
        curveHeightSlider.onValueChanged.RemoveListener(OnParabolicHeightSliderValueChange);
        traceDropDown.onValueChanged.AddListener(OnTraceTypeChange);
        EventCenter.inst.RemoveSocketEventListener<int, string>((int)SocketIoType.CHAT_TEXT, OnReceiveGift);
        EventCenter.inst.RemoveEventListener<int>((int)UIEventTag.EVENT_UI_TUWAN_PLAYER_INFO_SYNCED, OnTuwanUIdSynced);
        EventCenter.inst.RemoveEventListener((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY, OnFromWorldEnterLobby);
    }

    void OnSpeedSliderValueChange(float value)
    {
        speed = value;
        speedValueText.text = speed.ToString("F2");
    }

    void OnParabolicHeightSliderValueChange(float value)
    {
        curveHeight = value;
        curveHeightVaueText.text = curveHeight.ToString("F2");
    }

    void OnTraceTypeChange(int value)
    {
        traceType = (ETraceType)value;
    }


    // Start is called before the first frame update
    void Start()
    {
        speedSlider.minValue = 1;
        speedSlider.maxValue = 20;
        speedSlider.value = speed;
        speedValueText.text = speed.ToString("F2");

        curveHeightSlider.minValue = 0;
        curveHeightSlider.maxValue = 5;
        curveHeightSlider.value = curveHeight;
        curveHeightVaueText.text = curveHeight.ToString("F2");

        traceDropDown.value = (int)traceType;

        poolParent = new GameObject("PoolParent");

        networkInit = FindObjectOfType<TuwanNetworkInit>();


        //if (networkInit.StartType == AutoStartType.Host)
        //{
        //    networkInit.SwitchServer(true);
        //    networkInit.SwitchClient(true);
        //}
        //else if (networkInit.StartType == AutoStartType.Server)
        //{
        //    networkInit.SwitchServer(true);
        //}
        //else if (networkInit.StartType == AutoStartType.Client)
        //{
        //    networkInit.SwitchClient(true);
        //}
    }

    public void OnClientClick()
    {
        networkInit.SwitchClient(true);
    }

    public void OnHostClick()
    {
        networkInit.SwitchServer(true);
        networkInit.SwitchClient(true);
    }

    public void OnServerClick()
    {
        networkInit.SwitchServer(true);
    }

    public void OnDanceClick()
    {
        PlayerMovement playerMovement = GameObject.FindObjectsOfType<PlayerMovement>().Where(c => c.IsOwner).FirstOrDefault();
        playerMovement?.ChangeDance();
    }

    public void OnGiftClick()
    {
        PlaySvga playSvga = GameObject.FindFirstObjectByType<PlaySvga>();
        //playSvga?.Play();

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
        SVGAUtils.SendGiftAnim(giftUrl, playSvga, sender, receiverList, svgaGift, poolParent);

        ////对象池创建
        //GameObject svgaGiftObj = PoolManager.CreateGameObject(svgaGift, poolParent);
        //svgaGiftObj.transform.position = position;
        //svgaGiftObj.transform.rotation = currentPlayer.gameObject.transform.rotation;

        //BaseTrace trace = svgaGiftObj.GetComponent<BaseTrace>();
        //if (trace == null)
        //{
        //    if (traceType == ETraceType.Line)
        //    {
        //        trace = svgaGiftObj.AddComponent<LineTrace>();
        //    }
        //    else if (traceType == ETraceType.Arc)
        //    {
        //        trace = svgaGiftObj.AddComponent<ArcTrace>();
        //        (trace as ArcTrace).SetArcHeight(curveHeight);
        //    }
        //    else if (traceType == ETraceType.Bezier)
        //    {
        //        trace = svgaGiftObj.AddComponent<BezierTrace>();
        //        (trace as BezierTrace).SetBezierParamPosSimplely(position, host.transform.position, curveHeight);
        //    }
        //}
        //trace.SetDestination(host.transform, new Vector3(0, 2, 0));
        //trace.SetSpeed(speed);
    }


    private int ownerTuwanUId;
    private void UpdateMicSlots()
    {
        WheatResponse thisWheat = null;
        var wheatMap = LobbyData.inst.WheatMap;
        foreach (WheatResponse wheatRes in wheatMap.Values)
        {
            if (wheatRes.uid == ownerTuwanUId)
            {
                thisWheat = wheatRes;
            }
        }

        //普通观众
        if (thisWheat == null)
        {
            Bounds spawnArea = GameObject.Find("LobbySpawnArea").GetComponent<MeshRenderer>().bounds;
            Vector3 randomPos = new Vector3(Random.Range(spawnArea.min.x, spawnArea.max.x), spawnArea.center.y, Random.Range(spawnArea.min.z, spawnArea.max.z));
            EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_MOVE_AUDIENCE_TO_LOBBY_RANDOM_POSITION, randomPos);
        }
        //麦位主持或嘉宾
        else
        {
            int pos = thisWheat.position;
            if (pos == (int)EWheatPosition.EHost)
            {
                //关闭粒子
                //HostParticle.gameObject.SetActive(false);
                SetWheatParticleColor(Color.yellow, EWheatPosition.EHost);
                //发送消息给player，修改位置
                EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_MOVE_PLAYER_TO_WHEAT_POSITION, HostSlot.transform);
            }
            else if (pos >= (int)EWheatPosition.EGuest1 && pos <= (int)EWheatPosition.EGuest8)
            {
                //设置粒子颜色
                SetWheatParticleColor(Color.yellow, (EWheatPosition)pos);
                //3~10映射到0～7
                int guestIndex = pos - 3;
                if (guestIndex >= 0 && guestIndex < GuestSlotList.Length && guestIndex < GuestParticleList.Length)
                {
                    //发送消息给player，修改位置
                    EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_MOVE_PLAYER_TO_WHEAT_POSITION, GuestSlotList[guestIndex].transform);
                }
            }
        }
    }

    private void OnTuwanUIdSynced(int tuwanUId)
    {
        ownerTuwanUId = tuwanUId;
        UpdateMicSlots();
    }

    private void OnFromWorldEnterLobby()
    {
        UpdateMicSlots();
    }

    private void SetWheatParticleColor(Color color, EWheatPosition position)
    {
        int index = -1;
        if (position == EWheatPosition.EHost)
        {
            index = 2;
        }
        else
        {
            index = (int)position - 3;
        }
        if (index >= 0 && index < GuestParticleList.Length)
        {
            var par = GuestParticleList[index];
            ParticleSystem.MainModule mainModule = par.main;
            mainModule.startColor = color;
        }
    }

    //发送礼物
    private void SendGift(string giftUrl, int senderId, List<int> receiverIdList)
    {
        if (string.IsNullOrEmpty(giftUrl))
        {
            return;
        }

        if (senderId <= 0 || receiverIdList.Count <= 0)
        {
            return;
        }

        Player[] players = FindObjectsOfType<Player>();

        Player sender = players.Where((c) =>
        {
            if (c.CurPlayerInfo == null)
            {
                return false;
            }
            return c.CurPlayerInfo.uid == senderId;
        }).FirstOrDefault();

        List<Player> receiverList = players.Where((c) =>
        {
            if (c.CurPlayerInfo == null)
            {
                return false;
            }
            return receiverIdList.Contains(c.CurPlayerInfo.uid);
        }).ToList();

        if (sender == null || receiverList.Count <= 0)
        {
            return;
        }

        Transform senderTrans = sender.transform;
        List<Transform> receiverTransList = new List<Transform>();
        for (int i = 0; i < receiverList.Count; i++)
        {
            receiverTransList.Add(receiverList[i].transform);
        }

        PlaySvga playSvga = GameObject.FindFirstObjectByType<PlaySvga>();
        SVGAUtils.SendGiftAnim(giftUrl, playSvga, senderTrans, receiverTransList, svgaGift, poolParent);
    }
}
