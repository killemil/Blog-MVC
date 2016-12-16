using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Blog_MVC.Classes
{
    public class Utils
    {
        public static string CutText (string text , int maxLength = 66)
        {
            if (text.Length <= maxLength || text == null)
            {
                return text;
            }
            else
            {
                var shortText = text.Substring(0, maxLength) + "...";
                return shortText;
            }
        }
    }
}