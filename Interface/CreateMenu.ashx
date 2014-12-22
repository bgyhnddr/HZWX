<%@ WebHandler Language="C#" Class="CreateMenu" %>

using System;
using System.Web;
using System.IO;
using System.Net;
using System.Text;

public class CreateMenu : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        var responseContent = WXRequest.CreateMenu(context);
        context.Response.Write(responseContent);
        context.Response.End();
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}