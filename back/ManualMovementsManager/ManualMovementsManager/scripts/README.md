# Scripts para Inserir Dados no Banco

## O que são estes scripts?

Estes scripts inserem dados de exemplo no banco de dados para você testar o sistema.

## Qual script usar?

**Use o arquivo `InsertInitialData.sql`** - ele insere tudo de uma vez (produtos + COSIFs).

## Como executar?

### Passo 1: Execute a migration

```bash
PM> dotnet ef migrations remove
PM> dotnet ef migrations add Initial-ManualMovementsManager-Migrations -p ..\ManualMovementsManager.Infrastructure
PM> dotnet ef database update
```

### Passo 2: Execute o script

1. Abra o **SQL Server Management Studio**
2. Conecte no seu banco de dados
3. Abra o arquivo `InsertInitialData.sql`
4. Clique em **Executar** (F5)

### Passo 3: Verifique se funcionou

```sql
SELECT * FROM PRODUTO;
SELECT * FROM PRODUTO_COSIF;
```

## O que será inserido?

**10 Produtos:**

- SAV1 - Conta Poupança
- CHK1 - Conta Corrente
- INV1 - Fundo de Investimento
- CRD1 - Cartão de Crédito
- LON1 - Empréstimo Pessoal
- SAV2 - Poupança Premium
- CHK2 - Conta Empresarial
- INV2 - Fundo de Renda Fixa
- CRD2 - Cartão Platinum
- LON2 - Empréstimo Imobiliário

**20 COSIFs** (códigos contábeis para cada produto)

## Pronto

Agora você pode testar o sistema com dados reais no banco.
