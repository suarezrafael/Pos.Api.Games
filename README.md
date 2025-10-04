# Pos.Api.Games - Game Management REST API

![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Build](https://img.shields.io/badge/build-passing-brightgreen)
![Tests](https://img.shields.io/badge/tests-10%20passed-brightgreen)

API REST desenvolvida em .NET 8 para gerenciar usuários e jogos comprados, com autenticação JWT, persistência via EF Core e arquitetura DDD.

## 📋 Índice

- [Funcionalidades](#-funcionalidades)
- [Tecnologias](#-tecnologias)
- [Arquitetura](#-arquitetura)
- [Pré-requisitos](#-pré-requisitos)
- [Instalação](#-instalação)
- [Configuração](#-configuração)
- [Uso](#-uso)
- [API Endpoints](#-api-endpoints)
- [Testes](#-testes)
- [Estrutura do Projeto](#-estrutura-do-projeto)
- [Segurança](#-segurança)
- [Contribuindo](#-contribuindo)
- [Licença](#-licença)

## 🚀 Funcionalidades

### Autenticação e Autorização
- ✅ Registro de usuários com validação de senha forte
- ✅ Login com autenticação JWT
- ✅ Perfis de usuário (User e Admin)
- ✅ Proteção de endpoints por roles

### Gestão de Jogos (Admin)
- ✅ Cadastro de jogos
- ✅ Atualização de jogos
- ✅ Exclusão de jogos
- ✅ Listagem pública de jogos

### Promoções (Admin)
- ✅ Criação de promoções com desconto percentual
- ✅ Gestão de período de validade
- ✅ Consulta de promoções ativas
- ✅ Aplicação automática de descontos

### Biblioteca do Usuário
- ✅ Compra de jogos
- ✅ Visualização da biblioteca pessoal
- ✅ Registro do preço pago (com desconto aplicado)
- ✅ Proteção contra compras duplicadas

### Gestão de Usuários (Admin)
- ✅ Listagem de todos os usuários
- ✅ Visualização de detalhes do usuário
- ✅ Exclusão de usuários

## 🛠 Tecnologias

- **Framework**: .NET 8.0
- **API**: Minimal APIs
- **ORM**: Entity Framework Core 9.0
- **Banco de Dados**: SQL Server (LocalDB)
- **Autenticação**: JWT Bearer Authentication
- **Validação**: FluentValidation
- **Documentação**: Swagger/OpenAPI
- **Testes**: xUnit, Moq, FluentAssertions, SpecFlow
- **Segurança**: BCrypt.Net-Next

## 🏗 Arquitetura

O projeto segue os princípios de **Domain-Driven Design (DDD)** com separação em camadas:

```
Pos.Api.Games/
├── src/
│   ├── Pos.Api.Games.Api/              # Camada de Apresentação (Endpoints)
│   ├── Pos.Api.Games.Application/      # Camada de Aplicação (Serviços e DTOs)
│   ├── Pos.Api.Games.Domain/           # Camada de Domínio (Entidades e Interfaces)
│   └── Pos.Api.Games.Infrastructure/   # Camada de Infraestrutura (EF Core)
└── tests/
    └── Pos.Api.Games.Tests/            # Testes Unitários e BDD
```

### Princípios Aplicados

- **DDD**: Separação clara de domínio, aplicação e infraestrutura
- **Repository Pattern**: Abstração do acesso a dados
- **Unit of Work**: Gerenciamento de transações
- **Dependency Injection**: Inversão de dependências
- **Clean Code**: Código limpo e bem documentado

## 📦 Pré-requisitos

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (ou SQL Server)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [Visual Studio Code](https://code.visualstudio.com/)

## 💻 Instalação

1. Clone o repositório:
```bash
git clone https://github.com/suarezrafael/Pos.Api.Games.git
cd Pos.Api.Games
```

2. Restaure as dependências:
```bash
dotnet restore
```

3. Crie o banco de dados:
```bash
dotnet ef database update --project src/Pos.Api.Games.Infrastructure --startup-project src/Pos.Api.Games.Api
```

4. Execute a aplicação:
```bash
dotnet run --project src/Pos.Api.Games.Api
```

A API estará disponível em `https://localhost:7000` ou `http://localhost:5000`

## ⚙ Configuração

### appsettings.json

Configure a connection string e as chaves JWT no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=PosApiGamesDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  },
  "Jwt": {
    "Key": "YourSuperSecretKeyForJwtTokenGeneration123!MustBeAtLeast32Characters",
    "Issuer": "Pos.Api.Games",
    "Audience": "Pos.Api.Games"
  }
}
```

**⚠️ IMPORTANTE**: Em produção, use variáveis de ambiente para informações sensíveis!

## 📖 Uso

### Swagger UI

Acesse a documentação interativa da API:
- Desenvolvimento: `https://localhost:7000/swagger`

### Fluxo de Uso

1. **Registrar um usuário**:
```bash
POST /api/auth/register
{
  "name": "João Silva",
  "email": "joao@example.com",
  "password": "SenhaForte@123"
}
```

2. **Login**:
```bash
POST /api/auth/login
{
  "email": "joao@example.com",
  "password": "SenhaForte@123"
}
```

3. **Usar o token** no header Authorization:
```
Authorization: Bearer {seu-token-jwt}
```

## 📡 API Endpoints

### Autenticação
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/register` | Registrar novo usuário | ❌ |
| POST | `/api/auth/login` | Login de usuário | ❌ |

### Jogos
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/games` | Listar todos os jogos | ❌ |
| GET | `/api/games/{id}` | Obter jogo por ID | ❌ |
| POST | `/api/games` | Criar novo jogo | ✅ Admin |
| PUT | `/api/games/{id}` | Atualizar jogo | ✅ Admin |
| DELETE | `/api/games/{id}` | Deletar jogo | ✅ Admin |
| POST | `/api/games/{id}/purchase` | Comprar jogo | ✅ User |
| GET | `/api/games/library/my` | Minha biblioteca | ✅ User |

### Promoções
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/promotions` | Listar todas as promoções | ❌ |
| GET | `/api/promotions/active` | Listar promoções ativas | ❌ |
| GET | `/api/promotions/{id}` | Obter promoção por ID | ❌ |
| POST | `/api/promotions` | Criar promoção | ✅ Admin |
| DELETE | `/api/promotions/{id}` | Deletar promoção | ✅ Admin |

### Usuários
| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/users` | Listar todos os usuários | ✅ Admin |
| GET | `/api/users/{id}` | Obter usuário por ID | ✅ User/Admin |
| DELETE | `/api/users/{id}` | Deletar usuário | ✅ Admin |

## 🧪 Testes

O projeto possui **10 testes** (6 unitários + 4 BDD):

### Executar Testes
```bash
dotnet test
```

### Testes Unitários (xUnit + Moq)
- Registro de usuário com dados válidos
- Registro com email existente
- Login com credenciais válidas
- Login com email inválido
- Login com senha inválida
- Geração de token JWT

### Testes BDD (SpecFlow)
- Cenário: Usuário registra com credenciais válidas
- Cenário: Usuário não pode registrar com senha fraca
- Cenário: Usuário faz login com credenciais válidas
- Cenário: Usuário não pode fazer login com credenciais inválidas

### Cobertura
- AuthService: 100%

## 📁 Estrutura do Projeto

```
Pos.Api.Games/
├── src/
│   ├── Pos.Api.Games.Api/
│   │   ├── Endpoints/              # Minimal API endpoints
│   │   ├── Program.cs              # Configuração da aplicação
│   │   └── appsettings.json        # Configurações
│   ├── Pos.Api.Games.Application/
│   │   ├── DTOs/                   # Data Transfer Objects
│   │   ├── Services/               # Serviços de aplicação
│   │   └── Validators/             # Validadores FluentValidation
│   ├── Pos.Api.Games.Domain/
│   │   ├── Entities/               # Entidades de domínio
│   │   └── Interfaces/             # Interfaces de repositórios
│   └── Pos.Api.Games.Infrastructure/
│       ├── Data/                   # DbContext
│       └── Repositories/           # Implementação de repositórios
└── tests/
    └── Pos.Api.Games.Tests/
        ├── Unit/                   # Testes unitários
        └── Integration/            # Testes BDD
```

## 🔒 Segurança

### Senha Forte
Requisitos obrigatórios:
- Mínimo 8 caracteres
- Pelo menos 1 letra maiúscula
- Pelo menos 1 letra minúscula
- Pelo menos 1 número
- Pelo menos 1 caractere especial

### Autenticação JWT
- Tokens com expiração de 24 horas
- Assinatura HMAC-SHA256
- Claims de usuário (ID, email, role)

### Proteção de Rotas
- Endpoints públicos: Listagem de jogos e promoções
- Endpoints autenticados: Compra de jogos, biblioteca
- Endpoints admin: Gestão de jogos, promoções e usuários

## 📊 Diagrama de Arquitetura

```
┌─────────────────────────────────────────────────┐
│               Pos.Api.Games.Api                 │
│         (Minimal APIs + Swagger)                │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│          Pos.Api.Games.Application              │
│   (Services, DTOs, Validators)                  │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│           Pos.Api.Games.Domain                  │
│     (Entities, Interfaces, Business Rules)      │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
┌─────────────────────────────────────────────────┐
│        Pos.Api.Games.Infrastructure             │
│  (EF Core, Repositories, Data Access)           │
└────────────────┬────────────────────────────────┘
                 │
                 ▼
          ┌──────────────┐
          │  SQL Server  │
          └──────────────┘
```

## 🤝 Contribuindo

Contribuições são bem-vindas! Por favor:

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📝 Licença

Este projeto está sob a licença MIT. Veja o arquivo [LICENSE](LICENSE) para mais detalhes.

## 👨‍💻 Autor

**Rafael Suarez**
- GitHub: [@suarezrafael](https://github.com/suarezrafael)

## 📞 Suporte

Para suporte, envie um email para o repositório ou abra uma issue.

---

⭐ Se este projeto te ajudou, considere dar uma estrela!