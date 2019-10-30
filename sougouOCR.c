public void SougouOCR(string ImagePath)
{
		var Cookie = new CookieContainer();
		var url = "http://pic.sogou.com/pic/upload_pic.jsp";
		//Dim url = "http://ocr.shouji.sogou.com/v2/ocr/json"
		var Str = OCR_sougou_SogouPost(url, Cookie, OCR_sougou_Image_Bytes(ImagePath));
		var url2 = "http://pic.sogou.com/pic/ocr/ocrOnline.jsp?query=" + Str;
		var refer = "http://pic.sogou.com/resource/pic/shitu_intro/word_1.html?keyword=" + Str;
		var szReturn = OCR_sougou_SogouGet(url2, Cookie, refer);
		if (string.IsNullOrEmpty(szReturn))
		{
			return;
		}
		JObject jsons = JObject.Parse(szReturn);
		var szError = jsons.SelectToken("success").ToString();
		if (szError == "0")
		{
			return;
		}
		var n = jsons.SelectToken("result").Count;
		string szRes = "";
		for (var i = 0; i < n; i++)
		{
			szRes = szRes + Environment.NewLine + jsons.SelectToken("result(" + i + ").content").ToString().Replace("\n", "");
		}
	}


	public byte[] urlimageTobyte(string url)
	{
		byte[] bytedata = null;

		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			HttpWebResponse httpWebResponse = httpWebRequest.GetResponse();
			if (httpWebResponse.StatusCode == 200)
			{
				using (Stream stream = httpWebRequest.GetResponse().GetResponseStream())
				{
					var memoryStream = new MemoryStream();
					stream.CopyTo(memoryStream);
					var array = new byte[memoryStream.Length];
					memoryStream.Position = 0L;
					memoryStream.Read(array, 0, (int)memoryStream.Length);
					memoryStream.Close();
					bytedata = array;
					stream.Close();
				}
				httpWebResponse.Close();
			}
		}
		catch (Exception ex)
		{

		}
		return bytedata;
	}

 public byte[] OCR_sougou_Content_Length(Image img)
 {
//INSTANT C# TODO TASK: The 'On Error Resume Next' statement is not converted by Instant C#:
		On Error Resume Next
		var bytes = Encoding.UTF8.GetBytes("------WebKitFormBoundary8orYTmcj8BHvQpVU" + Environment.NewLine + "Content-Disposition: form-data; name=pic; filename=pic.jpg" + Environment.NewLine + "Content-Type: image/jpeg" + Environment.NewLine + Environment.NewLine);
		var Array = ImgToBytes(img);
		var bytes2 = Encoding.UTF8.GetBytes(Environment.NewLine + "------WebKitFormBoundary8orYTmcj8BHvQpVU--" + Environment.NewLine);
		byte[] array2 = null;
		array2 = new byte[(bytes.Length + Array.Length + bytes2.Length) + 1];
		bytes.CopyTo(array2, 0);
		Array.CopyTo(array2, bytes.Length);
		bytes2.CopyTo(array2, bytes.Length + Array.Length);
		return array2;
	}

public byte[] OCR_sougou_Image_Bytes(string url)
{
//INSTANT C# TODO TASK: The 'On Error Resume Next' statement is not converted by Instant C#:
		On Error Resume Next
		var bytes = Encoding.UTF8.GetBytes("------WebKitFormBoundary8orYTmcj8BHvQpVU" + Environment.NewLine + "Content-Disposition: form-data; name=pic; filename=pic.jpg" + Environment.NewLine + "Content-Type: image/jpeg" + Environment.NewLine + Environment.NewLine);
		var Array = urlimageTobyte(url);
		var bytes2 = Encoding.UTF8.GetBytes(Environment.NewLine + "------WebKitFormBoundary8orYTmcj8BHvQpVU--" + Environment.NewLine);
		byte[] array2 = null;
		array2 = new byte[(bytes.Length + Array.Length + bytes2.Length) + 1];
		bytes.CopyTo(array2, 0);
		Array.CopyTo(array2, bytes.Length);
		bytes2.CopyTo(array2, bytes.Length + Array.Length);
		return array2;
	}


	public string OCR_sougou_SogouPost(string url, CookieContainer cookie, byte[] content)
	{
		string result = "";
		try
		{
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "POST";
			httpWebRequest.CookieContainer = cookie;
			httpWebRequest.Timeout = 10000;
			httpWebRequest.Referer = "http://pic.sogou.com/resource/pic/shitu_intro/index.html";
			httpWebRequest.ContentType = "multipart/form-data; boundary=----WebKitFormBoundary8orYTmcj8BHvQpVU";
			httpWebRequest.Accept = "*/*";
			httpWebRequest.Headers.Add("Origin: http://pic.sogou.com");
			httpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)";
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.ProtocolVersion = new Version(1, 1);
			httpWebRequest.ContentLength = content.Length;
			var requestStream = httpWebRequest.GetRequestStream();
			requestStream.Write(content, 0, content.Length);
			requestStream.Close();

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
		}
		catch (Exception ex)
		{

		}
		return result;
	}

	public string OCR_sougou_SogouGet(string url, CookieContainer cookie, string refer)
	{
		string result = "";
		try
		{
			var Text = "";
			HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.Method = "GET";
			httpWebRequest.CookieContainer = cookie;
			httpWebRequest.Referer = refer;
			httpWebRequest.Timeout = 10000;
			httpWebRequest.Accept = "application/json";
			httpWebRequest.Headers.Add("X-Requested-With: XMLHttpRequest");
			httpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate");
			httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8";
			httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)";
			httpWebRequest.ServicePoint.Expect100Continue = false;
			httpWebRequest.ProtocolVersion = new Version(1, 1);


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
		}
		catch (Exception ex)
		{

		}
		return result;
	}

	public byte[] MergeByte(byte[] a, byte[] b, byte[] c)
	{
//INSTANT C# TODO TASK: The 'On Error Resume Next' statement is not converted by Instant C#:
		On Error Resume Next
		var array = new byte[a.Length + b.Length + c.Length];
		a.CopyTo(array, 0);
		b.CopyTo(array, a.Length);
		c.CopyTo(array, a.Length + b.Length);
		return array;
	}
