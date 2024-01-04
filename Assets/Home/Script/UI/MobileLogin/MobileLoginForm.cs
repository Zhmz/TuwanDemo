using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Tuwan.Script.UI.MobileLogin
{
    public class MobileLoginForm :  UGuiForm
    {

        public void OnAuthCodeClick()
        {


            string telNum = this.GetComponentsInChildren<InputField>().Where(c => c.name == "TelNum").FirstOrDefault()?.text;


            Debug.Log("��ȡ��֤��:" + telNum);

            if (long.TryParse(telNum, out long tel))
            {

                Logic.Login.RequestCode(tel);
            }

        }


        public void OnMobileLoginClick()
        {
            Debug.Log("������֤���¼");

            string telNum = this.GetComponentsInChildren<InputField>().Where(c => c.name == "TelNum").FirstOrDefault()?.text;

            string authCode = this.GetComponentsInChildren<InputField>().Where(c => c.name == "AuthCode").FirstOrDefault()?.text;

            Debug.Log("��¼:" + authCode);

            if (long.TryParse(telNum, out long tel) && int.TryParse(authCode, out int code))
            {
                bool success = Logic.Login.SMSLogin(tel, code);

                Debug.Log("��¼״̬:" + success);
                Debug.Log("Cookie:" + Const.Store.Config.CookieValue);

                Text resultText = this.GetComponentsInChildren<Text>().Where(c => c.name == "Result").FirstOrDefault();

                resultText.text = "��¼״̬:" + success + "\r\n"+ "Cookie:" + Const.Store.Config.CookieValue;

            }
        }

        public void OnGoBackClick()
        {

            this.Close();

            GameEntry.UI.OpenUIForm(UIFormId.LoginForm);
        }
    }

}
