using System.Text.Json;
using Api.Models;
using Api.Models.Produtos;
using Api.Models.Vendas;
using Microsoft.AspNetCore.Mvc;

namespace Api.Services;

public class LojaService
{
    private static string CaminhoEstoque =>
        Path.Combine(Directory.GetCurrentDirectory(), "Data", "estoque.json");

    private static string CaminhoMovimentacoes =>
        Path.Combine(Directory.GetCurrentDirectory(), "Data", "movimentacoes.json");

    private static string CaminhoBackupEstoque =>
        Path.Combine(Directory.GetCurrentDirectory(), "Data", "backup", "estoqueBackup.json");

    public Task<IList<Vendedor?>> CalcularComissaoVendedores(ListaVendas vendas)
    {
        IList<Vendedor?> listaVendedores = new List<Vendedor?>();
        foreach (var venda in vendas.Vendas)
        {
            var comissao = 0.00m;

            if (venda.Valor >= 500)
                comissao = venda.Valor * 0.05m;

            if (venda.Valor > 100 && venda.Valor < 500)
                comissao = venda.Valor * 0.01m;

            var vendedorExistente = listaVendedores.FirstOrDefault(v => v?.Nome == venda.Vendedor);

            if (vendedorExistente != null)
                vendedorExistente.Comissao += comissao;
            else
                listaVendedores.Add(new Vendedor
                {
                    Nome = venda.Vendedor,
                    Comissao = comissao
                });
        }

        return Task.FromResult(listaVendedores);
    }

    public async Task<Movimentacao> FazerUmaMovimentacaoAsync(int codigoProduto, int quantidade, string descricao)
    {
        var json = await File.ReadAllTextAsync(CaminhoEstoque);
        var estoque = JsonSerializer.Deserialize<ListaProduto>(json);

        if (estoque == null)
            throw new Exception("Erro ao ler estoque.json");

        var produto = estoque.Estoque.FirstOrDefault(p => p.CodigoProduto == codigoProduto);

        if (produto == null)
            throw new Exception("Produto não encontrado");

        if (quantidade > produto.Estoque)
            throw new Exception("Quantidade maior que o estoque disponível");

        produto.Estoque -= quantidade;

        var estoqueAtualizadoJson =
            JsonSerializer.Serialize(estoque, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(CaminhoEstoque, estoqueAtualizadoJson);

        var jsonMov = await File.ReadAllTextAsync(CaminhoMovimentacoes);
        var movimentacoes = JsonSerializer.Deserialize<ListaMovimentacoes>(jsonMov) ?? new ListaMovimentacoes();

        int novoId = movimentacoes.Movimentacoes.Any()
            ? movimentacoes.Movimentacoes.Max(m => m.CodigoMovimentacao) + 1
            : 1;

        var novaMovimentacao = new Movimentacao
        {
            CodigoMovimentacao = novoId,
            CodigoProduto = codigoProduto,
            Quantidade = quantidade,
            DescricaoMovimentacao = descricao
        };

        movimentacoes.Movimentacoes.Add(novaMovimentacao);

        var movimentacoesJson =
            JsonSerializer.Serialize(movimentacoes, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(CaminhoMovimentacoes, movimentacoesJson);

        return novaMovimentacao;
    }

    public async Task<ListaMovimentacoes> ObterMovimentacoesAsync()
    {
        var jsonMov = await File.ReadAllTextAsync(CaminhoMovimentacoes);
        var movimentacoes = JsonSerializer.Deserialize<ListaMovimentacoes>(jsonMov) ?? new ListaMovimentacoes();
        return movimentacoes;
    }

    public async Task<ListaProduto> ObterEstoqueAsync()
    {
        var json = await File.ReadAllTextAsync(CaminhoEstoque);
        var estoque = JsonSerializer.Deserialize<ListaProduto>(json) ?? new ListaProduto();
        return estoque;
    }

    public async Task ResetarLoja()
    {
        File.Copy(CaminhoBackupEstoque, CaminhoEstoque, overwrite: true);
        await File.WriteAllTextAsync(CaminhoMovimentacoes, "{\"Movimentacoes\":[]}");
    }

    public async Task<decimal> CalcularJuros(decimal valorOriginal, DateTime dataVencimento, DateTime dataPagamento,
        bool composto)
    {
        var dias = (dataPagamento - dataVencimento).Days;
        if (composto)
        {
            for (int i = 0; i < dias; i++)
                valorOriginal += valorOriginal * 0.025m;

            return Math.Round(valorOriginal, 2);
        }

        var jutos = valorOriginal * 0.025m * dias;
        return valorOriginal + jutos;
    }
}