<%@ WebHandler Language="C#" Class="SendAll" %>

using System;
using System.Web;

public class SendAll : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        WXRequest.SendAll(context);
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}