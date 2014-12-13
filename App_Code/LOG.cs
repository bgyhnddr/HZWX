using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

/// <summary>
/// LOG 的摘要说明
/// </summary>
public class LOG
{
    public static void Log(string log)
    {
        File.AppendAllText(HttpContext.Current.Server.MapPath("~") + "\\log.txt", log + "\r\n", Encoding.UTF8);
    }
}