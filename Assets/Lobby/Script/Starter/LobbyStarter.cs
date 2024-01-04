using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tuwan;
using FishNet.Object;
using GameFramework.Event;
using Tuwan.Const;

namespace Lobby
{
    public class LobbyStarter : NetworkBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            if (IsClient)
            {
                EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY);
                EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_ENTER_LOBBY_FORM, "LobbyOther");
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

    }
}
