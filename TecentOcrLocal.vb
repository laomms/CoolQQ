            Dim PicName = Path.GetFileName(PictureBox1.Image.Tag)
            Dim res = TecentOcrLocal(PictureBox1.Image, PictureBox1.Image.Tag)
            If res = "" Then Return
            Dim jsons As JObject = JObject.Parse(res)
            Dim szError = jsons.SelectToken("code").ToString
            Debug.Print(szError)
            If szError <> "0" Then MsgBox("图片有误！") : Return
            Dim n = jsons.SelectToken("data").SelectToken("items").Count
            If n = 0 Then Return
            Dim szRes As String = ""
            For i = 0 To n - 1
                szRes = szRes + vbNewLine + jsons.SelectToken("data").SelectToken("items(" & i & ").itemstring").ToString
            Next
            
            
            
    Public Function TecentOcrLocal(image As Image, path As String) As String
        Dim Authorization = HmacSha1Sign("Your AppId", "AKIDwlKK5MNAgww3vYo2tEJxMVP7ZmnY6PTH", "lGWaOyMUN4VilbCYeU8WhhSyEJ9oDNLb", "bucket-01", 2592000)
        Dim boundary As String = "--------------" & DateTime.Now.Ticks.ToString("x")
        Dim header As String = vbNewLine & "--" & boundary & vbNewLine & "Content-Disposition:form-data;name=""appid"";" & vbNewLine & vbNewLine & "1256493063" & vbNewLine
        header = header + "--" & boundary & vbNewLine & "Content-Disposition:form-data;name=""bucket"";" & vbNewLine & vbNewLine & "bucket-01" & vbNewLine
        header = header + "--" & boundary & vbNewLine & "Content-Disposition:form-data;name=""image""; filename=""" & path & """" & vbNewLine & "Content-Type: image/jpeg" & vbNewLine & vbNewLine
        Dim footer As String = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx" + vbNewLine + boundary + "--" & vbNewLine
        Dim fileStream = New FileStream(path, FileMode.Open, FileAccess.Read)
        Dim array = New Byte(fileStream.Length - 1) {}
        fileStream.Read(array, 0, CInt(fileStream.Length))
        fileStream.Close()
        Dim Data() = MergeByte(Encoding.ASCII.GetBytes(header), array, Encoding.ASCII.GetBytes(footer))
        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(“https://recognition.image.myqcloud.com/ocr/general”), HttpWebRequest)
        httpWebRequest.Method = "POST"
        httpWebRequest.ContentType = "multipart/form-data; boundary=" & boundary
        httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, Authorization)
        httpWebRequest.ContentLength = Data.Length
        Dim requestStream = httpWebRequest.GetRequestStream()
        requestStream.Write(Data, 0, Data.Length)
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
        Return result

    End Function
            
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
            
            
