-- Script para inserir COSIFs na tabela PRODUTO_COSIF
-- Execute este script após a migration e após inserir os produtos para popular a tabela com dados iniciais

-- Limpar dados existentes (opcional - descomente se necessário)
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

-- Verificar os dados inseridos
SELECT * FROM PRODUTO_COSIF ORDER BY COD_PRODUTO, COD_COSIF; 