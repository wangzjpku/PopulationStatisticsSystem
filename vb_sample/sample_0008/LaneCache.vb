Imports System.IO

Public Class LaneCache
    Private m_rows As Byte
    Private m_cols As Byte
    Private m_matrix(4) As Byte
    Private m_length As Byte

    Public Sub New(ByVal length As Byte)
        m_length = length
        ReDim m_matrix(m_length)
    End Sub

    public Sub Set(ByVal row As Byte, ByVal col As Byte, ByVal state As Byte)
        m_matrix(row, col) = state
    End Sub

    public Function Get(ByVal row As Byte, ByVal col As Byte, ByVal state As Byte) as Byte
        return m_matrix(row, col)
    End Function

    

    Public Sub Start(ByVal filepath As String)
        Dim data(51) As Byte

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

                Thread.Sleep(1)

                m_Count = m_Count + 1
                'one time 52 bytes, particular sample'
                m_Form1.Invoke(m_Form1.m_DataProcessDelegate, New Object() {data, m_Count})
            Else
                m_bRead = False
            End If
        Loop

        m_FD.Close()
        m_bRead = False

        'FIXME, thread resource collection?
    End Sub 'Run
End Class