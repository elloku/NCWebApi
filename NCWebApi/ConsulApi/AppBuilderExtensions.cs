using Consul;
using NCWebApi.Entity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Owin.Hosting;
using NCWebApi.Controllers;

namespace NCWebApi.ConsulApi
{
    public static class AppBuilderExtensions
    {
        private static IDisposable server = null;

        private static int port = 17685;
        /// <summary>
        /// 注册consul
        /// </summary>
        /// <param name="app"></param>
        /// <param name="lifetime"></param>
        /// <param name="serviceEntity"></param>
        /// <returns></returns>
        public static IApplicationBuilder RegisterConsul(this IApplicationBuilder app, ConsulEntity serviceEntity)
        {

            // 服务DLL路径 D:\Repository\mzzj\AssemblyServer

            // 加载服务dll
            var assemblyPath = new string("D:\\Repository\\mzzj\\AssemblyServer");

            // 扫描服务dll
            DirectoryInfo TheFolder = new DirectoryInfo(assemblyPath);
            var mediDlls = TheFolder.GetFiles("Mediinfo.Service.*.dll", SearchOption.AllDirectories).ToList();
            List<string> serviceNameList = new List<string>();

            // 遍历文件
            foreach (var t in mediDlls)
            {
                // 获取dll路径
                string dllPath = t.FullName;
                var assembly = Assembly.LoadFrom(dllPath);

                // 反射DLL
                ServiceInfo serviceInfo = new ServiceInfo(assembly);

                string serviceName = serviceInfo.GetServiceName();
                serviceNameList.Add(serviceName);
                // 输出信息
                Console.WriteLine("服务名称：" + serviceName);
                Console.WriteLine("版本号：" + serviceInfo.GetServiceVersion());
                Console.WriteLine("-------------------------------------------------------------");
            }

            // 配置服务允许访问的ip地址和端口号
            StartOptions startOptions = new StartOptions {Port = port};
            startOptions.Urls.Add("http://127.0.0.1:" + port);
            startOptions.Urls.Add("http://localhost:" + port);

            // 获取本机所有IP并注册
            string name = System.Net.Dns.GetHostName();
            System.Net.IPAddress[] ipadrlist = System.Net.Dns.GetHostAddresses(name);
            foreach (System.Net.IPAddress ipa in ipadrlist)
            {
                if (ipa.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    startOptions.Urls.Add("http://" + ipa.ToString() + ":" + port);
            }

            try
            {
                // 启动服务
               //server = WebApp.Start<Startup>(startOptions);
                Console.WriteLine("服务启动成功！端口号：" + port);
            }
            catch (Exception ex)
            {
                Console.WriteLine("服务启动发生错误：" + ex.ToString());
                Console.WriteLine("-------------------------------------------------------------");
                throw;
            }

            HealthController healthController = new HealthController();
            Console.WriteLine(healthController.Index());

            Console.WriteLine("正在向consul集群注册服务...");
            var consulClient = new ConsulClient(config => { config.Address = new Uri("http://127.0.0.1:8500"); });

            foreach (var serviceName in serviceNameList)
            {
                try
                {
                    string serviceId = serviceName + "@" + Guid.NewGuid();

                    // 注册服务
                    var registrtion = new AgentServiceRegistration
                    {
                        ID = serviceId,
                        Name = serviceEntity.ServiceName, //服务名称
                        Address = serviceEntity.ip, //ip地址
                        Tags = new[] { "V1" },
                        Port = Convert.ToInt32(serviceEntity.port),
                        Check = new AgentServiceCheck
                        {
                            DeregisterCriticalServiceAfter = TimeSpan.FromSeconds(5),
                            HTTP = "http://127.0.0.1:14446/Health",
                            Interval = TimeSpan.FromSeconds(10),
                            Timeout = TimeSpan.FromSeconds(5),
                        }
                    };
                    consulClient.Agent.ServiceRegister(registrtion).Wait();
                    Console.WriteLine("注册" + serviceName + "服务成功。");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("访问服务注册中心失败！注意：开发模式可以忽略该错误提示。" + ex.ToString());
                    Console.WriteLine("-------------------------------------------------------------");
                    Console.WriteLine();
                    throw ex;
                }
            }

            Console.WriteLine("注册服务已完成。");
            Console.ReadLine();
            return app;
        }


    }
}
