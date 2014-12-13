using System;
using System.Collections.Generic;
using System.Linq;
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
}