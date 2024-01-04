using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace World
{
    public static class SVGAUtils
    {
        public static void SendGiftAnimOnUI(string giftUrl,Transform receiver, GameObject giftGO, GameObject parentGO)
        {

        }


        public static void SendGiftAnim(string giftUrl, PlaySvga svga, Transform sender, List<Transform> receiverList, GameObject giftGO, GameObject parentGO)
        {
            Vector3 position = sender.position;
            position.y = position.y + 2;

            //普通创建
            //GameObject svgaGiftObj = GameObject.Instantiate(svgaGift, position, currentPlayer.gameObject.transform.rotation);

            ETraceType traceType = ETraceType.Bezier;
            //对象池创建
            for (int i = 0; i < receiverList.Count; i++)
            {
                Transform curReceiver = receiverList[i];

                GameObject svgaGiftObj = PoolManager.CreateGameObject(giftGO, parentGO);
                svgaGiftObj.transform.position = position;
                svgaGiftObj.transform.rotation = sender.rotation;

                svga.PlaySVGAWithURL(giftUrl);

                BaseTrace trace = svgaGiftObj.GetComponent<BaseTrace>();
                if (trace == null)
                {
                    if (traceType == ETraceType.Line)
                    {
                        trace = svgaGiftObj.AddComponent<LineTrace>();
                    }
                    else if (traceType == ETraceType.Arc)
                    {
                        trace = svgaGiftObj.AddComponent<ArcTrace>();
                        (trace as ArcTrace).SetArcHeight(3);
                    }
                    else if (traceType == ETraceType.Bezier)
                    {
                        trace = svgaGiftObj.AddComponent<BezierTrace>();
                        (trace as BezierTrace).SetBezierParamPosSimplely(position, curReceiver.position, 3);
                    }
                }
                trace.SetDestination(curReceiver, new Vector3(0, 2, 0));
                trace.SetSpeed(10);
            }
        }
    }
}
