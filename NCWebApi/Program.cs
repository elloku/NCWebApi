using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace NCWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
            Console.WriteLine(args);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var config = new ConfigurationBuilder().AddCommandLine(args)
                .Build();//��ȡ������Ϣ
            return WebHost.CreateDefaultBuilder(args)
                .UseUrls($"http://{ config["ip"]}:{config["port"]}")//����ip��ַ�Ͷ˿ڵ�ַ
                .UseStartup<Startup>();
        }
    }
}
