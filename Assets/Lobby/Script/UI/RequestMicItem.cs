using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Lobby
{
    public class RequestMicItem : MonoBehaviour
    {
        public HorizontalLayoutGroup InfoLayoutGroup;


        // Start is called before the first frame update
        void Start()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(InfoLayoutGroup.GetComponent<RectTransform>());
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
