# 🏦 Banking Engineering: Engine de Análise e Parametrização de Crédito Consignado

Este repositório contém um microsserviço focado na automação, validação e parametrização de propostas de **Crédito Consignado**, projetado para atuar como a ponte técnica entre regras operacionais de negócio e sistemas centrais (*core-banking*).

## 🎯 Objetivo do Projeto
Automatizar a esteira de pré-venda de produtos de crédito (Público, Trabalhador e INSS), garantindo aderência estrita às diretrizes regulatórias e otimizando o fluxo de aprovação sem fricção.

## 🛠️ Tecnologias Utilizadas
* **Linguagem:** C# (.NET Core)
* **Framework Web:** ASP.NET Core Web API (Endpoints assíncronos)
* **ORM / Acesso a Dados:** Entity Framework Core
* **Banco de Dados:** Estrutura relacional escalável preparada para SQL Server / PostgreSQL

## 💼 Regras de Negócio e Engenharia de Produto Implementadas
1. **Validação Automática de Margem Consignável:** Processamento em tempo real limitando o valor da parcela desejada a **35% da renda mensal comprovada**, em total conformidade com os padrões éticos e operacionais da **FEBRABAN**.
2. **Controle Paramétrico de Prazos:** Rejeição preemptiva na esteira caso a quantidade de parcelas informada exceda o teto regulamentar padrão do segmento (84 meses).
3. **Persistência Concorrente Assíncrona:** Arquitetura focada em alta disponibilidade e performance para responder de forma rápida a picos de submissões na esteira de pré-venda.
