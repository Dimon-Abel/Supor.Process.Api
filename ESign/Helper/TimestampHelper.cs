using System;

namespace ESign.Helper
{
    public static class TimestampHelper
    {
        /// <summary>    
        /// 获取当前时间戳（毫秒级）    
        /// </summary>        
        /// <returns>long</returns>    
        public static long GetTimestamp()
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (DateTime.Now.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位        
            return t;
        }


        /// <summary>    
        /// 获取时间戳（毫秒级）    
        /// </summary>        
        /// <returns>long</returns>    
        public static long GetTimestamp(string time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
            long t = (Convert.ToDateTime(time).Ticks - startTime.Ticks) / 10000;   //除10000调整为13位        
            return t;
        }

        public static void AddSecond(long lastTimestamp)
        {
            long currentTimestamp = GetTimestamp();
            //long step = currentTimestamp - 3 * 1000;
            long step = currentTimestamp - lastTimestamp;
        }

        /// <summary>
        /// 检查是否超过二维码有效期
        /// </summary>
        /// <param name="lastTimestamp"></param>
        /// <param name="expireTime"></param>
        /// <returns></returns>
        public static Boolean checkTimeout(long lastTimestamp, int expireTime)
        {
            bool flag = false;
            long currentTimestamp = GetTimestamp();
            //long step = currentTimestamp - 3 * 1000;
            // 到期时间毫秒
            long expireTimeMsec = expireTime * 1000;
            long step = currentTimestamp - lastTimestamp;
            if (step >= expireTimeMsec)
            {
                flag = true;
            }
            return flag;
        }
    }
}
