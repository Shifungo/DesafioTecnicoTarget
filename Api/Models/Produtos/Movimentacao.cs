namespace Api.Models.Produtos;

public class Movimentacao
{
    public int CodigoMovimentacao { get; set; }
    public int CodigoProduto { get; set; }
    public int Quantidade { get; set; }
    public string DescricaoMovimentacao { get; set; } = string.Empty;
}