namespace Destroy.Test
{
    using System;
    using System.IO;
    using System.Net;
    using System.Text;
    using System.Threading;

    public class HttpProcessor
    {
        private HttpServer server;

        private HttpListenerContext context;

        public HttpListenerRequest Request => context.Request;

        public HttpListenerResponse Response => context.Response;

        public HttpProcessor(HttpServer server, HttpListenerContext context)
        {
            this.server = server;
            this.context = context;
        }

        public void Process()
        {
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            if (context.Request.HttpMethod == "GET")
            {
                server.HandleGetRequest(this);
            }
            if (context.Request.HttpMethod == "POST")
            {
                server.HandlePostRequest(this);
            }
        }
    }

    public class HttpServer
    {
        private HttpListener listener;

        private string ip;

        private int port;

        private bool active;

        public HttpServer(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            this.active = true;
        }

        public void Listen()
        {
            listener = new HttpListener();
            string url = $"http://{ip}:{port}/";
            listener.Prefixes.Add(url);
            //开启对指定URL与端口的监听, 开始处理客户端请求
            listener.Start();

            while (active)
            {
                //同步等待客户端的消息
                HttpListenerContext context = listener.GetContext();
                //输出客户端请求的URL信息（从端口号之后内容 /sava...）
                Console.WriteLine(context.Request.RawUrl);
                //创建一个处理程序
                HttpProcessor processor = new HttpProcessor(this, context);
                //开启一个线程去处理消息, 实现并发
                Thread thread = new Thread(processor.Process);
                thread.Start();
            }
        }

        public void HandleGetRequest(HttpProcessor processor)
        {
            var query = processor.Request.QueryString;
            string response = null;
            string action = query.Get("action");
            switch (action)
            {
                case "":
                    {

                    }
                    break;
                case "1":
                    {

                    }
                    break;
                default:
                    {
                        response = "无效的命令";
                    }
                    break;
            }

            using (StreamWriter writer = new StreamWriter(processor.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write(response);
            }
        }

        public void HandlePostRequest(HttpProcessor processor)
        {
            //将发送到客户端的字符流,字符编码为UTF8，http请求一定要有返回值，返回的是字符串
            using (StreamWriter writer = new StreamWriter(processor.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write("Success Saved!");//返回存档成功
            }
        }

        public void Close()
        {
            active = false;
            listener.Close();
        }
    }
}