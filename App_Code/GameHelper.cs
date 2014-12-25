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
                if (AddGiftCount(openid))
                {
                    return RandomGift().ToString();
                }
                else
                {
                    return "等会再抽";
                }
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

    public static void AddGiftChance(string openid, OleDbConnection conn, OleDbTransaction tran = null)
    {

        string sqlstr = "UPDATE ContactList SET lotteryChance = lotteryChance+1 WHERE openid=@openid";
        var command = new OleDbCommand(sqlstr, conn, tran);
        command.Parameters.AddWithValue("@openid", openid);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        command.ExecuteNonQuery();

        WXRequest.CustomSend(string.Format(ReplyType.Message_Custom_Send_News,
            openid,
            "获得抽奖机会",
            "您介绍的好友使你获得一次抽奖机会,快来抽奖吧",
            WXRequest.GetOAuth2URL("http://112.124.112.166/weixintest/Game/HZGift/index.html?openid=" + openid, "redict"),
            "http://112.124.112.166/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg"));
    }

    public static bool AddGiftCount(string openid)
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
                        AddGiftChance(promote, conn, tran);
                    }
                }
                string sqlstr = "UPDATE ContactList SET lotteryCount = lotteryCount+1 WHERE openid=@openid";
                var command = new OleDbCommand(sqlstr, conn, tran);
                command.Parameters.AddWithValue("@openid", openid);
                command.ExecuteNonQuery();
            }
            tran.Commit();
            return true;
        }
        catch (Exception e)
        {
            if (tran != null)
            {
                tran.Rollback();
            }
            Console.WriteLine(e.Message + ":" + e.StackTrace);
            return false;
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

    public static string GetReply(XmlDocument xmldoc)
    {
        XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
        XmlNode EventKey = xmldoc.SelectSingleNode("/xml/EventKey");
        XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");


        return string.Format(ReplyType.Message_News_Main,
                            FromUserName.InnerText,
                            ToUserName.InnerText,
                            DateTime.Now.Ticks,
                            "1",
                             string.Format(ReplyType.Message_News_Item, "抽奖活动", "恭喜获得抽奖机会！",
                             "http://112.124.112.166/weixintest/Game/HZGift/themes/default/images/ChristmasTitle.jpg",
                             WXRequest.GetOAuth2URL("http://112.124.112.166/weixintest/Game/HZGift/index.html?openid=" + FromUserName.InnerText, "redict")));

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