using AutomacaoAndamentoProcessos.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AutomacaoAndamentoProcessos.Models.StatusEnum;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace AutomacaoAndamentoProcessos.Repository
{
    internal class Repository
    {
        int NRegistros;
        public List<Solicitacao> RetornaSolicitacoesPi(Solicitacao proc)
        {
            List<Solicitacao> Solicitacoes = new();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select * From Fila_Andamento where  [pi]='{proc.Pi}' and orgao='{proc.Orgao}' and [status] not in('10','11','4')", conexao);
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
            }
            catch (Exception)
            {
                throw;
            }
            return Solicitacoes;
        }

        public List<Solicitacao> RetornaSolicitacoes()
        {
            List<Solicitacao> Solicitacoes = new();
            for (int NOrg = 3; NOrg > 0; NOrg--)
            {
                SqlConnection conexao = ConexaoBanco();
                //SqlCommand cmd = new($"select * from Fila_Andamento where Numero_Processo in('0010062-70.2011.4.03.6119','0010398-74.2011.4.03.6119','0011006-72.2011.4.03.6119','0011365-22.2011.4.03.6119','0011366-07.2011.4.03.6119','5032010-84.2023.4.03.0000','5003594-03.2018.4.03.6105','0018042-13.2011.4.03.6105','0014067-46.2012.4.03.6105','0014533-40.2012.4.03.6105','0015905-24.2012.4.03.6105','0001726-71.2016.4.03.6905','0015801-32.2012.4.03.6105','0005954-69.2013.4.03.6105','0005958-09.2013.4.03.6105','0005973-75.2013.4.03.6105','0005991-96.2013.4.03.6105','0020602-49.2016.4.03.6105','0020615-48.2016.4.03.6105','0020618-03.2016.4.03.6105','0020663-07.2016.4.03.6105','0020614-63.2016.4.03.6105','0020654-45.2016.4.03.6105','0020656-15.2016.4.03.6105','0021506-69.2016.4.03.6105','5000264-09.2021.4.03.6132')", conexao);
                //SqlCommand cmd = new($"select * From Fila_Andamento  where pi in ('21880','23617','22049'  )", conexao); 
                SqlCommand cmd = new($"select top 50 * From Fila_Andamento where orgao='{NOrg}' and [status]='{(int)Status.Pendente}'  Order By Dt_Ult_Acao", conexao);
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
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return Solicitacoes;
        }

        public List<Solicitacao> RetornaSolicitacoesAtrasadas()
        {
            List<Solicitacao> Solicitacoes = new();
            for (int NOrg = 3; NOrg > 0; NOrg--)
            {
                SqlConnection conexao = ConexaoBanco();
                //SqlCommand cmd = new($"select top 50 * From Fila_Andamento where Numero_Processo in('0005932-77.2011.8.26.0197','0001065-61.1999.8.26.0197','197.01.2010.008831','0011574-36.2008.8.26.0197','0001207-94.2001.8.26.0197','0001250-31.2001.8.26.0197','0007021-58.1999.8.26.0197','0501276-75.2007.8.26.0224','0075845-27.1995.8.26.0224','0004035-58.2004.8.26.0197')", conexao);
                //SqlCommand cmd = new($"select top 50 * From Fila_Andamento where numero_processo='2317256'", conexao); 
                SqlCommand cmd = new($"select top 50 * From Fila_Andamento where orgao='{NOrg}' and [status]='{(int)Status.Processando}'  Order By Dt_Ult_Acao", conexao);
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
            SqlCommand cmd = new($"select a.Numero_Processo from Fila_Andamento as a inner join numeros_processos on numeros_processos.processo=a.Numero_Processo and encerrado = 1", conexao);
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
                DataSource = "MEDWS3",
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
        public int GravarLogErro(Solicitacao Solicitacoes, string numProc = null)
        {
            SqlConnection conexao = ConexaoBanco();
            int tamanho = 0;
            if (Solicitacoes.Log_Erro == null)
                Solicitacoes.Log_Erro = " ";
            else
                tamanho = Solicitacoes.Log_Erro.Length;

            if (tamanho > 200)
            {
                tamanho = 200;
            }
            Solicitacoes.Log_Erro = Solicitacoes.Log_Erro.Replace("'", " ");
            SqlCommand cmd = new();
            if (numProc == null)
                cmd = new($"update Fila_Andamento set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
            else
                cmd = new($"update Fila_Andamento set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_processo='{numProc}'", conexao);

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

        public int GravarLogErroUlt(Solicitacao Solicitacoes = null, string numProc = null)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            try
            {
                if (Solicitacoes.NumeroProcesso != null)
                    numProc = Solicitacoes.NumeroProcesso;

                var tamanho = Solicitacoes.Log_Erro.Length;
                if (tamanho > 200)
                {
                    tamanho = 200;
                }
                Solicitacoes.Log_Erro = Solicitacoes.Log_Erro.Replace("'", " ");

                if (numProc == null)
                    cmd = new($"update Fila_Andamento set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
                else
                    cmd = new($"update Fila_Andamento set [status]=4, Log_Erro='{Solicitacoes.Log_Erro.Substring(0, tamanho)}' where numero_processo='{numProc}'", conexao);

                conexao.Open();

                NRegistros = cmd.ExecuteNonQuery();
                conexao.Close();
            }
            catch (Exception)
            {
                return -1;
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
                //var tamanho = Andamento.Length-11;
                //if (Andamento.Substring(11,tamanho).ToLower().Contains("remetido ao dje relação") || Andamento.Substring(11, tamanho).ToLower().Contains("disponibilizado no dj eletrônico"))
                //{
                //    TextoAntes = $"DOE: {Processo.NumeroProcesso} -";
                //    Processo.Operador = "Publicação";
                //}
                //else

                TextoAntes = $"NET:{Processo.NumeroProcesso} -";
                Processo.Operador = "NET";
                var Aandamento = Andamento.Replace("'", " ");
                SqlCommand cmd = new($"insert into controle_andamentos(andamento_num_interno,andamento_data,andamento_andamento,andamento_operador) Values('{Processo.Pi}','{DateTime.Now.ToString("dd/MM/yyyy")}','{TextoAntes} {Aandamento}','{Processo.Operador}')", connection: conexao);
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
            SqlCommand cmd = new($" insert into Fila_Andamento(Numero_Processo,[PI],Dt_Proxima_Acao,Dt_Ult_Acao,Vencimento)select  processo,[PI],GETDATE(),GETDATE(),'15' from numeros_processos as a inner join controle as b on b.controle_proc_interno=a.[pi]  where processo not in  (select distinct Numero_Processo from Fila_Andamento) and b.controle_responsavel in('N.M.ADVOGADOS','AVARÉ') and b.controle_concluido='false' and b.controle_tipo_roteiro in('EF','EXH')", connection: conexao);
            SqlCommand cmd1 = new($" insert into Fila_Andamento(Numero_Processo,[PI],Dt_Proxima_Acao,Dt_Ult_Acao,Vencimento)select  processo,[PI],GETDATE(),GETDATE(),'1' from numeros_processos as a inner join controle as b on b.controle_proc_interno=a.[pi]  where  processo not in  (select distinct Numero_Processo from Fila_Andamento) and b.controle_responsavel in('N.M.ADVOGADOS','AVARÉ') and b.controle_concluido='false' and b.controle_tipo_roteiro not in('EF','EXH')", connection: conexao);
            conexao.Open();
            try
            {
                cmd.ExecuteNonQuery();
                cmd1.ExecuteNonQuery();
                conexao.Close();
                //Volta os casos que estão no status  4 e a data da proxima ação é amanha para o status 1 
                AlimentarFilaErro();
                //Volta os casos que estão no status 3 e 4 e a data da proxima ação é amanha para o status 1 
                AlimentarFilaSeguinte();
                //da um update status = 11 nos processos que existem na tabela fila mas não existem na tabela numero_processos
                RetirarFilaDeletados();
            }
            catch (Exception)
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
            catch (Exception)
            {
                throw;
            }
        }
        public void AlimentarFilaSeguinte()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [STATUS]='1',log_Erro='',Tentativas='0' where [STATUS] in('3','4') and Dt_Proxima_Acao<='{DateTime.Now.ToString("dd/MM/yyyy")}'", connection: conexao);
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

        public void AlimentarFilaErro()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [STATUS]='1',log_Erro='',Tentativas='0',orgao='1' where [STATUS] in('4') and log_erro like'%Processo não encontrado em nenhum site%' and orgao='3' and Dt_Proxima_Acao<='{DateTime.Now.ToString("dd/MM/yyyy")}'", connection: conexao);
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


        public void RetirarFilaDeletados()
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [Status]=11,Dt_Hora_Processamento='{DateTime.Now}'  where  not exists(select  processo from numeros_processos where  numeros_processos.processo=Fila_Andamento.Numero_Processo)", connection: conexao);
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
        public void RetiraFila(string? NProcesso)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"update Fila_Andamento set [STATUS]='10', Dt_Hora_Processamento='{DateTime.Now}' where numero_Processo='{NProcesso}'", connection: conexao);
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
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Erro}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.AddDays(5):dd/MM/yyyy}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}' and Tentativas='0'", connection: conexao);
            else
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Pendente}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}', Orgao='{novaOrg}',Dt_Hora_Processamento='{DateTime.Now}',Tentativas='0' where numero_processo='{Processo.NumeroProcesso}' and Tentativas='1'", connection: conexao);
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


        public int RetirarTabelaFilaDoiss(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            int novaOrg = Convert.ToInt32(Processo.Orgao) + 1;
            if (novaOrg > 3)
                cmd = new($"update Fila_Andamento set log_erro='Processo não encontrado em nenhum site', [Status]='{(int)Status.Erro}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.AddDays(5):dd/MM/yyyy}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
            else
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Pendente}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}', Orgao='{novaOrg}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}' and tentativas='1'", connection: conexao);
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
        public int RetirarTabelaFilaDois(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            int novaOrg = Convert.ToInt32(Processo.Orgao) + 1;
            if (novaOrg > 3)
                cmd = new($"update Fila_Andamento set log_erro='Processo não encontrado em nenhum site', [Status]='{(int)Status.Erro}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.AddDays(5):dd/MM/yyyy}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
            else
                cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Pendente}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}', Orgao='{novaOrg}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
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


        public int RetirarTabelaFilaPrimeira(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            cmd = new($"update Fila_Andamento set  Tentativas='1',[Status]='{(int)Status.Pendente}',Dt_Ult_Acao='{DateTime.Now:dd/MM/yyyy}',Dt_Proxima_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}', Orgao='{Processo.Orgao}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
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

        public int RetirarFilaDadosDesatualizados(Solicitacao Processo)
        {
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new();
            cmd = new($"update Fila_Andamento set [Status]='{(int)Status.Erro}',Log_Erro='Numero de Processo Desatualizado',Dt_Ult_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}',Dt_Proxima_Acao='{DateTime.Now.AddDays(5).ToString("dd/MM/yyyy")}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Processo.NumeroProcesso}'", connection: conexao);
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
            string? AAndamento = null;
            if (Processo.Andamento.Count > 0)
                AAndamento = Processo.Andamento[0].Replace("'", " ");

            SqlCommand cmd = new($"update Fila_Andamento set Andamento='{AAndamento}',[STATUS]='{(int)Status.Processado}',Dt_Ult_Acao='{DateTime.Now.ToString("dd/MM/yyyy")}',Dt_Proxima_Acao='{DateTime.Now.AddDays(Convert.ToDouble(Processo.Vencimento)).ToString("dd/MM/yyyy")}', Dt_Hora_Processamento='{DateTime.Now}',Dt_Ult_Andamento='{DateTime.Now}' where Numero_Processo='{Processo.NumeroProcesso}'", conexao);
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
            SqlCommand cmd = new($"update Controle_Andamentos set Andamento_Data='{DateTime.Now.AddMinutes(1).ToString("dd/MM/yyyy")}' where Andamento_Num='{Processo.Id}'", conexao);
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
            SqlCommand cmd = new($"insert into controle_andamentos(andamento_num_interno,andamento_data,andamento_andamento,andamento_operador) Values('{Processo.Pi}','{DateTime.Now.ToString("dd/MM/yyyy")}','PROCESSO CONSULTADO PELA AUTOMAÇÃO','AUTOMACAO')", connection: conexao);
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
            SqlCommand cmd = new($"update Fila_Andamento set [status]='{(int)Status.Processando}',Dt_Hora_Processamento='{DateTime.Now}' where numero_processo='{Solicitacoes.NumeroProcesso}'", conexao);
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
        public ProcessoAtual RetornarIdUltmoRegistro(int Pi)
        {
            ProcessoAtual Processo = new();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"SELECT top 1 andamento_data,andamento_andamento,andamento_num from controle_andamentos where andamento_num_interno='{Pi}' and andamento_andamento like('PROCESSO CONSULTADO PELA AUTOMAÇÃO') order by andamento_data desc", conexao);
            try
            {
                conexao.Open();
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
        public ProcessoAtual RetornarDataUltmoRegistro(int Pi)
        {
            ProcessoAtual Processo = new();
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"SELECT top 1 andamento_data,andamento_andamento,andamento_num from controle_andamentos where andamento_num_interno='{Pi}' and andamento_andamento like('PROCESSO CONSULTADO PELA AUTOMAÇÃO') order by andamento_data desc", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Processo.Dt_Ult_Acao = Convert.ToDateTime(reader[0].ToString());
                    Processo.Andamento = reader[1].ToString();
                    Processo.Id = reader[2].ToString();
                }

                if (Processo.Dt_Ult_Acao == null)
                {
                    conexao.Close();
                    conexao.Open();
                    cmd = new($"SELECT top 1 andamento_data,andamento_andamento,andamento_num from controle_andamentos where andamento_num_interno='{Pi}' order by andamento_data desc ", conexao);
                    cmd.ExecuteNonQuery();
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Processo.Dt_Ult_Acao = Convert.ToDateTime(reader[0].ToString());
                        Processo.Andamento = reader[1].ToString();
                        Processo.Id = reader[2].ToString();
                    }
                }


                conexao.Close();
            }
            catch (Exception)
            {
                throw;
            }

            return Processo;
        }
        public DateTime? RetornarDataUltmaAcao(string NumeroProcesso)
        {
            DateTime? Processo = null;
            SqlConnection conexao = ConexaoBanco();
            SqlCommand cmd = new($"select Dt_Ult_Andamento from Fila_Andamento where Numero_Processo='{NumeroProcesso}'", conexao);
            try
            {
                conexao.Open();
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader[0] != DBNull.Value && !string.IsNullOrEmpty(reader[0].ToString()))
                    {
                        Processo = Convert.ToDateTime(reader[0].ToString());
                    }
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
