<%@ WebHandler Language="C#" Class="GetopenidBycode" %>

using System;
using System.Web;

public class GetopenidBycode : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var code = context.Request.Params["code"];
        var responseContent = WXRequest.GetopenidBycode(code);
        context.Response.Write("{\"data\":\"" + responseContent + "\"}");
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}