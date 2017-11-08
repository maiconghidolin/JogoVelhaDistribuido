Public Class EscolheJogador

    Private Sub btnConectar_Click(sender As Object, e As EventArgs) Handles btnConectar.Click
        If txtIP.Text = "" Or txtPorta.Text = "" Then
            MessageBox.Show("Informe a porta e o IP.", "Atenção")
            Exit Sub
        End If
        Form1._portaAdversario = txtPorta.Text
        Form1._IPAdversario = Net.IPAddress.Parse(txtIP.Text)
        Me.Close()
    End Sub

End Class