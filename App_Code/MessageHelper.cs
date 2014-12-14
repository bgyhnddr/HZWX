using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml;

/// <summary>
/// MessageHelper 的摘要说明
/// </summary>
public class MessageHelper
{
	public MessageHelper()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}

    public string ReturnMessage(string postStr)
    {
        string responseContent = "";
        XmlDocument xmldoc = new XmlDocument();
        xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.GetEncoding("GB2312").GetBytes(postStr)));
        XmlNode MsgType = xmldoc.SelectSingleNode("/xml/MsgType");
        if (MsgType != null)
        {
            switch (MsgType.InnerText)
            {
                case "event":
                    responseContent = EventHandle(xmldoc);//事件处理
                    break;
                case "text":
                    responseContent = TextHandle(xmldoc);//接受文本消息处理
                    break;
                default:
                    break;
            }
        }
        return responseContent;
    }

    //事件
    public string EventHandle(XmlDocument xmldoc)
    {
        string responseContent = "";
        XmlNode Event = xmldoc.SelectSingleNode("/xml/Event");
        XmlNode EventKey = xmldoc.SelectSingleNode("/xml/EventKey");
        XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        switch(Event.InnerText)
        {
            case "CLICK":
                if (EventKey.InnerText.Equals("click_one"))//click_one
                {
                    responseContent = string.Format(ReplyType.Message_Text,
                        FromUserName.InnerText,
                        ToUserName.InnerText,
                        DateTime.Now.Ticks,
                        "你点击的是click_one");
                }
                else if (EventKey.InnerText.Equals("click_two"))//click_two
                {
                    responseContent = string.Format(ReplyType.Message_News_Main,
                        FromUserName.InnerText,
                        ToUserName.InnerText,
                        DateTime.Now.Ticks,
                        "2",
                         string.Format(ReplyType.Message_News_Item, "我要寄件", "",
                         "http://www.soso.com/orderPlace.jpg",
                         "http://www.soso.com/") +
                         string.Format(ReplyType.Message_News_Item, "订单管理", "",
                         "http://www.soso.com/orderManage.jpg",
                         "http://www.soso.com/"));
                }
                else if (EventKey.InnerText.Equals("click_three"))//click_three
                {
                    responseContent = string.Format(ReplyType.Message_News_Main,
                        FromUserName.InnerText,
                        ToUserName.InnerText,
                        DateTime.Now.Ticks,
                        "1",
                         string.Format(ReplyType.Message_News_Item, "标题", "摘要",
                         "http://www.soso.com/jieshao.jpg",
                         "http://www.soso.com/"));
                }
                break;
            case "subscribe":
                responseContent = string.Format(ReplyType.Message_News_Main,
                        FromUserName.InnerText,
                        ToUserName.InnerText,
                        DateTime.Now.Ticks,
                        "1",
                         string.Format(ReplyType.Message_News_Item, "内部点餐", "提供公司内部的点餐服务",
                         "http://112.124.112.166/weixintest/WeixinWeb/themes/default/images/hashiqi.jpg",
                         "http://112.124.112.166/Ordering/"));
                break;
        }
        return responseContent;
    }
    //接受文本消息
    public string TextHandle(XmlDocument xmldoc)
    {
        string responseContent = "";
        XmlNode ToUserName = xmldoc.SelectSingleNode("/xml/ToUserName");
        XmlNode FromUserName = xmldoc.SelectSingleNode("/xml/FromUserName");
        XmlNode Content = xmldoc.SelectSingleNode("/xml/Content");
        if (Content != null)
        {
            responseContent = string.Format(ReplyType.Message_Text,
                FromUserName.InnerText,
                ToUserName.InnerText,
                DateTime.Now.Ticks,
                "欢迎使用微信公共账号，您输入的内容为：" + Content.InnerText + "\r\n<a href=\"http://www.baidu.com\">点击进入</a>");
        }
        return responseContent;
    }
}