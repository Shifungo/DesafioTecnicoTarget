namespace Api.Models.Vendas;

public class ListaVendas
{
    public IList<Venda> Vendas { get; set; } = new List<Venda>();
}