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

    Private _filaJogadores As List(Of Model.FilaJogadores)

    Delegate Sub SetStatusCallback([text] As String)
    Delegate Sub BotaoToggleCallback([botao] As Button, [enabled] As Boolean)
    Delegate Sub BotaoTextCallback([botao] As Button, [texto] As String)

#End Region

#Region "Eventos"

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        _jogoEmAndamento = False
        _timeStamp = 0
        _serializer = New JavaScriptSerializer
        _filaJogadores = New List(Of Model.FilaJogadores)
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

            Dim threadVerificaFila As Threading.Thread = New Threading.Thread(AddressOf VerificaFila)
            threadVerificaFila.Start()

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
        If Ganhou() Then
            EnviaMensagem(botao.Name, Model.Enumeradores.TipoMensagem.Ganhar, _socketAdversario.GetStream())
            SetStatus("Você ganhou!")
        Else
            If Empatou() Then
                EnviaMensagem(botao.Name, Model.Enumeradores.TipoMensagem.Empatar, _socketAdversario.GetStream())
                SetStatus("Jogo empatado!")
            Else
                EnviaMensagem(botao.Name, Model.Enumeradores.TipoMensagem.Jogada, _socketAdversario.GetStream())
                SetStatus("Aguardando o outro jogador!")
            End If
        End If
    End Sub

#End Region

#Region "Funções"

    Private Function Ganhou() As Boolean
        ' horizontal
        If (btn1A.Text = "X" And btn2A.Text = "X" And btn3A.Text = "X") Or
           (btn1B.Text = "X" And btn2B.Text = "X" And btn3B.Text = "X") Or
           (btn1C.Text = "X" And btn2C.Text = "X" And btn3C.Text = "X") Then
            Return True
        End If
        ' vertical
        If (btn1A.Text = "X" And btn1B.Text = "X" And btn1C.Text = "X") Or
           (btn2A.Text = "X" And btn2B.Text = "X" And btn2C.Text = "X") Or
           (btn3A.Text = "X" And btn3B.Text = "X" And btn3C.Text = "X") Then
            Return True
        End If
        ' diagonal
        If (btn1A.Text = "X" And btn2B.Text = "X" And btn3C.Text = "X") Or
           (btn3A.Text = "X" And btn2B.Text = "X" And btn1C.Text = "X") Then
            Return True
        End If

        Return False
    End Function

    Private Function Empatou() As Boolean
        Dim tabuleiroCompleto = True
        For Each controle In Me.Controls
            If TypeOf controle Is Button And controle.Tag <> "" Then
                If controle.Text = "" Then
                    tabuleiroCompleto = False
                    Exit For
                End If
            End If
        Next
        Return tabuleiroCompleto
    End Function

    Private Sub Perdeu()

    End Sub

    Private Sub BloquearBotoes()
        For Each controle In Me.Controls
            If TypeOf controle Is Button And controle.Tag <> "" Then
                BotaoToggle(controle, False)
            End If
        Next
    End Sub

    Private Sub DesbloquearBotoes()
        For Each controle In Me.Controls
            If TypeOf controle Is Button And controle.Tag <> "" Then
                BotaoToggle(controle, True)
            End If
        Next
    End Sub

    Private Sub EsperaNovaConexao()
        While True
            Dim socket = _tcpListener.AcceptTcpClient()

            Dim ctThread As Threading.Thread = New Threading.Thread(AddressOf MantemConexao)
            ctThread.Start(socket)
        End While
    End Sub

    Private Sub MantemConexao(socket As TcpClient)
        Dim networkStream As NetworkStream = socket.GetStream()
        While True
            Try
                Dim mensagem As Model.Mensagem = LeMensagem(networkStream, socket)

                If mensagem.TipoMensagem = Model.Enumeradores.TipoMensagem.EntrarFila Then
                    _filaJogadores.Add(New Model.FilaJogadores With {.IP = Net.IPAddress.Parse(mensagem.IP), .Porta = mensagem.Porta, .Timestamp = mensagem.Timestamp})
                    SetStatus("Novo jogador na fila!")
                End If
                If mensagem.TipoMensagem = Model.Enumeradores.TipoMensagem.Convite Then
                    AceitaConviteJogo()
                End If
                If mensagem.TipoMensagem = Model.Enumeradores.TipoMensagem.Jogada Then
                    PreencherJogadaAdversario(mensagem.Mensagem, mensagem.Timestamp)
                End If
                If mensagem.TipoMensagem = Model.Enumeradores.TipoMensagem.Ganhar Then
                    Dim botao As Button = DirectCast(Me.Controls(mensagem.Mensagem), Button)
                    BotaoText(botao, _outroJogador)
                    SetStatus("Jogador " & _outroJogador & " ganhou!")
                End If
                If mensagem.TipoMensagem = Model.Enumeradores.TipoMensagem.Empatar Then
                    Dim botao As Button = DirectCast(Me.Controls(mensagem.Mensagem), Button)
                    BotaoText(botao, _outroJogador)
                    SetStatus("Jogo empatado!")
                End If

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
            BotaoToggle(btnAdversario, False)
            enviaMensagem("", Model.Enumeradores.TipoMensagem.EntrarFila, _socketAdversario.GetStream())
            LimparTabuleiro(False)
        Catch ex As Exception
            MessageBox.Show("Não foi possível se conectar com o adversário.", "Atenção")
        End Try
    End Sub

    Private Sub VerificaFila()
        While True

            If Not _jogoEmAndamento Then
                If _filaJogadores IsNot Nothing AndAlso _filaJogadores.Count > 0 Then
                    _filaJogadores = _filaJogadores.OrderBy(Function(x) x.Timestamp).ToList()
                    Dim proximoJogador = _filaJogadores.First()
                    _filaJogadores.Remove(proximoJogador)
                    EnviaConviteJogo(proximoJogador)
                End If
            End If

            Threading.Thread.Sleep(2000)
        End While
    End Sub

    Private Sub EnviaConviteJogo(ByVal adversario As Model.FilaJogadores)
        _IPAdversario = adversario.IP
        _portaAdversario = adversario.Porta
        _socketAdversario = New TcpClient
        _socketAdversario.Connect(_IPAdversario, _portaAdversario)
        BotaoToggle(btnAdversario, False)
        LimparTabuleiro(True)
        enviaMensagem("", Model.Enumeradores.TipoMensagem.Convite, _socketAdversario.GetStream())
        _jogoEmAndamento = True
    End Sub

    Private Sub AceitaConviteJogo()
        LimparTabuleiro(False)
        _jogoEmAndamento = True
    End Sub

    Private Sub LimparTabuleiro(enabled As Boolean)
        For Each controle In Me.Controls
            If TypeOf controle Is Button And controle.Tag <> "" Then
                controle.Text = ""
                If enabled Then
                    BotaoToggle(controle, True)
                Else
                    BotaoToggle(controle, False)
                End If
            End If
        Next
    End Sub

    Private Sub PreencherJogadaAdversario(nomeBotao As String, timestamp As Integer)
        If timestamp > _timeStamp Then
            Dim botao As Button = DirectCast(Me.Controls(nomeBotao), Button)
            BotaoText(botao, _outroJogador)
            DesbloquearBotoes()
            SetStatus("Sua vez!")
        End If
    End Sub

