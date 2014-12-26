using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Web;
using System.Xml;

/// <summary>
/// GameHelper 的摘要说明
/// </summary>
public class GameHelper
{
	public GameHelper()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public static string Lottery(string openid,string action)
    {
        if (HasChance(openid))
        {

            if (action == "get")
            {
                return AddGiftCount(openid);
            }
            else
            {
                return string.Empty;
            }
        }
        else
        {
            return "已经没有机会了";
        }
    }

    public static bool HasChance(string openid)
    {
        try
        {
            var table = GameHelper.GetUserInfoTable(openid);
            if (table.Rows.Count > 0)
            {
                var chance = int.Parse(table.Rows[0]["lotteryChance"].ToString());
                var count = int.Parse(table.Rows[0]["lotteryCount"].ToString());
                return chance > count;
            }
            else
            {
                return false;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine(e.Message + ":" + e.StackTrace);
            return false;
        }
    }

    public static void AddGiftChance(string openid,string oriopenid, OleDbConnection conn, OleDbTransaction tran = null)
    {

        string sqlstr = "UPDATE ContactList SET lotteryChance = lotteryChance+1 WHERE openid=@openid";
        var command = new OleDbCommand(sqlstr, conn, tran);
        command.Parameters.AddWithValue("@openid", openid);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        command.ExecuteNonQuery();

        var userInfo = WXRequest.GetUserInfo(oriopenid);

        var userJson = JsonConvert.DeserializeObject<JObject>(userInfo);

        var name = "您介绍的好友使你获得一次抽奖机会,快来抽奖吧";

        if (userJson["nickname"] != null)
        {
            name = "您的好友" + userJson["nickname"].ToString() + "初次抽奖令你获得额外一次抽奖机会！";
        }

        WXRequest.CustomSend(string.Format(ReplyType.Message_Custom_Send_News,
            openid,
            "获得抽奖机会",
            name,
            WXRequest.GetOAuth2URL("http://112.124.112.166/weixintest/Game/HZGift/index.html?openid=" + openid, "redict"),
            "http://112.124.112.166/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg"));
    }

    public static string AddGiftCount(string openid)
    {

        var table = GameHelper.GetUserInfoTable(openid);
        var conn = Global.GetConnection();
        OleDbTransaction tran = null;
        try
        {
            conn.Open();
            tran = conn.BeginTransaction();

            if (table.Rows.Count > 0)
            {
                var promote = table.Rows[0]["promoteId"].ToString();
                if (!string.IsNullOrWhiteSpace(promote))
                {
                    if (int.Parse(table.Rows[0]["lotteryCount"].ToString()) == 0)
                    {
                        AddGiftChance(promote, openid, conn, tran);
                    }
                }
                string sqlstr = "UPDATE ContactList SET lotteryCount = lotteryCount+1 WHERE openid=@openid";
                var command = new OleDbCommand(sqlstr, conn, tran);
                command.Parameters.AddWithValue("@openid", openid);
                command.ExecuteNonQuery();
            }

            var gift = RandomGift().ToString();

            InsertPresent(conn, openid, gift.ToString(), tran);

            tran.Commit();
            return gift;
        }
        catch (Exception e)
        {
            if (tran != null)
            {
                tran.Rollback();
            }
            Console.WriteLine(e.Message + ":" + e.StackTrace);
            return "等会再抽";
        }
        finally
        {
            conn.Close();
        }
    }

    public static int RandomGift()
    {
        var random = new Random(DateTime.Now.Millisecond);
        var num = random.Next(0, 100);

        int returnNum;

        if (num <= 5)
        {
            returnNum = 1;
        }
        else if(num>5&&num<=20)
        {
            returnNum = 2;
        }
        else if(num>20&&num<=50)
        {
            returnNum = 3;
        }
        else
        {
            returnNum = 4;
        }


        return returnNum;
    }



    public static bool IsInDatabase(string openid, OleDbConnection conn, OleDbTransaction tran = null)
    {
        string sqlstr = "select openid from ContactList where openid=@openid";
        var command = new OleDbCommand(sqlstr, conn);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        command.Parameters.AddWithValue("@openid", openid);
        var reader = command.ExecuteReader(CommandBehavior.SingleResult);
        return reader.HasRows;
    }

    public static bool IsInDatabase(int id, OleDbConnection conn, OleDbTransaction tran = null)
    {
        return IsInDatabase(GetOpenidById(id.ToString(), conn, tran), conn, tran);
    }

    public static string GetLotteryReply(XmlDocument xmldoc)
    {
        XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");


        return string.Format(ReplyType.Message_News_Main,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "3",
                             string.Format(ReplyType.Message_News_Item, "抽奖活动", "恭喜获得抽奖机会！",
                             "http://112.124.112.166/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg",
                             WXRequest.GetOAuth2URL("http://112.124.112.166/weixintest/Game/HZGift/index.html?openid=" + FromUserName.InnerText, "redict")) +
                             string.Format(ReplyType.Message_News_Item, "内部点餐", "员工内部点餐入口。",
                             "http://112.124.112.166/weixintest/WeixinWeb/themes/default/images/hashiqi.jpg",
                             "http://112.124.112.166/Ordering/") +
                             string.Format(ReplyType.Message_News_Item, "微信JS", "微信内部JS测试",
                             "http://112.124.112.166/weixintest/Game/HZGift/themes/default/images/present.png",
                             "http://112.124.112.166/weixintest/WeixinWeb/WeixinJSBridge.html"));

    }

    public static string GetGiftListReply(XmlDocument xmldoc) {
        XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");


        return string.Format(ReplyType.Message_Text,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            GetGiftList(FromUserName.InnerText));
    }

    public static string GetGiftList(string openid)
    {
        var conn = Global.GetConnection();
        try
        {
            var sql = "SELECT [gift], COUNT(gift) AS [count] FROM PresentList WHERE [openid] = @openid AND [get] = false GROUP BY [gift] ORDER BY [gift]";
            var command = new OleDbCommand(sql, conn);
            command.Parameters.AddWithValue("@openid", openid);
            var adapter = new OleDbDataAdapter(command);
            DataTable table = new DataTable();
            adapter.Fill(table);

            var returnString = "我的中奖记录\r\n";
            returnString += "未领奖记录:\r\n";
            foreach (DataRow row in table.Rows)
            {
                returnString += row["gift"].ToString() + "等奖:" + row["count"].ToString() + "\r\n";
            }

            returnString += "已经领奖记录:\r\n";

            sql = "SELECT [gift], COUNT(gift) AS [count] FROM PresentList WHERE [openid] = @openid AND [get] = true GROUP BY [gift] ORDER BY [gift]";
            command = new OleDbCommand(sql, conn);
            command.Parameters.AddWithValue("@openid", openid);
            adapter = new OleDbDataAdapter(command);
            table = new DataTable();
            adapter.Fill(table);

            foreach (DataRow row in table.Rows)
            {
                returnString += row["gift"].ToString() + "等奖:" + row["count"].ToString() + "\r\n";
            }

            return returnString;
        }
        catch (Exception ex)
        {
            return "有错误发生:" + ex.Message + ex.StackTrace;
        }
    }

    public static bool InsertUserInfo(OleDbConnection conn, string openid, bool subscribe = true, OleDbTransaction tran = null, string promoteId = "", int lotteryChance = 1, int lotteryCount = 0)
    {
        string sqlstr = @"INSERT INTO ContactList(openid,subscribe,lotteryChance,lotteryCount,promoteId) ";
        sqlstr += " values(@openid,@subscribe,@lotteryChance,@lotteryCount,@promoteId)";
        var command = new OleDbCommand(sqlstr, conn);
        command.Parameters.AddWithValue("@openid", openid);
        command.Parameters.AddWithValue("@subscribe", subscribe);
        command.Parameters.AddWithValue("@lotteryChance", lotteryChance);
        command.Parameters.AddWithValue("@lotteryCount", lotteryCount);
        command.Parameters.AddWithValue("@promoteId", promoteId);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        var count = command.ExecuteNonQuery();
        return count > 0;
    }

    public static bool InsertPresent(OleDbConnection conn, string openid, string gift, OleDbTransaction tran = null)
    {
        string sqlstr = @"INSERT INTO PresentList(openid,gift) ";
        sqlstr += " values(@openid,@gift)";
        var command = new OleDbCommand(sqlstr, conn);
        command.Parameters.AddWithValue("@openid", openid);
        command.Parameters.AddWithValue("@gift", gift);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        var count = command.ExecuteNonQuery();
        return count > 0;
    }

    public static string GetOpenidById(string id, OleDbConnection conn, OleDbTransaction tran = null)
    {
        if (!string.IsNullOrWhiteSpace(id))
        {
            var intId = 0;
            int.TryParse(id, out intId);
            string sqlstr = "select openid from ContactList where id=@id";
            var command = new OleDbCommand(sqlstr, conn);
            command.Parameters.AddWithValue("@id", intId);
            if (tran != null)
            {
                command.Transaction = tran;
            }
            var adatper = new OleDbDataAdapter(command);
            DataTable table = new DataTable();
            adatper.Fill(table);
            if (table.Rows.Count > 0)
            {
                return table.Rows[0]["openid"].ToString();
            }
            else
            {
                return string.Empty;
            }
        }
        return string.Empty;
    }

    public static string GetUserInfo(string openid, string promoteId)
    {
        UpdateUserInfo(openid, promoteId);
        return JsonConvert.SerializeObject(GetUserInfoTable(openid));
    }

    public static void UpdateUserInfo(string openid, string promoteId = "")
    {
        var userInfo = GetUserInfoByRequest(openid);

        var subscribe = false;
        if (!string.IsNullOrWhiteSpace(userInfo))
        {
            var userJson = JsonConvert.DeserializeObject<JObject>(userInfo);
            var subi = 0;
            int.TryParse(userJson["subscribe"].ToString(),out subi);
            subscribe = subi == 1;
        }
        var userTable = GetUserInfoTable(openid);

        var conn = Global.GetConnection();
        OleDbTransaction tran = null;
        try
        {
            conn.Open();
            tran = conn.BeginTransaction();

            var proId = GetOpenidById(promoteId, conn, tran);
            if (userTable.Rows.Count == 0)
            {
                InsertUserInfo(conn, openid, subscribe, tran, proId);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(userInfo))
                {
                    SubscribeHandle.UpdateSubscribe(conn, openid, subscribe, tran);
                }

                if(string.IsNullOrWhiteSpace(userTable.Rows[0]["promoteId"].ToString()))
                {
                    UpdatePromoteId(conn, openid, proId, tran);
                }
            }
            tran.Commit();
        }
        catch (Exception e)
        {
            if (tran != null)
            {
                tran.Rollback();
            }
            Console.WriteLine(e.Message + ":" + e.StackTrace);
        }
        finally
        {
            conn.Close();
        }
        return;
    }

