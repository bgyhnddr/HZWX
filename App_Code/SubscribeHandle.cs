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
/// SubscribeHandle 的摘要说明
/// </summary>
public class SubscribeHandle
{
    public SubscribeHandle()
    {
    }


    public static bool Subscribe(string openid)
    {
        var conn = Global.GetConnection();
        try
        {
            conn.Open();
            if (GameHelper.IsInDatabase(openid, conn))
            {
                UpdateSubscribe(conn, openid, true);
            }
            else
            {
                GameHelper.InsertUserInfo(conn, openid);
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + ":" + e.StackTrace);
        }
        finally
        {
            conn.Close();
        }
        return false;
    }

    public static void UnSubscribe(string openid)
    {
        var conn = Global.GetConnection();

        try
        {
            conn.Open();
            if (GameHelper.IsInDatabase(openid, conn))
            {
                UpdateSubscribe(conn, openid, false);
            }
            else
            {
                GameHelper.InsertUserInfo(conn, openid, false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + ":" + e.StackTrace);
        }
        finally
        {
            conn.Close();
        }
    }


    public static bool UpdateSubscribe(OleDbConnection conn, string openid, bool subscribe, OleDbTransaction tran = null)
    {
        string sqlstr = "UPDATE ContactList SET subscribe = @subscribe WHERE openid=@openid";
        var command = new OleDbCommand(sqlstr, conn);
        command.Parameters.AddWithValue("@subscribe", subscribe);
        command.Parameters.AddWithValue("@openid", openid);
        if (tran != null)
        {
            command.Transaction = tran;
        }
        var count = command.ExecuteNonQuery();
        return count > 0;
    }

    public static string GetSubscribeResopnse(XmlDocument xmldoc)
    {
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        Subscribe(FromUserName.InnerText);
        return GameHelper.GetReply(xmldoc);
    }

    public static string GetUnSubscribeResopnse(XmlDocument xmldoc)
    {
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        UnSubscribe(FromUserName.InnerText);

        return string.Empty;
    }


    public static bool IsSubscribe(string openid)
    {
        var conn = Global.GetConnection();
        try
        {
            string sqlstr = "select openid from ContactList where openid=@openid and subscribe=true";
            conn.Open();
            var command = new OleDbCommand(string.Format(sqlstr, openid), conn);
            command.Parameters.AddWithValue("@openid", openid);
            var reader = command.ExecuteReader(CommandBehavior.SingleResult);
            return reader.HasRows;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message + ":" + e.StackTrace);
            return false;
        }
        finally
        {
            conn.Close();
        }
    }

}