Imports System.Net.Sockets
Imports System.Text
Imports System.Web.Script.Serialization

Public Class Form1

#Region "Variáveis"

    Private Const _jogadorAtual = "X"
    Private Const _outroJogador = "O"
    Private _jogoEmAndamento As Boolean
    Private _timeStamp As Integer
    Private _tcpListener As TcpListener
    Private _serializer As JavaScriptSerializer

    Public Shared _portaAdversario As Integer
    Public Shared _IPAdversario As Net.IPAddress
    Private _socketAdversario As TcpClient

    Private _filaJogadores As Queue(Of Object)

#End Region

#Region "Eventos"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _jogoEmAndamento = False
        _timeStamp = 0
        _serializer = New JavaScriptSerializer
        _filaJogadores = New Queue(Of Object)
    End Sub

    Private Sub Form1_FormClosed(sender As Object, e As FormClosedEventArgs) Handles MyBase.FormClosed
        _tcpListener.Stop()
        Environment.Exit(0)
    End Sub

    Private Sub btnConectar_Click(sender As Object, e As EventArgs) Handles btnConectar.Click
        Try
            _tcpListener = New TcpListener(Net.IPAddress.Parse(txtIP.Text), txtPorta.Text)
            _tcpListener.Start()

            Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf EsperaNovaConexao)
            ctThread.Start()

            txtIP.Enabled = False
            txtPorta.Enabled = False
            btnConectar.Enabled = False
        Catch ex As Exception
            MessageBox.Show("IP e porta já estão em uso ou são inválidos")
        End Try
    End Sub

    Private Sub btnAdversario_Click(sender As Object, e As EventArgs) Handles btnAdversario.Click
        Dim formEscolheAdversario As New EscolheJogador
        formEscolheAdversario.StartPosition = FormStartPosition.CenterParent
        formEscolheAdversario.ShowDialog()

        ConectarComAdversario()
    End Sub

    Private Sub button_Click(sender As Object, eventArgs As EventArgs) Handles btn3C.Click, btn3B.Click, btn3A.Click, btn2C.Click, btn2B.Click, btn2A.Click, btn1C.Click, btn1B.Click, btn1A.Click
        Dim botao As Button = DirectCast(sender, Button)
        botao.Text = _jogadorAtual
        BloquearBotoes()
        ' Envia para o outro jogado o botao.Name
        txtStatus.Text = "Aguardando o outro jogador!"
    End Sub

#End Region

#Region "Funções"

    Private Sub Ganhar()

    End Sub

    Private Sub BloquearBotoes()
        For Each controle In Me.Controls
            If TypeOf controle Is Button Then
                controle.Enabled = False
            End If
        Next
    End Sub

    Private Sub DesbloquearBotoes()
        For Each controle In Me.Controls
            If TypeOf controle Is Button Then
                If controle.Text = "" Then
                    controle.Enabled = True
                End If
            End If
        Next
    End Sub

    Private Sub EsperaNovaConexao()
        While True
            Dim socket = _tcpListener.AcceptTcpClient()
            _filaJogadores.Enqueue(New With {.Socket = socket})

            Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf MantemConexao)
            ctThread.Start(socket)
        End While
    End Sub

    Private Sub MantemConexao(socket As TcpClient)
        Dim networkStream As NetworkStream = socket.GetStream()
        While True
            Try
                Dim mensagem As Model.Mensagem = LeMensagem(networkStream, socket)
                _timeStamp = Math.Max(_timeStamp, mensagem.Timestamp)

            Catch ex As Exception
                Exit While
            End Try
        End While
    End Sub

    Private Sub ConectarComAdversario()
        Try
            _socketAdversario = New TcpClient
            _socketAdversario.Connect(_IPAdversario, _portaAdversario)
        Catch ex As Exception
            MessageBox.Show("Não foi possível se conectar com o adversário.", "Atenção")
        End Try
    End Sub

    Private Function LeMensagem(ByVal stream As NetworkStream, ByVal socket As TcpClient) As Model.Mensagem
        Dim bytes(socket.ReceiveBufferSize) As Byte
        stream.Read(bytes, 0, CInt(socket.ReceiveBufferSize))
        Dim returndata As String = Encoding.UTF8.GetString(bytes)
        Dim mensagem As Model.Mensagem = _serializer.Deserialize(Of Model.Mensagem)(returndata.Replace(vbNullChar, ""))
        Return mensagem
    End Function

    Private Sub enviaMensagem(ByVal msg As String, ByVal tipoMensagm As Model.Enumeradores.TipoMensagem, ByVal stream As NetworkStream)
        _timeStamp += 1
        Dim mensagem As New Model.Mensagem
        mensagem.TipoMensagm = tipoMensagm
        mensagem.Timestamp = _timeStamp
        mensagem.Mensagem = msg

        Dim resultado As String = _serializer.Serialize(mensagem)
        Dim sendBytes As [Byte]() = Encoding.UTF8.GetBytes(resultado)
        stream.Write(sendBytes, 0, sendBytes.Length)
    End Sub

    Private Sub VerificaFila()
        ' verifica se tem jogador na fila e chama para jogar
        While True

        End While
    End Sub

#End Region

End Class
