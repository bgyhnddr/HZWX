<%@ WebHandler Language="C#" Class="GetAccessToken" %>

using System;
using System.Web;

public class GetAccessToken : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {

        var password = context.Request.Params["password"];
        var responseContent = WXCheck.GetAccessToken(password);
        context.Response.Write(responseContent);
        context.Response.End();
    }


    
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}