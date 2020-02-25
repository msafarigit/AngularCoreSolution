using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Ags.ResourceProxy
{
	public interface IProxyService
	{
		Task<string> RequestTokenJson(string tokenUri, List<KeyValuePair<string, string>> formData, string clientName);
		Task<HttpResponseMessage> ForwardRequestToServer(HttpRequest request, string url, string clientName);
	}
}
