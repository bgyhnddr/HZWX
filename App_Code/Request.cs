using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

/// <summary>
/// Request 的摘要说明
/// </summary>
public class Request
{
	public Request()
	{
	}
    public static string CreateMenu(HttpContext context)
    {
        var token = context.Request.Params["token"];
        var menu = context.Request.Params["menu"];

        var data = Encoding.UTF8.GetBytes(menu);
        string posturl = string.Format("https://api.weixin.qq.com/cgi-bin/menu/create?access_token={0}",token);
        var request = (HttpWebRequest)WebRequest.Create(posturl);
        CookieContainer cookieContainer = new CookieContainer();
        request.CookieContainer = cookieContainer;
        request.AllowAutoRedirect = true;
        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = data.Length;
        var outstream = request.GetRequestStream();
        outstream.Write(data, 0, data.Length);
        outstream.Close();
        //发送请求并获取相应回应数据
        var response = (HttpWebResponse)request.GetResponse();
        //直到request.GetResponse()程序才开始向目标网页发送Post请求
        var instream = response.GetResponseStream();
        var sr = new StreamReader(instream, Encoding.UTF8);
        //返回结果网页（html）代码
        string content = sr.ReadToEnd();
        string err = string.Empty;
        return content;
    }
}