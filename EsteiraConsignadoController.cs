using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ConsignadoEngine.Controllers
{
    // =========================================================================
    // 1. MODELOS DE DOMÍNIO (REGRAS DE NEGÓCIO SEGUNDO PADRÕES FEBRABAN)
    // =========================================================================
    public class PropostaConsignado
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        
        [Required(ErrorMessage = "O CPF do cliente é obrigatório.")]
        public string CpfCliente { get; set; }
        
        [Required]
        public decimal RendaMensalComprovada { get; set; }
        
        [Required]
        public decimal ValorParcelaDesejada { get; set; }
        
        [Required]
        public int QuantidadeParcelas { get; set; }
        
        public string TipoBeneficio { get; set; } // Exemplos: INSS, SIAPE, Governamental
        public string StatusProposta { get; set; } = "Em_Analise"; // Em_Analise, Aprovada, Recusada
        public string MotivoStatus { get; set; }
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }

    // =========================================================================
    // 2. CONTEXTO DO BANCO DE DADOS (ENTITY FRAMEWORK CORE / SQL)
    // =========================================================================
    public class ConsignadoDbContext : DbContext
    {
        public ConsignadoDbContext(DbContextOptions<ConsignadoDbContext> options) : base(options) { }
        public DbSet<PropostaConsignado> Propostas { get; set; }
    }

    // =========================================================================
    // 3. CONTROLADOR API (ENDPOINT ASSÍNCRONO DE ALTA PERFORMANCE)
    // =========================================================================
    [ApiController]
    [Route("api/[controller]")]
    public class EsteiraConsignadoController : ControllerBase
    {
        private readonly ConsignadoDbContext _context;

        public EsteiraConsignadoController(ConsignadoDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Endpoint assíncrono para análise e parametrização automática de propostas.
        /// Aplica validação de margem consignável (Lei 10.820 / Normativas FEBRABAN).
        /// </summary>
        [HttpPost("analisar-proposta")]
        public async Task<IActionResult> ProcessarProposta([FromBody] PropostaConsignado proposta)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Regra de Negócio Crítica: Cálculo da Margem Consignável Máxima Permitida (Padrão 35%)
            decimal margemMaximaPermitida = proposta.RendaMensalComprovada * 0.35m;

            // Validação 1: Estouro de Margem do Cliente
            if (proposta.ValorParcelaDesejada > margemMaximaPermitida)
            {
                proposta.StatusProposta = "Recusada";
                proposta.MotivoStatus = $"Recusa por estouro de margem. Parcela desejada excede o limite regulatório de 35% (Máximo: R$ {margemMaximaPermitida:F2}).";
            }
            // Validação 2: Limite Regulamentar de Prazo (Padrão INSS de 84 meses)
            else if (proposta.QuantidadeParcelas > 84)
            {
                proposta.StatusProposta = "Recusada";
                proposta.MotivoStatus = "Prazo de amortização excede o limite regulamentar permitido de 84 parcelas.";
            }
            // Sucesso: Proposta Parametrizada e Aprovada na Pré-Venda
            else
            {
                proposta.StatusProposta = "Aprovada";
                proposta.MotivoStatus = "Proposta em conformidade com as regras de margem consignável e políticas de crédito vigentes.";
            }

            // Persistência assíncrona no Banco de Dados Relacional
            _context.Propostas.Add(proposta);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                PropostaId = proposta.Id,
                Status = proposta.StatusProposta,
                Justificativa = proposta.MotivoStatus,
                MargemDisponivelCalculada = margemMaximaPermitida,
                DataProcessamento = proposta.DataCriacao
            });
        }
    }
}
