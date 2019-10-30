public void IIDOCR(string ImagePath)
 {

		var Authorization = HmacSha1Sign(Convert.ToInt64("1256493063"), "your id", "your password", "bucket-01", 2592000);
		var Text = "";
		var jsonText = "{\"appid\":\"1256493063\",\"bucket\":\"bucket-01\",\"url\":\"" + ImagePath + "\"}";
		var data = Encoding.UTF8.GetBytes(jsonText);
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create("https://recognition.image.myqcloud.com/ocr/general");
		httpWebRequest.KeepAlive = true;
		httpWebRequest.Method = "POST";
		httpWebRequest.ContentType = "application/json; charset=utf-8";
		httpWebRequest.Host = "recognition.image.myqcloud.com";
		httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " + Authorization);
		var requestStream = httpWebRequest.GetRequestStream();
		requestStream.Write(data, 0, data.Length);
		requestStream.Close();
		string result = "";

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
						Debug.Print(result);
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

		if (string.IsNullOrEmpty(result))
		{
			return;
		}
		int i = 0;
		JObject jsons = JObject.Parse(result);
		var szError = jsons.SelectToken("code").ToString();
		Debug.Print(szError);
		if (szError != "0")
		{
			return;
		}
		var n = jsons.SelectToken("data").SelectToken("items").Count;
		if (n == 0)
		{
			return;
		}
		string szRes = "";
		string szResult = "";
		for (i = 0; i < n; i++)
		{
			szRes = szRes + jsons.SelectToken("data").SelectToken("items(" + i + ").itemstring").ToString().Replace(" ", "");
			szResult = szResult + Environment.NewLine + jsons.SelectToken("data").SelectToken("items(" + i + ").itemstring").ToString();
		}

	}


	public string HmacSha1Sign(long appId, string secretId, string secretKey, string bucketName, long expired)
	{
		long now = (DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds / 1000;
		int rdm = Convert.ToInt32(DateTime.Now.ToString("yyMMddHHmm"));
		var hmacsha1 = new HMACSHA1(Encoding.UTF8.GetBytes(secretKey));
		string plainText = "a=" + appId + "&b=" + bucketName + "&k=" + secretId + "&e=" + (now + expired) + "&t=" + now + "&r=" + rdm + "&u=0&f=";
		var dataBuffer = Encoding.UTF8.GetBytes(plainText);
		var hashBytes = hmacsha1.ComputeHash(dataBuffer);
		List<byte> bytes = new List<byte>();
		bytes.AddRange(hashBytes);
		bytes.AddRange(dataBuffer);
		return Convert.ToBase64String(bytes.ToArray());
	}
