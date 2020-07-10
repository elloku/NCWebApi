using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NCWebApi
{
    public class ServiceInfo
    {
        private readonly Assembly assembly;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="assembly"></param>
        public ServiceInfo(Assembly assembly)
        {
            this.assembly = assembly;
        }

        /// <summary>
        /// 获取服务名
        /// </summary>
        /// <returns></returns>
        public string GetServiceName()
        {
            var assemblyTitle = assembly.GetName().Name;
            string serviceName = assemblyTitle.Substring("Mediinfo.Service.".Length).Replace('.', '-');
            return serviceName;
        }

        /// <summary>
        /// 获取服务版本
        /// </summary>
        /// <returns></returns>
        public string GetServiceVersion()
        {
            return "V1";
        }
    }
}
