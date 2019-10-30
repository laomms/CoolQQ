

Encoding encoder = Encoding.UTF8;
			string PostData = HttpUtility.UrlEncode(":image/jpeg;base64," + urlimageTobase64(TextBox2.Text));
			byte[] bytes = encoder.GetBytes("imgBase=data" + PostData + "&lang=auto&company=");
			var value = PostStrData("http://aidemo.youdao.com/ocrapi1", bytes, "", "http://aidemo.youdao.com/ocrdemo");
			if (value == "")
			{
				return;
			}
			JObject jsons = JObject.Parse(value);
			var szError = jsons.SelectToken("errorCode").ToString();
			Debug.Print(szError);
			if (szError != "0")
			{
				MessageBox.Show("系统正忙或者图片格式太大！");
				return;
			}
			string szRes = "";
			var n = jsons.SelectToken("lines").Count;
			for (var i = 0; i < n; i++)
			{
				szRes = szRes + Environment.NewLine + jsons.SelectToken("lines(" + i + ").words").ToString();
			}



	Public Function PostStrData(url As string, data As byte(), cookie As string, referer As string) As string;
		var Text = "";
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
		httpWebRequest.Method = "POST";
		httpWebRequest.Timeout = 10000;
		httpWebRequest.Referer = referer;
		httpWebRequest.Accept = "*/*";
		httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
		httpWebRequest.ServicePoint.Expect100Continue = true;
		httpWebRequest.ProtocolVersion = new Version(1, 1);
		httpWebRequest.ContentLength = data.Length;
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
		return result;
	}
