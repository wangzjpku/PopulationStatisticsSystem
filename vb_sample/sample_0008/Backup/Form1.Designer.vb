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
        Me.components = New System.ComponentModel.Container
        Me.grpRecv = New System.Windows.Forms.GroupBox
        Me.RcvTextBox = New System.Windows.Forms.TextBox
        Me.ExitButton = New System.Windows.Forms.Button
        Me.SerialPort1 = New System.IO.Ports.SerialPort(Me.components)
        Me.cmbPortName = New System.Windows.Forms.ComboBox
        Me.grpSetting = New System.Windows.Forms.GroupBox
        Me.cmbHandShake = New System.Windows.Forms.ComboBox
        Me.cmbBaudRate = New System.Windows.Forms.ComboBox
        Me.ConnectButton = New System.Windows.Forms.Button
        Me.UI_Button = New System.Windows.Forms.Button
        Me.Button_IN = New System.Windows.Forms.Button
        Me.Button_OUT = New System.Windows.Forms.Button
        Me.Button_ADULT = New System.Windows.Forms.Button
        Me.Button_CHILD = New System.Windows.Forms.Button
        Me.grpRecv.SuspendLayout()
        Me.grpSetting.SuspendLayout()
        Me.SuspendLayout()
        '
        'grpRecv
        '
        Me.grpRecv.Controls.Add(Me.RcvTextBox)
        Me.grpRecv.Location = New System.Drawing.Point(19, 66)
        Me.grpRecv.Name = "grpRecv"
        Me.grpRecv.Size = New System.Drawing.Size(980, 218)
        Me.grpRecv.TabIndex = 1
        Me.grpRecv.TabStop = False
        Me.grpRecv.Tag = ""
        Me.grpRecv.Text = "受信データ (PIR, 40kHz、235kHz、Time)"
        '
        'RcvTextBox
        '
        Me.RcvTextBox.AcceptsReturn = True
        Me.RcvTextBox.BackColor = System.Drawing.SystemColors.Window
        Me.RcvTextBox.Font = New System.Drawing.Font("ＭＳ ゴシック", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.RcvTextBox.Location = New System.Drawing.Point(8, 16)
        Me.RcvTextBox.Multiline = True
        Me.RcvTextBox.Name = "RcvTextBox"
        Me.RcvTextBox.ReadOnly = True
        Me.RcvTextBox.Size = New System.Drawing.Size(963, 196)
        Me.RcvTextBox.TabIndex = 6
        '
        'ExitButton
        '
        Me.ExitButton.Location = New System.Drawing.Point(878, 290)
        Me.ExitButton.Name = "ExitButton"
        Me.ExitButton.Size = New System.Drawing.Size(112, 24)
        Me.ExitButton.TabIndex = 7
        Me.ExitButton.Text = "終了"
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
        Me.cmbPortName.Size = New System.Drawing.Size(96, 20)
        Me.cmbPortName.TabIndex = 0
        '
        'grpSetting
        '
        Me.grpSetting.Controls.Add(Me.cmbHandShake)
        Me.grpSetting.Controls.Add(Me.cmbBaudRate)
        Me.grpSetting.Controls.Add(Me.cmbPortName)
        Me.grpSetting.Location = New System.Drawing.Point(19, 12)
        Me.grpSetting.Name = "grpSetting"
        Me.grpSetting.Size = New System.Drawing.Size(440, 48)
        Me.grpSetting.TabIndex = 3
        Me.grpSetting.TabStop = False
        Me.grpSetting.Text = "シリアルポート設定"
        '
        'cmbHandShake
        '
        Me.cmbHandShake.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbHandShake.FormattingEnabled = True
        Me.cmbHandShake.Location = New System.Drawing.Point(256, 16)
        Me.cmbHandShake.Name = "cmbHandShake"
        Me.cmbHandShake.Size = New System.Drawing.Size(176, 20)
        Me.cmbHandShake.TabIndex = 2
        '
        'cmbBaudRate
        '
        Me.cmbBaudRate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cmbBaudRate.FormattingEnabled = True
        Me.cmbBaudRate.Location = New System.Drawing.Point(128, 16)
        Me.cmbBaudRate.Name = "cmbBaudRate"
        Me.cmbBaudRate.Size = New System.Drawing.Size(112, 20)
        Me.cmbBaudRate.TabIndex = 1
        '
        'ConnectButton
        '
        Me.ConnectButton.Location = New System.Drawing.Point(483, 25)
        Me.ConnectButton.Name = "ConnectButton"
        Me.ConnectButton.Size = New System.Drawing.Size(61, 24)
        Me.ConnectButton.TabIndex = 3
        Me.ConnectButton.Text = "接続"
        Me.ConnectButton.UseVisualStyleBackColor = True
        '
        'UI_Button
        '
        Me.UI_Button.Location = New System.Drawing.Point(550, 25)
        Me.UI_Button.Name = "UI_Button"
        Me.UI_Button.Size = New System.Drawing.Size(69, 24)
        Me.UI_Button.TabIndex = 8
        Me.UI_Button.Text = "UI 表示"
        Me.UI_Button.UseVisualStyleBackColor = True
        '
        'Button_IN
        '
        Me.Button_IN.Location = New System.Drawing.Point(662, 25)
        Me.Button_IN.Name = "Button_IN"
        Me.Button_IN.Size = New System.Drawing.Size(51, 24)
        Me.Button_IN.TabIndex = 9
        Me.Button_IN.Text = "IN"
        Me.Button_IN.UseVisualStyleBackColor = True
        '
        'Button_OUT
        '
        Me.Button_OUT.Location = New System.Drawing.Point(719, 25)
        Me.Button_OUT.Name = "Button_OUT"
        Me.Button_OUT.Size = New System.Drawing.Size(51, 24)
        Me.Button_OUT.TabIndex = 10
        Me.Button_OUT.Text = "OUT"
        Me.Button_OUT.UseVisualStyleBackColor = True
        '
        'Button_ADULT
        '
        Me.Button_ADULT.Location = New System.Drawing.Point(776, 25)
        Me.Button_ADULT.Name = "Button_ADULT"
        Me.Button_ADULT.Size = New System.Drawing.Size(51, 24)
        Me.Button_ADULT.TabIndex = 11
        Me.Button_ADULT.Text = "ADULT"
        Me.Button_ADULT.UseVisualStyleBackColor = True
        '
        'Button_CHILD
        '
        Me.Button_CHILD.Location = New System.Drawing.Point(833, 25)
        Me.Button_CHILD.Name = "Button_CHILD"
        Me.Button_CHILD.Size = New System.Drawing.Size(51, 24)
        Me.Button_CHILD.TabIndex = 12
        Me.Button_CHILD.Text = "CHILD"
        Me.Button_CHILD.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1015, 325)
        Me.Controls.Add(Me.Button_CHILD)
        Me.Controls.Add(Me.Button_ADULT)
        Me.Controls.Add(Me.Button_OUT)
        Me.Controls.Add(Me.Button_IN)
        Me.Controls.Add(Me.UI_Button)
        Me.Controls.Add(Me.ConnectButton)
        Me.Controls.Add(Me.grpSetting)
        Me.Controls.Add(Me.ExitButton)
        Me.Controls.Add(Me.grpRecv)
        Me.Name = "Form1"
        Me.Text = "APC SerialMonitor(DEMO)"
        Me.grpRecv.ResumeLayout(False)
        Me.grpRecv.PerformLayout()
        Me.grpSetting.ResumeLayout(False)
        Me.ResumeLayout(False)

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
    Friend WithEvents UI_Button As System.Windows.Forms.Button
    Friend WithEvents Button_IN As System.Windows.Forms.Button
    Friend WithEvents Button_OUT As System.Windows.Forms.Button
    Friend WithEvents Button_ADULT As System.Windows.Forms.Button
    Friend WithEvents Button_CHILD As System.Windows.Forms.Button

End Class
