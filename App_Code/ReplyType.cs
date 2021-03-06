﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// ReplyType 的摘要说明
/// </summary>
public class ReplyType
{
	public ReplyType()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
    }
    /// <summary>
    /// 普通文本消息
    /// </summary>
    public static string Message_Text
    {
        get
        {
            return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[text]]></MsgType>
                            <Content><![CDATA[{3}]]></Content>
                            </xml>";
        }
    }
    /// <summary>
    /// 图文消息主体
    /// </summary>
    public static string Message_News_Main
    {
        get
        {
            return @"<xml>
                            <ToUserName><![CDATA[{0}]]></ToUserName>
                            <FromUserName><![CDATA[{1}]]></FromUserName>
                            <CreateTime>{2}</CreateTime>
                            <MsgType><![CDATA[news]]></MsgType>
                            <ArticleCount>{3}</ArticleCount>
                            <Articles>
                            {4}
                            </Articles>
                            </xml> ";
        }
    }
    /// <summary>
    /// 图文消息项
    /// </summary>
    public static string Message_News_Item
    {
        get
        {
            return @"<item>
                            <Title><![CDATA[{0}]]></Title> 
                            <Description><![CDATA[{1}]]></Description>
                            <PicUrl><![CDATA[{2}]]></PicUrl>
                            <Url><![CDATA[{3}]]></Url>
                            </item>";
        }
    }

    /// <summary>
    /// 文本消息项
    /// </summary>
    public static string Message_Custom_Send_Text
    {
        get
        {
            return "{{\"touser\":\"{0}\",\"msgtype\":\"text\",\"text\":{{\"content\":\"{1}\"}}}}";
        }
        
    }

    public static string Message_Custom_Send_News
    {
        get
        {
            return "{{\"touser\":\"{0}\",\"msgtype\":\"news\",\"news\":{{\"articles\":[{{\"title\":\"{1}\",\"description\":\"{2}\",\"url\":\"{3}\",\"picurl\":\"{4}\"}}]}}}}";
        }
    }

}