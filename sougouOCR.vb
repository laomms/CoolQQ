Public Sub SougouOCR(ImagePath As String)
        Dim Cookie = New CookieContainer()
        Dim url = "http://pic.sogou.com/pic/upload_pic.jsp"
        'Dim url = "http://ocr.shouji.sogou.com/v2/ocr/json"
        Dim Str = OCR_sougou_SogouPost(url, Cookie, OCR_sougou_Image_Bytes(ImagePath))
        Dim url2 = "http://pic.sogou.com/pic/ocr/ocrOnline.jsp?query=" + Str
        Dim refer = "http://pic.sogou.com/resource/pic/shitu_intro/word_1.html?keyword=" + Str
        Dim szReturn = OCR_sougou_SogouGet(url2, Cookie, refer)
        If szReturn = "" Then Return
        Dim jsons As JObject = JObject.Parse(szReturn)
        Dim szError = jsons.SelectToken("success").ToString
        If szError = "0" Then Return
        Dim n = jsons.SelectToken("result").Count
        Dim szRes As String = ""
        For i = 0 To n - 1
            szRes = szRes + vbNewLine + jsons.SelectToken("result(" & i & ").content").ToString.Replace(vbLf, "")
        Next
    End Sub


    Public Function urlimageTobyte(ByVal url As String) As Byte()
        Dim bytedata As Byte() = Nothing

        Try
            Dim httpWebRequest As HttpWebRequest = DirectCast(WebRequest.Create(url), HttpWebRequest)
            httpWebRequest.Method = "GET"
            Dim httpWebResponse As HttpWebResponse = httpWebRequest.GetResponse()
            If httpWebResponse.StatusCode = 200 Then
                Using stream As Stream = httpWebRequest.GetResponse().GetResponseStream()
                    Dim memoryStream = New MemoryStream()
                    stream.CopyTo(memoryStream)
                    Dim array = New Byte(memoryStream.Length - 1) {}
                    memoryStream.Position = 0L
                    memoryStream.Read(array, 0, CInt(memoryStream.Length))
                    memoryStream.Close()
                    bytedata = array
                    stream.Close()
                End Using
                httpWebResponse.Close()
            End If
        Catch ex As Exception

        End Try
        Return bytedata
    End Function

 Public Function OCR_sougou_Content_Length(img As Image) As Byte()
        On Error Resume Next
        Dim bytes = Encoding.UTF8.GetBytes("------WebKitFormBoundary8orYTmcj8BHvQpVU" & vbNewLine & "Content-Disposition: form-data; name=pic; filename=pic.jpg" & vbNewLine & "Content-Type: image/jpeg" & vbNewLine & vbNewLine)
        Dim Array = ImgToBytes(img)
        Dim bytes2 = Encoding.UTF8.GetBytes(vbNewLine & "------WebKitFormBoundary8orYTmcj8BHvQpVU--" & vbNewLine)
        Dim array2 As Byte()
        ReDim array2(bytes.Length + Array.Length + bytes2.Length)
        bytes.CopyTo(array2, 0)
        Array.CopyTo(array2, bytes.Length)
        bytes2.CopyTo(array2, bytes.Length + Array.Length)
        Return array2
    End Function

Public Function OCR_sougou_Image_Bytes(url As String) As Byte()
        On Error Resume Next
        Dim bytes = Encoding.UTF8.GetBytes("------WebKitFormBoundary8orYTmcj8BHvQpVU" & vbNewLine & "Content-Disposition: form-data; name=pic; filename=pic.jpg" & vbNewLine & "Content-Type: image/jpeg" & vbNewLine & vbNewLine)
        Dim Array = urlimageTobyte(url)
        Dim bytes2 = Encoding.UTF8.GetBytes(vbNewLine & "------WebKitFormBoundary8orYTmcj8BHvQpVU--" & vbNewLine)
        Dim array2 As Byte()
        ReDim array2(bytes.Length + Array.Length + bytes2.Length)
        bytes.CopyTo(array2, 0)
        Array.CopyTo(array2, bytes.Length)
        bytes2.CopyTo(array2, bytes.Length + Array.Length)
        Return array2
    End Function


    Public Function OCR_sougou_SogouPost(url As String, cookie As CookieContainer, content As Byte()) As String
        Dim result As String = ""
        Try
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
        Catch ex As Exception

        End Try
        Return result
    End Function

    Public Function OCR_sougou_SogouGet(url As String, cookie As CookieContainer, refer As String) As String
        Dim result As String = ""
        Try
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
        Catch ex As Exception

        End Try
        Return result
    End Function

    Public Function MergeByte(ByVal a As Byte(), ByVal b As Byte(), ByVal c As Byte()) As Byte()
        On Error Resume Next
        Dim array = New Byte(a.Length + b.Length + c.Length - 1) {}
        a.CopyTo(array, 0)
        b.CopyTo(array, a.Length)
        c.CopyTo(array, a.Length + b.Length)
        Return array
    End Function
