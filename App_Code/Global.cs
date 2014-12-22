using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.OleDb;

/// <summary>
/// Global 的摘要说明
/// </summary>
public class Global
{
	public Global()
	{
		//
		// TODO: 在此处添加构造函数逻辑
		//
	}


    public static OleDbConnection GetConnection()
    {
        return new OleDbConnection(ServerHandler.GetConnectionString("HZGift"));
    }
}