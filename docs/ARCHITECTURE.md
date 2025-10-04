# Arquitetura do Sistema - Pos.Api.Games

## Visão Geral

O sistema segue os princípios de **Domain-Driven Design (DDD)** com uma arquitetura em camadas limpa e bem definida.

## Camadas da Aplicação

### 1. API Layer (Pos.Api.Games.Api)
**Responsabilidade**: Apresentação e Endpoints HTTP

- **Tecnologia**: ASP.NET Core Minimal APIs
- **Componentes**:
  - `Program.cs`: Configuração da aplicação, DI, middleware
  - `Endpoints/`: Mapeamento de rotas HTTP
  - `appsettings.json`: Configurações da aplicação

### 2. Application Layer (Pos.Api.Games.Application)
**Responsabilidade**: Lógica de Aplicação e Orquestração

- **DTOs**: Objetos de transferência de dados
- **Services**: Serviços de aplicação (AuthService, GameService, PromotionService, UserService)
- **Validators**: Validadores FluentValidation

### 3. Domain Layer (Pos.Api.Games.Domain)
**Responsabilidade**: Núcleo do Negócio

- **Entities**: User, Game, PurchasedGame, Promotion, UserRole
- **Interfaces**: IRepository<T>, IUnitOfWork

### 4. Infrastructure Layer (Pos.Api.Games.Infrastructure)
**Responsabilidade**: Acesso a Dados e Infraestrutura

- **Data**: ApplicationDbContext, Migrations
- **Repositories**: Repository<T>, UnitOfWork

## Fluxo de Dados

```
Cliente (HTTP) → API Layer → Application Layer → Domain Layer → Database
```

## Padrões de Design

- **Repository Pattern**: Abstrai acesso a dados
- **Unit of Work**: Coordena múltiplas operações
- **Dependency Injection**: Inversão de controle
- **DTO Pattern**: Separa modelo de domínio de transferência

## Segurança

- JWT Bearer Authentication
- BCrypt para hash de senhas
- Role-based Authorization (User, Admin)
- Validação de senha forte

## Banco de Dados

Entidades e relacionamentos:
- User ← PurchasedGame → Game
- Game → Promotion

## Testes

- **Unitários**: xUnit + Moq
- **BDD**: SpecFlow
- Cobertura: AuthService 100%
