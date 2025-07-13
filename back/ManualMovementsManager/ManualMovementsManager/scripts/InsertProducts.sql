-- Script para inserir produtos na tabela PRODUTO
-- Execute este script após a migration para popular a tabela com dados iniciais

-- Limpar dados existentes (opcional - descomente se necessário)
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

-- Verificar os dados inseridos
SELECT * FROM PRODUTO ORDER BY COD_PRODUTO; 