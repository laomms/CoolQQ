           
            fileName = System.IO.Path.GetFullPath(open.FileName)
            PictureBox1.Image = New Bitmap(open.FileName)
            Me.PictureBox1.Image.Tag = fileName
            Me.PictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom
            szBase64 = Convert.ToBase64String(System.IO.File.ReadAllBytes(fileName))
            Dim encoder As Encoding = Encoding.UTF8
            Dim PostData As String = HttpUtility.UrlEncode(":image/jpeg;base64," & szBase64)
            Dim bytes As Byte() = encoder.GetBytes("imgBase=data" & PostData & "&lang=auto&company=")
            Dim value = PostStrData("http://aidemo.youdao.com/ocrapi1", bytes, "", "http://aidemo.youdao.com/ocrdemo")
            If value = "" Then Return
            Dim jsons As JObject = JObject.Parse(value)
            Dim szError = jsons.SelectToken("errorCode").ToString
            Debug.Print(szError)
            If szError <> "0" Then MsgBox("系统正忙或者图片格式太大！") : Return
            Dim szRes As String = ""
            Dim n = jsons.SelectToken("lines").Count
            For i = 0 To n - 1
                szRes = szRes + vbNewLine + jsons.SelectToken("lines(" & i & ").words").ToString
            Next
            
            
            
            
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
