<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class APC_MainForm
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
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(APC_MainForm))
        Me.Label_IN = New System.Windows.Forms.Label()
        Me.Label_OUT = New System.Windows.Forms.Label()
        Me.Label_ADULT = New System.Windows.Forms.Label()
        Me.Label_CHILD = New System.Windows.Forms.Label()
        Me.Label_Height = New System.Windows.Forms.Label()
        Me.ButtonRecord = New System.Windows.Forms.Button()
        Me.CheckedListBox_RecordType = New System.Windows.Forms.CheckedListBox()
        Me.CounterResetButton = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'Label_IN
        '
        Me.Label_IN.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_IN.Font = New System.Drawing.Font("Microsoft Sans Serif", 48.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_IN.Location = New System.Drawing.Point(213, 160)
        Me.Label_IN.Name = "Label_IN"
        Me.Label_IN.Size = New System.Drawing.Size(123, 66)
        Me.Label_IN.TabIndex = 0
        Me.Label_IN.Text = "0"
        Me.Label_IN.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_OUT
        '
        Me.Label_OUT.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_OUT.Font = New System.Drawing.Font("Microsoft Sans Serif", 48.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_OUT.Location = New System.Drawing.Point(213, 253)
        Me.Label_OUT.Name = "Label_OUT"
        Me.Label_OUT.Size = New System.Drawing.Size(123, 63)
        Me.Label_OUT.TabIndex = 1
        Me.Label_OUT.Text = "0"
        Me.Label_OUT.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_ADULT
        '
        Me.Label_ADULT.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_ADULT.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_ADULT.Location = New System.Drawing.Point(545, 199)
        Me.Label_ADULT.Name = "Label_ADULT"
        Me.Label_ADULT.Size = New System.Drawing.Size(97, 47)
        Me.Label_ADULT.TabIndex = 2
        Me.Label_ADULT.Text = "0"
        Me.Label_ADULT.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_CHILD
        '
        Me.Label_CHILD.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_CHILD.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_CHILD.Location = New System.Drawing.Point(545, 273)
        Me.Label_CHILD.Name = "Label_CHILD"
        Me.Label_CHILD.Size = New System.Drawing.Size(97, 45)
        Me.Label_CHILD.TabIndex = 3
        Me.Label_CHILD.Text = "0"
        Me.Label_CHILD.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_Height
        '
        Me.Label_Height.BackColor = System.Drawing.Color.LightSkyBlue
        Me.Label_Height.Font = New System.Drawing.Font("Microsoft Sans Serif", 36.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label_Height.ForeColor = System.Drawing.Color.Black
        Me.Label_Height.Location = New System.Drawing.Point(495, 339)
        Me.Label_Height.Name = "Label_Height"
        Me.Label_Height.Size = New System.Drawing.Size(105, 50)
        Me.Label_Height.TabIndex = 4
        Me.Label_Height.Text = "0"
        Me.Label_Height.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'ButtonRecord
        '
        Me.ButtonRecord.BackColor = System.Drawing.SystemColors.MenuHighlight
        Me.ButtonRecord.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None
        Me.ButtonRecord.Font = New System.Drawing.Font("High Tower Text", 18.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ButtonRecord.ForeColor = System.Drawing.SystemColors.ButtonHighlight
        Me.ButtonRecord.Location = New System.Drawing.Point(12, 333)
        Me.ButtonRecord.Name = "ButtonRecord"
        Me.ButtonRecord.Size = New System.Drawing.Size(176, 42)
        Me.ButtonRecord.TabIndex = 0
        Me.ButtonRecord.Text = "Record Start"
        Me.ButtonRecord.UseVisualStyleBackColor = False
        '
        'CheckedListBox_RecordType
        '
        Me.CheckedListBox_RecordType.CheckOnClick = True
        Me.CheckedListBox_RecordType.Font = New System.Drawing.Font("Broadway", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckedListBox_RecordType.ForeColor = System.Drawing.SystemColors.MenuHighlight
        Me.CheckedListBox_RecordType.FormattingEnabled = True
        Me.CheckedListBox_RecordType.Items.AddRange(New Object() {"IN_1", "OUT_1", "IN_2", "OUT_2", "IN_1_OUT_1"})
        Me.CheckedListBox_RecordType.Location = New System.Drawing.Point(205, 332)
        Me.CheckedListBox_RecordType.Name = "CheckedListBox_RecordType"
        Me.CheckedListBox_RecordType.Size = New System.Drawing.Size(102, 109)
        Me.CheckedListBox_RecordType.TabIndex = 1
        '
        'CounterResetButton
        '
        Me.CounterResetButton.Font = New System.Drawing.Font("SimSun", 15.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(134, Byte))
        Me.CounterResetButton.Location = New System.Drawing.Point(12, 378)
        Me.CounterResetButton.Name = "CounterResetButton"
        Me.CounterResetButton.Size = New System.Drawing.Size(176, 40)
        Me.CounterResetButton.TabIndex = 5
        Me.CounterResetButton.Text = "Counter Reset"
        Me.CounterResetButton.UseVisualStyleBackColor = True
        '
        'APC_MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.SystemColors.Highlight
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.ClientSize = New System.Drawing.Size(687, 726)
        Me.Controls.Add(Me.CounterResetButton)
        Me.Controls.Add(Me.CheckedListBox_RecordType)
        Me.Controls.Add(Me.ButtonRecord)
        Me.Controls.Add(Me.Label_Height)
        Me.Controls.Add(Me.Label_CHILD)
        Me.Controls.Add(Me.Label_ADULT)
        Me.Controls.Add(Me.Label_OUT)
        Me.Controls.Add(Me.Label_IN)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.Name = "APC_MainForm"
        Me.Text = "Automatic People Counter (DEMO)"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label_IN As System.Windows.Forms.Label
    Friend WithEvents Label_OUT As System.Windows.Forms.Label
    Friend WithEvents Label_ADULT As System.Windows.Forms.Label
    Friend WithEvents Label_CHILD As System.Windows.Forms.Label
    Friend WithEvents Label_Height As System.Windows.Forms.Label
    Friend WithEvents ButtonRecord As System.Windows.Forms.Button
    Friend WithEvents CheckedListBox_RecordType As System.Windows.Forms.CheckedListBox
    Friend WithEvents CounterResetButton As System.Windows.Forms.Button
End Class
