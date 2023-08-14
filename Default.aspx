<%@ Page Title="Home Page" Language="vb" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.vb" Inherits="TodoList._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .d-none {
            display: none !important;
        }
        .table {
        width: 75%;
        max-width: 75%;
        margin-bottom: 1rem;
        background-color: transparent;
        }

        .table th,
        .table td {
            padding: 0.75rem;
            vertical-align: top;
            border-top: 1px solid #dee2e6;
        }

        .table thead th {
            vertical-align: bottom;
            border-bottom: 2px solid #dee2e6;
        }

        .table tbody + tbody {
            border-top: 2px solid #dee2e6;
        }

        .table-sm th,
        .table-sm td {
            padding: 0.3rem;
        }

        .table-bordered {
            border: 1px solid #dee2e6;
        }

        .table-bordered th,
        .table-bordered td {
            border: 1px solid #dee2e6;
        }

        .table-bordered thead th,
        .table-bordered thead td {
            border-bottom-width: 2px;
        }

        .mensagem {
            display: block;
            margin-top: 10px;
            padding: 5px;
            text-align: center;
            font-weight: bold;
        }

        .sucesso {
            color: green;
        }

        .erro {
            color: red;
        }

        .riscado {
            text-decoration: line-through;
        }

    </style>
    <form runat="server">
        <div class="container">   
            <!-- Botão para abrir o modal -->
            <button type="button" class="btn btn-primary mb-3" data-toggle="modal" data-target="#criarTarefaModal">
                Adicionar Tarefa
            </button>
            <br />
      
            <!-- Tabela para exibir as tarefas existentes -->
            <div class="table-responsive">
                <br />
                <asp:GridView ID="gridTarefas" runat="server" OnRowDataBound="gridTarefas_RowDataBound" AutoGenerateColumns="False" CssClass="table table-striped" OnRowCommand="gridTarefas_RowCommand" DataKeyNames="ID">
                    <Columns>
                        <asp:BoundField DataField="ID" HeaderText="ID" Visible="false" />
                        <asp:BoundField DataField="TITULO" HeaderText="Tarefa" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />
                        <asp:BoundField DataField="DESCRICAO" HeaderText="Descrição" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" />                        
                        <asp:TemplateField HeaderText="Concluído" HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" ItemStyle-Width="100px">
                            <ItemTemplate>
                                <asp:CheckBox ID="chkConcluido" runat="server" CssClass="chkConcluido" Checked='<%# Convert.ToBoolean(Eval("CONCLUIDO")) %>' OnCheckedChanged="chkConcluido_CheckedChanged" AutoPostBack="true" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderStyle-CssClass="text-center" ItemStyle-CssClass="text-center" ItemStyle-Width="150px">
                            <ItemTemplate>
                                <asp:Button runat="server" CommandName="Editar" CommandArgument='<%# Eval("ID") %>' Text="Editar" CssClass="btn btn-info btn-sm" />
                                <asp:Button runat="server" CommandName="Excluir" CommandArgument='<%# Eval("ID") %>' Text="Excluir" CssClass="btn btn-danger btn-sm" OnClientClick="return confirm('Deseja realmente excluir esta tarefa?');" />
                            </ItemTemplate>
                        </asp:TemplateField>
                    </Columns>
                </asp:GridView>
            </div>
        </div>

         <!-- Modal para criar nova tarefa -->
        <div class="modal fade" id="criarTarefaModal" tabindex="-1" role="dialog" aria-labelledby="criarTarefaModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="criarTarefaModalLabel">Nova Tarefa
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </h5>
                    </div>
                    <div class="modal-body">
                        <!-- Formulário para criar nova tarefa -->
                        <div class="mb-3">
                            <label for="titulo" class="form-label">Título da Tarefa</label>
                            <input type="text" class="form-control" id="titulotarefa" runat="server" placeholder="Digite o título da tarefa">
                        </div>
                        <br />
                        <div class="mb-3">
                            <label for="descricao" class="form-label">Descrição da Tarefa</label>
                        </div>
                        <div class="mb-3">
                            <textarea id="criadescricao" runat="server" class="form-control" rows="5" placeholder="Digite a descrição da tarefa"></textarea>                    
                        </div>

                        <button type="button" runat="server" class="btn btn-primary" onserverclick="SalvarTarefa_Click" id="btnSalvarTarefa">Salvar</button>
                        <button type="button" class="btn btn-secondary" id="cancelarTarefa" data-dismiss="modal">Cancelar</button>
                
                    </div>
                </div>
            </div>
        </div>

        <!-- Modal para editar tarefa -->
        <div class="modal fade" id="editarTarefaModal" tabindex="-1" role="dialog" aria-labelledby="editarTarefaModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="editarTarefaModalLabel">Editar Tarefa
                            <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </h5>
                    </div>
                    <div class="modal-body">
                        <!-- Formulário para editar a tarefa -->
                        <div class="mb-3">
                            <input type="text" class="form-control" id="idedittarefa" visible="false" runat="server" />
                            <label for="edittarefa" class="form-label">Título da Tarefa</label>
                            <input type="text" class="form-control" id="editatitulotarefa" runat="server" />
                        </div>
                        <br />
                        <div class="mb-3">
                            <label for="descricao" class="form-label">Descrição da Tarefa</label>
                        </div>
                        <div class="mb-3">
                            <textarea id="editadescricao" runat="server" class="form-control" rows="5" placeholder="Digite a descrição da tarefa"></textarea>                    
                        </div>
                        
                        <button type="button" runat="server" class="btn btn-primary" onserverclick="salvaredit_Click" id="salvaredit">Salvar</button>
                        <button type="button" class="btn btn-secondary" id="cancelaredit" data-dismiss="modal">Cancelar</button>

                    </div>
                </div>
            </div>
        </div>
    </form>

    <script type="text/javascript">
        function updateTaskTitleStyle(checkbox) {
            var tarefaCell = $(checkbox).closest('tr').find('td:nth-child(2)');
            tarefaCell.toggleClass('riscado', checkbox.checked);
        }
    </script>


    <%--<script>
        function checkBoxClicked(checkbox) {
            console.log('checkBoxClicked function called');
            var tarefaCell = $(checkbox).closest('tr').find('td:nth-child(1)');
            var descricaoCell = $(checkbox).closest('tr').find('td:nth-child(2)');

            if (checkbox.checked) {
                tarefaCell.addClass('riscado');
                descricaoCell.addClass('riscado');
            } else {
                tarefaCell.removeClass('riscado');
                descricaoCell.removeClass('riscado');
            }

            return true;
        };


    </script>--%>

</asp:Content>
