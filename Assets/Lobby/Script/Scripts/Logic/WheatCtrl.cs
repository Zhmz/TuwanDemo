
using Tuwan.Const;
using Tuwan.Lobby.Entity;
using Tuwan.Proto;
using Tuwan.Script.Logic;
using UnityEngine;


namespace Tuwan.Lobby.Logic
{
    public class WheatCtrl
    {
        //申请上麦
        public static void ApplyWheat()
        {
            string cookie = Store.Config.CookieValue.Replace("Tuwan_Passport=", "");
            NetManager.inst.Emit(SocketResquestName.Setorder, new
            {
                state = 0,
                passport = cookie,
                typeid = Store.Config.RoomInfo.Typeid,
                uid = Store.Config.UserInfo.uid,
                cid = Store.Config.Cid,
                channel = Store.Config.RoomInfo.Channel,
            });
        }
        //拒绝上麦
        public static void UnAgreeWheat(int toUid)
        {
            string cookie = Store.Config.CookieValue.Replace("Tuwan_Passport=", "");
            NetManager.inst.Emit(SocketResquestName.Setorder, new
            {
                state = -1,
                passport = cookie,
                typeid = Store.Config.RoomInfo.Typeid,
                uid = toUid,
                cid = Store.Config.Cid,
                channel = Store.Config.RoomInfo.Channel,
            });
        }
        //同意上麦
        public static void AgreeWheat(int toUid)
        {
            NetManager.inst.Emit(SocketResquestName.Setwheat, new
            {
                state = 0,
                typeid = Store.Config.RoomInfo.Typeid,
                uid = toUid,
                cid = Store.Config.Cid,
                position = 3,
            });
        }
        //下麦
        public static void DownWheat()
        {
            NetManager.inst.Emit(SocketResquestName.Setwheat, new
            {
                state = -1,
                uid = Store.Config.UserInfo.uid,
                cid = Store.Config.Cid,
                position = 0,
            });
        }


    }
}
