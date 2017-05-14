Imports System.IO

Public Class Recorder
    Private Shared m_bRecord As Boolean
    Private Shared m_filepath As String
    Private Shared m_FD As FileStream

    Public Shared Function Start(ByVal filepath As String) As Boolean
        m_filepath = filepath
        m_FD = File.OpenWrite(filepath)
        m_bRecord = True
        Return True
    End Function

    Public Shared Function StopForce() As Boolean
        m_bRecord = False
        m_FD.Close()
        Return True
    End Function

    Public Shared Function IsStart() As Boolean
        Return m_bRecord
    End Function

    Public Shared Function Write(ByVal data() As Byte,
                                ByVal idx As Integer,
                                ByVal size As Integer) As Boolean
        If m_bRecord = False Then
            Return False
        End If

        ' write all bytes
        ' My.Computer.FileSystem.WriteAllBytes(m_filepath, data, True)

        For i As Integer = idx To size - 1
            m_FD.WriteByte(data(i))
        Next i

        ' pad 16bytes per line for vim/sublime text 3
        'For i As Integer = 0 To 11
        'm_FD.WriteByte(0)
        'Next i
        Return True
    End Function
End Class