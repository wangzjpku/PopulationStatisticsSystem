'****************************************************************************
'*	Copylight(C) 2011 Kanazawa-soft-design,LLC.All Rights Reserved.
'****************************************************************************
'*
'*	@file	Form1.vb
'*
'*	@brief	シリアルポート通信プログラム.
'*
'*	@author	金澤 宣明
'*
Imports System
Imports System.IO.Ports
Imports System.Text
Imports System.Threading

Public Class Form1
    ' Ito add
    Public data(259) As Byte
    Shared PeopleIn, PeopleInOld As Integer
    Shared PeopleOut, PeopleOutOld As Integer

    ' increment by Board's counting result
    Shared PeopleIn_Board, PeopleOut_Board As Integer
    Shared MaxPIR As Integer
    Shared MaxHeight As Integer

    Shared m_SerialPortDataReceivedCount As Integer

    Const PEOPLE_RESULT_SOURCE_VB As Byte = 0
    Const PEOPLE_RESULT_SOURCE_MCU As Byte = 1

    Public Delegate Sub DataProcessDelegate(ByVal data() As Byte, ByVal length As Integer, ByVal count As Integer, ByVal fromFile As Boolean)
    Public Delegate Sub DataProcessEndDelegate()

    Public Delegate Sub ResultProcessDelegate(ByVal People() As Integer, ByVal count As Integer, ByVal source As Byte)
    Public Delegate Sub RefreshUIDelegate()

    Public m_DataProcessDelegate As DataProcessDelegate
    Public m_ResultProcessDelegate As ResultProcessDelegate

    Private m_FilePath As String
    Private m_SampleReader = New SampleDataReader(Me)
    Private Shared m_LastDataTime As UInt32
    Public Shared m_Log = New Log()
    Private m_ExecuteFileRunning As Boolean = False
    Public Shared m_AsciiEncoder As New ASCIIEncoding()

    Public Shared m_MCUNumber As Integer = 0
    Public Shared m_MCUMode As Integer = 0
    Public Shared m_T1 As Double
    Public Shared m_T2 As Double
    Public Shared m_VBCounting As Boolean

    Public m_RefreshUIDelegate As RefreshUIDelegate

    '****************************************************************************'
    '*
    '*	@brief	ボーレート格納用のクラス定義.
    '*
    Private Class BuadRateItem
        Inherits Object

        Private m_name As String = ""
        Private m_value As Integer = 0

        '表示名称
        Public Property NAME() As String
            Set(ByVal value As String)
                m_name = value
            End Set
            Get
                Return m_name
            End Get
        End Property

        'ボーレート設定値.
        Public Property BAUDRATE() As Integer
            Set(ByVal value As Integer)
                m_value = value
            End Set
            Get
                Return m_value
            End Get
        End Property

        'コンボボックス表示用の文字列取得関数.
        Public Overrides Function ToString() As String
            Return m_name
        End Function

    End Class

    '****************************************************************************'
    '*
    '*	@brief	制御プロトコル格納用のクラス定義.
    '*
    Private Class HandShakeItem
        Inherits Object

        Private m_name As String = ""
        Private m_value As Handshake = Handshake.None

        '表示名称
        Public Property NAME() As String
            Set(ByVal value As String)
                m_name = value
            End Set
            Get
                Return m_name
            End Get
        End Property

        '制御プロトコル設定値.
        Public Property HANDSHAKE() As Handshake
            Set(ByVal value As Handshake)
                m_value = value
            End Set
            Get
                Return m_value
            End Get
        End Property

        'コンボボックス表示用の文字列取得関数.
        Public Overrides Function ToString() As String
            Return m_name
        End Function

    End Class

    Private Delegate Sub Delegate_RcvDataToTextBox(ByVal data As String)

    '****************************************************************************'
    '*
    '*	@brief	ダイアログの初期処理.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        '利用可能なシリアルポート名の配列を取得する.
        Dim PortList As String()
        PortList = SerialPort.GetPortNames()

        cmbPortName.Items.Clear()

        'シリアルポート名をコンボボックスにセットする.
        Dim PortName As String
        For Each PortName In PortList
            cmbPortName.Items.Add(PortName)
        Next PortName

        If cmbPortName.Items.Count > 0 Then
            cmbPortName.SelectedIndex = cmbPortName.Items.Count - 1
        End If

        cmbBaudRate.Items.Clear()

        'ボーレート選択コンボボックスに選択項目をセットする.
        Dim baud As BuadRateItem
        baud = New BuadRateItem
        baud.NAME = "4800bps"
        baud.BAUDRATE = 4800
        cmbBaudRate.Items.Add(baud)

        baud = New BuadRateItem
        baud.NAME = "9600bps"
        baud.BAUDRATE = 9600
        cmbBaudRate.Items.Add(baud)

        baud = New BuadRateItem
        baud.NAME = "19200bps"
        baud.BAUDRATE = 19200
        cmbBaudRate.Items.Add(baud)

        baud = New BuadRateItem
        baud.NAME = "115200bps"
        baud.BAUDRATE = 115200
        cmbBaudRate.Items.Add(baud)
        cmbBaudRate.SelectedIndex = 3

        cmbHandShake.Items.Clear()

        'フロー制御選択コンボボックスに選択項目をセットする.
        Dim ctrl As HandShakeItem
        ctrl = New HandShakeItem
        ctrl.NAME = "なし"
        ctrl.HANDSHAKE = Handshake.None
        cmbHandShake.Items.Add(ctrl)

        ctrl = New HandShakeItem
        ctrl.NAME = "XON/XOFF制御"
        ctrl.HANDSHAKE = Handshake.XOnXOff
        cmbHandShake.Items.Add(ctrl)

        ctrl = New HandShakeItem
        ctrl.NAME = "RTS/CTS制御"
        ctrl.HANDSHAKE = Handshake.RequestToSend
        cmbHandShake.Items.Add(ctrl)

        ctrl = New HandShakeItem
        ctrl.NAME = "XON/XOFF + RTS/CTS制御"
        ctrl.HANDSHAKE = Handshake.RequestToSendXOnXOff
        cmbHandShake.Items.Add(ctrl)
        cmbHandShake.SelectedIndex = 0

        '受信用のテキストボックスをクリアする.
        RcvTextBox.Clear()

        ' Ito add
        'Label1.CheckForIllegalCrossThreadCalls = False
        'Label2.CheckForIllegalCrossThreadCalls = False
        'Label3.CheckForIllegalCrossThreadCalls = False
        PeopleIn = 0
        PeopleInOld = 0
        PeopleOut = 0
        PeopleOutOld = 0

        ' load configuration data from text file
        If IO.File.Exists("C:\temp\APCconfiguration.ini") Then
            Dim Reader As New IO.StreamReader("C:\temp\APCconfiguration.ini")
            TextBox_Thresh1.Text = Reader.ReadLine
            TextBox_Thresh2.Text = Reader.ReadLine
            Reader.Close()
            If TextBox_Thresh1.Text <= 20 OrElse TextBox_Thresh1.Text > 30 Then
                MsgBox("Please Confirm T1 Value")
            ElseIf TextBox_Thresh2.Text <= 20 OrElse TextBox_Thresh2.Text > 30 Then
                MsgBox("Please Confirm T2 Value")
            End If
        End If

        'File Choose Setting
        'AddHandler FileButton.Click, AddressOf FileButton_Click
        m_DataProcessDelegate = New DataProcessDelegate(AddressOf DataProcess)
        m_ResultProcessDelegate = New ResultProcessDelegate(AddressOf ResultProcess)
        m_RefreshUIDelegate = New RefreshUIDelegate(AddressOf RefreshUI)

        ' command line
        ParseCommandLine()
    End Sub

    '****************************************************************************'
    '*
    '*	@brief	ダイアログの終了処理.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        'シリアルポートをオープンしている場合、クローズする.
        If SerialPort1.IsOpen Then
            SerialPort1.Close()
        End If

        ' save configuration data to text file
        Dim Writer As New IO.StreamWriter("C:\temp\APCconfiguration.ini")
        Writer.WriteLine(TextBox_Thresh1.Text)
        Writer.WriteLine(TextBox_Thresh2.Text)
        Writer.Close()
    End Sub

    '****************************************************************************'
    '*
    '*	@brief	[終了]ボタンを押したときの処理.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click

        'ダイアログをクローズする.
        Me.Close()

    End Sub

    '****************************************************************************'
    '*
    '*	@brief	[接続]/[切断]ボタンを押したときにシリアルポートのオープン/クローズを行う.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Private Sub ConnectButton_Click(ByVal sender As System.Object,
                                    ByVal e As System.EventArgs) Handles ConnectButton.Click
        If SerialPort1.IsOpen = True Then
            'シリアルポートをクローズする.
            SerialPort1.Close()

            'ボタンの表示を[切断]から[接続]に変える.
            ConnectButton.Text = "CONN"
        Else
            'オープンするシリアルポートをコンボボックスから取り出す.
            SerialPort1.PortName = cmbPortName.SelectedItem.ToString()

            'ボーレートをコンボボックスから取り出す.
            Dim baud As BuadRateItem
            baud = cmbBaudRate.SelectedItem
            SerialPort1.BaudRate = baud.BAUDRATE

            'データビットをセットする. (データビット = 8ビット)
            SerialPort1.DataBits = 8

            'パリティビットをセットする. (パリティビット = なし)
            SerialPort1.Parity = Parity.None

            'ストップビットをセットする. (ストップビット = 1ビット)
            SerialPort1.StopBits = StopBits.One

            'フロー制御をコンボボックスから取り出す.
            Dim ctrl As HandShakeItem
            ctrl = cmbHandShake.SelectedItem
            SerialPort1.Handshake = ctrl.HANDSHAKE

            '文字コードをセットする.
            SerialPort1.Encoding = Encoding.ASCII
            'SerialPort1.ReadTimeout = 1000
            'SerialPort1.ReadBufferSize = 20 * 1024

            ' Ito add
            ' Init Counting part 
            Form2.CountProcessInit()

            Try
                'シリアルポートをオープンする.
                SerialPort1.Open()

                ' Send Handshake Frame
                sendHandShakeFrame()
                'ボタンの表示を[接続]から[切断]に変える.
                ConnectButton.Text = "DISC"

                If VBCountingCheckBox.Checked Then
                    sendCommandFrame()
                End If
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try

            ' Ito add
            'Do While PeopleIn <> PeopleInOld OrElse PeopleOut <> PeopleOutOld
            '
            'If PeopleIn <> PeopleInOld OrElse PeopleOut <> PeopleOutOld Then
            'Label2.Text = "IN:" & Str(PeopleIn)
            'Label2.Text = "OUT:" & Str(PeopleOut)
            'PeopleInOld = PeopleIn
            'PeopleOutOld = PeopleOut
            'End If
            '
            '
            'Loop

            'yatunami add

            APC_MainForm.Show()
        End If
    End Sub

    '****************************************************************************'
    '*
    '*	@brief	データ受信が発生したときのイベント処理.
    '*
    '*	@param	[in]	sender	イベントの送信元のオブジェクト.
    '*	@param	[in]	e		イベント情報.
    '*
    '*	@retval	なし.
    '*
    Public Delegate Sub ItoDelegate()

    Public Shared Sub CouterReset()
        Dim People(3) As Integer

        PeopleIn = 0
        PeopleOut = 0
        MaxPIR = 0
        MaxHeight = 0

        People(0) = PeopleIn
        People(1) = PeopleOut
        People(2) = MaxPIR
        People(3) = MaxHeight

        PeopleIn_Board = 0
        PeopleOut_Board = 0
        ResultProcess(People, 0, PEOPLE_RESULT_SOURCE_VB)
    End Sub

    Private Shared Sub ResultProcess(ByVal People() As Integer, ByVal count As Integer, ByVal source As Byte)
        Dim logStr As String
        Dim heightStr As String

        APC_MainForm.Label_IN.Text = People(0)
        APC_MainForm.Label_OUT.Text = People(1)
        If PeopleOut <> People(1) Then
            My.Computer.Audio.Play("C:\GetOFF.wav", AudioPlayMode.Background)
        ElseIf PeopleIn <> People(0) Then
            My.Computer.Audio.Play("C:\GetON.wav", AudioPlayMode.Background)
        End If

        If source = PEOPLE_RESULT_SOURCE_VB Then
            logStr = DateTime.Now.ToString() & ">People,Sample=" & count
            heightStr = ",height=" & People(3)

            If PeopleOut <> People(1) Then
                Form1.m_Log.WriteLine(logStr & ",OUT=" & People(1) & heightStr)
            End If

            If PeopleIn <> People(0) Then
                Form1.m_Log.WriteLine(logStr & ",IN=" & People(0) & heightStr)
            End If
        End If

        PeopleIn = People(0)
        PeopleOut = People(1)
        MaxPIR = People(2)
        MaxHeight = People(3)

        If Form1.Label2.IsHandleCreated Then
            Form1.Label2.Invoke(New ItoDelegate(AddressOf Form1.Label2_update), New Object() {})
        Else
            Form1.Label2_update()
        End If

        If Form1.Label3.IsHandleCreated Then
            Form1.Label3.Invoke(New ItoDelegate(AddressOf Form1.Label3_update), New Object() {})
        Else
            Form1.Label3_update()
        End If

        If Form1.Label4.IsHandleCreated Then
            Form1.Label4.Invoke(New ItoDelegate(AddressOf Form1.Label4_update), New Object() {})
        Else
            Form1.Label4_update()
        End If

        If Form1.Label5.IsHandleCreated Then
            Form1.Label5.Invoke(New ItoDelegate(AddressOf Form1.Label5_update), New Object() {})
        Else
            Form1.Label5_update()
        End If

        If Form1.LabelPIRDATA.IsHandleCreated Then
            Form1.LabelPIRDATA.Invoke(New ItoDelegate(AddressOf Form1.ShowPIRdata), New Object() {})
        Else
            Form1.ShowPIRdata()
        End If
    End Sub

    Private m_LastSampleTimestamp As Double = 0.0F

    ' data process
    Public Shared Sub DataProcess(ByVal data() As Byte,
                                    ByVal length As Integer,
                                    ByVal count As Integer,
                                    ByVal fromFile As Boolean)
        '受信データを読み込む.
        'Dim data(54) As Byte
        Dim i As Integer
        '受信したデータをテキストボックスに書き込む.
        Dim MeasuredData As String = ""

        ' pir and ultrasonic
        For i = 0 To 55
            If data(i) <= 15 Then
                MeasuredData = MeasuredData & "0" & Hex(data(i))
            Else
                MeasuredData = MeasuredData & Hex(data(i))
            End If
            If i = 15 Or i = 31 Or i = 47 Then
                MeasuredData = MeasuredData & "-"
            Else
                MeasuredData = MeasuredData & " "
            End If
        Next

        ' timestamp
        MeasuredData = MeasuredData & "-" & " "
        For i = 96 To 99
            If data(i) <= 15 Then
                MeasuredData = MeasuredData & "0"
            End if
            MeasuredData = MeasuredData & Hex(data(i)) & " "
        Next

        If Form1.IsHandleCreated Then
            Form1.BeginInvoke(New Delegate_RcvDataToTextBox(AddressOf Form1.RcvDataToTextBox), MeasuredData & vbCrLf)
        Else
            Form1.RcvDataToTextBox(MeasuredData & vbCrLf)
        End If

        ' serial data timestamp
        If length = 52 Then
            ' [48,51]
            m_LastDataTime = BitConverter.ToUInt32(data, 48)
        Else ' [96,99]
            m_LastDataTime = BitConverter.ToUInt32(data, 96)
        End If

        Dim sampleTimestamp As Double = (DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds
        ' Console.WriteLine(sampleTimestamp - Form1.m_LastSampleTimestamp & ">" & m_LastDataTime & ">sample")
        Form1.m_LastSampleTimestamp = sampleTimestamp

        ' MCU Counting, send sample to MCU
        If Form1.VBCountingCheckBox.Checked = False Then
            If fromFile And Form1.FileCheckBox.Checked Then
                'send next sample when MCU process end'
                Form1.m_SampleReader.SetPausePerSample(True)
                If (Form1.sendSampleFrame(data, length, count) = False) Then
                    Console.WriteLine("err>cannot send sample to MCU")
                Else
                    'Console.WriteLine((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds & ">send sample to MCU")
                End If
                Return
            End If
        End If

        ' VB counting, set by UI or command line
        If Form1.VBCountingCheckBox.Checked OrElse Form1.m_ExecuteFileRunning Then
            ' Ito add 
            Dim People(3) As Integer
            People = Form2.ExeCountProcess(data, count)
            ResultProcess(People, count, PEOPLE_RESULT_SOURCE_VB)
        End If

        ' record sample
        If length = 52 Then
            Recorder.Write(data, 0, length)
        ElseIf length = 100 Then
            Recorder.Write(data, 0, length)
        End If
    End Sub

    Sub Label2_update()
        Label2.Text = "IN:" & Str(PeopleIn)
        APC_MainForm.Label_IN.Text = PeopleIn

        Dim TotalLoad As Integer
        If PeopleIn > PeopleOut Then
            TotalLoad = PeopleIn - PeopleOut
        Else
            TotalLoad = 0
        End If
        APC_MainForm.Label_ADULT.Text = TotalLoad
    End Sub

    Sub Label3_update()
        Label3.Text = "OUT:" & Str(PeopleOut)
        APC_MainForm.Label_OUT.Text = PeopleOut
    End Sub

    Sub Label4_update()

        Dim PIRbar As String

        If MaxPIR < 20 Then
            PIRbar = "PIR:+"
        ElseIf MaxPIR < 25 Then
            PIRbar = "PIR:+++"
        ElseIf MaxPIR < 30 Then
            PIRbar = "PIR:++++++"
        ElseIf MaxPIR < 40 Then
            PIRbar = "PIR:+++++++++"
        Else
            PIRbar = "PIR:++++++++++++"
        End If
        Label4.Text = PIRbar
        'Label4.Text = "PIR:" & Str(MaxPIR)
    End Sub

    Sub Label5_update()
        Label5.Text = "H:" & Str(MaxHeight)
        ' avoid height is cleared
        'If MaxHeight >= 140 Then
        APC_MainForm.Label_Height.Text = MaxHeight
        'End If
    End Sub

    Sub ShowPIRdata()
        Dim SenNum As Byte
        Dim iValue As Integer
        Dim CnvVal(8 * 2 - 1) As Double
        For SenNum = 0 To 8 - 1
            iValue = data(SenNum * 2) + data((SenNum * 2) + 1) * 256
            CnvVal(SenNum) = 80 * (5 * iValue / 1024 - 1) - 18
            CnvVal(SenNum) = CnvVal(SenNum) - Form2.GetMeanValue(SenNum) + 20
        Next
        For SenNum = 8 * 2 To 8 * 3 - 1
            iValue = data(SenNum * 2) + data((SenNum * 2) + 1) * 256
            If iValue = 0 Then
                CnvVal(SenNum - 8) = 0
            Else
                CnvVal(SenNum - 8) = 210 - iValue
            End If
        Next
        LabelPIRDATA.Text = "PIR" & Str(Math.Floor(CnvVal(0) * 10)) & " " & Str(Math.Floor(CnvVal(1) * 10)) & " " & Str(Math.Floor(CnvVal(2) * 10)) & " " & Str(Math.Floor(CnvVal(3) * 10)) & " " & Str(Math.Floor(CnvVal(4) * 10)) & " " & Str(Math.Floor(CnvVal(5) * 10)) & " " & Str(Math.Floor(CnvVal(6) * 10)) & " " & Str(Math.Floor(CnvVal(7) * 10)) & " : " & Str(CnvVal(8)) & " " & Str(CnvVal(9)) & " " & Str(CnvVal(10)) & " " & Str(CnvVal(11)) & " " & Str(CnvVal(12)) & " " & Str(CnvVal(13)) & " " & Str(CnvVal(14)) & " " & Str(CnvVal(15))
    End Sub

    '****************************************************************************'
    '*
    '*	@brief	受信データをテキストボックスに書き込む.
    '*
    '*	@param	[in]	data	受信した文字列.
    '*
    '*	@retval	なし.
    '*
    Private Sub RcvDataToTextBox(ByVal data As String)
        '受信データをテキストボックスの最後に追記する.
        If IsNothing(data) = False Then
            RcvTextBox.AppendText(data)
        End If
    End Sub

    Private Sub UI_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        APC_MainForm.Show()
    End Sub

    Private Sub Button_IN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        APC_MainForm.Label_IN.Text = Val(APC_MainForm.Label_IN.Text) + 1
    End Sub

    Private Sub Button_OUT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        APC_MainForm.Label_OUT.Text = Val(APC_MainForm.Label_OUT.Text) + 1
    End Sub

    Private Sub Button_ADULT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        APC_MainForm.Label_ADULT.Text = Val(APC_MainForm.Label_ADULT.Text) + 1
    End Sub

    Private Sub Button_CHILD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs)
        APC_MainForm.Label_CHILD.Text = Val(APC_MainForm.Label_CHILD.Text) + 1
    End Sub

    Public Sub Calibration_Started()
        Me.Label1.Text = "Calibrating"
    End Sub

    Public Sub Calibration_Ended()
        Me.Label1.Text = "Counting"
    End Sub

    Private Sub ResetButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles ResetButton.Click
        Form2.SetCounterReset()
    End Sub

    Private Sub CalibButton_Click(ByVal sender As Object, ByVal e As EventArgs) Handles CalibButton.Click
        Dim MeanValue(7) As Double
        Dim Mean As String = "CAL"

        For SenNum As Byte = 0 To 8 - 1
            MeanValue(SenNum) = Form2.GetMeanValue(SenNum)
            'Mean = Mean & Str(Math.Floor(MeanValue(SenNum) * 10)) & " "
            Mean = Mean & Str(Math.Round(MeanValue(SenNum), 8)) & " "
        Next
        Label8.Text = Mean
    End Sub

    Private Sub SampleDataReader_End()
        Me.FileButton.Text = "FILE"
        Me.ConnectButton.Enabled = True
        If m_ExecuteFileRunning Then
            Me.Close()
        End If
    End Sub

    Private Sub ParseCommandLine()
        Dim filepath As String = ""
        Dim dirpath As String = ""

        For Each argument As String In My.Application.CommandLineArgs
            If argument.ToLower.StartsWith("-f") Then
                filepath = argument.Remove(0, 2)
                'MsgBox(filepath)
            ElseIf argument.ToLower.StartsWith("-d") Then
                dirpath = argument.Remove(0, 2)
            End If
        Next

        If filepath <> "" Then
            ExecuteFile(filepath, False)

            m_ExecuteFileRunning = True
        ElseIf dirpath <> "" Then
            m_ExecuteFileRunning = True
        End If
    End Sub

    Private Sub ExecuteFile(ByVal filepath As String, ByVal reDrawUI As Boolean)
        Dim myThread As Thread

        m_LastDataTime = 0
        PeopleIn = 0
        PeopleInOld = 0
        PeopleOut = 0
        PeopleOutOld = 0
        Form2.CountProcessInit()

        If reDrawUI Then
            APC_MainForm.Show()
            Me.Text = "APC SerialMonitor(DEMO)" & "-" & filepath
            FileButton.Text = "STOP"
            ConnectButton.Enabled = False
        End If

        m_FilePath = filepath

        myThread = New Thread(New ThreadStart(AddressOf ThreadFunction))
        myThread.IsBackground = True
        myThread.Start()
    End Sub

    Private Sub FileButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileButton.Click
        If m_SampleReader.IsStart() Then
            FileButton.Text = "FILE"
            ConnectButton.Enabled = True
            m_SampleReader.StopForce()
        Else
            Dim fd As OpenFileDialog = New OpenFileDialog()

            m_SampleReader.SetReadCycle(0)
            fd.Title = "Please Choose APC Data File"
            fd.InitialDirectory = "."
            fd.Filter = "Bin files (*.bin)|*.bin|All files (*.*)|*.*"
            fd.FilterIndex = 0
            fd.RestoreDirectory = True
            If fd.ShowDialog() = DialogResult.OK Then
                ExecuteFile(fd.FileName, True)
            End If
        End If
    End Sub

    Private Sub ThreadFunction()
        m_SampleReader.Start(m_FilePath)
        Me.Invoke(New DataProcessEndDelegate(AddressOf Me.SampleDataReader_End))
    End Sub

    Private Function sendHandShakeFrame() As Boolean
        If SerialPort1.IsOpen Then
            Dim buffer(8) As Byte

            'handshake frame
            buffer(0) = Frame.HandshakeFrame_StartCode_0
            buffer(1) = Frame.HandshakeFrame_StartCode_1
            buffer(2) = 1
            buffer(3) = Frame.EndCode
            buffer(4) = Frame.EndCode
            SerialPort1.Write(buffer, 0, 5)
            Return True
        Else
            MsgBox("Please choose serial!")
        End If
        Return False
    End Function

    Private Function sendSampleFrame(ByVal data() As Byte, ByVal length As Integer, ByVal count As Integer) As Boolean
        If SerialPort1.IsOpen Then
            Dim buffer(8) As Byte

            'sample frame
            buffer(0) = Frame.SampleFrame_104_StartCode_0
            buffer(1) = Frame.SampleFrame_104_StartCode_1
            buffer(2) = length
            SerialPort1.Write(buffer, 0, 3)
            SerialPort1.Write(data, 0, length)
            buffer(0) = Frame.EndCode
            SerialPort1.Write(buffer, 0, 1)
            Return True
        Else
            MsgBox("Please choose serial!")
        End If
        Return False
    End Function

    Public Shared Sub DiscardInBuffer()
        Form1.SerialPort1.DiscardInBuffer()
    End Sub

    Public Shared Function sendCommandFrame() As Boolean
        If Form1.SerialPort1.IsOpen Then
            Dim command As String = ""
            Dim buffer(62) As Byte

            ' MCU Number
            If Form1.OneMCUCheckbox.Checked Then
                command = command & "number=0,"
            ElseIf Form1.TwoMCUCheckBox.Checked Then
                command = command & "number=1,"
            ElseIf Form1.ThreeMCUCheckBox.Checked Then
                command = command & "number=2,"
            End If

            ' MCU Mode
            If Form1.LiveCheckbox.Checked Then
                command = command & "mode=0,"
            ElseIf Form1.FileCheckBox.Checked Then
                command = command & "mode=1,"
            ElseIf Form1.CollectCheckbox.Checked Then
                command = command & "mode=2,"
            End If

            ' Threshold
            Dim t1_Integer As Integer = Format(Convert.ToDouble(Form1.TextBox_Thresh1.Text), "0.00") * 100
            Dim t2_Integer As Integer = Format(Convert.ToDouble(Form1.TextBox_Thresh2.Text), "0.00") * 100

            command = command & "t1=" & t1_Integer & ",t2=" & t2_Integer
            'command frame
            buffer(0) = Frame.CommandFrame_StartCode_0
            buffer(1) = Frame.CommandFrame_StartCode_1
            buffer(2) = command.Length
            Form1.SerialPort1.Write(buffer, 0, 3)

            buffer = m_AsciiEncoder.GetBytes(command)
            Form1.SerialPort1.Write(buffer, 0, buffer.Length)
            buffer(0) = Frame.EndCode
            Form1.SerialPort1.Write(buffer, 0, 1)
            Return True
        Else
            MsgBox("Please choose serial!")
        End If
        Return False
    End Function

    Private Sub SendCommandButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SendCommandButton.Click
        sendCommandFrame()
    End Sub

    Const SERIAL_READ_STATE_LIVE As Byte = 0
    Const SERIAL_READ_STATE_WAIT_SAMPLE_55 As Byte = 1
    Const SERIAL_READ_STATE_WAIT_SAMPLE_104 As Byte = 2
    Const SERIAL_READ_STATE_WAIT_PEOPLE As Byte = 3
    Const SERIAL_READ_STATE_WAIT_LOG As Byte = 4
    Const SERIAL_READ_STATE_WAIT_HANDSHAKE As Byte = 5
    Const SERIAL_READ_STATE_WAIT_SAMPLE_ACK As Byte = 6

    Private m_SerialReadState As Byte
    Private m_FrameLength As Integer
    Private m_FrameHead(1) As Byte
    Private m_FrameHead_ValidLength As Integer

    Public Shared Sub RefreshUI()
        ' MCU Number
        Form1.OneMCUCheckbox.Checked = m_MCUNumber = 0
        Form1.TwoMCUCheckBox.Checked = m_MCUNumber = 1
        Form1.ThreeMCUCheckBox.Checked = m_MCUNumber = 2
        ' MCU Mode
        Form1.LiveCheckbox.Checked = m_MCUMode = 0
        Form1.FileCheckBox.Checked = m_MCUMode = 1
        Form1.CollectCheckbox.Checked = m_MCUMode = 2
        ' MCU Threshold
        Form1.TextBox_Thresh1.Text = Form1.m_T1
        Form1.TextBox_Thresh2.Text = Form1.m_T2
    End Sub

    Private Function OnPeopleFrame(ByVal data() As Byte) As Boolean
        Dim iPeopleCnt(3) As Integer
        Dim count As Integer

        'sample count
        count = BitConverter.ToInt32(data, 0)
        'direction
        If data(4) = 1 Then
            PeopleOut_Board = PeopleOut_Board + 1
        ElseIf data(4) = 2 Then
            PeopleIn_Board = PeopleIn_Board + 1
        Else
            Return False
        End If

        iPeopleCnt(0) = PeopleIn_Board
        iPeopleCnt(1) = PeopleOut_Board
        iPeopleCnt(2) = 30
        'height
        iPeopleCnt(3) = data(5)

        Me.Invoke(Me.m_ResultProcessDelegate, New Object() {iPeopleCnt, count, PEOPLE_RESULT_SOURCE_MCU})
    End Function

    ' compare whether equal valid start code first byte
    Private Function EqualStartCode_0(ByVal startcode_0 As Byte) As Boolean
        If startcode_0 = Frame.SampleFrame_104_StartCode_0 Or
            startcode_0 = Frame.CommandFrame_StartCode_0 Or
            startcode_0 = Frame.PeopleFrame_StartCode_0 Or
            startcode_0 = Frame.LogFrame_StartCode_0 Or
            startcode_0 = Frame.HandshakeFrame_StartCode_0 Then
            Return True
        End If
        Return False
    End Function

    ' set read state accord start code
    Private Function NextReadState(ByVal startcode_0 As Byte, ByVal startcode_1 As Byte) As Byte
        If Frame.isSampleFrame55(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_SAMPLE_55
        ElseIf Frame.isSampleFrame104(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_SAMPLE_104
        ElseIf Frame.isPeopleFrame(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_PEOPLE
        ElseIf Frame.isLogFrame(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_LOG
        ElseIf Frame.isHandshakeFrame(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_HANDSHAKE
        ElseIf Frame.isSampleAckFrame(startcode_0, startcode_1) Then
            Return SERIAL_READ_STATE_WAIT_SAMPLE_ACK
        Else
            Console.WriteLine(Hex(startcode_0) & "," & Hex(startcode_1))
        End If
        Return SERIAL_READ_STATE_LIVE
    End Function

    Private Function SerialReadFrame(ByVal data() As Byte) As Boolean
        Dim length As Byte

        If m_SerialReadState = SERIAL_READ_STATE_LIVE And
            SerialPort1.BytesToRead >= 2 Then
            ' when full frame length is even, maybe occur cannot read 
            ' valid frame because start code length is 2Bytes and m_FrameHead(0) 
            ' maybe is last byte by previous frame, so need to save m_FrameHead(1)
            ' to make up valid start code
            If m_FrameHead_ValidLength = 0 Then
                m_FrameHead(0) = SerialPort1.ReadByte()
                m_FrameHead(1) = SerialPort1.ReadByte()
            ElseIf m_FrameHead_ValidLength = 1 Then
                ' make up valid start code
                m_FrameHead(1) = SerialPort1.ReadByte()
            Else
                MsgBox("ASSERT(m_FrameHead_ValidLength=" & m_FrameHead_ValidLength & ")")
            End If

            m_FrameHead_ValidLength = 0
            m_SerialReadState = NextReadState(m_FrameHead(0), m_FrameHead(1))
            If m_SerialReadState = SERIAL_READ_STATE_LIVE Then
                'remain head(1) to make up?
                If EqualStartCode_0(m_FrameHead(1)) Then
                    m_FrameHead(0) = m_FrameHead(1)
                    m_FrameHead_ValidLength = 1
                End If
            Else 'valid frame
                m_FrameLength = 0
            End If
        End If

        If m_SerialReadState = SERIAL_READ_STATE_WAIT_SAMPLE_55 Then
            If SerialPort1.BytesToRead >= 53 Then
                length = SerialPort1.Read(data, 0, 53)    ' 8 x 3 x 2 + 4 + 1
                If length < 53 Then
                    Console.WriteLine("err,length=" & length & " < 53")
                Else
                    m_SerialReadState = SERIAL_READ_STATE_LIVE
                End If
                m_SerialPortDataReceivedCount = m_SerialPortDataReceivedCount + 1
                Me.Invoke(Me.m_DataProcessDelegate, New Object() {data, 52, m_SerialPortDataReceivedCount, False})
            Else
                'Console.WriteLine("SerialPort1.BytesToRead=" & SerialPort1.BytesToRead & " < 53")
            End If
        ElseIf m_SerialReadState <> SERIAL_READ_STATE_LIVE Then
            'frame body length
            If m_FrameLength = 0 Then
                If SerialPort1.BytesToRead >= 1 Then
                    m_FrameLength = SerialPort1.ReadByte()
                End If
            End If

            If m_FrameLength > 0 And SerialPort1.BytesToRead >= m_FrameLength + 1 Then
                'frame body+frame end code
                length = SerialPort1.Read(data, 0, m_FrameLength + 1)
                If length < m_FrameLength + 1 Then
                    Console.WriteLine("err,length=" & length & " < " & m_FrameLength + 1)
                Else
                    If m_SerialReadState = SERIAL_READ_STATE_WAIT_SAMPLE_104 Then
                        m_SerialPortDataReceivedCount = m_SerialPortDataReceivedCount + 1
                        Me.Invoke(Me.m_DataProcessDelegate, New Object() {data, m_FrameLength, m_SerialPortDataReceivedCount, False})
                    ElseIf m_SerialReadState = SERIAL_READ_STATE_WAIT_PEOPLE Then
                        OnPeopleFrame(data)
                        Me.m_SampleReader.SampleResume()
                    ElseIf m_SerialReadState = SERIAL_READ_STATE_WAIT_LOG Then
                        Form1.m_Log.WriteBytes(data, 0, length)
                        Console.Write((DateTime.Now - New DateTime(1970, 1, 1)).TotalMilliseconds & ">")
                        Console.Write(m_AsciiEncoder.GetString(data, 0, length))
                    ElseIf m_SerialReadState = SERIAL_READ_STATE_WAIT_SAMPLE_ACK Then
                        Me.m_SampleReader.SampleResume()
                    ElseIf m_SerialReadState = SERIAL_READ_STATE_WAIT_HANDSHAKE Then
                        Dim str As String = m_AsciiEncoder.GetString(data, 0, length)

                        Form1.m_Log.WriteLine("handshake frame>" & str)

                        ' number=1,mode=1,t1=2154,t2=2120'
                        Dim items As String() = str.Split(",")
                        For Each item As String In items
                            Dim kv As String() = item.Split("=")

                            If kv(0).Equals("number") Then
                                Form1.m_MCUNumber = Int(kv(1))
                            ElseIf kv(0).Equals("mode") Then
                                Form1.m_MCUMode = Int(kv(1))
                            ElseIf kv(0).Equals("t1") Then
                                Form1.m_T1 = Convert.ToDouble(kv(1)) / 100.0
                            ElseIf kv(0).Equals("t2") Then
                                Form1.m_T2 = Convert.ToDouble(kv(1)) / 100.0
                            End If
                        Next

                        Me.Invoke(Me.m_RefreshUIDelegate)
                    End If
                    m_SerialReadState = SERIAL_READ_STATE_LIVE
                End If
            End If
        End If
        Return True
    End Function

    Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        'シリアルポートをオープンしていない場合、処理を行わない.
        If SerialPort1.IsOpen = False Then
            Return
        End If

        Try
            SerialReadFrame(data)
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub VBCountingCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles VBCountingCheckBox.CheckedChanged
        LiveCheckbox.Checked = Not VBCountingCheckBox.Checked
        FileCheckBox.Checked = False
        CollectCheckbox.Checked = VBCountingCheckBox.Checked

        LiveCheckbox.Enabled = Not VBCountingCheckBox.Checked
        FileCheckBox.Enabled = False
        CollectCheckbox.Enabled = Not VBCountingCheckBox.Checked

        OneMCUCheckbox.Enabled = Not VBCountingCheckBox.Checked
        TwoMCUCheckBox.Enabled = Not VBCountingCheckBox.Checked
        ThreeMCUCheckBox.Enabled = False

        SendCommandButton.Enabled = Not VBCountingCheckBox.Checked
        If SerialPort1.IsOpen Then
            sendCommandFrame()
        End If
    End Sub

    Private Sub OneMCUCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OneMCUCheckbox.CheckedChanged
        If OneMCUCheckbox.Checked Then
            TwoMCUCheckBox.Checked = False
            ThreeMCUCheckBox.Checked = False
        Else
            if TwoMCUCheckBox.Checked = False And ThreeMCUCheckBox.Checked = False Then
                OneMCUCheckbox.Checked = True
            End if
        End If
    End Sub

    Private Sub TwoMCUCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TwoMCUCheckBox.CheckedChanged
        If TwoMCUCheckBox.Checked Then
            OneMCUCheckbox.Checked = False
            ThreeMCUCheckBox.Checked = False
        Else
            if OneMCUCheckbox.Checked = False And ThreeMCUCheckBox.Checked = False Then
                TwoMCUCheckBox.Checked = True
            End if
        End If
    End Sub

    Private Sub ThreeMCUCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ThreeMCUCheckBox.CheckedChanged
        If ThreeMCUCheckBox.Checked Then
            OneMCUCheckbox.Checked = False
            TwoMCUCheckBox.Checked = False
        Else
            if OneMCUCheckbox.Checked = False And TwoMCUCheckBox.Checked = False Then
                ThreeMCUCheckBox.Checked = True
            End if
        End If
    End Sub

    Private Sub LiveCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LiveCheckbox.CheckedChanged
        If LiveCheckbox.Checked Then
            FileCheckBox.Checked = Not LiveCheckbox.Checked
            CollectCheckbox.Checked = Not LiveCheckbox.Checked
        Else
            if FileCheckBox.Checked = False And CollectCheckbox.Checked = False Then
                LiveCheckbox.Checked = True
            End if
        End If
    End Sub

    Private Sub FileCheckBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles FileCheckBox.CheckedChanged
        If FileCheckBox.Checked Then
            LiveCheckbox.Checked = Not FileCheckBox.Checked
            CollectCheckbox.Checked = Not FileCheckBox.Checked
        Else
            if LiveCheckbox.Checked = False And CollectCheckbox.Checked = False Then
                FileCheckBox.Checked = True
            End if
        End If
    End Sub

    Private Sub CollectCheckbox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CollectCheckbox.CheckedChanged
        If CollectCheckbox.Checked Then
            LiveCheckbox.Checked = Not CollectCheckbox.Checked
            FileCheckBox.Checked = Not CollectCheckbox.Checked
        Else
            if LiveCheckbox.Checked = False And FileCheckBox.Checked = False Then
                CollectCheckbox.Checked = True
            End if
        End If
    End Sub
End Class