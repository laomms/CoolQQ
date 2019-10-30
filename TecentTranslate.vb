Dim TimeStamp As Integer = (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds
            Dim r As System.Random = New Random(System.Environment.TickCount)
            Dim Nonce = r.Next(10000000, 99999999)
            Dim value = HttpUtility.UrlEncode(RichTextBox1.Text, Encoding.UTF8).ToUpper()
            Dim params = New SortedDictionary(Of String, String) From {
                {"app_id", "2116216563"},
                {"time_stamp", TimeStamp.ToString},
                {"nonce_str", Nonce},
                {"text", value},
                {"source", "zh"},
                {"target", "en"}
            }
            params.Add("sign", GetSign(params))
            Dim Data = GetUrlValue(params)
            Dim request = CType(WebRequest.Create("https://api.ai.qq.com/fcgi-bin/nlp/nlp_texttranslate"), HttpWebRequest)
            request.AllowAutoRedirect = True
            request.Method = "POST"
            request.ContentType = "application/x-www-form-urlencoded"
            Dim temp = Encoding.UTF8.GetBytes(Data)
            request.ContentLength = temp.Length
            Using stream = request.GetRequestStream()
                stream.Write(temp, 0, temp.Length)
            End Using
            Dim response = CType(request.GetResponse(), HttpWebResponse)
            Dim result As String = ""
            Using stream As Stream = request.GetResponse().GetResponseStream()
                Using reader As New StreamReader(stream)
                    result = reader.ReadToEnd()
                    reader.Close()
                End Using
                stream.Close()
            End Using
            Dim jsons As JObject = JObject.Parse(result)
            Dim szResult As String = jsons.SelectToken("data.target_text").ToString

 Private Function GetSign(ByVal params As SortedDictionary(Of String, String)) As String
        Dim str = $"{GetUrlValue(params)}&app_key={"lMaNsVDFw6asBJji"}"
        Using md5csp = New MD5CryptoServiceProvider()
            Dim temp = Encoding.UTF8.GetBytes(str)
            temp = md5csp.ComputeHash(temp)
            Return BitConverter.ToString(temp).Replace("-", "")
        End Using
    End Function

    Private Function GetUrlValue(ByVal params As SortedDictionary(Of String, String)) As String
        Dim sb = New StringBuilder()
        For Each item In params
            If sb.Length > 0 Then
                sb.Append("&"c)
            End If
            sb.Append($"{item.Key}={item.Value}")
        Next item
        Return sb.ToString()
    End Function
