﻿using System;
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
        xmldoc.Load(new System.IO.MemoryStream(System.Text.Encoding.UTF8.GetBytes(postStr)));
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
                switch (EventKey.InnerText)
                {
                    case "HZGift":
                        responseContent = GameHelper.GetLotteryReply(xmldoc);
                        break;
                    case "HZGiftList":
                        responseContent = GameHelper.GetGiftListReply(xmldoc);
                        break;
                }
                break;
            case "subscribe":
                responseContent = SubscribeHandle.GetSubscribeResopnse(xmldoc);
                break;
            case "unsubscribe":
                responseContent = SubscribeHandle.GetUnSubscribeResopnse(xmldoc);
                break;

            //if (EventKey.InnerText.Equals("click_one"))//click_one
            //{
            //    responseContent = string.Format(ReplyType.Message_Text,
            //        FromUserName.InnerText,
            //        ToUserName.InnerText,
            //        DateTime.Now.Ticks,
            //        "你点击的是click_one");
            //}

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