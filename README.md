# Microondas Digital

Esta é uma aplicação ASP.NET Core MVC que simula o painel e funcionalidades de um micro-ondas digital. A arquitetura é composta por dois projetos: **MicroondasDigital** (servidor API) e **MicroondasCliente** (cliente web). Permite iniciar, pausar, retomar e parar o aquecimento, além de escolher receitas predefinidas ou programas customizados.

## Tecnologias

- Linguagem: C#
- Framework: .NET 10 (ASP.NET Core MVC)
- Front‑end: Razor views + Bootstrap 5
- Autenticação: JWT (JSON Web Tokens)
- Sem banco de dados (gravação em arquivo JSON)

## Arquitetura

### MicroondasDigital (Servidor API)
- Porta: `5161`
- Responsável por gerenciar o estado do micro-ondas
- Fornece endpoints REST com autenticação JWT
- Mantém dados em sessão e repositório JSON

### MicroondasCliente (Cliente Web)
- Porta: `5200`
- Interface web para operar o micro-ondas
- Comunica-se com a API via HTTP
- Armazena JWT Token na sessão

## Executando o projeto

### Pré-requisitos
- [.NET 10 SDK](https://dotnet.microsoft.com/) instalado

### Via Terminal

1. **Compilar ambos os projetos:**
   ```bash
   dotnet build coodesh-micro-ondas.sln
   ```

2. **Executar o servidor (MicroondasDigital):**
   ```bash
   dotnet run --project MicroondasDigital/MicroondasDigital.csproj
   ```
   Acesse `http://localhost:5161` (interface do painel)

3. **Em outro terminal, executar o cliente (MicroondasCliente):**
   ```bash
   dotnet run --project MicroondasCliente/MicroondasCliente.csproj
   ```
   Acesse `http://localhost:5200` (interface do usuário)

## Funcionalidades

### Autenticação
- Login com JWT (padrão: `admin` / `123456`)
- Token com expiração configurável (60 minutos)
- Autorização em todos os endpoints da API

### Controle do Micro-ondas
-  Iniciar aquecimento manual (com tempo e potência configuráveis)
- Início rápido (+30 segundos)
- Pausar e retomar aquecimento
- Parar aquecimento
- Contagem regressiva em tempo real
- Display com caracteres de progresso

### Receitas Predefinidas
- Pipoca (3 min, potência 7)
- Leite (5 min, potência 5)
- Carne (14 min, potência 4)
- Frango (8 min, potência 7)
- Feijão (8 min, potência 9)

### Programas Customizados
- Criar novos programas personalizados
- Editar programas existentes
- Deletar programas
- Persistência em `customPrograms.json`
- Configuração de tempo, potência e caractere de progresso

### Validações
- Validação de entrada de tempo (1-120 segundos para modo padrão)
- Validação de potência (1-10)
- Prevenção de duplicação de nomes e caracteres em programas customizados
- Mensagens de erro customizadas

## Observações

Este repositório faz parte de um desafio técnico; ao submeter uma solução no Coodesh, inclua o link ao código e siga as instruções da plataforma.

> This is a challenge by [Coodesh](https://coodesh.com/)

## Suporte

Para tirar dúvidas sobre o processo envie uma mensagem diretamente a um especialista no chat da plataforma. 
