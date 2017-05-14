Imports System.IO
Imports System.Threading

Public Class SampleDataReader
    Private Shared m_bRead As Boolean
    Private Shared m_filepath As String
    Private Shared m_FD As FileStream
    Private Shared m_Count As Integer
    Private m_Form1 As Form1
    Private m_ReadCycle As Integer
    Private m_PausePerSample As Boolean
    Private m_Pause As Boolean

    Public Sub New(ByVal form1 As Form1)
        m_Form1 = form1
        m_bRead = False
        m_ReadCycle = 0
        m_Pause = False
        m_PausePerSample = False
    End Sub

    Public Sub SetPausePerSample(ByVal pausePerSample As Boolean)
        m_PausePerSample = pausePerSample
    End Sub

    Public Sub SamplePause()
        m_Pause = True
    End Sub

    Public Sub SampleResume()
        m_Pause = False
    End Sub

    Public Function IsSamplePause() As Boolean
        Return m_Pause
    End Function

    Public Sub SetReadCycle(ByVal cycle As Integer)
        m_ReadCycle = cycle
    End Sub

    Public Sub Start(ByVal filepath As String)
        Dim data(99) As Byte

        m_filepath = filepath
        m_bRead = True
        m_FD = File.OpenRead(m_filepath)
        m_Count = 0

        Do While m_bRead
            Dim realLen = m_FD.Read(data, 0, data.Length)

            If (realLen = data.Length) Then
                If data(0) = 1 And data(1) = 2 Then
                    Continue Do
                End If

                Thread.Sleep(m_ReadCycle)

                m_Count = m_Count + 1
                'one time 52 bytes, particular sample'
                m_Form1.Invoke(m_Form1.m_DataProcessDelegate, New Object() {data, data.Length, m_Count, True})

                If m_PausePerSample Then
                    ' wait for resume'
                    SamplePause()
                    Do While IsSamplePause()
                        Thread.Sleep(2)
                    Loop
                End If
            Else
                m_bRead = False
            End If
        Loop

        m_FD.Close()
        m_bRead = False

        'FIXME, thread resource collection?
    End Sub 'Run

    Public Function StopForce() As Boolean
        m_bRead = False
        SampleResume()
        Return True
    End Function

    Public Function IsStart() As Boolean
        Return m_bRead
    End Function
End Class