public void GetOCR(string szImagePath)
{

		if (string.IsNullOrEmpty(szImagePath))
		{
			return;
		}
		UTF8Encoding encoding = new UTF8Encoding();
		var szUrl1 = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=client_id&client_secret=client_secret";
		var szToken = GetBaiduToken(szUrl1);
		var szUrl2 = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + szToken;
		byte[] bys = encoding.GetBytes("url=" + szImagePath);
		var szReturn = PostStrData(szUrl2, bys, "", "");
		if (szReturn.Contains("error_code"))
		{
			return;
		}
		if (szReturn == "")
		{
			return;
		}
		JObject jsons = JObject.Parse(szReturn);
		var n = jsons.SelectToken("words_result").Count;
		string szRes = "";
		string szResult = "";
		int i = 0;
		for (i = 0; i < n; i++)
		{
			szRes = szRes + jsons.SelectToken("words_result(" + i + ").words").ToString().Replace(" ", "");
			szResult = szResult + Environment.NewLine + jsons.SelectToken("words_result(" + i + ").words").ToString().Replace(" ", "");
		}


		for (i = ReplaceArray.GetLowerBound(0); i <= ReplaceArray.GetUpperBound(0); i++)
		{
			szRes = Regex.Replace(szRes, ReplaceArray(i), "");
		}
	}

	public string GetBaiduToken(string url)
	{
		string szToken = "";
		HttpWebRequest szRequest = (HttpWebRequest)WebRequest.Create(url);
		szRequest.Method = "POST";
		HttpWebResponse szrResponse = (HttpWebResponse)szRequest.GetResponse();
		try
		{
			using (Stream stream = szRequest.GetResponse().GetResponseStream())
			{
				using (StreamReader szReader = new StreamReader(stream))
				{
					var szResult = szReader.ReadToEnd();
					JObject jsons = JObject.Parse(szResult);
					szToken = jsons.SelectToken("access_token");
					szReader.Close();
				}
				stream.Close();
			}
		}
		catch (Exception ex)
		{

		}
		return szToken;
	}
