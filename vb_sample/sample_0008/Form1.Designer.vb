<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナで必要です。
    'Windows フォーム デザイナを使用して変更できます。  
    'コード エディタを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.grpRecv = New System.Windows.Forms.GroupBox()
        Me.RcvTextBox = New System.Windows.Forms.TextBox()
        Me.ExitButton = New System.Windows.Forms.Button()
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.cmbPortName = New System.Windows.Forms.ComboBox()
        Me.grpSetting = New System.Windows.Forms.GroupBox()
        Me.cmbHandShake = New System.Windows.Forms.ComboBox()
        Me.cmbBaudRate = New System.Windows.Forms.ComboBox()
        Me.ConnectButton = New System.Windows.Forms.Button()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.TextBox_Thresh1 = New System.Windows.Forms.TextBox()
        Me.TextBox_Thresh2 = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.ResetButton = New System.Windows.Forms.Button()
        Me.LabelPIRDATA = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.CalibButton = New System.Windows.Forms.Button()
        Me.FileButton = New System.Windows.Forms.Button()
        Me.SendCommandButton = New System.Windows.Forms.Button()
        Me.VBCountingCheckBox = New System.Windows.Forms.CheckBox()
        Me.OneMCUCheckbox = New System.Windows.Forms.CheckBox()
        Me.TwoMCUCheckBox = New System.Windows.Forms.CheckBox()
        Me.ThreeMCUCheckBox = New System.Windows.Forms.CheckBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.CollectCheckbox = New System.Windows.Forms.CheckBox()
        Me.FileCheckBox = New System.Windows.Forms.CheckBox()
        Me.LiveCheckbox = New System.Windows.Forms.CheckBox()
        Me.grpRecv.SuspendLayout()
        Me.grpSetting.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpRecv
        '
        Me.grpRecv.Controls.Add(Me.RcvTextBox)
        Me.grpRecv.Location = New System.Drawing.Point(17, 150)
        Me.grpRecv.Name = "grpRecv"
        Me.grpRecv.Size = New System.Drawing.Size(1180, 380)
        Me.grpRecv.TabIndex = 1
        Me.grpRecv.TabStop = False
        Me.grpRecv.Tag = ""
        Me.grpRecv.Text = "Data (PIR, 40kHz、235kHz、Time)"
        '
        'RcvTextBox
        '
        Me.RcvTextBox.AcceptsReturn = True
        Me.RcvTextBox.BackColor = System.Drawing.SystemColors.InfoText
        Me.RcvTextBox.Font = New System.Drawing.Font("Courier New", 15.0!)
        Me.RcvTextBox.ForeColor = System.Drawing.Color.Lime
        Me.RcvTextBox.Location = New System.Drawing.Point(0, 16)
        Me.RcvTextBox.Multiline = True
        Me.RcvTextBox.Name = "RcvTextBox"
        Me.RcvTextBox.ReadOnly = True
        Me.RcvTextBox.Size = New System.Drawing.Size(1172, 358)
        Me.RcvTextBox.TabIndex = 6
        '
        'ExitButton
        '
        Me.ExitButton.Location = New System.Drawing.Point(566, 530)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(66, 24)
        Me.ExitButton.TabIndex = 7
        Me.ExitButton.Text = "EXIT"
        Me.ExitButton.UseVisualStyleBackColor = True
        '
        'SerialPort1
        '
        '
        'cmbPortName
        '
        Me.cmbPortName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbPortName.FormattingEnabled = True
        Me.cmbPortName.Location = New System.Drawing.Point(16, 16)
        Me.cmbPortName.Name = "cmbPortName"
        Me.cmbPortName.Size = New System.Drawing.Size(76, 22)
        Me.cmbPortName.TabIndex = 0
        '
        'grpSetting
        '
        Me.grpSetting.Controls.Add(Me.cmbHandShake)
        Me.grpSetting.Controls.Add(Me.cmbBaudRate)
        Me.grpSetting.Controls.Add(Me.cmbPortName)
        Me.grpSetting.Font = New System.Drawing.Font("SimSun", 10.5!)
        Me.grpSetting.Location = New System.Drawing.Point(19, 12)
        Me.grpSetting.Name = "grpSetting"
        Me.grpSetting.Size = New System.Drawing.Size(415, 48)
        Me.grpSetting.TabIndex = 3
        Me.grpSetting.TabStop = False
        Me.grpSetting.Text = "Serial Port"
        '
        'cmbHandShake
        '
        Me.cmbHandShake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbHandShake.FormattingEnabled = True
        Me.cmbHandShake.Location = New System.Drawing.Point(226, 16)
        Me.cmbHandShake.Name = "cmbHandShake"
        Me.cmbHandShake.Size = New System.Drawing.Size(176, 22)
        Me.cmbHandShake.TabIndex = 2
        '
        'cmbBaudRate
        '
        Me.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBaudRate.FormattingEnabled = True
        Me.cmbBaudRate.Location = New System.Drawing.Point(108, 16)
        Me.cmbBaudRate.Name = "cmbBaudRate"
        Me.cmbBaudRate.Size = New System.Drawing.Size(100, 22)
        Me.cmbBaudRate.TabIndex = 1
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(500, 39)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(61, 24)
        Me.ConnectButton.TabIndex = 3
        Me.ConnectButton.Text = "CONN"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.ForeColor = System.Drawing.SystemColors.HotTrack
        Me.Label1.Location = New System.Drawing.Point(23, 531)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(53, 12)
        Me.Label1.TabIndex = 13
        Me.Label1.Text = "STOPPING"
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(106, 530)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(23, 12)
        Me.Label2.TabIndex = 14
        Me.Label2.Text = "IN:"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(170, 530)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(29, 12)
        Me.Label3.TabIndex = 15
        Me.Label3.Text = "OUT:"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(242, 530)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(23, 12)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "PIR"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Location = New System.Drawing.Point(381, 531)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(41, 12)
        Me.Label5.TabIndex = 17
        Me.Label5.Text = "HEIGHT"
        '
        'TextBox_Thresh1
        '
        Me.TextBox_Thresh1.Location = New System.Drawing.Point(603, 18)
        Me.TextBox_Thresh1.Name = "TextBox_Thresh1"
        Me.TextBox_Thresh1.Size = New System.Drawing.Size(51, 21)
        Me.TextBox_Thresh1.TabIndex = 18
        Me.TextBox_Thresh1.Text = "23"
        '
        'TextBox_Thresh2
        '
        Me.TextBox_Thresh2.Location = New System.Drawing.Point(603, 37)
        Me.TextBox_Thresh2.Name = "TextBox_Thresh2"
        Me.TextBox_Thresh2.Size = New System.Drawing.Size(51, 21)
        Me.TextBox_Thresh2.TabIndex = 19
        Me.TextBox_Thresh2.Text = "22"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(575, 20)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(17, 12)
        Me.Label6.TabIndex = 20
        Me.Label6.Text = "T1"
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(575, 39)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(17, 12)
        Me.Label7.TabIndex = 21
        Me.Label7.Text = "T2"
        '
        'ResetButton
        '
        Me.ResetButton.Location = New System.Drawing.Point(485, 531)
        Me.ResetButton.Name = "ResetButton"
        Me.ResetButton.Size = New System.Drawing.Size(75, 23)
        Me.ResetButton.TabIndex = 22
        Me.ResetButton.Text = "Cnt.Reset"
        Me.ResetButton.UseVisualStyleBackColor = True
        '
        'LabelPIRDATA
        '
        Me.LabelPIRDATA.AutoSize = True
        Me.LabelPIRDATA.Location = New System.Drawing.Point(104, 549)
        Me.LabelPIRDATA.Name = "LabelPIRDATA"
        Me.LabelPIRDATA.Size = New System.Drawing.Size(23, 12)
        Me.LabelPIRDATA.TabIndex = 23
        Me.LabelPIRDATA.Text = "PIR"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Location = New System.Drawing.Point(104, 425)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(0, 12)
        Me.Label8.TabIndex = 24
        '
        'CalibButton
        '
        Me.CalibButton.Location = New System.Drawing.Point(22, 560)
        Me.CalibButton.Name = "CalibButton"
        Me.CalibButton.Size = New System.Drawing.Size(61, 18)
        Me.CalibButton.TabIndex = 25
        Me.CalibButton.Text = "Calib."
        Me.CalibButton.UseVisualStyleBackColor = True
        '
        'FileButton
        '
        Me.FileButton.Location = New System.Drawing.Point(500, 12)
        Me.FileButton.Name = "FileButton"
        Me.FileButton.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.FileButton.Size = New System.Drawing.Size(61, 27)
        Me.FileButton.TabIndex = 26
        Me.FileButton.Text = "FILE"
        Me.FileButton.UseVisualStyleBackColor = True
        '
        'SendCommandButton
        '
        Me.SendCommandButton.Font = New System.Drawing.Font("SimSun", 10.5!)
        Me.SendCommandButton.Location = New System.Drawing.Point(561, 76)
        Me.SendCommandButton.Name = "SendCommandButton"
        Me.SendCommandButton.RightToLeft = System.Windows.Forms.RightToLeft.No
        Me.SendCommandButton.Size = New System.Drawing.Size(93, 27)
        Me.SendCommandButton.TabIndex = 29
        Me.SendCommandButton.Text = "SendCommand"
        Me.SendCommandButton.UseVisualStyleBackColor = True
        '
        'VBCountingCheckBox
        '
        Me.VBCountingCheckBox.AutoSize = True
        Me.VBCountingCheckBox.Font = New System.Drawing.Font("SimSun", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.VBCountingCheckBox.Location = New System.Drawing.Point(19, 116)
        Me.VBCountingCheckBox.Name = "VBCountingCheckBox"
        Me.VBCountingCheckBox.Size = New System.Drawing.Size(138, 24)
        Me.VBCountingCheckBox.TabIndex = 30
        Me.VBCountingCheckBox.Text = "VB Counting"
        Me.VBCountingCheckBox.UseVisualStyleBackColor = True
        '
        'OneMCUCheckbox
        '
        Me.OneMCUCheckbox.AutoSize = True
        Me.OneMCUCheckbox.Location = New System.Drawing.Point(15, 18)
        Me.OneMCUCheckbox.Name = "OneMCUCheckbox"
        Me.OneMCUCheckbox.Size = New System.Drawing.Size(47, 18)
        Me.OneMCUCheckbox.TabIndex = 27
        Me.OneMCUCheckbox.Text = "ONE"
        Me.OneMCUCheckbox.UseVisualStyleBackColor = True
        '
        'TwoMCUCheckBox
        '
        Me.TwoMCUCheckBox.AutoSize = True
        Me.TwoMCUCheckBox.Location = New System.Drawing.Point(68, 18)
        Me.TwoMCUCheckBox.Name = "TwoMCUCheckBox"
        Me.TwoMCUCheckBox.Size = New System.Drawing.Size(131, 18)
        Me.TwoMCUCheckBox.TabIndex = 28
        Me.TwoMCUCheckBox.Text = "TWO(Slave Left)"
        Me.TwoMCUCheckBox.UseVisualStyleBackColor = True
        '
        'ThreeMCUCheckBox
        '
        Me.ThreeMCUCheckBox.AutoSize = True
        Me.ThreeMCUCheckBox.Enabled = False
        Me.ThreeMCUCheckBox.Location = New System.Drawing.Point(202, 18)
        Me.ThreeMCUCheckBox.Name = "ThreeMCUCheckBox"
        Me.ThreeMCUCheckBox.Size = New System.Drawing.Size(61, 18)
        Me.ThreeMCUCheckBox.TabIndex = 29
        Me.ThreeMCUCheckBox.Text = "THREE"
        Me.ThreeMCUCheckBox.UseVisualStyleBackColor = True
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.TwoMCUCheckBox)
        Me.GroupBox1.Controls.Add(Me.ThreeMCUCheckBox)
        Me.GroupBox1.Controls.Add(Me.OneMCUCheckbox)
        Me.GroupBox1.Font = New System.Drawing.Font("SimSun", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.GroupBox1.Location = New System.Drawing.Point(20, 67)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(269, 43)
        Me.GroupBox1.TabIndex = 28
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "MCU Number"
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.CollectCheckbox)
        Me.GroupBox2.Controls.Add(Me.FileCheckBox)
        Me.GroupBox2.Controls.Add(Me.LiveCheckbox)
        Me.GroupBox2.Font = New System.Drawing.Font("SimSun", 10.5!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.GroupBox2.Location = New System.Drawing.Point(290, 67)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(265, 43)
        Me.GroupBox2.TabIndex = 30
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "MCU Work Mode"
        '
        'CollectCheckbox
        '
        Me.CollectCheckbox.AutoSize = True
        Me.CollectCheckbox.Location = New System.Drawing.Point(188, 18)
        Me.CollectCheckbox.Name = "CollectCheckbox"
        Me.CollectCheckbox.Size = New System.Drawing.Size(75, 18)
        Me.CollectCheckbox.TabIndex = 29
        Me.CollectCheckbox.Text = "Collect"
        Me.CollectCheckbox.UseVisualStyleBackColor = True
        '
        'FileCheckBox
        '
        Me.FileCheckBox.AutoSize = True
        Me.FileCheckBox.Enabled = False
        Me.FileCheckBox.Location = New System.Drawing.Point(132, 18)
        Me.FileCheckBox.Name = "FileCheckBox"
        Me.FileCheckBox.Size = New System.Drawing.Size(54, 18)
        Me.FileCheckBox.TabIndex = 28
        Me.FileCheckBox.Text = "File"
        Me.FileCheckBox.UseVisualStyleBackColor = True
        '
        'LiveCheckbox
        '
        Me.LiveCheckbox.AutoSize = True
        Me.LiveCheckbox.Location = New System.Drawing.Point(15, 18)
        Me.LiveCheckbox.Name = "LiveCheckbox"
        Me.LiveCheckbox.Size = New System.Drawing.Size(117, 18)
        Me.LiveCheckbox.TabIndex = 27
        Me.LiveCheckbox.Text = "Live Counting"
        Me.LiveCheckbox.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1276, 595)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.VBCountingCheckBox)
        Me.Controls.Add(Me.SendCommandButton)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.FileButton)
        Me.Controls.Add(Me.CalibButton)
        Me.Controls.Add(Me.Label8)
        Me.Controls.Add(Me.LabelPIRDATA)
        Me.Controls.Add(Me.ResetButton)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.Label6)
        Me.Controls.Add(Me.TextBox_Thresh2)
        Me.Controls.Add(Me.TextBox_Thresh1)
        Me.Controls.Add(Me.Label5)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.ConnectButton)
        Me.Controls.Add(Me.grpSetting)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.grpRecv)
        Me.Name = "Form1"
        Me.Text = "APC SerialMonitor(DEMO)"
        Me.grpRecv.ResumeLayout(False)
        Me.grpRecv.PerformLayout()
        Me.grpSetting.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents grpRecv As System.Windows.Forms.GroupBox
    Friend WithEvents RcvTextBox As System.Windows.Forms.TextBox
    Friend WithEvents ExitButton As System.Windows.Forms.Button
    Friend WithEvents SerialPort1 As System.IO.Ports.SerialPort
    Friend WithEvents cmbPortName As System.Windows.Forms.ComboBox
    Friend WithEvents grpSetting As System.Windows.Forms.GroupBox
    Friend WithEvents cmbBaudRate As System.Windows.Forms.ComboBox
    Friend WithEvents cmbHandShake As System.Windows.Forms.ComboBox
    Friend WithEvents ConnectButton As System.Windows.Forms.Button
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label5 As System.Windows.Forms.Label
    Friend WithEvents TextBox_Thresh1 As System.Windows.Forms.TextBox
    Friend WithEvents TextBox_Thresh2 As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents ResetButton As System.Windows.Forms.Button
    Friend WithEvents LabelPIRDATA As System.Windows.Forms.Label
    Friend WithEvents Label8 As System.Windows.Forms.Label
    Friend WithEvents CalibButton As System.Windows.Forms.Button
    Friend WithEvents FileButton As System.Windows.Forms.Button
    Friend WithEvents SendCommandButton As System.Windows.Forms.Button
    Friend WithEvents VBCountingCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents OneMCUCheckbox As System.Windows.Forms.CheckBox
    Friend WithEvents TwoMCUCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents ThreeMCUCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents CollectCheckbox As System.Windows.Forms.CheckBox
    Friend WithEvents FileCheckBox As System.Windows.Forms.CheckBox
    Friend WithEvents LiveCheckbox As System.Windows.Forms.CheckBox

End Class
