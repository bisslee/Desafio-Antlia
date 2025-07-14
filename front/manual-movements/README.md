# Manual Movements Frontend

Este é o frontend da aplicação Manual Movements, desenvolvido em Angular 18 com TypeScript e Tailwind CSS.

## Características

- **Angular 18**: Framework mais recente do Angular
- **TypeScript**: Tipagem estática para melhor desenvolvimento
- **Tailwind CSS**: Framework CSS utilitário para design responsivo
- **Reactive Forms**: Formulários reativos com validação
- **Componentes Standalone**: Arquitetura moderna do Angular
- **Testes Unitários**: Cobertura de testes com Jasmine/Karma
- **HTTP Client**: Comunicação com API REST
- **Design Responsivo**: Interface adaptável a diferentes dispositivos

## Estrutura do Projeto

```text
src/
├── app/
│   ├── components/
│   │   ├── movement-main/          # Componente principal
│   │   ├── movement-form/          # Formulário de movimentos
│   │   └── movement-grid/          # Grid de movimentos
│   ├── models/                     # Interfaces TypeScript
│   ├── services/                   # Serviços de API
│   └── environments/               # Configurações de ambiente
├── styles.css                      # Estilos globais
└── index.html                      # Template principal
```

## Funcionalidades Implementadas

### ✅ Movimentos Manuais

- Formulário de inclusão de movimentos
- Validação de campos obrigatórios
- Listagem de movimentos
- Integração com API

### ✅ Produtos

- Listagem de produtos
- Seleção de produtos no formulário

### ✅ COSIF

- Listagem de códigos COSIF
- Seleção de COSIF no formulário
- Filtragem por produto selecionado

## Pré-requisitos

- Node.js 18+
- npm ou yarn
- Angular CLI 18

## Instalação

1. Clone o repositório
2. Instale as dependências:

```bash
npm install
```

## Desenvolvimento

Para iniciar o servidor de desenvolvimento:

```bash
ng serve
```

A aplicação estará disponível em `http://localhost:4200/`

## Build

Para gerar o build de produção:

```bash
ng build
```

## API

O frontend se conecta com a API .NET que deve estar rodando em:

- **Desenvolvimento**: `https://localhost:7094/`
- **Produção**: Configurado via environment

### Configuração de Dados Mock vs API Real

Para alternar entre dados mock e API real, edite o arquivo `src/environments/environment.ts`:

```typescript
// Para usar API real (padrão)
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7094/api/v1',
  useMockData: false
};

// Para usar dados mock
export const environment = {
  production: false,
  apiUrl: 'https://localhost:7094/api/v1',
  useMockData: true
};
```

**Dados Mock Incluídos:**
- 5 produtos diferentes (SAV001, CHK002, INV003, CRD004, LON005)
- COSIFs filtrados por produto
- 2 movimentos de exemplo

## Design System

O projeto utiliza um design system baseado no arquivo `modelo.html`:

### Cores

- **Primary**: `#3b82f6` (Azul)
- **Brand Blue**: `#2D6CB8` (Azul da marca)
- **Secondary**: `#64748b` (Cinza)

### Componentes CSS

- `.btn-primary`: Botão primário
- `.btn-secondary`: Botão secundário
- `.btn-outline`: Botão outline
- `.form-input`: Campo de entrada
- `.form-select`: Campo de seleção
- `.card`: Container de card
- `.table-header`: Cabeçalho de tabela
- `.table-cell`: Célula de tabela

## Arquitetura

### Componentes

- **`MovementMainComponent`**: Componente "inteligente" que orquestra a página
- **`MovementFormComponent`**: Componente "burro" que exibe o formulário
- **`MovementGridComponent`**: Componente "burro" que exibe os dados em tabela

### Serviços

- **`MovementService`**: Comunicação com API de movimentos
- **`ProductService`**: Comunicação com API de produtos e COSIF

### Fluxo de Interação

1. Carregamento inicial dos movimentos e produtos
2. Seleção de produto carrega COSIFs correspondentes
3. Preenchimento e validação do formulário
4. Envio para API e atualização da lista
5. Reset do formulário

## Melhores Práticas Implementadas

1. **Arquitetura Modular**: Separação clara de responsabilidades
2. **TypeScript**: Tipagem estática em todo o projeto
3. **Reactive Forms**: Formulários com validação robusta
4. **Serviços**: Comunicação com API centralizada
5. **Componentes Standalone**: Arquitetura moderna do Angular
6. **Testes Unitários**: Cobertura de testes
7. **Responsividade**: Design adaptável
8. **Acessibilidade**: Labels e estrutura semântica
9. **Performance**: Lazy loading e otimizações
10. **Manutenibilidade**: Código limpo e bem estruturado
