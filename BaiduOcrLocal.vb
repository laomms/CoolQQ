 
            fileName = System.IO.Path.GetFullPath(open.FileName)
            PictureBox1.Image = New Bitmap(open.FileName)
            Me.PictureBox1.Image.Tag = fileName
            Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            szBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName))
            Dim encoding As New UTF8Encoding
            Dim szUrl1 = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=your client_id&client_secret=your client_secret"
            Dim szToken = GetBaiduToken(szUrl1)
            Dim szUrl2 = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" & szToken
            Dim PostData As String = HttpUtility.UrlEncode(szBase64)
            Dim bys As Byte() = encoding.GetBytes("image=" & PostData & "&language_type=CHN_ENG")
            Dim szReturn = PostStrData(szUrl2, bys, "", "")
            If szReturn = "" Then Return
            Dim jsons As JObject = JObject.Parse(szReturn)
            Dim n = jsons.SelectToken("words_result").Count
            Dim szRes As String = ""
            For i = 0 To n - 1
                szRes = szRes + vbNewLine + jsons.SelectToken("words_result(" & i & ").words").ToString
            Next
            
            
     Public Function GetBaiduToken(url As String) As String
        Dim szToken As String = ""
        Dim szRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        szRequest.Method = "POST"
        Dim szrResponse As HttpWebResponse = DirectCast(szRequest.GetResponse(), HttpWebResponse)
        Try
            Using stream As Stream = szRequest.GetResponse().GetResponseStream()
                Using szReader As New StreamReader(stream)
                    Dim szResult = szReader.ReadToEnd()
                    Dim jsons As JObject = JObject.Parse(szResult)
                    szToken = jsons.SelectToken("access_token")
                    szReader.Close()
                End Using
                stream.Close()
            End Using
        Catch ex As Exception

        End Try
        Return szToken
    End Function
    
    Public Function PostStrData(url As String, data As Byte(), cookie As String, referer As String) As String
        Dim Text = ""
        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        httpWebRequest.Method = "POST"
        httpWebRequest.Timeout = 10000
        httpWebRequest.Referer = referer
        httpWebRequest.Accept = "*/*"
        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
        httpWebRequest.ServicePoint.Expect100Continue = True
        httpWebRequest.ProtocolVersion = New Version(1, 1)
        httpWebRequest.ContentLength = data.Length
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
        Return result
    End Function
    
