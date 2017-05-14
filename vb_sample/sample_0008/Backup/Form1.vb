'****************************************************************************
'*	Copylight(C) 2011 Kanazawa-soft-design,LLC.All Rights Reserved.
'****************************************************************************
'*
'*	@file	Form1.vb
'*
'*	@brief	�V���A���|�[�g�ʐM�v���O����.
'*
'*	@author	���V �閾
'*
Imports System
Imports System.IO.Ports
Imports System.Text

Public Class Form1

	'****************************************************************************'
	'*
	'*	@brief	�{�[���[�g�i�[�p�̃N���X��`.
	'*
	Private Class BuadRateItem 
		Inherits  Object

		Private m_name	As String  = ""
		Private m_value As Integer = 0

		'�\������
		Public Property NAME() As String
			Set(ByVal value As String)
				m_name = value
			End Set
			Get
				Return m_name
			End Get
		End Property

		'�{�[���[�g�ݒ�l.
		Public Property BAUDRATE() As Integer
			Set(ByVal value As Integer)
				m_value = value
			End Set
			Get
				Return m_value
			End Get
		End Property

		'�R���{�{�b�N�X�\���p�̕�����擾�֐�.
		Public Overrides Function ToString As String
			Return m_name
		End Function

	End Class

	'****************************************************************************'
	'*
	'*	@brief	����v���g�R���i�[�p�̃N���X��`.
	'*
	Private Class HandShakeItem 
		Inherits  Object

		Private m_name	As String  = ""
		Private m_value As Handshake = Handshake.None

		'�\������
		Public Property NAME() As String
			Set(ByVal value As String)
				m_name = value
			End Set
			Get
				Return m_name
			End Get
		End Property

		'����v���g�R���ݒ�l.
		Public Property HANDSHAKE() As Handshake
			Set(ByVal value As Handshake)
				m_value = value
			End Set
			Get
				Return m_value
			End Get
		End Property

		'�R���{�{�b�N�X�\���p�̕�����擾�֐�.
		Public Overrides Function ToString As String
			Return m_name
		End Function

	End Class

	Private Delegate Sub Delegate_RcvDataToTextBox( data As String )

	'****************************************************************************'
	'*
	'*	@brief	�_�C�A���O�̏�������.
	'*
	'*	@param	[in]	sender	�C�x���g�̑��M���̃I�u�W�F�N�g.
	'*	@param	[in]	e		�C�x���g���.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

		'���p�\�ȃV���A���|�[�g���̔z����擾����.
		Dim PortList As String()
		PortList = SerialPort.GetPortNames()

		cmbPortName.Items.Clear()

		'�V���A���|�[�g�����R���{�{�b�N�X�ɃZ�b�g����.
		Dim PortName As String
		For Each PortName In PortList
			cmbPortName.Items.Add(PortName)
		Next PortName

		If cmbPortName.Items.Count > 0 Then
			cmbPortName.SelectedIndex = 0
		End If

		cmbBaudRate.Items.Clear()

		'�{�[���[�g�I���R���{�{�b�N�X�ɑI�����ڂ��Z�b�g����.
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

		'�t���[����I���R���{�{�b�N�X�ɑI�����ڂ��Z�b�g����.
		Dim ctrl As HandShakeItem
		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "�Ȃ�"
		ctrl.HANDSHAKE = Handshake.None
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "XON/XOFF����"
		ctrl.HANDSHAKE = Handshake.XOnXOff
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "RTS/CTS����"
		ctrl.HANDSHAKE = Handshake.RequestToSend
		cmbHandShake.Items.Add(ctrl)

		ctrl		   = New HandShakeItem
		ctrl.NAME	   = "XON/XOFF + RTS/CTS����"
		ctrl.HANDSHAKE = Handshake.RequestToSendXOnXOff
		cmbHandShake.Items.Add(ctrl)
		cmbHandShake.SelectedIndex = 0

        '��M�p�̃e�L�X�g�{�b�N�X���N���A����.
        RcvTextBox.Clear()

	End Sub

	'****************************************************************************'
	'*
	'*	@brief	�_�C�A���O�̏I������.
	'*
	'*	@param	[in]	sender	�C�x���g�̑��M���̃I�u�W�F�N�g.
	'*	@param	[in]	e		�C�x���g���.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub Form1_FormClosed(ByVal sender As System.Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed

		'�V���A���|�[�g���I�[�v�����Ă���ꍇ�A�N���[�Y����.
		If SerialPort1.IsOpen Then
			SerialPort1.Close()
		End If

	End Sub

	'****************************************************************************'
	'*
	'*	@brief	[�I��]�{�^�����������Ƃ��̏���.
	'*
	'*	@param	[in]	sender	�C�x���g�̑��M���̃I�u�W�F�N�g.
	'*	@param	[in]	e		�C�x���g���.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub ExitButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExitButton.Click

		'�_�C�A���O���N���[�Y����.
		Me.Close()

	End Sub

	'****************************************************************************'
	'*
	'*	@brief	[�ڑ�]/[�ؒf]�{�^�����������Ƃ��ɃV���A���|�[�g�̃I�[�v��/�N���[�Y���s��.
	'*
	'*	@param	[in]	sender	�C�x���g�̑��M���̃I�u�W�F�N�g.
	'*	@param	[in]	e		�C�x���g���.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub ConnectButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ConnectButton.Click

		If SerialPort1.IsOpen = True Then

			'�V���A���|�[�g���N���[�Y����.
			SerialPort1.Close()

			'�{�^���̕\����[�ؒf]����[�ڑ�]�ɕς���.
			ConnectButton.Text = "�ڑ�"
		Else

			'�I�[�v������V���A���|�[�g���R���{�{�b�N�X������o��.
			SerialPort1.PortName = cmbPortName.SelectedItem.ToString()

			'�{�[���[�g���R���{�{�b�N�X������o��.
			Dim baud As BuadRateItem
			baud = cmbBaudRate.SelectedItem
			SerialPort1.BaudRate = baud.BAUDRATE

			'�f�[�^�r�b�g���Z�b�g����. (�f�[�^�r�b�g = 8�r�b�g)
			SerialPort1.DataBits = 8

			'�p���e�B�r�b�g���Z�b�g����. (�p���e�B�r�b�g = �Ȃ�)
			SerialPort1.Parity = Parity.None

			'�X�g�b�v�r�b�g���Z�b�g����. (�X�g�b�v�r�b�g = 1�r�b�g)
			SerialPort1.StopBits = StopBits.One

			'�t���[������R���{�{�b�N�X������o��.
			Dim ctrl As HandShakeItem
			ctrl = cmbHandShake.SelectedItem
			SerialPort1.Handshake = ctrl.HANDSHAKE

			'�����R�[�h���Z�b�g����.
			SerialPort1.Encoding = Encoding.Unicode

			Try
				'�V���A���|�[�g���I�[�v������.
				SerialPort1.Open()

				'�{�^���̕\����[�ڑ�]����[�ؒf]�ɕς���.
				ConnectButton.Text = "�ؒf"
			Catch ex As Exception
				MsgBox( ex.Message )
			End Try

		End If

	End Sub


	'****************************************************************************'
	'*
	'*	@brief	�f�[�^��M�����������Ƃ��̃C�x���g����.
	'*
	'*	@param	[in]	sender	�C�x���g�̑��M���̃I�u�W�F�N�g.
	'*	@param	[in]	e		�C�x���g���.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub SerialPort1_DataReceived(ByVal sender As System.Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived

		'�V���A���|�[�g���I�[�v�����Ă��Ȃ��ꍇ�A�������s��Ȃ�.
		If SerialPort1.IsOpen = False Then
			Return
		End If

		Try
			'��M�f�[�^��ǂݍ���.
            Dim data(54) As Byte
            Dim i As Integer

            If SerialPort1.ReadByte() = 85 And SerialPort1.ReadByte() = 85 Then


                SerialPort1.Read(data, 0, 52)

                '��M�����f�[�^���e�L�X�g�{�b�N�X�ɏ�������.
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
	'*	@brief	��M�f�[�^���e�L�X�g�{�b�N�X�ɏ�������.
	'*
	'*	@param	[in]	data	��M����������.
	'*
	'*	@retval	�Ȃ�.
	'*
	Private Sub RcvDataToTextBox( data As String )

		'��M�f�[�^���e�L�X�g�{�b�N�X�̍Ō�ɒǋL����.
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
