﻿using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Security.Claims;
using System.Text;

namespace SampleCallApi
{
    internal class Program
    {
        const string username = "behranmotor";//نام کاربری
        const string password = "bm*1402@10@28";//کلمه عبور
        const string key      = "HBcEgIvhneAj5UCJMmk6qC726OQCeCiQ";//کد امنیتی
        static void Main(string[] args)
        {
            var token = CreateToken();

            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("token", token);

            string svcCredentials = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(username + ":" + password));

            client.DefaultRequestHeaders.Add("Authorization", "Basic " + svcCredentials);

            var webRequest = new HttpRequestMessage(HttpMethod.Get, "http://wsqceng.saipacorp.com/qceng/MonitoringList");
            //var webRequest = new HttpRequestMessage(HttpMethod.Get, "http://localhost:5000/Nonconformity");

            var response = client.Send(webRequest);

            var dataStream = response.Content.ReadFromJsonAsync(typeof(object)).Result;

            Console.WriteLine(dataStream);
        }

        static string CreateToken(/*User user*/)
        {
            List<Claim> claims = new List<Claim>
                    {
                        new Claim("user", username),
                        new Claim("method", "MonitoringList")
                    };

            //var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Convert.ToBase64String(Encoding.UTF8.GetBytes(password + password))));
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                                            claims: claims,
                                            expires: DateTime.UtcNow.AddMinutes(12),
                                            signingCredentials: credentials);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }
    }
}
