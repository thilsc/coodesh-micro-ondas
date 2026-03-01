# Microondas Digital

Esta é uma aplicação ASP.NET Core MVC que simula o painel e funcionalidades de um micro-ondas digital. Permite iniciar, pausar, retomar e parar o aquecimento, além de escolher receitas predefinidas ou programas customizados.

## Tecnologias

- Linguagem: C#
- Framework: .NET 10 (ASP.NET Core MVC)
- Front‑end: Razor views + Bootstrap 5
- Sem banco de dados (dados em memória/repositório estático)

## Executando o projeto

1. Certifique‑se de ter o [.NET 10 SDK](https://dotnet.microsoft.com/) instalado.
2. Abra um terminal na raiz do repositório.
3. Execute:
   ```bash
   dotnet build MicroondasDigital/MicroondasDigital.csproj
   dotnet run --project MicroondasDigital/MicroondasDigital.csproj
   ```
4. Acesse `http://localhost:5161` no navegador.

> O projeto também pode ser aberto em VS Code e executado usando as tasks `build`/`watch` configuradas.

## Funcionalidades atuais

- Validação de entrada de tempo e potência com mensagens customizadas.
- Painel de status com contagem regressiva.
- Receitas rápidas (pipoca, leite, carne, frango, feijão).
- Programas customizados configuráveis e executáveis (lista carregada de `customPrograms.json`).

## Observações

Este repositório faz parte de um desafio técnico; ao submeter uma solução no Coodesh, inclua o link ao código e siga as instruções da plataforma.

> This is a challenge by [Coodesh](https://coodesh.com/)

## Suporte

Para tirar dúvidas sobre o processo envie uma mensagem diretamente a um especialista no chat da plataforma. 
