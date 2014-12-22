<%@ WebHandler Language="C#" Class="Lottery" %>

using System;
using System.Web;

public class Lottery : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var openid = context.Request.Params["openid"];
        var action = context.Request.Params["action"];
        var responseContent = GameHelper.Lottery(openid, action);
        context.Response.Write("{\"data\":\"" + responseContent + "\"}");
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}