using System;

namespace Project.Scripts.LuHeUtility
{
    public static class LuHeUtility
    {
        public static string GetCurrentTime()
        {
            DateTime dateTime = DateTime.Now;
            string strNowTime =
                // $"{dateTime.Year:D}:{dateTime.Month:D}:{dateTime.Day:D}:{dateTime.Hour:D}:{dateTime.Minute:D}:{dateTime.Second:D}";
                $"{dateTime.Hour:D}:{dateTime.Minute:D}:{dateTime.Second:D}";
            
            return strNowTime;
        }
    }
    
    
}