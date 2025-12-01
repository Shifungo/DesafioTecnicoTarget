using System.Text.Json.Serialization;

namespace Api.Models.Produtos;

public class ListaProduto
{
    [JsonPropertyName("estoque")]
    public List<Produto> Estoque { get; set; } = new();
}
