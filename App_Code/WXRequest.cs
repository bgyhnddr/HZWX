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
public class WXRequest
{
    public WXRequest()
	{
	}
    public static string CreateMenu(HttpContext context)
    {
        var requestUrl = "https://api.weixin.qq.com/cgi-bin/menu/create?access_token=" + 
            LOG.GetSavedAccessToken();
        return SendRequest(requestUrl, context.Request.Params["menu"]);
    }


    public static string CustomSend(string content)
    {
        var requestUrl = "https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token=" +
            LOG.GetSavedAccessToken();
        return SendRequest(requestUrl, content);
    }

    public static string GetUserInfo(string openid)
    {
        var requestUrl = "https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN";
        return SendRequest(string.Format(requestUrl, openid, LOG.GetSavedAccessToken()));
        //{"errcode":48001,"errmsg":"api unauthorized"}
        //{"errcode":40013,"errmsg":"invalid appid"}
    }

    public static string SendRequest(string url, string postString = "")
    {
        var data = Encoding.UTF8.GetBytes(postString);
        string posturl = url;
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