    public static DataTable GetUserInfoTable(string openid)
    {
        var conn = Global.GetConnection();
        var table = new DataTable();
        try
        {
            string sqlstr = "select * from ContactList where openid=@openid";
            var command = new OleDbCommand(sqlstr, conn);
            command.Parameters.AddWithValue("@openid", openid);
            var adatper = new OleDbDataAdapter(command);
            adatper.Fill(table);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + ":" + e.StackTrace);
        }
        finally
        {
            conn.Close();
        }
        return table;
    }

    public static string GetUserInfoByRequest(string openid)
    {
        var resString = WXRequest.GetUserInfo(openid);
        var resJson = JsonConvert.DeserializeObject<JObject>(resString);
        if (resJson["errcode"] != null)
        {
            return string.Empty;
        }
        else
        {
            return resString;
        }
    }

    public static bool UpdatePromoteId(OleDbConnection conn, string openid, string promoteId, OleDbTransaction tran = null)
    {
        string sqlstr = "UPDATE ContactList SET promoteId = @promoteId WHERE openid=@openid";
        var command = new OleDbCommand(sqlstr, conn);
        command.Parameters.AddWithValue("@promoteId", promoteId);
        command.Parameters.AddWithValue("@openid", openid);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        var count = command.ExecuteNonQuery();
        return count > 0;
    }
}