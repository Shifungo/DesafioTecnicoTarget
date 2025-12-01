## 🎯 Desafio Técnico Target Sistemas: API de Gestão de Loja

-----

## 1\. 💰 Módulo Comercial: Cálculo de Comissões (`/Loja/comissao`)

### Endpoint

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **`POST`** | `/Loja/comissao` | Calcula e retorna a comissão devida aos vendedores com base nas vendas. |

### Implementação

  * A rota recebe uma lista de vendas (`ListaVendas`) no **corpo da requisição** (`[FromBody]`).
  * O serviço `_lojaService` realiza o processamento das vendas para apurar as comissões individuais.

### Exemplo de Resultado (`200 OK`)

```json
[
  {
    "nome": "João Silva",
    "comissao": 495.677
  },
  {
    "nome": "Maria Souza",
    "comissao": 465.9495
  },
  {
    "nome": "Carlos Oliveira",
    "comissao": 379.3715
  },
  {
    "nome": "Ana Lima",
    "comissao": 404.9805
  }
]
```

-----

## 2\. 📦 Módulo Logística/Estoque: Movimentação e Rastreabilidade

Este módulo principal (`/Loja/movimentacao`) é suportado por três endpoints auxiliares que garantem a rastreabilidade e a gestão do inventário.

### Endpoint Principal: Registrar Movimentação

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **`POST`** | `/Loja/movimentacao` | Registra uma entrada ou saída no estoque para rastreamento. |

  * **Parâmetros (`[FromQuery]`):** `codigoProduto` (int), `quantidade` (int), `descricao` (string).
  * **Lógica de Negócio:** Realiza a **verificação de estoque** antes de registrar a movimentação e gera um registro de rastreabilidade.
  * **Validação:** Retorna `400 Bad Request` se a `quantidade` for menor ou igual a zero, ou em caso de exceção no serviço (ex: falta de estoque para saída).

### Endpoints Auxiliares (Gestão e Consulta)

Para facilitar a gestão e a auditoria das movimentações de estoque, foram criadas as seguintes rotas de consulta e utilidade:

| Método | Rota | Função Logística |
| :--- | :--- | :--- |
| **`GET`** | `/Loja/movimentacoes` | Traz **todas as movimentações** registradas, garantindo a rastreabilidade. |
| **`GET`** | `/Loja/produtos` | Traz o **estoque atual** (`ListaProduto`) para consulta de saldo. |
| **`PUT`** | `/Loja/resetar` | Reseta o estoque para o estado original e **remove todas as movimentações** (útil para testes). |

-----

## 3\. 💸 Módulo Financeiro: Cálculo de Juros (`/Loja/juros`)

### Endpoint

| Método | Rota | Descrição |
| :--- | :--- | :--- |
| **`GET`** | `/Loja/juros` | Calcula o valor dos juros sobre um título em atraso, com opção de cálculo simples ou composto. |

### Implementação

  * **Parâmetros (`[FromQuery]`):**
      * `valorOriginal` (decimal)
      * `dataVencimento` (string - formato **`dd/MM/yyyy`**)
      * `dataPagamento` (string, opcional - formato **`dd/MM/yyyy`**; assume `DateTime.Now` se nulo)
      * `jurosComposto` (bool, opcional - `false` por padrão)
  * **Flexibilidade:** Permite a escolha entre **juros simples** ou **juros compostos** no cálculo.
  * **Validações Críticas:**
      * Verifica o formato da data usando `DateTime.TryParseExact` (`dd/MM/yyyy`).
      * Garante que a data de pagamento não seja anterior à data de vencimento.
  * **Retorno:** O valor decimal dos juros calculados, ou `400 Bad Request` com a mensagem de erro da validação.
---


## 📝 Nota do Desenvolvedor

A estrutura, formatação e organização deste arquivo **README.md** foram geradas com o auxílio de uma **Ferramenta de Inteligência Artificial (IA)**. O código-fonte da API, a lógica de negócio, os *controllers* e a solução técnica em .NET Core 10 foram desenvolvidos integralmente por mim.

---
