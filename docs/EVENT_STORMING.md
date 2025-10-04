# Event Storming - Pos.Api.Games

## O que é Event Storming?

Event Storming é uma técnica colaborativa de modelagem de domínio que foca em eventos do negócio para entender o sistema.

## Eventos de Domínio (Domain Events)

### Autenticação e Usuários
1. **UserRegistered** - Usuário se registrou no sistema
2. **UserLoggedIn** - Usuário fez login
3. **UserProfileUpdated** - Perfil do usuário foi atualizado
4. **UserDeleted** - Usuário foi removido do sistema

### Jogos
5. **GameCreated** - Novo jogo foi cadastrado
6. **GameUpdated** - Informações do jogo foram atualizadas
7. **GameDeleted** - Jogo foi removido do catálogo
8. **GamePurchased** - Usuário comprou um jogo

### Promoções
9. **PromotionCreated** - Nova promoção foi criada
10. **PromotionActivated** - Promoção ficou ativa
11. **PromotionExpired** - Promoção expirou
12. **PromotionDeleted** - Promoção foi removida

## Comandos (Commands)

### Autenticação
- **RegisterUser** → UserRegistered
- **LoginUser** → UserLoggedIn

### Gestão de Jogos (Admin)
- **CreateGame** → GameCreated
- **UpdateGame** → GameUpdated
- **DeleteGame** → GameDeleted

### Compras (User)
- **PurchaseGame** → GamePurchased

### Gestão de Promoções (Admin)
- **CreatePromotion** → PromotionCreated
- **DeletePromotion** → PromotionDeleted

### Gestão de Usuários (Admin)
- **DeleteUser** → UserDeleted

## Agregados (Aggregates)

### User Aggregate
- **Root**: User
- **Entidades**: PurchasedGame
- **Invariantes**:
  - Email único
  - Senha forte obrigatória
  - Não pode comprar o mesmo jogo duas vezes

### Game Aggregate
- **Root**: Game
- **Entidades**: Promotion
- **Invariantes**:
  - Preço deve ser maior que zero
  - Título obrigatório

### Promotion Aggregate
- **Root**: Promotion
- **Invariantes**:
  - Data de término deve ser maior que data de início
  - Desconto entre 0% e 100%

## Contextos Delimitados (Bounded Contexts)

### Identity Context
- Autenticação
- Autorização
- Gestão de usuários

### Catalog Context
- Catálogo de jogos
- Informações dos jogos

### Sales Context
- Compras
- Biblioteca do usuário
- Histórico de preços

### Marketing Context
- Promoções
- Descontos

## Fluxos Principais

### Fluxo de Registro e Compra
```
1. RegisterUser (User) → UserRegistered
2. LoginUser (User) → UserLoggedIn
3. PurchaseGame (User) → GamePurchased
```

### Fluxo de Administração
```
1. LoginUser (Admin) → UserLoggedIn
2. CreateGame (Admin) → GameCreated
3. CreatePromotion (Admin) → PromotionCreated
```

### Fluxo de Promoção
```
1. CreatePromotion (Admin) → PromotionCreated
2. Sistema aplica desconto automaticamente
3. PurchaseGame (User) → GamePurchased (com desconto)
```

## Políticas (Policies)

### Política de Desconto
**WHEN** PromotionCreated **AND** Promotion.IsActive **AND** Now >= StartDate **AND** Now <= EndDate
**THEN** Apply discount to Game.Price

### Política de Compra Única
**WHEN** PurchaseGame
**THEN** Check if user already owns the game
**IF** Yes → Reject purchase
**IF** No → Create PurchasedGame

### Política de Autenticação
**WHEN** UserRegistered
**THEN** Hash password with BCrypt
**AND** Assign role "User"
**AND** Generate JWT token

## Storytelling

### História 1: Novo Usuário Compra Jogo em Promoção

**Persona**: João, jogador casual de 25 anos

1. João acessa a API e vê a lista de jogos
2. Ele nota que "Cyberpunk 2077" está com 30% de desconto
3. João decide se registrar: nome, email e senha forte
4. Sistema valida os dados e cria o usuário com role "User"
5. João recebe um token JWT
6. João usa o token para comprar "Cyberpunk 2077"
7. Sistema verifica que João não possui o jogo
8. Sistema aplica o desconto de 30%
9. Sistema registra a compra com o preço pago (com desconto)
10. João agora pode ver o jogo em sua biblioteca

### História 2: Admin Cria Jogo e Promoção

**Persona**: Maria, administradora da plataforma

1. Maria faz login como Admin
2. Maria cadastra novo jogo "Elden Ring":
   - Título: "Elden Ring"
   - Descrição: "Action RPG..."
   - Gênero: "RPG"
   - Preço: R$ 199,90
   - Publisher: "FromSoftware"
3. Sistema valida os dados e cria o jogo
4. Maria decide criar uma promoção de lançamento:
   - Nome: "Promoção de Lançamento"
   - Desconto: 20%
   - Período: 7 dias
5. Sistema cria a promoção ativa
6. Usuários agora veem "Elden Ring" com preço promocional de R$ 159,92

## Métricas e Indicadores

### KPIs do Negócio
- Número de usuários registrados
- Número de jogos vendidos
- Taxa de conversão (visitas → compras)
- Valor médio de compra
- Efetividade de promoções

### Eventos para Análise
- GamePurchased → Analisa vendas
- UserRegistered → Crescimento de base
- PromotionCreated → Campanhas de marketing

## Pontos de Integração Futura

### Event Bus (Futuro)
- Publicar eventos para outros sistemas
- Notificações por email
- Dashboards em tempo real

### Microsserviços (Futuro)
- Identity Service
- Catalog Service
- Sales Service
- Marketing Service

## Conclusão

O Event Storming revelou:
- 4 bounded contexts principais
- 12 eventos de domínio
- 8 comandos principais
- 3 agregados core
- Políticas de negócio claras

Essa análise guiou o design DDD da aplicação e identificou oportunidades futuras de evolução para arquitetura orientada a eventos.
