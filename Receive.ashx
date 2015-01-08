<%@ WebHandler Language="C#" Class="ReceiveTicket" %>

using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;
using System.Xml;

public class ReceiveTicket : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        var signature = context.Request.Params["msg_signature"];
        var timestamp = context.Request.Params["timestamp"];
        var nonce = context.Request.Params["nonce"];
        string postString = string.Empty, xmlString = string.Empty;
        using (System.IO.Stream stream = context.Request.InputStream)
        {
            Byte[] postBytes = new Byte[stream.Length];
            stream.Read(postBytes, 0, (Int32)stream.Length);
            postString = System.Text.Encoding.UTF8.GetString(postBytes);
        }
        if (Check(signature, timestamp, nonce, postString, ref xmlString))
        {
            context.Response.Write(Handle(xmlString));
        }
        else
        {
            LOG.Log("checked faild");
            context.Response.Write("Faild");
        }

        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

    public static string Handle(string postString)
    {
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(postString)));
        XmlNode MsgType = xmldoc.SelectSingleNode("/xml/SuiteTicket");
        LOG.SaveSuiteTicket(MsgType.InnerText);
        return "success";
    }

    public static bool Check(string signature, string timestamp, string nonce, string postString,ref string xmlString)
    {



        string sToken = "weixin";
        string sCorpID = "tjb1f5ea1c41b16c93";
        string sEncodingAESKey = "D792D9PqTyjXSFSVCKNTAMhaJH12UszbpGv329fxldX";
        Tencent.WXBizMsgCrypt wxcpt = new Tencent.WXBizMsgCrypt(sToken, sEncodingAESKey, sCorpID);
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(postString)));
        XmlNode MsgType = xmldoc.SelectSingleNode("/xml/Encrypt");
        var result = wxcpt.VerifyURL(signature, timestamp, nonce, MsgType.InnerText, ref xmlString);
        return result == 0;
    }
}