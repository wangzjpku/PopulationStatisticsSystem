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
        Me.Label_IN = New System.Windows.Forms.Label
        Me.Label_OUT = New System.Windows.Forms.Label
        Me.Label_ADULT = New System.Windows.Forms.Label
        Me.Label_CHILD = New System.Windows.Forms.Label
        Me.SuspendLayout()
        '
        'Label_IN
        '
        Me.Label_IN.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_IN.Font = New System.Drawing.Font("HGS創英角ｺﾞｼｯｸUB", 72.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_IN.Location = New System.Drawing.Point(296, 235)
        Me.Label_IN.Name = "Label_IN"
        Me.Label_IN.Size = New System.Drawing.Size(174, 101)
        Me.Label_IN.TabIndex = 0
        Me.Label_IN.Text = "0"
        Me.Label_IN.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_OUT
        '
        Me.Label_OUT.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_OUT.Font = New System.Drawing.Font("HGS創英角ｺﾞｼｯｸUB", 72.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_OUT.Location = New System.Drawing.Point(296, 371)
        Me.Label_OUT.Name = "Label_OUT"
        Me.Label_OUT.Size = New System.Drawing.Size(174, 101)
        Me.Label_OUT.TabIndex = 1
        Me.Label_OUT.Text = "0"
        Me.Label_OUT.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_ADULT
        '
        Me.Label_ADULT.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_ADULT.Font = New System.Drawing.Font("HGS創英角ｺﾞｼｯｸUB", 48.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_ADULT.Location = New System.Drawing.Point(760, 294)
        Me.Label_ADULT.Name = "Label_ADULT"
        Me.Label_ADULT.Size = New System.Drawing.Size(138, 76)
        Me.Label_ADULT.TabIndex = 2
        Me.Label_ADULT.Text = "0"
        Me.Label_ADULT.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'Label_CHILD
        '
        Me.Label_CHILD.BackColor = System.Drawing.Color.DarkSeaGreen
        Me.Label_CHILD.Font = New System.Drawing.Font("HGS創英角ｺﾞｼｯｸUB", 48.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(128, Byte))
        Me.Label_CHILD.Location = New System.Drawing.Point(760, 401)
        Me.Label_CHILD.Name = "Label_CHILD"
        Me.Label_CHILD.Size = New System.Drawing.Size(138, 76)
        Me.Label_CHILD.TabIndex = 3
        Me.Label_CHILD.Text = "0"
        Me.Label_CHILD.TextAlign = System.Drawing.ContentAlignment.MiddleRight
        '
        'APC_MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackgroundImage = CType(resources.GetObject("$this.BackgroundImage"), System.Drawing.Image)
        Me.ClientSize = New System.Drawing.Size(956, 1065)
        Me.Controls.Add(Me.Label_CHILD)
        Me.Controls.Add(Me.Label_ADULT)
        Me.Controls.Add(Me.Label_OUT)
        Me.Controls.Add(Me.Label_IN)
        Me.Name = "APC_MainForm"
        Me.Text = "Automatic People Counter (DEMO)"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Label_IN As System.Windows.Forms.Label
    Friend WithEvents Label_OUT As System.Windows.Forms.Label
    Friend WithEvents Label_ADULT As System.Windows.Forms.Label
    Friend WithEvents Label_CHILD As System.Windows.Forms.Label
End Class
