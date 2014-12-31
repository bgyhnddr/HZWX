<%@ WebHandler Language="C#" Class="GetUserInfoByCode" %>

using System;
using System.Web;

public class GetUserInfoByCode : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var code = context.Request.Params["code"];
        var info = OrderHelper.GetUserInofByCode(code);
        LOG.Log("{\"data\":" + info + "}");
        context.Response.Write("{\"data\":" + info + "}");
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}