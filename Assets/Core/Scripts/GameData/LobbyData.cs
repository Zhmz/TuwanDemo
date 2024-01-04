using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tuwan.Const;
using Tuwan.Proto;
using Unity.VisualScripting;
using UnityEngine.PlayerLoop;

namespace Tuwan
{
    public enum EWheatPosition
    {
        EHost = 2,
        EGuest1 = 3,
        EGuest2 = 4,
        EGuest3 = 5,
        EGuest4 = 6,
        EGuest5 = 7,
        EGuest6 = 8,
        EGuest7 = 9,
        EGuest8 = 10,
    }

    public class LobbyData
    {
        private volatile static LobbyData m_instance;
        //线程锁。当多线程访问时，同一时刻仅允许一个线程访问
        private static object m_locker = new object();
        //私有化构造
        private LobbyData() { }
        //单例初始化
        public static LobbyData inst
        {
            get
            {
                //线程锁。防止同时判断为null时同时创建对象
                lock (m_locker)
                {
                    if (m_instance == null)
                    {
                        m_instance = new LobbyData();
                    }
                }
                return m_instance;
            }
        }
        public Dictionary<int, WheatResponse> WheatMap = new Dictionary<int, WheatResponse>();//麦位

        public void initWheatInfo(string json)
        {
            WheatMap.Clear();
            List<WheatResponse> WheatList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<WheatResponse>>(json);
            for (int i = 0; i < WheatList.Count; i++)
            {
                WheatMap.Add(WheatList[i].position, WheatList[i]);
            }
        }
        //自己是否是主持人
        public bool JudgeSelfHoster()
        {
            if (WheatMap.ContainsKey(2))
            {
                return WheatMap[2].uid == Store.Config.UserInfo.uid;
            }
            else
            {
                return false;
            }
        }
        //找到自己麦位
        public int GetSelfPosition()
        {
            int wheat = -1;
            foreach (var key in WheatMap.Keys)
            {
                if (WheatMap[key].uid == Store.Config.UserInfo.uid)
                {
                    wheat = key;
                    break;
                }
            }
            return wheat;
        }
        //判断自己是否在麦上
        public bool JudgeSelfIsOnWheat()
        {
            return GetSelfPosition() > 2;
        }
    }
}
