using System.Collections;
using System.Collections.Generic;
using GameFramework.Event;
using Tuwan;
using Tuwan.Const;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class LobbyBGMForm : UGuiForm
    {
        public Button ExitButton;
        public GameObject BGMItem;
        public GameObject ScrollViewContent;

        public AudioSource Audio;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);
            initMusicList();
            OnChangeBgm(0);
        }
        private void initMusicList()
        {
            for (int i = 0; i < Store.BgmList.Count; i++)
            {
                GameObject item = Instantiate(BGMItem, ScrollViewContent.transform);
                LobbyBGMItem itemScipt = item.GetComponent<LobbyBGMItem>();
                itemScipt.Init(i, Store.BgmList[i]["musicName"]);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollViewContent.GetComponent<RectTransform>());
        }
        void OnChangeBgm(int Idx)
        {
            if (Store.BgmList[Idx] != null && Store.BgmList[Idx].ContainsKey("path"))
            {
                string musicPath = Store.BgmList[Idx]["path"];
                AudioClip musicClip = Resources.Load<AudioClip>(musicPath);
                if (musicClip != null)
                {
                    Debug.Log("AudioClip加载成功:" + musicPath);
                    Audio.clip = musicClip;
                    Audio.Play();
                }

            }
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);
            EventCenter.inst.AddEventListener<int>((int)UIEventTag.EVENT_UI_CHANGE_BGM, OnChangeBgm);
            ExitButton.onClick.AddListener(OnExitButtonClick);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            ExitButton.onClick.RemoveListener(OnExitButtonClick);
            EventCenter.inst.RemoveEventListener<int>((int)UIEventTag.EVENT_UI_CHANGE_BGM, OnChangeBgm);
        }

        void OnExitButtonClick()
        {
            Close();
        }
    }
}
