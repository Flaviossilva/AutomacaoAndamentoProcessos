using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomacaoAndamentoProcessos.Models
{
    internal class StatusEnum
    {
        public enum Status
        {
            Pendente = 1,
            Processando = 2,
            Processado = 3,
            Desativado= 10,
            Erro = 4
        };

    }
}
