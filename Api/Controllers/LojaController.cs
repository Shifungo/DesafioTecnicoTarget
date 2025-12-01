using System.Globalization;
using Api.Models;
using Api.Models.Produtos;
using Api.Models.Vendas;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LojaController : Controller
{
    private readonly LojaService _lojaService;

    public LojaController(LojaService lojaService)
    {
        _lojaService = lojaService;
    }

    //Tarefa 1
    [HttpPost]
    [Route("comissao")]
    public async Task<ActionResult<IList<Vendedor?>>> ObterComissoes([FromBody] ListaVendas vendas)
    {
        var resultado = await _lojaService.CalcularComissaoVendedores(vendas);
        return Ok(resultado);
    }

    //Tarefa 2
    [HttpPost]
    [Route("movimentacao")]
    public async Task<ActionResult<Movimentacao>> FazerMovimentacao([FromQuery] int codigoProduto,
        [FromQuery] int quantidade, [FromQuery] string descricao)
    {
        if (quantidade <= 0) return BadRequest("Quantidade deve ser maior que zero.");
        try
        {
            var resultado = await _lojaService.FazerUmaMovimentacaoAsync(codigoProduto, quantidade, descricao);
            return Ok(resultado);
        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }

    //tarefa 3
    [HttpGet]
    [Route("juros")]
    public async Task<ActionResult<Decimal>> CalcularJuros([FromQuery] decimal valorOriginal,
        [FromQuery] string dataVencimento, [FromQuery] string? dataPagamento = null,
        [FromQuery] bool jurosComposto = false)
    {
        string formatoEsperado = "dd/MM/yyyy";
        DateTime vencimentoValido;
        DateTime pagamentoValido;

        if (!DateTime.TryParseExact(dataVencimento, formatoEsperado, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out vencimentoValido))
        {
            return BadRequest($"Data de vencimento inválida. Use o formato {formatoEsperado}.");
        }

        if (string.IsNullOrEmpty(dataPagamento))
        {
            pagamentoValido = DateTime.Now;
        }
        else
        {
            if (!DateTime.TryParseExact(dataPagamento, formatoEsperado, CultureInfo.InvariantCulture,
                    DateTimeStyles.None, out pagamentoValido))
            {
                return BadRequest($"Data de pagamento inválida. Use o formato {formatoEsperado}.");
            }
        }

        if (pagamentoValido < vencimentoValido)
            return BadRequest("A data de pagamento não pode ser menor que a data de vencimento.");

        var resultado =
            await _lojaService.CalcularJuros(valorOriginal, vencimentoValido, pagamentoValido, jurosComposto);
        return Ok(resultado);
    }

    [HttpGet]
    [Route("movimentacoes")]
    public async Task<ActionResult<ListaMovimentacoes>> ObterMovimentacoes()
    {
        var resultado = await _lojaService.ObterMovimentacoesAsync();
        return Ok(resultado);
    }

    [HttpGet]
    [Route("produtos")]
    public async Task<ActionResult<ListaProduto>> ObterEstoque()
    {
        var resultado = await _lojaService.ObterEstoqueAsync();
        return Ok(resultado);
    }

    [HttpPut]
    [Route("loja/resetar")]
    public async Task ResetarLoja()
    {
        await _lojaService.ResetarLoja();
    }
}