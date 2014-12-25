<%@ WebHandler Language="C#" Class="WXHandler" %>

using System;
using System.Web;
using System.Text;

public class WXHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        string postString = string.Empty;
        using (System.IO.Stream stream = context.Request.InputStream)
        {
            Byte[] postBytes = new Byte[stream.Length];
            stream.Read(postBytes, 0, (Int32)stream.Length);
            postString = Encoding.UTF8.GetString(postBytes);
        }
        if (context.Request.HttpMethod.ToUpper() == "POST")
        {
            if (WXCheck.CheckSignature(context))
            {
                Handle(postString);
            }
            else
            {
                context.Response.Write("Check Signature Faild");
                context.Response.End();
            }
        }
    }

    /// <summary>
    /// 处理信息并应答
    /// </summary>
    private void Handle(string postStr)
    {
        MessageHelper helper = new MessageHelper();
        string responseContent = helper.ReturnMessage(postStr);
        if (!string.IsNullOrWhiteSpace(responseContent))
        {
            HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
            HttpContext.Current.Response.Write(responseContent);
        }
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}