            Dim Cookie = New CookieContainer()
            Dim url = "http://pic.sogou.com/pic/upload_pic.jsp"
            'Dim url = "http://ocr.shouji.sogou.com/v2/ocr/json"
            Dim Str = OCR_sougou_SogouPost(url, Cookie, OCR_sougou_Content_Length(PictureBox1.Image))
            Dim url2 = "http://pic.sogou.com/pic/ocr/ocrOnline.jsp?query=" + Str
            Dim refer = "http://pic.sogou.com/resource/pic/shitu_intro/word_1.html?keyword=" + Str
            Dim szReturn = OCR_sougou_SogouGet(url2, Cookie, refer)
            Dim jsons As JObject = JObject.Parse(szReturn)
            Dim szError = jsons.SelectToken("success").ToString
            If szError = "0" Then Return
            Dim n = jsons.SelectToken("result").Count
            Dim szRes As String = ""
            For i = 0 To n - 1
                szRes = szRes + vbNewLine + jsons.SelectToken("result(" & i & ").content").ToString.Replace(vbLf, "")
            Next
            
            
    Public Function OCR_sougou_SogouGet(url As String, cookie As CookieContainer, refer As String) As String
        Dim Text = ""
        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        httpWebRequest.Method = "GET"
        httpWebRequest.CookieContainer = cookie
        httpWebRequest.Referer = refer
        httpWebRequest.Timeout = 10000
        httpWebRequest.Accept = "application/json"
        httpWebRequest.Headers.Add("X-Requested-With: XMLHttpRequest")
        httpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate")
        httpWebRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8"
        httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)"
        httpWebRequest.ServicePoint.Expect100Continue = False
        httpWebRequest.ProtocolVersion = New Version(1, 1)
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

 Public Function OCR_sougou_SogouPost(url As String, cookie As CookieContainer, content As Byte()) As String

        Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
        httpWebRequest.Method = "POST"
        httpWebRequest.CookieContainer = cookie
        httpWebRequest.Timeout = 10000
        httpWebRequest.Referer = "http://pic.sogou.com/resource/pic/shitu_intro/index.html"
        httpWebRequest.ContentType = "multipart/form-data; boundary=----WebKitFormBoundary8orYTmcj8BHvQpVU"
        httpWebRequest.Accept = "*/*"
        httpWebRequest.Headers.Add("Origin: http://pic.sogou.com")
        httpWebRequest.Headers.Add("Accept-Encoding: gzip,deflate")
        httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko)"
        httpWebRequest.ServicePoint.Expect100Continue = False
        httpWebRequest.ProtocolVersion = New Version(1, 1)
        httpWebRequest.ContentLength = content.Length
        Dim requestStream = httpWebRequest.GetRequestStream()
        requestStream.Write(content, 0, content.Length)
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
