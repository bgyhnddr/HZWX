<%@ WebHandler Language="C#" Class="UpdateOrder" %>

using System;
using System.Web;

public class UpdateOrder : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var order = context.Request.Params["order"];
        OrderHelper.UpdateOrder(order);
        context.Response.Write("{\"data\":true}");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}