using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ServerHandler 的摘要说明
/// </summary>
public class ServerHandler
{
	public ServerHandler()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}


    static public string GetServerPath()
    {
        return HttpContext.Current.Server.MapPath("~");
    }

    static public string GetConnectionString(string type)
    {
        var conn = string.Empty;
        switch (type)
        {
            case "HZGift":
                conn = GetHZGiftConnString();
                break;
            default:
                var constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Persist Security Info=False";
                conn = string.Format(constr,
            HttpContext.Current.Server.MapPath("~") + "\\Game\\HZGift\\Data\\Database.mdb");
                break;
        }
        return conn;
    }

    static private string GetHZGiftConnString()
    {
        var constr = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Persist Security Info=False";
        return string.Format(constr,
            HttpContext.Current.Server.MapPath("~") + "\\Game\\HZGift\\Data\\Database.mdb");
    }
}