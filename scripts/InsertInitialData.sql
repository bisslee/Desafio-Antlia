-- Script combinado para inserir dados iniciais (Produtos e COSIFs)
-- Execute este script após a migration para popular as tabelas com dados iniciais
-- Ordem de execução: 1. Produtos, 2. COSIFs

BEGIN TRANSACTION;

-- =====================================================
-- 1. INSERIR PRODUTOS
-- =====================================================
PRINT 'Inserindo produtos...';

-- Limpar dados existentes de produtos (opcional - descomente se necessário)
-- DELETE FROM PRODUTO;

-- Inserir produtos
INSERT INTO PRODUTO (Id, COD_PRODUTO, DES_PRODUTO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'SAV1', 'Conta Poupança', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CHK1', 'Conta Corrente', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'INV1', 'Fundo de Investimento', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CRD1', 'Cartão de Crédito', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'LON1', 'Empréstimo Pessoal', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'SAV2', 'Poupança Premium', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CHK2', 'Conta Empresarial', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'INV2', 'Fundo de Renda Fixa', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CRD2', 'Cartão Platinum', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'LON2', 'Empréstimo Imobiliário', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

PRINT 'Produtos inseridos com sucesso!';

-- =====================================================
-- 2. INSERIR COSIFs
-- =====================================================
PRINT 'Inserindo COSIFs...';

-- Limpar dados existentes de COSIFs (opcional - descomente se necessário)
-- DELETE FROM PRODUTO_COSIF;

-- Inserir COSIFs para Conta Poupança (SAV1)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'SAV1', '1.1.1.01.01', '1.1.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'SAV1', '1.1.1.01.02', '1.1.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Conta Corrente (CHK1)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'CHK1', '1.1.1.02.01', '1.1.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CHK1', '1.1.1.02.02', '1.1.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Fundo de Investimento (INV1)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'INV1', '1.2.1.01.01', '1.2.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'INV1', '1.2.1.01.02', '1.2.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'INV1', '1.2.1.01.03', '1.2.3', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Cartão de Crédito (CRD1)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'CRD1', '1.3.1.01.01', '1.3.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CRD1', '1.3.1.01.02', '1.3.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Empréstimo Pessoal (LON1)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'LON1', '1.3.1.02.01', '1.3.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'LON1', '1.3.1.02.02', '1.3.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Poupança Premium (SAV2)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'SAV2', '1.1.1.03.01', '1.1.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'SAV2', '1.1.1.03.02', '1.1.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Conta Empresarial (CHK2)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'CHK2', '1.1.1.04.01', '1.1.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CHK2', '1.1.1.04.02', '1.1.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Fundo de Renda Fixa (INV2)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'INV2', '1.2.1.02.01', '1.2.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'INV2', '1.2.1.02.02', '1.2.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Cartão Platinum (CRD2)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'CRD2', '1.3.1.03.01', '1.3.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'CRD2', '1.3.1.03.02', '1.3.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

-- Inserir COSIFs para Empréstimo Imobiliário (LON2)
INSERT INTO PRODUTO_COSIF (Id, COD_PRODUTO, COD_COSIF, COD_CLASSIFICACAO, CreatedAt, CreatedBy, UpdatedAt, UpdatedBy, STA_STATUS)
VALUES 
    (NEWID(), 'LON2', '1.3.1.04.01', '1.3.1', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active'),
    (NEWID(), 'LON2', '1.3.1.04.02', '1.3.2', GETDATE(), 'SYSTEM', GETDATE(), 'SYSTEM', 'Active');

PRINT 'COSIFs inseridos com sucesso!';

-- =====================================================
-- 3. VERIFICAR DADOS INSERIDOS
-- =====================================================
PRINT 'Verificando dados inseridos...';

-- Contar produtos inseridos
DECLARE @ProductCount INT = (SELECT COUNT(*) FROM PRODUTO);
PRINT 'Total de produtos inseridos: ' + CAST(@ProductCount AS VARCHAR(10));

-- Contar COSIFs inseridos
DECLARE @CosifCount INT = (SELECT COUNT(*) FROM PRODUTO_COSIF);
PRINT 'Total de COSIFs inseridos: ' + CAST(@CosifCount AS VARCHAR(10));

-- Mostrar resumo dos produtos
SELECT 'PRODUTOS' as Tabela, COD_PRODUTO as Codigo, DES_PRODUTO as Descricao FROM PRODUTO ORDER BY COD_PRODUTO;

-- Mostrar resumo dos COSIFs
SELECT 'COSIFS' as Tabela, COD_PRODUTO as Produto, COD_COSIF as Cosif, COD_CLASSIFICACAO as Classificacao FROM PRODUTO_COSIF ORDER BY COD_PRODUTO, COD_COSIF;

COMMIT TRANSACTION;

PRINT 'Script executado com sucesso! Todos os dados foram inseridos.'; 