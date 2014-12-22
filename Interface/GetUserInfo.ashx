<%@ WebHandler Language="C#" Class="GetUserInfo" %>

using System;
using System.Web;

public class GetUserInfo : IHttpHandler {

    public void ProcessRequest(HttpContext context)
    {
        var openid = context.Request.Params["openid"];
        var promoteId = context.Request.Params["promotedId"];
        var responseContent = GameHelper.GetUserInfo(openid, promoteId);
        context.Response.Write("{\"data\":" + responseContent + "}");
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}