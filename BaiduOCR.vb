Public Sub GetOCR(szImagePath As String)  
     
        If szImagePath = "" Then Return
        Dim encoding As New UTF8Encoding
        Dim szUrl1 = "https://aip.baidubce.com/oauth/2.0/token?grant_type=client_credentials&client_id=client_id&client_secret=client_secret"
        Dim szToken = GetBaiduToken(szUrl1)
        Dim szUrl2 = "https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" & szToken
        Dim bys As Byte() = encoding.GetBytes("url=" & szImagePath)
        Dim szReturn = PostStrData(szUrl2, bys, "", "")
        If szReturn.Contains("error_code") Then Return
        If szReturn = "" Then Return
        Dim jsons As JObject = JObject.Parse(szReturn)
        Dim n = jsons.SelectToken("words_result").Count
        Dim szRes As String = ""
        Dim szResult As String = ""
        Dim i As Integer
        For i = 0 To n - 1
            szRes = szRes + jsons.SelectToken("words_result(" & i & ").words").ToString.Replace(" ", "")
            szResult = szResult + vbNewLine + jsons.SelectToken("words_result(" & i & ").words").ToString.Replace(" ", "")
        Next


        For i = LBound(ReplaceArray) To UBound(ReplaceArray)
            szRes = Regex.Replace(szRes, ReplaceArray(i), "")
        Next 
    End Sub
    
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
