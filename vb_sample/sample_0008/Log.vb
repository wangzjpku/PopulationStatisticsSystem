Imports System.IO
Imports System.Text.UnicodeEncoding
Imports System.Text

Public Class Log
    Private m_filepath As String
    Private Shared m_FD As FileStream
    Private Shared m_uni As New ASCIIEncoding()

    Public Sub New()
        Dim subdir As String = "log"
        Dim ymd As String = DateTime.Now.ToString("yyyyMMdd")

        Try
            MkDir(subdir)
        Catch ex As IOException
        End Try

        m_filepath = subdir & "/APC_LOG_" & ymd & ".log"
        m_FD = File.OpenWrite(m_filepath)
        m_FD.SetLength(0)
    End Sub

    Public Function WriteBytes(ByVal data() As Byte) As Boolean
        m_FD.Write(data, 0, data.Length)
        Return True
    End Function

    Public Function WriteBytes(ByVal data() As Byte, ByVal offset As Integer, ByVal length As Integer) As Boolean
        m_FD.Write(data, offset, length)
        Return True
    End Function

    Public Function Write(ByVal data As String) As Boolean
        WriteBytes(m_uni.GetBytes(data))
        Console.Write(data)
        Return True
    End Function

    Public Function WriteLine(ByVal data As String) As Boolean
        Dim newline = New Byte() {13, 10}

        WriteBytes(m_uni.GetBytes(data))
        WriteBytes(m_uni.GetBytes(vbCrLf))
        Console.WriteLine(data)
        Return True
    End Function
End Class