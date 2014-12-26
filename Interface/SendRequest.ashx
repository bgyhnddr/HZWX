<%@ WebHandler Language="C#" Class="SendRequest" %>

using System;
using System.Web;

public class SendRequest : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var url = context.Request.Params["requestUrl"];
        var content = context.Request.Params["content"];
        content = HttpUtility.UrlDecode(content);
        context.Response.Write("{\"data\":" + WXRequest.SendRequest(url.Replace("ACCESS_TOKEN", WXCheck.GetAccessTokenString()), content) + "}");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}