 Public Sub IIDOCR(ImagePath As String) 

        Dim Authorization = HmacSha1Sign("1256493063", "your id", "your password", "bucket-01", 2592000)
        Dim Text = ""
        Dim jsonText = "{""appid"":""1256493063"",""bucket"":""bucket-01"",""url"":""" & ImagePath & """}"
        Dim data = Encoding.UTF8.GetBytes(jsonText)
        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create("https://recognition.image.myqcloud.com/ocr/general"), HttpWebRequest)
        httpWebRequest.KeepAlive = True
        httpWebRequest.Method = "POST"
        httpWebRequest.ContentType = "application/json; charset=utf-8"
        httpWebRequest.Host = "recognition.image.myqcloud.com"
        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "Basic " & Authorization)
        Dim requestStream = httpWebRequest.GetRequestStream()
        requestStream.Write(data, 0, data.Length)
        requestStream.Close()
        Dim result As String = ""

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
                        Debug.Print(result)
                        reader.Close()
                    End Using
                    stream.Close()
                End Using
            End If
            httpWebResponse.Close()
        Catch ex As Exception

        End Try

        If result = "" Then Return
        Dim i As Integer
        Dim jsons As JObject = JObject.Parse(result)
        Dim szError = jsons.SelectToken("code").ToString
        Debug.Print(szError)
        If szError <> "0" Then Return
        Dim n = jsons.SelectToken("data").SelectToken("items").Count
        If n = 0 Then Return
        Dim szRes As String = ""
        Dim szResult As String = ""
        For i = 0 To n - 1
            szRes = szRes + jsons.SelectToken("data").SelectToken("items(" & i & ").itemstring").ToString.Replace(" ", "")
            szResult = szResult + vbNewLine + jsons.SelectToken("data").SelectToken("items(" & i & ").itemstring").ToString
        Next      

    End Sub
    
    
    Public Function HmacSha1Sign(ByVal appId As Long, ByVal secretId As String, ByVal secretKey As String, ByVal bucketName As String, ByVal expired As Long) As String
        Dim now As Long = (DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds / 1000
        Dim rdm As Integer = DateTime.Now.ToString("yyMMddHHmm")
        Dim hmacsha1 = New HMACSHA1(Encoding.UTF8.GetBytes(secretKey))
        Dim plainText As String = "a=" & appId & "&b=" & bucketName & "&k=" & secretId & "&e=" & now + expired & "&t=" & now & "&r=" & rdm & "&u=0&f="
        Dim dataBuffer = Encoding.UTF8.GetBytes(plainText)
        Dim hashBytes = hmacsha1.ComputeHash(dataBuffer)
        Dim bytes As List(Of Byte) = New List(Of Byte)()
        bytes.AddRange(hashBytes)
        bytes.AddRange(dataBuffer)
        Return Convert.ToBase64String(bytes.ToArray())
    End Function
