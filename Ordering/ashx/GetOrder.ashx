<%@ WebHandler Language="C#" Class="GetOrder" %>

using System;
using System.Web;

public class GetOrder : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var user = context.Request.Params["user"];
        var result = OrderHelper.GetOrder(user);
        context.Response.Write("{\"data\":" + result + "}");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}