#End Region

#Region "Helpers"

    Private Function LeMensagem(ByVal stream As NetworkStream, ByVal socket As TcpClient) As Model.Mensagem
        Dim bytes(socket.ReceiveBufferSize) As Byte
        stream.Read(bytes, 0, CInt(socket.ReceiveBufferSize))
        Dim returndata As String = Encoding.UTF8.GetString(bytes)
        Dim mensagem As Model.Mensagem = _serializer.Deserialize(Of Model.Mensagem)(returndata.Replace(vbNullChar, ""))
        Return mensagem
    End Function

    Private Sub EnviaMensagem(ByVal msg As String, ByVal tipoMensagem As Model.Enumeradores.TipoMensagem, ByVal stream As NetworkStream)
        _timeStamp += 1
        Dim mensagem As New Model.Mensagem
        mensagem.TipoMensagem = tipoMensagem
        mensagem.Timestamp = _timeStamp
        mensagem.Mensagem = msg
        mensagem.IP = txtIP.Text
        mensagem.Porta = txtPorta.Text

        Dim resultado As String = _serializer.Serialize(mensagem)
        Dim sendBytes As [Byte]() = Encoding.UTF8.GetBytes(resultado)
        stream.Write(sendBytes, 0, sendBytes.Length)
    End Sub

    Private Sub SetStatus(ByVal texto As String)
        If Me.txtStatus.InvokeRequired Then
            Dim d As New SetStatusCallback(AddressOf SetStatus)
            Me.Invoke(d, New Object() {texto})
        Else
            Me.txtStatus.Text = texto
        End If
    End Sub

    Private Sub BotaoToggle(ByVal botao As Button, ByVal enabled As Boolean)
        If botao.InvokeRequired Then
            Dim d As New BotaoToggleCallback(AddressOf BotaoToggle)
            Me.Invoke(d, New Object() {botao, enabled})
        Else
            botao.Enabled = enabled
        End If
    End Sub

    Private Sub BotaoText(ByVal botao As Button, ByVal texto As String)
        If botao.InvokeRequired Then
            Dim d As New BotaoTextCallback(AddressOf BotaoText)
            Me.Invoke(d, New Object() {botao, texto})
        Else
            botao.Text = texto
        End If
    End Sub

#End Region

End Class
