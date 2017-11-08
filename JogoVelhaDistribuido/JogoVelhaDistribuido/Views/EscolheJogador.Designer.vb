<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class EscolheJogador
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.txtPorta = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtIP = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnConectar = New System.Windows.Forms.Button()
        Me.SuspendLayout()
        '
        'txtPorta
        '
        Me.txtPorta.BackColor = System.Drawing.SystemColors.ControlLight
        Me.txtPorta.Location = New System.Drawing.Point(36, 67)
        Me.txtPorta.Name = "txtPorta"
        Me.txtPorta.Size = New System.Drawing.Size(100, 20)
        Me.txtPorta.TabIndex = 18
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(34, 50)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(88, 13)
        Me.Label2.TabIndex = 17
        Me.Label2.Text = "Porta Adversário:"
        '
        'txtIP
        '
        Me.txtIP.BackColor = System.Drawing.SystemColors.ControlLight
        Me.txtIP.Location = New System.Drawing.Point(36, 26)
        Me.txtIP.Name = "txtIP"
        Me.txtIP.Size = New System.Drawing.Size(100, 20)
        Me.txtIP.TabIndex = 16
        Me.txtIP.Text = "127.0.0.1"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(34, 9)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 13)
        Me.Label1.TabIndex = 15
        Me.Label1.Text = "IP Adversário:"
        '
        'btnConectar
        '
        Me.btnConectar.Location = New System.Drawing.Point(36, 95)
        Me.btnConectar.Name = "btnConectar"
        Me.btnConectar.Size = New System.Drawing.Size(100, 23)
        Me.btnConectar.TabIndex = 14
        Me.btnConectar.Text = "Conectar"
        Me.btnConectar.UseVisualStyleBackColor = True
        '
        'EscolheJogador
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(170, 127)
        Me.ControlBox = False
        Me.Controls.Add(Me.txtPorta)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtIP)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnConectar)
        Me.Name = "EscolheJogador"
        Me.Text = "Escolher Jogador Adversário"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub

    Friend WithEvents txtPorta As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtIP As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents btnConectar As Button
End Class
