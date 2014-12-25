<%@ WebHandler Language="C#" Class="Promote" %>

using System;
using System.Web;

public class Promote : IHttpHandler {
    public void ProcessRequest(HttpContext context)
    {
        var promoteId = context.Request.Params["promoteId"];
        context.Response.Redirect(WXRequest.GetOAuth2URL("http://112.124.112.166/weixintest/Game/HZGift/index.html?promoteId=" + promoteId));
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}