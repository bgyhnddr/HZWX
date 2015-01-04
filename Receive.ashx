<%@ WebHandler Language="C#" Class="ReceiveTicket" %>

using System;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Security.Cryptography;

public class ReceiveTicket : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        if (WXCheck.CheckSignature(context))
        {
            string postString = string.Empty;
            using (System.IO.Stream stream = context.Request.InputStream)
            {
                Byte[] postBytes = new Byte[stream.Length];
                stream.Read(postBytes, 0, (Int32)stream.Length);
                postString = System.Text.Encoding.UTF8.GetString(postBytes);
            }
            context.Response.Write(Handle(postString));
        }
        else
        {
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
        LOG.Log(postString);
        var obj = JsonConvert.DeserializeObject<JObject>(postString);
        switch (obj["InfoType"].ToString())
        {
            case "suite_ticket":
                RijndaelManaged rijndaelCipher = new RijndaelManaged();
                break;
        }
        return "success";
    }
}