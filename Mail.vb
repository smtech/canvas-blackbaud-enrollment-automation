Public Sub Main()
        ' define the local key and vector byte arrays
        Dim key() As Byte = _
          {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, _
          15, 16, 17, 18, 19, 20, 21, 22, 23, 24}

        Dim iv() As Byte = {8, 7, 6, 5, 4, 3, 2, 1}

        ' instantiate the class with the arrays

        Dim des As New cTripleDES(key, iv)
        '***********************************************************************************
        'Use this section to encryt your passwords initially.

        'Dim newEncryptedData As String = des.Encrypt("")
        'Dim newEncryptedData2 As String = des.Encrypt("")
        'MsgBox("should be user: " + newEncryptedData)
        'MsgBox("should be pwd: " + newEncryptedData2)
        'Dim strFile As String
        ' Dim strText As String
        'strFile = "C:\VisualStudio2005\Canvas\AddDrop\test.txt"
        'strText = "test"
        'File.AppendAllText(strFile, "user: " + newEncryptedData)
        'File.AppendAllText(strFile, "PWD: " + newEncryptedData2)
        '***********************************************************************************

        ' for the example, define a variable with the encrypted value
        'Dim encryptedData As String = "++XIiGymvbg="
        Dim UserPwd As String = CType(Dts.Variables("EncryptPwd").Value, String)
        Dim UserName As String = CType(Dts.Variables("EncryptUser").Value, String)
        'MsgBox("here: " + encryptedData)

        ' now, decrypt the data
        Dim pwd As String = des.Decrypt(UserPwd)
        'MsgBox("should be : " + pwd)

        Dim user As String = des.Decrypt(UserName)
        'MsgBox("should be : " + user)

       

        'This section sends mail
        Dim mail As MailMessage = New MailMessage()
        'mail.To.Add("")
        ' mail.To.Add("")
        mail.To.Add("carloscollazo@stmarksschool.org")
       
       
        mail.From = New MailAddress(user)
        mail.Subject = "Add/Drop Monitoring"
        mail.Attachments.Add(New Attachment("E:\Data\AddDrops\AddDrops.csv"))

        Dim Body As String
        mail.Body = "Attached are the Add/Drops Import Stats for today: " + vbNewLine + Dts.Variables("StatusMessage").Value.ToString()

        mail.IsBodyHtml = True
        Dim smtp As SmtpClient = New SmtpClient()
        smtp.Host = "smtp.gmail.com"
        smtp.Credentials = New System.Net.NetworkCredential(user, pwd)
        smtp.EnableSsl = True
        smtp.Port = 587
        smtp.Send(mail)
        Dts.TaskResult = Dts.Results.Success
    End Sub

End Class
