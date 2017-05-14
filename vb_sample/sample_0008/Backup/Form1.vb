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

Public Class Form1

	'****************************************************************************'
	'*
	'*	@brief	ボーレート格納用のクラス定義.
	'*
	Private Class BuadRateItem 
		Inherits  Object

		Private m_name	As String  = ""
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
		Public Overrides Function ToString As String
			Return m_name
		End Function

	End Class

	'****************************************************************************'
	'*
	'*	@brief	制御プロトコル格納用のクラス定義.
	'*
	Private Class HandShakeItem 
		Inherits  Object

		Private m_name	As String  = ""
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
		Public Overrides Function ToString As String
			Return m_name
		End Function

	End Class

	Private Delegate Sub Delegate_RcvDataToTextBox( data As String )

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
			cmbPortName.SelectedIndex = 0
		End If

		cmbBaudRate.Items.Clear()

		'ボーレート選択コンボボックスに選択項目をセットする.
		Dim baud As BuadRateItem
		baud		  = New BuadRateItem
		baud.NAME	  = "4800bps"
		baud.BAUDRATE = 4800
		cmbBaudRate.Items.Add(baud)

		baud		  = New BuadRateItem
		baud.NAME	  = "9600bps"
		baud.BAUDRATE = 9600
		cmbBaudRate.Items.Add(baud)

		baud		  = New BuadRateItem
		baud.NAME	  = "19200bps"
		baud.BAUDRATE = 19200
		cmbBaudRate.Items.Add(baud)

		baud		  = New BuadRateItem
		baud.NAME	  = "115200bps"
		baud.BAUDRATE = 115200
		cmbBaudRate.Items.Add(baud)
        cmbBaudRate.SelectedIndex = 3

		cmbHandShake.Items.Clear()

		'フロー制御選択コンボボックスに選択項目をセットする.
		Dim ctrl As HandShakeItem
		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "なし"
		ctrl.HANDSHAKE = Handshake.None
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "XON/XOFF制御"
		ctrl.HANDSHAKE = Handshake.XOnXOff
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "RTS/CTS制御"
		ctrl.HANDSHAKE = Handshake.RequestToSend
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "XON/XOFF + RTS/CTS制御"
		ctrl.HANDSHAKE = Handshake.RequestToSendXOnXOff
		cmbHandShake.Items.Add(ctrl)
		cmbHandShake.SelectedIndex = 0

        '受信用のテキストボックスをクリアする.
        RcvTextBox.Clear()

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
	Private Sub ConnectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectButton.Click

		If SerialPort1.IsOpen = True Then

			'シリアルポートをクローズする.
			SerialPort1.Close()

			'ボタンの表示を[切断]から[接続]に変える.
			ConnectButton.Text = "接続"
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
			SerialPort1.Encoding = Encoding.Unicode

			Try
				'シリアルポートをオープンする.
				SerialPort1.Open()

				'ボタンの表示を[接続]から[切断]に変える.
				ConnectButton.Text = "切断"
			Catch ex As Exception
				MsgBox( ex.Message )
			End Try

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
	Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

		'シリアルポートをオープンしていない場合、処理を行わない.
		If SerialPort1.IsOpen = False Then
			Return
		End If

		Try
			'受信データを読み込む.
            Dim data(54) As Byte
            Dim i As Integer

            If SerialPort1.ReadByte() = 85 And SerialPort1.ReadByte() = 85 Then


                SerialPort1.Read(data, 0, 52)

                '受信したデータをテキストボックスに書き込む.
                Dim MeasuredData As String
                MeasuredData = ""

                For i = 0 To 51

                    If data(i) <= 15 Then

                        MeasuredData = MeasuredData & "0" & Hex(data(i)) & " "

                    Else

                        MeasuredData = MeasuredData & Hex(data(i)) & " "

                    End If

                Next

                Invoke(New Delegate_RcvDataToTextBox(AddressOf Me.RcvDataToTextBox), MeasuredData & vbCrLf)

            End If


        Catch ex As Exception
            MsgBox(ex.Message)
        End Try

	End Sub

	'****************************************************************************'
	'*
	'*	@brief	受信データをテキストボックスに書き込む.
	'*
	'*	@param	[in]	data	受信した文字列.
	'*
	'*	@retval	なし.
	'*
	Private Sub RcvDataToTextBox( data As String )

		'受信データをテキストボックスの最後に追記する.
		If IsNothing( data ) = False Then
			RcvTextBox.AppendText( data )
		End If

	End Sub

    Private Sub UI_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles UI_Button.Click

        APC_MainForm.Show()

    End Sub

    Private Sub Button_IN_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_IN.Click
        APC_MainForm.Label_IN.Text = Val(APC_MainForm.Label_IN.Text) + 1
    End Sub

    Private Sub Button_OUT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_OUT.Click
        APC_MainForm.Label_OUT.Text = Val(APC_MainForm.Label_OUT.Text) + 1
    End Sub

    Private Sub Button_ADULT_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_ADULT.Click
        APC_MainForm.Label_ADULT.Text = Val(APC_MainForm.Label_ADULT.Text) + 1
    End Sub

    Private Sub Button_CHILD_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button_CHILD.Click
        APC_MainForm.Label_CHILD.Text = Val(APC_MainForm.Label_CHILD.Text) + 1
    End Sub
End Class
