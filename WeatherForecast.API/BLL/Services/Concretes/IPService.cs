using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WeatherForecast.API.Models;
using WeatherForecast.API.DAL.Repositories.Interfaces;
using WeatherForecast.API.BLL.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace WeatherForecast.API.BLL.Services.Concretes
{
    public class IPService : IIPService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IPService(
            IHttpContextAccessor httpContextAccessor
        )
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //TODO: handle complex scenarios (X-Forwarded-For header, Cloudflare, loadbalancer etc.)
        public IPAddress GetCurrentHttpClientIPAddress()
        {
            var ip = _httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress;

            if (ip == null)
            {
                throw new Exception($"{nameof(IPService)}.{nameof(GetCurrentHttpClientIPAddress)}: Can't get client IP");
            }

            return ip;
        }
    }
}
