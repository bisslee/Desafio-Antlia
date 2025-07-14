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

## 4. Plano de Execução - Frontend (Angular)

A interface será uma Single Page Application (SPA) desenvolvida em Angular, com foco em reatividade e experiência do usuário.

### 4.1. Estrutura dos Componentes

- **`MovementMainComponent`**: Componente "inteligente" (smart component) que orquestra a página, gerencia o estado e a comunicação com os serviços.
- **`MovementFormComponent`**: Componente "burro" (dumb component) que apenas exibe o formulário, emite eventos e recebe dados via `@Input`. Utiliza `ReactiveFormsModule` para validações.
- **`MovementGridComponent`**: Componente "burro" que exibe os dados em uma tabela do Angular Material (`<mat-table>`). Recebe a lista de movimentos via `@Input`.

### 4.2. Serviços

- **`MovementService`**: Encapsula toda a lógica de comunicação com a API para obter e criar movimentos.
- **`ProductService`**: Responsável por buscar os dados de Produtos e COSIFs para popular os `selects` do formulário.

### 4.3. Fluxo de Interação do Usuário

1. A página é carregada e o `MovementMainComponent` solicita ao `MovementService` a lista inicial de movimentos, que é exibida na `MovementGridComponent`.
2. O usuário clica no botão "Novo" no `MovementFormComponent`, que habilita os campos do formulário.
3. Os combos de "Produto" e "Cosif" são preenchidos através de chamadas ao `ProductService`.
4. O usuário preenche o formulário e clica em "Incluir".
5. O `MovementFormComponent` emite um evento com os dados do formulário.
6. O `MovementMainComponent` captura o evento, chama o `MovementService` para enviar os dados à API.
7. Após o sucesso da criação, o `MovementService` notifica o `MovementMainComponent`, que solicita a atualização da lista de movimentos.
8. A `MovementGridComponent` é atualizada automaticamente com o novo registro.
9. O formulário é limpo e desabilitado.

## Pré-requisitos

- Node.js 18+
- npm ou yarn
- Angular CLI 18

## API

O frontend se conecta com a API .NET que deve estar rodando em:

- **Desenvolvimento**: `https://localhost:7094/`
- **Produção**: Configurado via environment

## Funcionalidades

### Movimentos Manuais

- ✅ Formulário de inclusão de movimentos
- ✅ Validação de campos obrigatórios
- ✅ Listagem de movimentos
- ✅ Integração com API

### Produtos

- ✅ Listagem de produtos
- ✅ Seleção de produtos no formulário

### COSIF

- ✅ Listagem de códigos COSIF
- ✅ Seleção de COSIF no formulário

## Design System

O projeto utiliza um design system baseado no arquivo `index.html` original:

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
