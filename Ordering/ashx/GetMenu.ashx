<%@ WebHandler Language="C#" Class="GetMenu" %>

using System;
using System.Web;

public class GetMenu : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.Write("Hello World");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}