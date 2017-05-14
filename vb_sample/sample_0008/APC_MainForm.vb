Imports System.IO

Public Class APC_MainForm
    Private Sub APC_MainForm_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        Me.CheckedListBox_RecordType.SetItemChecked(0, True)
    End Sub

    Private Sub Label_IN_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label_IN.TextChanged
        'My.Computer.Audio.Play("C:\GetON.wav", AudioPlayMode.Background)
    End Sub

    Private Sub Label_OUT_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles Label_OUT.TextChanged
        'My.Computer.Audio.Play("C:\GetOFF.wav", AudioPlayMode.Background)
    End Sub

    Private Sub CheckedListBox_RecordType_SelectedValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles CheckedListBox_RecordType.SelectedValueChanged
        Dim idx As Integer
        Dim selectIdx = Me.CheckedListBox_RecordType.SelectedIndex

        If Me.CheckedListBox_RecordType.GetItemChecked(selectIdx) Then
            For idx = 0 To Me.CheckedListBox_RecordType.Items.Count - 1
                If idx <> selectIdx Then
                    If Me.CheckedListBox_RecordType.GetItemChecked(selectIdx) Then
                        Me.CheckedListBox_RecordType.SetItemChecked(idx, False)
                    Else
                        Me.CheckedListBox_RecordType.SetItemChecked(idx, True)
                    End If
                End If
            Next
        Else
            Me.CheckedListBox_RecordType.SetItemChecked(selectIdx, True)
        End If
    End Sub

    Private Function GetRecordType() As String
        Dim idx As Integer

        For idx = 0 To Me.CheckedListBox_RecordType.Items.Count - 1
            If Me.CheckedListBox_RecordType.GetItemChecked(idx) Then
                Return Me.CheckedListBox_RecordType.Items(idx)
            End If
        Next
        Return ""
    End Function

    Public Const BUTTON_CLICK As Byte = 0
    Public Const SOFTWARE_AUTO As Byte = 1

    Public Shared Sub RecordToggle(ByVal reason As Byte)
        If Recorder.IsStart() Then
            APC_MainForm.ButtonRecord.Text = "Record Start"
            APC_MainForm.CheckedListBox_RecordType.Enabled = True

            Recorder.StopForce()
        Else
            Dim timestamp As String = DateTime.Now.ToString("yyyyMMdd_HHmmss")
            Dim subdir As String = DateTime.Now.ToString("MMdd")
            Dim type As String = APC_MainForm.GetRecordType()

            APC_MainForm.ButtonRecord.Text = "Record Stop"
            APC_MainForm.CheckedListBox_RecordType.Enabled = False

            If type.Equals("") Then
                MsgBox("Please Choose Record Type!")
                Return
            End If

            Try
                MkDir(subdir)
            Catch ex As IOException
            End Try

            Dim message, title, defaultValue As String
            Dim people_tag As Object

            message = "Enter a %People_%Tag for the test, default is 10_150"
            title = "Input People and Tag"
            defaultValue = "10_150"

            ' user click need to input tag
            If reason = BUTTON_CLICK Then
                people_tag = InputBox(message, title, defaultValue)

                ' If user has clicked Cancel, set myValue to defaultValue 
                If people_tag Is "" Then people_tag = defaultValue
            Else
                people_tag = defaultValue
            End If

            'Send Command Frame to MCU, initialize counter
            Form1.sendCommandFrame()
            'prepare record sample, clear in buffer for calibration of next time use sample file
            Form1.DiscardInBuffer()

            Form2.CountProcessInit()
            Recorder.Start(subdir & "/APC_" & type & "_" & people_tag & "_" & timestamp & ".bin")
        End If
    End Sub

    Private Sub ButtonRecord_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ButtonRecord.Click
        RecordToggle(BUTTON_CLICK)
    End Sub

    Private Sub CounterResetButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CounterResetButton.Click
        Form1.CouterReset()
        Me.Label_IN.Text = "0"
        Me.Label_OUT.Text = "0"
        Form2.iPeopleCnt_In = 0
        Form2.iPeopleCnt_Out = 0
        Form2.iFilterdPIRMax = 0
        Form2.iKeepHeight = 0
    End Sub
End Class