Public Class ScriptMain

	' The execution engine calls this method when the task executes.
	' To access the object model, use the Dts object. Connections, variables, events,
	' and logging features are available as static members of the Dts class.
	' Before returning from this method, set the value of Dts.TaskResult to indicate success or failure.
	' 
	' To open Code and Text Editor Help, press F1.
	' To open Object Browser, press Ctrl+Alt+J.

	Public Sub Main()
		'
		' Add your code here
        ' 
        Dim key() As Byte = _
          {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, _
          15, 16, 17, 18, 19, 20, 21, 22, 23, 24}

        Dim iv() As Byte = {8, 7, 6, 5, 4, 3, 2, 1}

        ' instantiate the class with the arrays
        Dim des As New cTripleDES(key, iv)

        '****************************************************************************************************************************************
        'Use this section to encryt your passwords initially.
        'Dim newEncryptedData As String = des.Encrypt("")
        'MsgBox("should be Test: " + newEncryptedData)
        'Dim strFile As String
        'Dim strText As String
        'strFile = "C:\VisualStudio2005\Canvas\AddDrop\test.txt"
        'strText = "test"
        'File.AppendAllText(strFile, "accesstoken: " + newEncryptedData)
        '           File.AppendAllText(strFile, "PWD: " + newEncryptedData2)
        '*******************************************************************************************************************************************************************

        Dim baseurl As String = Dts.Variables("baseurl").Value.ToString() '"https://stmarksschool.test.instructure.com/"
        Dim account As String = Dts.Variables("account").Value.ToString() '"1"
        Dim accesstokenencrypted As String = CType(Dts.Variables("TokenKey").Value, String) '""
        Dim importtype As String = Dts.Variables("importtype").Value.ToString() '"instructure_csv"
        Dim fileaddress As String = Dts.Variables("fileaddress").Value.ToString() '"E:\Data\AddDrops\AddDrops.csv"
        Dim accesstoken As String = des.Decrypt(accesstokenencrypted)
        'MsgBox("accesstoken is- " + accesstoken)

        Dim sisurl As String = baseurl + "api/v1/accounts/" + account
        Dim uploadUrl As String = sisurl + "/sis_imports.json"

        Dim nvc As New NameValueCollection

        nvc.Add("import_type", importtype)
        nvc.Add("access_token", accesstoken)
        nvc.Add("extension", "csv")


        Dim mywebclient As New WebClient()
        mywebclient.QueryString = nvc
        mywebclient.Headers.Add("Content-Type", "application/x-www-form-urlencoded")
        Dim filecontents As Byte() = System.IO.File.ReadAllBytes(fileaddress)
        Dim responseArray As Byte() = mywebclient.UploadData(uploadUrl, "POST", filecontents)

        Dim response As String = System.Text.Encoding.ASCII.GetString(responseArray)

        Dim stringarray As String() = response.Split(CChar(","))



        Dim SISid As String
        SISid = "0"
        For Each s As String In stringarray
            If (s.Contains("id")) And SISid = "0" Then
                SISid = s.Substring(s.IndexOf(":") + 1, s.Length - s.IndexOf(":") - 1)

            End If

        Next

        Dts.Variables("SISId").Value = SISid

        ' MsgBox(Dts.Variables("SISId").Value.ToString())

        Dts.TaskResult = Dts.Results.Success


	End Sub

