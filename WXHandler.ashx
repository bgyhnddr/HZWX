<%@ WebHandler Language="C#" Class="WXHandler" %>

using System;
using System.Web;
using System.Text;

public class WXHandler : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        if (WXCheck.CheckSignature(context))
        {
            string postString = string.Empty;
            if (context.Request.HttpMethod.ToUpper() == "POST")
            {
                using (System.IO.Stream stream = context.Request.InputStream)
                {
                    Byte[] postBytes = new Byte[stream.Length];
                    stream.Read(postBytes, 0, (Int32)stream.Length);
                    postString = Encoding.UTF8.GetString(postBytes);
                    Handle(postString);
                }
            }
        }
        else
        {
            context.Response.Write("not pass");
            context.Response.End();
        }
    }

    /// <summary>
    /// 处理信息并应答
    /// </summary>
    private void Handle(string postStr)
    {
        MessageHelper helper = new MessageHelper();
        string responseContent = helper.ReturnMessage(postStr);

        HttpContext.Current.Response.ContentEncoding = Encoding.UTF8;
        HttpContext.Current.Response.Write(responseContent);
    }
    
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}