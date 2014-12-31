using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;
using System.Configuration;

/// <summary>
/// Global 的摘要说明
/// </summary>
public class Global
{
    public const string OAuth2URL = "https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type=code&scope={2}&state={3}#wechat_redirect";
    public static string OrderFoodConnStr = ConfigurationManager.ConnectionStrings["OrderFoodConnStr"].ConnectionString; 

	public Global()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}
    public static OleDbConnection GetConnection()
    {
        return new OleDbConnection(ServerHandler.GetConnectionString("HZGift"));
    }
}