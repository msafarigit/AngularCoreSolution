using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using Microsoft.Extensions.Hosting;

namespace Ags.ResourceProxy
{
	public class ProxyConfigService : IProxyConfigService
	{
		private ProxyConfig config;
		//Projects that target the Microsoft.NET.Sdk.Web SDK implicitly reference the Microsoft.AspNetCore.App framework.
		//
		private IWebHostEnvironment hostingEnvironment { get; }
		public string ConfigPath { get; }

		public virtual ProxyConfig Config
		{
			get
			{
				if (config == null)
				{
					string proxyText = File.ReadAllText(Path.Join(hostingEnvironment.ContentRootPath, ConfigPath));
					JObject proxyObject = JObject.Parse(proxyText);
					config = proxyObject.ToObject<ProxyConfig>();
				}
				return config;
			}
		}

		public ProxyConfigService(IWebHostEnvironment hostingEnvironment, string configPath)
		{
			this.hostingEnvironment = hostingEnvironment ?? throw new ArgumentNullException(nameof(hostingEnvironment));
			ConfigPath = configPath;
		}

		public NetworkCredential GetCredentials(ServerUrl serverUrlConfig)
		{
			NetworkCredential credentials = null;
			if (serverUrlConfig.UseAppPoolIdentity)
				credentials = CredentialCache.DefaultNetworkCredentials;
			else if (serverUrlConfig.Domain != null)
				credentials = new NetworkCredential(serverUrlConfig.Username, serverUrlConfig.Password, serverUrlConfig.Domain);
			return credentials;
		}

		public ServerUrl GetProxyServerUrlConfig(string queryStringUrl)
		{
			return Config.ServerUrls.FirstOrDefault(su => queryStringUrl.Contains(su.Url));
		}

		public List<KeyValuePair<string, string>> GetOAuth2FormData(ServerUrl su, string proxyReferrer)
		{
			return new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("client_id", su.ClientId),
				new KeyValuePair<string, string>("client_secret", su.ClientSecret),
				new KeyValuePair<string, string>("grant_type", "client_credentials"),
				new KeyValuePair<string, string>("redirect_uri", proxyReferrer),
				new KeyValuePair<string, string>("f", "json"),
			};
		}

		public List<KeyValuePair<string, string>> GetPortalExchangeTokenFormData(ServerUrl su, string proxyReferrer, string portalCode)
		{
			return new List<KeyValuePair<string, string>> {
				new KeyValuePair<string, string>("client_id", su.ClientId),
				new KeyValuePair<string, string>("redirect_uri", proxyReferrer),
				new KeyValuePair<string, string>("grant_type", "authorization_code"),
				new KeyValuePair<string, string>("code", portalCode),
				new KeyValuePair<string, string>("f", "json")
			};
		}

		/// <summary>
		/// Determines if the referring URL is allowed to use the proxy.
		/// </summary>
		/// <param name="referer"></param>
		/// <returns></returns>
		public bool IsAllowedReferrer(string referer)
		{
			if (Config.AllowedReferrers == null || Config.AllowedReferrers.Length == 0)
			{
				return false; // Assume someone forgot to set this node in the config, take the safe path
			}
			if (Config.AllowedReferrers[0] == "*")
			{
				return true;  // User has defined all, let any site use proxy. Only use in development.
			}
			string uriAuthority = new Uri(referer).GetLeftPart(UriPartial.Authority);
			return Config.AllowedReferrers.Any(r => r.ToLower() == uriAuthority.ToLower());
		}

	}
}
