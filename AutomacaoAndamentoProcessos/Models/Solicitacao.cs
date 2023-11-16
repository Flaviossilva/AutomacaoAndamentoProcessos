using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoAndamentoProcessos.Models
{
    public class Solicitacao
    {
        public int Id { get; set; }
        public string? NumeroProcesso { get; set; }
        public int Pi { get; set; }
        public int? Vencimento { get; set; }
        public string? Log_Erro { get; set; }
        public int? Tentativas { get; set; }
        public DateTime? Dt_Ult_Acao { get; set; }
        public DateTime? Dt_Proxima_Acao { get; set; }
        public int Status { get; set; }
        public string? Orgao { get; set; }
        public string? Operador { get; set; }
        public List<string> Andamento = new List<string>();

    }
}
