Public Sub Main()

        'Dim SISId As String = "815"
        ' define the local key and vector byte arrays
        Dim key() As Byte = _
          {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, _
          15, 16, 17, 18, 19, 20, 21, 22, 23, 24}

        Dim iv() As Byte = {8, 7, 6, 5, 4, 3, 2, 1}

        ' instantiate the class with the arrays
        Dim des As New cTripleDES(key, iv)
        Dim accesstokenencrypted As String = CType(Dts.Variables("TokenKey").Value, String) '""
        Dim accesstoken As String = des.Decrypt(accesstokenencrypted)

        Dim checkStatusurl As String = Dts.Variables("baseurl").Value.ToString() + "api/v1/accounts/" + Dts.Variables("account").Value.ToString() + "/sis_imports/" + Dts.Variables("SISId").Value.ToString()
        Dim process_warnings As String
        Dim workflow_state As String = ""
        Dim response As String = ""
        Dim StatusMessage As String = ""

        Dim nvc As New NameValueCollection
        'Threading.Thread.Sleep(10000)

        nvc.Add("access_token", accesstoken)


        Dim mywebclient As New WebClient()
        mywebclient.QueryString = nvc
        mywebclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")

        'Do While workflow_state = "" Or workflow_state = """created"""



        Dim responseArray As Byte() = mywebclient.DownloadData(checkStatusurl)
        response = Encoding.UTF8.GetString(responseArray)

        Dim stringarray As String() = response.Split(CChar(","))

        For Each s As String In stringarray
            'If (s.Contains("workflow_state")) Then
            'workflow_state = s.Substring(s.IndexOf(":") + 1, s.Length - s.IndexOf(":") - 1)
            'ElseIf (s.Contains("process_warnings")) Then
            'process_warnings = s.Substring(s.IndexOf(":") + 1, s.Length - s.IndexOf(":") - 1)
            'End If
            StatusMessage = StatusMessage + s + vbNewLine
        Next

        'Loop
        Dts.Variables("StatusMessage").Value = StatusMessage
        Dts.TaskResult = Dts.Results.Success
	End Sub