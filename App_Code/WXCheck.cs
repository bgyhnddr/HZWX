using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Security;

/// <summary>
/// WXCheck 的摘要说明
/// </summary>
public class WXCheck
{
    static string Token = "weixin";
	static public bool CheckSignature(HttpContext context)
	{
        var signature = context.Request.Params["signature"];
        var timestamp = context.Request.Params["timestamp"];
        var nonce = context.Request.Params["nonce"];

        //加密/校验流程：  
        //1. 将token、timestamp、nonce三个参数进行字典序排序  
        string[] ArrTmp = { Token, timestamp, nonce };
        Array.Sort(ArrTmp);//字典排序  
        //2.将三个参数字符串拼接成一个字符串进行sha1加密  
        string tmpStr = string.Join("", ArrTmp);
        var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        var shaHash = sha1.ComputeHash(Encoding.Default.GetBytes(tmpStr));

        byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.Default.GetBytes(tmpStr));
        var comstr = BitConverter.ToString(hashedBytes);
        //3.开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。  
        if (comstr.Replace("-", "").ToLower() == signature)
        {
            return true;
        }
        else
        {
            return false;
        }  
	}

    static public bool CheckSignature(string signature, string timestamp, string nonce)
    {
        //加密/校验流程：  
        //1. 将token、timestamp、nonce三个参数进行字典序排序  
        string[] ArrTmp = { Token, timestamp, nonce };
        Array.Sort(ArrTmp);//字典排序  
        //2.将三个参数字符串拼接成一个字符串进行sha1加密  
        string tmpStr = string.Join("", ArrTmp);
        var sha1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
        var shaHash = sha1.ComputeHash(Encoding.Default.GetBytes(tmpStr));

        byte[] hashedBytes = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.Default.GetBytes(tmpStr));
        var comstr = BitConverter.ToString(hashedBytes);
        //3.开发者获得加密后的字符串可与signature对比，标识该请求来源于微信。  
        if (comstr.Replace("-", "").ToLower() == signature)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    #region 获取微信凭证
    static public string GetAccessToken(string password)
    {
        var content = string.Empty;
        if (password == ConfigurationManager.AppSettings["password"].ToString())
        {
            content = "{\"access_token\":\"" + GetAccessTokenString() + "\"}";
        }
        else
        {
            content = "{\"access_token\":\"密码错误,禁止获取token\"}";
        }
        return content;
    }

    static public string GetAccessTokenString(string url="")
    {
        var token = LOG.GetSavedAccessToken(url);
        if (string.IsNullOrWhiteSpace(token))
        {
            var appid = ConfigurationManager.AppSettings["appid"].ToString();
            var secret = ConfigurationManager.AppSettings["secret"].ToString();
            var getAccessTokenUrl = "https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(string.Format(getAccessTokenUrl, appid, secret));
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            var instream = response.GetResponseStream();
            var sr = new StreamReader(instream, Encoding.UTF8);
            //返回结果网页（html）代码
            var content = sr.ReadToEnd();
            LOG.SaveAccessToken(content);
            var o = JsonConvert.DeserializeObject<JObject>(content);
            token = o["access_token"].ToString();
        }
        return token;
    }
    #endregion
}