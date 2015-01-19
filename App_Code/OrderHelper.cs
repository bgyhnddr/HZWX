using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using System.Data;

/// <summary>
/// OrderHelper 的摘要说明
/// </summary>
public class OrderHelper
{
	public OrderHelper()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static string GetUserInofByCode(string code)
    {
        return GetUserInfo(GetUserId(code));
    }

    public static string GetAccessToken()
    {
        var url = "\\Ordering";
        var token = LOG.GetSavedAccessToken(url);
        if (string.IsNullOrWhiteSpace(token))
        {
            var appIS = LOG.GetAppIS();

            var resObj = JsonConvert.DeserializeObject<JObject>(WXRequest.SendRequest(
                string.Format("https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}", appIS.appid, appIS.secret)));

            LOG.SaveAccessToken(JsonConvert.SerializeObject(resObj), url);

            return resObj["access_token"].ToString();

        }
        return token;
    }

    public static string GetUserId(string code)
    {
        var resObj = JsonConvert.DeserializeObject<JObject>(WXRequest.SendRequest(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}&agentid=2",
            GetAccessToken(), code)));

        return resObj["UserId"] != null ? resObj["UserId"].ToString() : string.Empty;
    }

    public static string GetUserInfo(string userId)
    {
        var returnStr = WXRequest.SendRequest(string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}",
            GetAccessToken(), userId));
        return returnStr;
    }

    public static string GetOrder(string user)
    {
        var conn = GetConnection();
        var sql = "SELECT * FROM [orderfood].[dbo].[OrderList] WHERE DATEDIFF(day,[Date],getdate())=0 AND Name=@Name";
        var comm = new SqlCommand(sql, conn);
        comm.Parameters.AddWithValue("@Name", user);
        var adapter = new SqlDataAdapter(comm);
        var table = new DataTable();
        adapter.Fill(table);
        return JsonConvert.SerializeObject(table);
    }

    public static string UpdateOrder(string order)
    {
        var conn = GetConnection();
        SqlTransaction tran = null;
        try
        {
            conn.Open();
            tran = conn.BeginTransaction();
            var jObj = JsonConvert.DeserializeObject<JObject>(order);
            if (jObj["id"] == null)
            {
                var sql = "INSERT INTO [orderfood].[dbo].[OrderList]([Name],[Type],[Store],[OrderFood],[Money],[Comment],[Date]) VALUES (@Name,@Type,@Store,@OrderFood,@Money,@Comment,@Date)";
                var comm = new SqlCommand(sql, conn, tran);
                comm.Parameters.AddWithValue("@Name", jObj["Name"].ToString());
                comm.Parameters.AddWithValue("@Type", "午餐");
                comm.Parameters.AddWithValue("@Store", jObj["Store"]);
                comm.Parameters.AddWithValue("@OrderFood", jObj["OrderFood"].ToString());
                comm.Parameters.AddWithValue("@Money", int.Parse(jObj["Money"].ToString()));
                comm.Parameters.AddWithValue("@Comment", int.Parse(jObj["Comment"].ToString()));
                comm.Parameters.AddWithValue("@Date", DateTime.Now);
                comm.ExecuteNonQuery();
            }
            else
            {
                var sql = @"UPDATE [orderfood].[dbo].[OrderList]
                            SET [Name] = @Name
                               ,[Store] = @Store
                               ,[OrderFood] = @OrderFood
                               ,[Money] = @Money
                               ,[Comment] = @Comment
                               WHERE id=@id";
                var comm = new SqlCommand(sql, conn, tran);
                comm.Parameters.AddWithValue("@Name", jObj["Name"].ToString());
                comm.Parameters.AddWithValue("@Store", jObj["Store"]);
                comm.Parameters.AddWithValue("@OrderFood", jObj["OrderFood"].ToString());
                comm.Parameters.AddWithValue("@Money", int.Parse(jObj["Money"].ToString()));
                comm.Parameters.AddWithValue("@Comment", int.Parse(jObj["Comment"].ToString()));
                comm.Parameters.AddWithValue("@id", int.Parse(jObj["id"].ToString()));
                comm.ExecuteNonQuery();
            }
            tran.Commit();
            return string.Empty;
        }
        catch
        {
            if (tran != null)
            {
                tran.Rollback();
            }
            return string.Empty;
        }
        finally
        {
            conn.Close();
        }
    }

    public static string GetMenu()
    {

    }


    public static SqlConnection GetConnection()
    {
        return new SqlConnection(ConfigurationManager.ConnectionStrings["OrderFoodConnStr"].ConnectionString);
    }
}