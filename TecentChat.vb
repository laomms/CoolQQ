 Dim jsonText = "{""Text"":""你好"",""ProjectId"":0}"
        Dim data = Encoding.UTF8.GetBytes(jsonText)
        Dim TimeStamp As Integer = (DateTime.UtcNow - New DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds  
        Dim szDate = DateTime.UtcNow.ToString("yyyy-MM-dd")  
        Dim univDateTime = DateTime.UtcNow.ToUniversalTime()
        Dim r As System.Random = New Random(System.Environment.TickCount)
        Dim Nonce = r.Next(10000000, 99999999)  
Dim SecretId = your SecretId
Dim SecretKey = your SecretKey
        Dim httpRequestMethod = "POST"
        Dim canonicalUri = "/"
        'Dim canonicalQueryString = "Limit=10&Offset=0"
        Dim canonicalHeaders = "content-type:application/json" + vbLf + "host:" + "aai.ap-guangzhou.tencentcloudapi.com" + vbLf
        Dim signedHeaders = "content-type;host"
        Dim hashedRequestPayload = LCase(GenerateSHA256String(jsonText))  '2de754245439f7be7115c0d563dff680733be2e29507177744d1e4e7e6464c37
        Dim canonicalRequest = httpRequestMethod + vbLf + canonicalUri + vbLf + vbLf + canonicalHeaders + vbLf + signedHeaders + vbLf + hashedRequestPayload

        Dim credentialScope = szDate + "/aai/tc3_request"
        Dim hashedCanonicalRequest = LCase(GenerateSHA256String(canonicalRequest))
        Dim stringToSign = "TC3-HMAC-SHA256" + vbLf + TimeStamp.ToString + vbLf + credentialScope + vbLf + hashedCanonicalRequest

        Dim secretDate() As Byte = sign256(Encoding.UTF8.GetBytes("TC3" + SecretKey), szDate)
        Dim secretService() As Byte = sign256(secretDate, "aai")
        Dim secretSigning() As Byte = sign256(secretService, "tc3_request")
        Dim Signature = LCase(BytesToString(sign256(secretSigning, stringToSign)))

        Dim Authorization = "TC3-HMAC-SHA256 Credential=" & SecretId & "/" & szDate & "/aai/tc3_request, SignedHeaders=content-type;host, Signature=" & Signature


        Dim url = "https://aai.ap-guangzhou.tencentcloudapi.com"
        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        httpWebRequest.KeepAlive = True
        httpWebRequest.Method = "POST"
        httpWebRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8"
        httpWebRequest.ContentType = "application/json"
        httpWebRequest.Host = "aai.ap-guangzhou.tencentcloudapi.com"
        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, Authorization)
        httpWebRequest.Headers.Add("X-TC-Action: Chat")
        httpWebRequest.Headers.Add("X-TC-RequestClient: APIExplorer")
        httpWebRequest.Headers.Add("X-TC-Version: 2018-05-22")
        httpWebRequest.Headers.Add("X-TC-Timestamp:" & TimeStamp.ToString)
        httpWebRequest.Headers.Add("X-TC-Region: ap-guangzhou-open")
        httpWebRequest.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)"
        Dim requestStream = httpWebRequest.GetRequestStream()
        requestStream.Write(data, 0, data.Length)
        requestStream.Close()
        Dim result As String = ""
        Dim szRes As String = ""
        Try
            Dim httpWebResponse As HttpWebResponse = httpWebRequest.GetResponse()
            If httpWebResponse.ContentEncoding.ToLower().Contains("gzip") Then
                Using stream As Stream = New System.IO.Compression.GZipStream(httpWebResponse.GetResponseStream, IO.Compression.CompressionMode.Decompress)
                    Using reader As New StreamReader(stream)
                        result = reader.ReadToEnd()
                        Debug.Print(result)
                        reader.Close()
                    End Using
                    stream.Close()
                End Using
            Else
                Using stream As Stream = httpWebRequest.GetResponse().GetResponseStream()
                    Using reader As New StreamReader(stream)
                        result = reader.ReadToEnd()
                        Dim jsons As JObject = JObject.Parse(result)
                        szRes = jsons.SelectToken("Response").SelectToken("Answer").ToString
                        reader.Close()
                    End Using
                    stream.Close()
                End Using
            End If
            httpWebResponse.Close()
        Catch ex As Exception

        End Try
        MsgBox(szRes)
