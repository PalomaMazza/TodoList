Imports MySql.Data.MySqlClient
Imports System
Imports System.Data
Imports System.Data.SqlClient
Imports System.Configuration
Imports System.Web.UI.Page
Imports System.Web.UI.HtmlControls
Public Class _Default

    Inherits Page

    Protected idedittarefa As HtmlInputText
    Protected editatitulotarefa As HtmlInputText
    Protected editadescricao As HtmlTextArea
    Protected titulotarefa As HtmlInputText
    Protected criadescricao As HtmlTextArea


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            CarregarTarefas()
        End If
    End Sub

    Protected Sub CarregarTarefas()
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim selectTarefasQuery As String = "SELECT ID, TITULO, DESCRICAO, CONCLUIDO FROM T1_TAREFAS"
            Using cmd As New MySqlCommand(selectTarefasQuery, connection)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    gridTarefas.DataSource = reader
                    gridTarefas.DataBind()
                End Using
            End Using
        End Using
    End Sub

    Protected Sub gridTarefas_RowCommand(ByVal sender As Object, ByVal e As GridViewCommandEventArgs) Handles gridTarefas.RowCommand
        If e.CommandName = "Excluir" Then
            Dim tarefaId As Integer = Convert.ToInt32(e.CommandArgument)
            ExcluirTarefa(tarefaId)
            CarregarTarefas()
        End If

        If e.CommandName = "Editar" Then
            Dim tarefaId As Integer = e.CommandArgument

            CarregarEditTarefas(tarefaId)

            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "AbrirModalEdicao", "$('#editarTarefaModal').modal('show');", True)
        End If
    End Sub
    Protected Sub ExcluirTarefa(ByVal tarefaId As Integer)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            'Excluir a tarefa da tabela T1_TAREFAS
            Dim deleteTarefaQuery As String = "DELETE FROM T1_TAREFAS WHERE ID = @TarefaId"
            Using cmd As New MySqlCommand(deleteTarefaQuery, connection)
                cmd.Parameters.AddWithValue("@TarefaId", tarefaId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
    Protected Sub CarregarEditTarefas(ID As Integer)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            'Busca os dados da tarefa
            Dim selectTarefasQuery As String = "SELECT ID, TITULO, DESCRICAO, CONCLUIDO FROM T1_TAREFAS WHERE ID = @ID"
            Using cmd As New MySqlCommand(selectTarefasQuery, connection)
                cmd.Parameters.AddWithValue("@ID", ID)
                Using reader As MySqlDataReader = cmd.ExecuteReader()
                    If reader.Read() Then
                        idedittarefa.Value = reader("ID").ToString()
                        editatitulotarefa.Value = reader("TITULO").ToString()
                        editadescricao.Value = reader("DESCRICAO").ToString()
                    End If
                End Using
            End Using

        End Using
    End Sub

    Protected Sub AtualizarConclusaoTarefa(ByVal tarefaId As Integer, ByVal concluido As Boolean)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim updateTarefaQuery As String = "UPDATE T1_TAREFAS SET CONCLUIDO = @Concluido WHERE ID = @TarefaId"
            Using cmd As New MySqlCommand(updateTarefaQuery, connection)
                cmd.Parameters.AddWithValue("@Concluido", concluido)
                cmd.Parameters.AddWithValue("@TarefaId", tarefaId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Protected Sub MarcarItensConcluidos(ByVal tarefaId As Integer)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim updateItensQuery As String = "UPDATE t2_itens_tarefas SET CONCLUIDO = true WHERE TAREFA_ID = @TarefaId"
            Using cmd As New MySqlCommand(updateItensQuery, connection)
                cmd.Parameters.AddWithValue("@TarefaId", tarefaId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub

    Protected Sub RemoverTarefa_Click(sender As Object, e As EventArgs)
        Dim button As Button = DirectCast(sender, Button)
        Dim tarefaId As Integer = Convert.ToInt32(button.CommandArgument)

        ' Conexão com o banco de dados
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            ' Excluir a tarefa da tabela T1_TAREFAS
            Dim deleteTarefaQuery As String = "DELETE FROM T1_TAREFAS WHERE ID = @TarefaId"
            Using cmd As New MySqlCommand(deleteTarefaQuery, connection)
                cmd.Parameters.AddWithValue("@TarefaId", tarefaId)
                cmd.ExecuteNonQuery()
            End Using
        End Using

        ' Recarregar as tarefas após a remoção
        CarregarTarefas()
    End Sub

    Protected Sub SalvarTarefa_Click(sender As Object, e As EventArgs)
        ' Verifique se os campos estão preenchidos
        If String.IsNullOrWhiteSpace(titulotarefa.Value) Or String.IsNullOrWhiteSpace(criadescricao.Value) Then
            ' Exiba uma mensagem de erro
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ShowError", "alert('Preencha todos os campos antes de salvar.');", True)
            Return
        End If

        ' Verifique se o título já existe na tabela
        If TarefaTituloExiste(titulotarefa.Value) Then
            ' Exiba uma mensagem de erro
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ShowError", "alert('Já existe uma tarefa com esse título.');", True)
            Return
        End If

        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            'Inserir dados na tabela T1_TAREFAS
            Dim insertTarefaQuery As String = "INSERT INTO T1_TAREFAS (Titulo, Descricao, Concluido) VALUES (@Titulo, @Descricao, 0)"
            Using cmd As New MySqlCommand(insertTarefaQuery, connection)
                cmd.Parameters.AddWithValue("@Titulo", titulotarefa.Value.ToString())
                cmd.Parameters.AddWithValue("@Descricao", criadescricao.Value.ToString())
                cmd.ExecuteNonQuery()
            End Using
        End Using

        'Recarregar as tarefas após a inserção
        CarregarTarefas()

        'Fechar o modal
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "CloseModalScript", "$('#criarTarefaModal').modal('hide');", True)
    End Sub

    Private Function TarefaTituloExiste(titulo As String) As Boolean
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim selectTarefaQuery As String = "SELECT COUNT(*) FROM T1_TAREFAS WHERE Titulo = @Titulo"
            Using cmd As New MySqlCommand(selectTarefaQuery, connection)
                cmd.Parameters.AddWithValue("@Titulo", titulo)
                Dim count As Integer = Convert.ToInt32(cmd.ExecuteScalar())
                Return count > 0
            End Using
        End Using
    End Function
    Protected Sub salvaredit_Click(sender As Object, e As EventArgs)
        Dim idTarefa As Integer = Integer.Parse(idedittarefa.Value)
        Dim novoTitulo As String = editatitulotarefa.Value

        'Atualizar a tabela t1_tarefas com o novo título e tipo de descrição
        AtualizarTarefa(idTarefa, novoTitulo)

        'Recarregar a GridView
        CarregarTarefas()

        'Fechar o modal
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "CloseModalScript", "$('#editarTarefaModal').modal('hide');", True)
    End Sub

    Private Sub AtualizarTarefa(idTarefa As Integer, novoTitulo As String)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim query As String
            Dim command As MySqlCommand


            query = "UPDATE t1_tarefas SET TITULO = @NovoTitulo, Descricao = @NovaDescricao WHERE ID = @IdTarefa"
                command = New MySqlCommand(query, connection)
                command.Parameters.AddWithValue("@NovoTitulo", novoTitulo)
            command.Parameters.AddWithValue("@NovaDescricao", editadescricao.Value)
            command.Parameters.AddWithValue("@IdTarefa", idTarefa)


            command.ExecuteNonQuery()
        End Using
    End Sub

End Class

