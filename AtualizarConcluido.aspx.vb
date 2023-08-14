Imports MySql.Data.MySqlClient
Imports System.Configuration
Imports System.IO
Imports Newtonsoft.Json.Linq
Imports System.Web.Script.Serialization

Public Class AtualizarConcluido
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Request.HttpMethod = "POST" Then
            Dim requestData As String = New System.IO.StreamReader(Request.InputStream).ReadToEnd()
            Dim data As Dictionary(Of String, Object) = New JavaScriptSerializer().Deserialize(Of Dictionary(Of String, Object))(requestData)

            Dim tarefaId As Integer = CInt(data("id"))
            Dim concluido As Boolean = CBool(data("concluido"))

            AtualizarConcluidoNaTabela(tarefaId, concluido)

            Dim responseJson As String = New JavaScriptSerializer().Serialize(New With {
                .success = True,
                .message = "Atualização concluída com sucesso"
            })

            Response.Clear()
            Response.ContentType = "application/json"
            Response.Write(responseJson)
            Response.End()
        Else
            Response.Write("Método não permitido")
        End If
    End Sub


    Private Sub AtualizarConcluidoNaTabela(ByVal tarefaId As Integer, ByVal concluido As Boolean)
        Dim connectionString As String = ConfigurationManager.ConnectionStrings("ConnectionStringTodoList").ConnectionString

        Using connection As New MySqlConnection(connectionString)
            connection.Open()

            Dim query As String = "UPDATE t1_tarefas SET CONCLUIDO = @Concluido WHERE ID = @TarefaId"
            Using cmd As New MySqlCommand(query, connection)
                cmd.Parameters.AddWithValue("@Concluido", concluido)
                cmd.Parameters.AddWithValue("@TarefaId", tarefaId)
                cmd.ExecuteNonQuery()
            End Using
        End Using
    End Sub
End Class
