public string LanguageDetect(string szContet)
{
		var jsonText = "{\"Text\":\"" + Regex.Replace(szContet, "^[A-Za-z0-9]+$", "").Replace("\n", "").Trim + "\",\"ProjectId\":0}";
		var data = Encoding.UTF8.GetBytes(jsonText);
		int TimeStamp = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds;
		var szDate = DateTime.UtcNow.ToString("yyyy-MM-dd");
		var univDateTime = DateTime.UtcNow.ToUniversalTime();
		System.Random r = new Random(System.Environment.TickCount);
		var Nonce = r.Next(10000000, 99999999);
		var SecretId = Your SecretId
                var SecretKey == Your SecretKey
                var httpRequestMethod == "POST";
		var canonicalUri = "/";
		var canonicalHeaders = "content-type:application/json" + "\n" + "host:" + "tmt.ap-guangzhou.tencentcloudapi.com" + "\n";
		var signedHeaders = "content-type;host";
		var hashedRequestPayload = GenerateSHA256String(jsonText).ToLower();
		var canonicalRequest = httpRequestMethod + "\n" + canonicalUri + "\n" + "\n" + canonicalHeaders + "\n" + signedHeaders + "\n" + hashedRequestPayload;

		var credentialScope = szDate + "/tmt/tc3_request";
		var hashedCanonicalRequest = GenerateSHA256String(canonicalRequest).ToLower();
		var stringToSign = "TC3-HMAC-SHA256" + "\n" + TimeStamp.ToString() + "\n" + credentialScope + "\n" + hashedCanonicalRequest;

		byte[] secretDate = sign256(Encoding.UTF8.GetBytes("TC3" + SecretKey), szDate);
		byte[] secretService = sign256(secretDate, "tmt");
		byte[] secretSigning = sign256(secretService, "tc3_request");
		var Signature = BytesToString(sign256(secretSigning, stringToSign)).ToLower();

		var Authorization = "TC3-HMAC-SHA256 Credential=" + SecretId + "/" + szDate + "/tmt/tc3_request, SignedHeaders=content-type;host, Signature=" + Signature;

		var url = "https://tmt.ap-guangzhou.tencentcloudapi.com";
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.KeepAlive = true;
		httpWebRequest.Method = "POST";
		httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
		httpWebRequest.ContentType = "application/json";
		httpWebRequest.Host = "tmt.ap-guangzhou.tencentcloudapi.com";
		httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, Authorization);
		httpWebRequest.Headers.Add("X-TC-Action: LanguageDetect");
		httpWebRequest.Headers.Add("X-TC-RequestClient: APIExplorer");
		httpWebRequest.Headers.Add("X-TC-Version: 2018-03-21");
		httpWebRequest.Headers.Add("X-TC-Timestamp:" + TimeStamp.ToString());
		httpWebRequest.Headers.Add("X-TC-Region: ap-guangzhou-open");
		httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
		var requestStream = httpWebRequest.GetRequestStream();
		requestStream.Write(data, 0, data.Length);
		requestStream.Close();
		string result = "";
		string szRes = "";
		try
		{
			HttpWebResponse httpWebResponse = httpWebRequest.GetResponse();
			if (httpWebResponse.ContentEncoding.ToLower().Contains("gzip"))
			{
				using (Stream stream = new System.IO.Compression.GZipStream(httpWebResponse.GetResponseStream, System.IO.Compression.CompressionMode.Decompress))
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						result = reader.ReadToEnd();
						Debug.Print(result);
						reader.Close();
					}
					stream.Close();
				}
			}
			else
			{
				using (Stream stream = httpWebRequest.GetResponse().GetResponseStream())
				{
					using (StreamReader reader = new StreamReader(stream))
					{
						result = reader.ReadToEnd();
						JObject jsons = JObject.Parse(result);
						szRes = jsons.SelectToken("Response").SelectToken("Lang").ToString();
						reader.Close();
					}
					stream.Close();
				}
			}
			httpWebResponse.Close();
		}
		catch (Exception ex)
		{

		}

		return szRes;
	}
 public string GenerateSHA256String(object inputString)
 {
		SHA256 sha256 = SHA256Managed.Create();
		byte[] bytes = Encoding.UTF8.GetBytes(inputString);
		byte[] hash = sha256.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();

		for (int i = 0; i < hash.Length; i++)
		{
			stringBuilder.Append(hash[i].ToString("X2"));
		}
		return stringBuilder.ToString();
	}
	public byte[] sign256(byte[] key, string msg)
	{
		System.Text.UTF8Encoding myEncoder = new System.Text.UTF8Encoding();
		byte[] XML = myEncoder.GetBytes(msg);
		System.Security.Cryptography.HMACSHA256 myHMACSHA256 = new System.Security.Cryptography.HMACSHA256(key);
		byte[] HashCode = myHMACSHA256.ComputeHash(XML);
		return HashCode;
	}
	public string BytesToString(byte[] Input)
	{
		System.Text.StringBuilder Result = new System.Text.StringBuilder(Input.Length * 2);
		string Part = null;
		foreach (byte b in Input)
		{
			Part = Convert.ToString(b, 16).ToUpper();
			if (Part.Length == 1)
			{
				Part = "0" + Part;
			}
			Result.Append(Part);
		}
		return Result.ToString();
	}
