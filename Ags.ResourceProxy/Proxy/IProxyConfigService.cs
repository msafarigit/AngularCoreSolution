using System;
using System.Net;
using System.Collections.Generic;

namespace Infrastructure
{
	public interface IProxyConfigService
	{
		ProxyConfig Config { get; }
		string ConfigPath { get; }
		bool IsAllowedReferrer(string referer);
		List<KeyValuePair<string, string>> GetOAuth2FormData(ServerUrl su, string proxyReferrer);
		List<KeyValuePair<string, string>> GetPortalExchangeTokenFormData(ServerUrl su, string proxyReferrer, string portalCode);
		NetworkCredential GetCredentials(ServerUrl serverUrlConfig);
		ServerUrl GetProxyServerUrlConfig(string queryStringUrl);
	}

	public partial class ProxyConfig
	{
		public virtual string[] AllowedReferrers { get; set; } // Uris that are allowed to use the proxy.
		public virtual ServerUrl[] ServerUrls { get; set; } // Objects containing proxy configuration for each server URL to be proxied. If URL hitting the proxy contains one of these URLs it will be proxied.
		public virtual int TokenCacheMinutes { get; set; } // Time that tokens from ArcGIS server will be cached, this should be <= the timeout parameter on the token received.
	}

	public class ServerUrl
	{
		public virtual string Url { get; set; } // Root URL to which the settings apply.
		public virtual bool UseAppPoolIdentity { get; set; } // Set to utilize the current running app pool identity to make requests to the server.
		public virtual string Domain { get; set; } // Server domain.
		public virtual string Username { get; set; } // ArcGIS Server login user-name.
		public virtual string Password { get; set; } // ArcGIS Server login password.
		public virtual string ClientId { get; set; } // Application client id - when using Enterprise Portal
		public virtual string ClientSecret { get; set; } // Application client secret - when using Enterprise Portal
		public virtual string Oauth2Endpoint { get; set; } // ArcGIS Server Oauth2-Endpoint
	}
}
