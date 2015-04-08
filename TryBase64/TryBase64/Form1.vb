Imports System.Threading

Public NotInheritable Class Form1

    ' :: 폼 로드
    Private Sub p_Me_Load( _
                    ByVal sender As Object, _
                    ByVal e As EventArgs) Handles MyBase.Load
        '
        Me.Text = "TryBase64 ver 1.04"
        Me.StartPosition = FormStartPosition.Manual
        Me.Location = New Point(100, 100)
        Me.FormBorderStyle = Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        '
        Me.Label2.Text &= " (I wanna be drag and drop files.)"
        '
        Me.TextBox1.BorderStyle = BorderStyle.FixedSingle
        Me.TextBox1.WordWrap = False
        Me.TextBox1.ScrollBars = ScrollBars.Both
        Me.TextBox1.TabIndex = 1

        '
        Me.AllowDrop = True
        AddHandler Me.DragEnter, AddressOf Me.p_Me_DragEnter
        AddHandler Me.DragDrop, AddressOf Me.p_Me_DragDrop
    End Sub

    ' :: 드래그 엔터
    Private Sub p_Me_DragEnter(ByVal sender As Object, ByVal dea As DragEventArgs)
        If (dea.Data.GetDataPresent(DataFormats.FileDrop)) Then
            dea.Effect = DragDropEffects.Copy
        End If
    End Sub

    ' :: 드래그 드롭
    Private Sub p_Me_DragDrop(ByVal sender As Object, ByVal dea As DragEventArgs)
        Dim t_dragObjs As Object = dea.Data.GetData(DataFormats.FileDrop)
        Dim t_filePaths() As String = TryCast(t_dragObjs, String())
        If (t_filePaths.Length = 1) Then
            Dim t_filePath As String = t_filePaths(0)
            Me.OpenFileDialog1.FileName = t_filePath
            Me._TargetFilePath = t_filePath
            Me.TextBox1.Text = Me._TargetFilePath
        End If
    End Sub

    ' :: Clear
    Private Sub p_Button1_Click( _
                    ByVal sender As Object, _
                    ByVal e As EventArgs) Handles Button1.Click
        '
        Me.TextBox1.Clear()
        Me._TargetFilePath = Nothing
    End Sub

    Private _TargetFilePath As String = Nothing
    ' :: Open
    Private Sub p_Button2_Click( _
                    ByVal sender As Object, _
                    ByVal e As EventArgs) Handles Button2.Click
        '
        Dim t_dr As DialogResult = Me.OpenFileDialog1.ShowDialog(Me)
        If (t_dr.Equals(DialogResult.OK)) Then
            Me._TargetFilePath = Me.OpenFileDialog1.FileName
            Me.TextBox1.Text = Me._TargetFilePath
        End If
    End Sub

    ' :: Base64 스트링 저장
    Private Sub p_Button3_Click( _
                    ByVal sender As Object, _
                    ByVal e As EventArgs) Handles Button3.Click
        '
        MBase64Task.fToString(Me._TargetFilePath)
    End Sub

    ' :: Base64 바이너리 저장
    Private Sub p_Button4_Click( _
                    ByVal sender As Object, _
                    ByVal e As EventArgs) Handles Button4.Click
        '
        MBase64Task.fToBinary(Me._TargetFilePath)
    End Sub

End Class


Module MBase64Task

    ' -
    Private _WorkThread As Thread = Nothing
    ' -
    Private _TargetFilePath As String = Nothing

    ' -
    Private Const _EndString_Base64String As String = "_Base64String"
    ' -
    Private Const _EndString_Base64Binary As String = "_Base64Binary"


    ' ::
    Public Sub fToString(ByVal TargetFilePath As String)
        If (TargetFilePath Is Nothing) Then Exit Sub

        If (Not My.Computer.FileSystem.FileExists(TargetFilePath)) Then Exit Sub

        If (_WorkThread Is Nothing) Then
            _TargetFilePath = TargetFilePath
            _WorkThread = New Thread(AddressOf p_ToString)
            _WorkThread.IsBackground = True
            _WorkThread.Start()
        End If
    End Sub
    ' ::
    Private Sub p_ToString()
        Dim t_SaveFilePath As String = Nothing

        If (My.Computer.FileSystem.FileExists(_TargetFilePath)) Then
            Try
                t_SaveFilePath = _TargetFilePath & _EndString_Base64String
                Dim t_Bytes() As Byte = My.Computer.FileSystem.ReadAllBytes(_TargetFilePath)
                Dim t_SaveStr As String = Convert.ToBase64String(t_Bytes, 0, t_Bytes.Length, Base64FormattingOptions.InsertLineBreaks)
                My.Computer.FileSystem.WriteAllText(t_SaveFilePath, t_SaveStr, False, System.Text.Encoding.UTF8)
            Catch
                t_SaveFilePath = Nothing
            End Try
        End If
        _WorkThread = Nothing
        _TargetFilePath = Nothing

        If (Not t_SaveFilePath Is Nothing) Then
            MessageBox.Show("Completed As File: " & vbCrLf & t_SaveFilePath, "Notifications")
        End If
    End Sub


    ' ::
    Public Sub fToBinary(ByVal TargetFilePath As String)
        If (TargetFilePath Is Nothing) Then Exit Sub

        If (Not My.Computer.FileSystem.FileExists(TargetFilePath)) Then Exit Sub

        If (_WorkThread Is Nothing) Then
            _TargetFilePath = TargetFilePath
            _WorkThread = New Thread(AddressOf p_ToBinary)
            _WorkThread.IsBackground = True
            _WorkThread.Start()
        End If
    End Sub
    ' ::
    Private Sub p_ToBinary()
        Dim t_SaveFilePath As String = Nothing

        If (My.Computer.FileSystem.FileExists(_TargetFilePath)) Then
            Try
                t_SaveFilePath = _TargetFilePath & _EndString_Base64Binary
                Dim t_Str As String = My.Computer.FileSystem.ReadAllText(_TargetFilePath)
                Dim t_SaveBytes() As Byte = Convert.FromBase64String(t_Str)
                My.Computer.FileSystem.WriteAllBytes(t_SaveFilePath, t_SaveBytes, False)
            Catch
                t_SaveFilePath = Nothing
            End Try
        End If
        _WorkThread = Nothing
        _TargetFilePath = Nothing

        If (Not t_SaveFilePath Is Nothing) Then
            MessageBox.Show("Completed As File: " & vbCrLf & t_SaveFilePath, "Notifications")
        End If
    End Sub

End Module

