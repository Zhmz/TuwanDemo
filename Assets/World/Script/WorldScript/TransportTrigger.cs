using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FishNet.Managing.Scened;
using FishNet.Object;
using GameFramework.Event;
using Tuwan;
using Tuwan.Const;
using UnityEngine;
using World;

public class TransportTrigger : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        //Server
        Player currentPlayer = GetPlayerOwnedObject(other);
        if (currentPlayer == null)
        {
            return;
        }

        /* Create a lookup handle using this objects scene.
           * This is one of many ways FishNet knows what scene to load
           * for the clients. */
        SceneLookupData lookupData = new SceneLookupData("LobbyOther");
        SceneLoadData sld = new SceneLoadData(lookupData)
        {
            /* Set automatically unload to false
             * so the server does not unload this scene when
             * there are no more connections in it. */
            Options = new LoadOptions()
            {
                AutomaticallyUnload = false
            },
            /* Also move the client object to the new scene. 
            * This step is not required but may be desirable. */
            MovedNetworkObjects = new NetworkObject[] { currentPlayer.NetworkObject },
            //Load scenes as additive.
            ReplaceScenes = ReplaceOption.None,
            //Set the preferred active scene so the client changes active scenes.
            PreferredActiveScene = lookupData,
        };

        base.SceneManager.LoadConnectionScenes(currentPlayer.Owner, sld);


        if (IsClient)
        {
            EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_FROM_WORLD_ENTER_LOBBY);
            EventCenter.inst.EventTrigger((int)UIEventTag.EVENT_UI_ENTER_LOBBY_FORM, "LobbyOther");
        }
    }

    private Player GetPlayerOwnedObject(Collider other)
    {
        /* When an object exits this trigger unload the level for the client. */
        Player player = other.GetComponent<Player>();
        //Not the player object.
        if (player == null)
            return null;
        //No owner??
        if (!player.Owner.IsActive)
            return null;

        return player;
    }

    private void OnTriggerExit(Collider other)
    {

    }

    private void OnTriggerStay(Collider other)
    {

    }
}
