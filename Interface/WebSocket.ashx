<%@ WebHandler Language="C#" Class="WebSocekt" %>

using System;
using System.Web;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Web.WebSockets;
using System.Text;
using System.Threading;

public class WebSocekt : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
    }

    private async Task ProcessChat(AspNetWebSocketContext context)
    {
        WebSocket socket = context.WebSocket;
        while (true)
        {
            if (socket.State == WebSocketState.Open)
            {
                ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[2048]);
                WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                string userMsg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);
                userMsg = "你发送了：" + userMsg + "于" + DateTime.Now.ToLongTimeString();
                buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(userMsg));
                await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
            }
            else
            {
                break;
            }
        }
    }

    
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}