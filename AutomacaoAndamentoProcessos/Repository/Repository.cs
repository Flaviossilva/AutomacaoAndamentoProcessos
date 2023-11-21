using AutomacaoAndamentoProcessos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using static AutomacaoAndamentoProcessos.Models.StatusEnum;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace AutomacaoAndamentoProcessos.Repository
{
    internal class Repository
    {
        int NRegistros;
        public List<Solicitacao> RetornaSolicitacoes()
        {
            List<Solicitacao> Solicitacoes = new();
            for (int NOrg = 1; NOrg < 4; NOrg++)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"select top 50 * From Fila_Andamento where orgao='{NOrg}' and status='{(int)Status.Pendente}'", conexao);
                try
                {
                    conexao.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Solicitacao Lersolicitacao = new()
                        {
                            NumeroProcesso = reader[1].ToString(),
                            Pi = (int)reader[2],
                            Orgao = reader[4].ToString(),
                            Vencimento = Convert.ToInt32(reader[5].ToString()),
                            Status = Convert.ToInt32(reader[7].ToString()),
                            Tentativas = Convert.ToInt32(reader[8].ToString()),
                            Dt_Ult_Acao = Convert.ToDateTime(reader[9].ToString()),
                            Dt_Proxima_Acao = Convert.ToDateTime(reader[10].ToString()),
                        };
                        Solicitacoes.Add(Lersolicitacao);
                    }
                    conexao.Close();
                    if (Solicitacoes.Count > 0)
                    {
                        break;
                    }
                    NOrg++;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Solicitacoes;
        }
        public static List<Solicitacao> RetornaSolicitacoesOrg()
        {
            List<Solicitacao> Solicitacoes = new();

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select  * From Fila_Andamento where orgao is null and status='{(int)Status.Pendente}'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Solicitacao Lersolicitacao = new()
                    {
                        NumeroProcesso = reader[1].ToString(),
                        Pi = (int)reader[2],
                        Vencimento = Convert.ToInt32(reader[5].ToString()),
                        Status = Convert.ToInt32(reader[7].ToString()),
                        Tentativas = Convert.ToInt32(reader[8].ToString()),
                        Dt_Ult_Acao = Convert.ToDateTime(reader[9].ToString()),
                        Dt_Proxima_Acao = Convert.ToDateTime(reader[10].ToString()),
                    };
                    Solicitacoes.Add(Lersolicitacao);

                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return Solicitacoes;
        }
        public static List<Solicitacao> RetornaSolicitacoesEncerradas()
        {
            List<Solicitacao> Solicitacoes = new();

            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select a.Numero_Processo from Fila_Andamento as a inner join numeros_processos on numeros_processos.processo=a.Numero_Processo and encerrado !='false'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Solicitacao Lersolicitacao = new()
                    {
                        NumeroProcesso = reader[0].ToString(),
                    };
                    Solicitacoes.Add(Lersolicitacao);

                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return Solicitacoes;
        }
        public static List<Solicitacao> RetornaSolicitacoesAll()
        {
            List<Solicitacao> Solicitacoes = new();
            for (int NOrg = 1; NOrg < 4; NOrg++)
            {
                SqlConnection conexao = ConexaoBanco();
                SqlCommand cmd = new($"select * From Fila_Andamento where orgao=null", conexao);
                try
                {
                    conexao.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Solicitacao Lersolicitacao = new()
                        {
                            NumeroProcesso = reader[0].ToString(),
                            Pi = (int)reader[1],
                            Vencimento = Convert.ToInt32(reader[4].ToString()),
                            Status = Convert.ToInt32(reader[6].ToString()),
                            Tentativas = Convert.ToInt32(reader[7].ToString()),
                            Dt_Ult_Acao = Convert.ToDateTime(reader[8].ToString()),
                            Dt_Proxima_Acao = Convert.ToDateTime(reader[9].ToString()),
                        };
                        Solicitacoes.Add(Lersolicitacao);
                    }
                    conexao.Close();
                    if (Solicitacoes.Count > 0)
                    {
                        break;
                    }
                    NOrg++;
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Solicitacoes;
        }
        public static SqlConnection ConexaoBanco()
        {
            SqlConnectionStringBuilder builder = new()
            {
                //DataSource = "VC0003\\SQLEXPRESS01",
                DataSource = "MEDWS1",
                InitialCatalog = "Compromissos",
                UserID = "loteador",
                TrustServerCertificate = true,
                Password = "mediterraneo"
            };
            string conexao = builder.ConnectionString;
            SqlConnection conexaoBanco = new(conexao);
            return conexaoBanco;
        }
        public int GravarConsultados(Solicitacao Solicitacoes)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set[status]='{Solicitacoes.Status}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public int GravarLogErro(Solicitacao Solicitacoes)
        {
            SqlConnection conexao = ConexaoBanco();
            var tamanho = Solicitacoes.Log_Erro.Length;
            if (tamanho > 200)
            {
                tamanho = 200;
            }
            SqlCommand cmd = new();
            cmd = new($"update Fila_Andamento set Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
               conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public int GravarLogErroChrome(string Erro)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set Log_Erro='{Erro}' where numero_processo='11111'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public int InserirTextoTabelaFila(Solicitacao Processo)
        {
            foreach (var Andamento in Processo.Andamento)
            {


                SqlConnection conexao = ConexaoBanco();
                string TextoAntes;
                if (Andamento.Contains("Remetido as DJE") || Andamento.Contains("Disponibilizado no DJ Eletrônico"))
                {
                    TextoAntes = "DOE:";
                    Processo.Operador = "Publicação";
                }
                else
                {
                    TextoAntes = "NET:";
                    Processo.Operador = "NET";
                }
                var Aandamento = Andamento.Replace("'", " ");
                SqlCommand cmd = new($"insert into controle_andamentos(andamento_num_interno,andamento_data,andamento_andamento,andamento_operador) Values('{Processo.Pi}','{DateTime.Now}','{TextoAntes} {Aandamento}','{Processo.Operador}')", connection: conexao);
                conexao.Open();
                try
                {
                    NRegistros = cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception ex)
                {
                    Processo.Log_Erro = ex.Message;
                    GravarLogErro(Processo);
                    throw;
                }
            }
            return NRegistros;
        }
        public void AlimentarFila()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($" insert into Fila_Andamento(Numero_Processo,[PI],Dt_Proxima_Acao,Dt_Ult_Acao,Vencimento)select  processo,[PI],GETDATE(),GETDATE(),'7' from numeros_processos as a inner join controle as b on b.controle_proc_interno=a.[pi]  where processo not in  (select distinct Numero_Processo from Fila_Andamento) and b.controle_responsavel in('N.M.ADVOGADOS','AVARÉ') and b.controle_concluido='false' and b.controle_tipo_roteiro in('EF','EXH')", connection: conexao);
            SqlCommand cmd1 = new($" insert into Fila_Andamento(Numero_Processo,[PI],Dt_Proxima_Acao,Dt_Ult_Acao,Vencimento)select  processo,[PI],GETDATE(),GETDATE(),'2' from numeros_processos as a inner join controle as b on b.controle_proc_interno=a.[pi]  where  processo not in  (select distinct Numero_Processo from Fila_Andamento) and b.controle_responsavel in('N.M.ADVOGADOS','AVARÉ') and b.controle_concluido='false' and b.controle_tipo_roteiro not in('EF','EXH')", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conexao.Close();
                AlimentarFilaSeguinte();
            }
            catch (Exception )
            {
                throw;
            }
        }
        public void DeletarDuplicada()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new(@"Delete T FROM(SELECT *, DupRank = ROW_NUMBER() OVER (PARTITION BY Numero_Processo ORDER BY (SELECT NULL))FROM Fila_Andamento) AS T WHERE DupRank > 1", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception )
            {
                throw;
            }
        }
        public void AlimentarFilaSeguinte()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [STATUS]='1',log_Erro='' where Dt_Proxima_Acao='{DateTime.Now.AddDays(1)}' and [STATUS] in('3','4')", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
                DeletarDuplicada();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public void RetiraFila(string NProcesso)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [STATUS]='10' where numero_Processo='{NProcesso}'", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
        }
        public int RetirarTabelaFila(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            int novaOrg = Convert.ToInt32(Processo.Orgao) + 1;
            if (novaOrg > 3)
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Erro}',Dt_Ult_Acao='{DateTime.Now}',Dt_Proxia_Acao'{DateTime.Now.AddDays(7)}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
            else
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Pendente}',Dt_Ult_Acao='{DateTime.Now}',Orgao='{novaOrg}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception ex)
            {
                Processo.Log_Erro = ex.Message;
                GravarLogErro(Processo);
                throw;
            }
            return NRegistros;
        }
        public int InserirTextoTabelaAndamento(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();

            foreach (var Andamento in Processo.Andamento)
            {


                var AAndamento = Andamento.Replace("'", " ");
                SqlCommand cmd = new($"update Fila_Andamento set Andamento='{AAndamento}',[STATUS]='{(int)Status.Processado}',Dt_Proxima_Acao='{DateTime.Now.AddDays(Convert.ToDouble(Processo.Vencimento))}' where Numero_Processo='{Processo.NumeroProcesso}'", conexao);
                conexao.Open();
                try
                {
                    NRegistros = cmd.ExecuteNonQuery();
                    conexao.Close();
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return NRegistros;
        }
        public int InserirOrgTabela(string orgao, string numeroProcesso)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update  Fila_Andamento set Orgao='{orgao}' where numero_processo='{numeroProcesso}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public void InserirOrgNull()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update  Fila_Andamento set Orgao='1' where Orgao is null", conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

        }
        public int AtualizarTextoTabela(ProcessoAtual Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Controle_Andamentos set Andamento_Data='{DateTime.Now}' where Andamento_Num='{Processo.Id}'", conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public int AtualizarInserirTextoTabela(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"insert into controle_andamentos(andamento_num_interno,andamento_data,andamento_andamento,andamento_operador) Values('{Processo.Pi}','{DateTime.Now}','PROCESSO CONSULTADO PELA AUTOMAÇÃO','AUTOMACAO')", connection: conexao);
            conexao.Open();
            try
            {
                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return NRegistros;
        }
        public void AlterarConsultados(Solicitacao Solicitacoes)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set status='{(int)Status.Processando}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
            try
            {
                conexao.Open();
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
            return;
        }
        public void AlterarProcessando()
        {
            SqlConnection conexao = ConexaoBanco();
            conexao.Open();
            SqlCommand cmd = new($"update Fila_Andamento set status='{(int)Status.Pendente}' where status='{(int)Status.Processando}'", conexao);
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
            conexao.Close();
        }
        public int QuantidadeTotalRegistros()
        {
            int QuantidadeTotalRegistros = 0;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select Count(*) From Fila_Andamento where [STATUS]='1' and dt_proxima_acao<=GETDATE()", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    QuantidadeTotalRegistros = (int)reader[0];
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }
            return QuantidadeTotalRegistros;
        }
        public static ProcessoAtual RetornarDataUltmoRegistro(int Pi)
        {
            ProcessoAtual Processo = new();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"SELECT top 1 andamento_data,andamento_andamento,andamento_num from controle_andamentos where andamento_num_interno='{Pi}' and andamento_andamento like('PROCESSO CONSULTADO PELA AUTOMAÇÃO') order by andamento_data desc", conexao);
            try
            {
                conexao.Open();
                int retorno = cmd.ExecuteNonQuery();
                if (retorno < 0)
                {
                    cmd = new($"SELECT top 1 andamento_data,andamento_andamento,andamento_num from controle_andamentos where andamento_num_interno='{Pi}' order by andamento_data desc ", conexao);
                    cmd.ExecuteNonQuery();
                }
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Processo.Dt_Ult_Acao = Convert.ToDateTime(reader[0].ToString());
                    Processo.Andamento = reader[1].ToString();
                    Processo.Id = reader[2].ToString();
                }
                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return Processo;
        }
    }
}
