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

```
src/
├── app/
│   ├── components/          # Componentes reutilizáveis
│   │   └── movement-form/   # Formulário de movimentos
│   ├── models/              # Interfaces e tipos TypeScript
│   │   ├── customer.model.ts
│   │   ├── product.model.ts
│   │   └── api-response.model.ts
│   ├── services/            # Serviços para comunicação com API
│   │   ├── api.service.ts
│   │   ├── customer.service.ts
│   │   ├── product.service.ts
│   │   └── manual-movement.service.ts
│   └── app.component.*      # Componente principal
├── environments/            # Configurações de ambiente
└── styles.css              # Estilos globais com Tailwind
```

## Pré-requisitos

- Node.js 18+ 
- npm ou yarn
- Angular CLI 18

## Instalação

1. Clone o repositório
2. Navegue para a pasta do projeto:
   ```bash
   cd front/manual-movements-frontend
   ```

3. Instale as dependências:
   ```bash
   npm install
   ```

## Desenvolvimento

Para iniciar o servidor de desenvolvimento:

```bash
ng serve
```

O aplicativo estará disponível em `http://localhost:4200/`.

## Build

Para gerar uma build de produção:

```bash
ng build
```

Os arquivos serão gerados na pasta `dist/`.

## Testes

Para executar os testes unitários:

```bash
ng test
```

Para executar os testes em modo watch:

```bash
ng test --watch
```

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

## Próximos Passos

- [ ] Implementar autenticação
- [ ] Adicionar paginação na listagem
- [ ] Implementar filtros avançados
- [ ] Adicionar gráficos e relatórios
- [ ] Implementar cache de dados
- [ ] Adicionar testes E2E
- [ ] Implementar PWA
- [ ] Adicionar internacionalização (i18n)
