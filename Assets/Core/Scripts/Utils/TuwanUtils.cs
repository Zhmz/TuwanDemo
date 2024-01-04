using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tuwan.Const;

namespace Tuwan
{
    public static class TuwanUtils
    {
        // 生成UUID
        public static string GetUUID()
        {
            return S4() + S4();
        }

        private static string S4()
        {
            return ((int)((1 + new System.Random().NextDouble()) * 0x10000)).ToString("X4").Substring(1);
        }
        public static string ReplacePath(string iconPath)
        {
            string resPath = "";
            foreach (var entry in Store.UserAvatarUrls)
            {
                if (iconPath.Contains(entry.Key))
                {
                    resPath = iconPath.Replace(entry.Key, entry.Value);
                    return resPath;
                }
            }
            return resPath;
        }


    }
}
