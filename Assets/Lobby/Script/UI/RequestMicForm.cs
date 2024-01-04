using System.Collections;
using System.Collections.Generic;
using Tuwan;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class RequestMicForm : UGuiForm
    {
        public Button ExitButton;
        public GameObject RequestItem;
        public GameObject ScrollViewContent;

        protected override void OnInit(object userData)
        {
            base.OnInit(userData);

            for (int i = 0; i < 10; i++)
            {
                GameObject item = Instantiate(RequestItem, ScrollViewContent.transform);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollViewContent.GetComponent<RectTransform>());
        }

        protected override void OnOpen(object userData)
        {
            base.OnOpen(userData);

            ExitButton.onClick.AddListener(OnExitButtonClick);
        }

        protected override void OnClose(bool isShutdown, object userData)
        {
            base.OnClose(isShutdown, userData);

            ExitButton.onClick.RemoveListener(OnExitButtonClick);
        }

        void OnExitButtonClick()
        {
            Close();
        }
    }
}
