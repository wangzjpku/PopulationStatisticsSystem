Imports System.IO
Imports System.Text.UnicodeEncoding
Imports System.Text

Public Class Frame
    Public Const SampleFrame_55_StartCode_0 As Byte = &H55
    Public Const SampleFrame_55_StartCode_1 As Byte = &H55
    Public Const SampleFrame_104_StartCode_0 As Byte = &H68
    Public Const SampleFrame_104_StartCode_1 As Byte = &H68
    Public Const SampleFrameAck_StartCode_0 As Byte = &H68
    Public Const SampleFrameAck_StartCode_1 As Byte = &H69
    Public Const CommandFrame_StartCode_0 As Byte = &H7A
    Public Const CommandFrame_StartCode_1 As Byte = &HB7
    Public Const PeopleFrame_StartCode_0 As Byte = &H6A
    Public Const PeopleFrame_StartCode_1 As Byte = &H2E
    Public Const LogFrame_StartCode_0 As Byte = &H3F
    Public Const LogFrame_StartCode_1 As Byte = &H34

    Public Const HandshakeFrame_StartCode_0 As Byte = &H08
    Public Const HandshakeFrame_StartCode_1 As Byte = &H15

    Public Const EndCode As Byte = &H0

    Public Sub New()
    End Sub

    Public Shared Function isSampleFrame55(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = SampleFrame_55_StartCode_0 And startcode_1 = SampleFrame_55_StartCode_1
    End Function

    Public Shared Function isSampleFrame104(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = SampleFrame_104_StartCode_0 And startcode_1 = SampleFrame_104_StartCode_1
    End Function

    Public Shared Function isSampleAckFrame(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = SampleFrameAck_StartCode_0 And startcode_1 = SampleFrameAck_StartCode_1
    End Function

    Public Shared Function isCommandFrame(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = CommandFrame_StartCode_0 And startcode_1 = CommandFrame_StartCode_1
    End Function

    Public Shared Function isPeopleFrame(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = PeopleFrame_StartCode_0 And startcode_1 = PeopleFrame_StartCode_1
    End Function

    Public Shared Function isLogFrame(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = LogFrame_StartCode_0 And startcode_1 = LogFrame_StartCode_1
    End Function

    Public Shared Function isHandshakeFrame(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Boolean
        Return startcode_0 = HandshakeFrame_StartCode_0 And startcode_1 = HandshakeFrame_StartCode_1
    End Function
End Class