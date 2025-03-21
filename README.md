# QueroPlaces - Sistema Brasileiro de Endereços e Geocodificação

## 🚀 Principais Funcionalidades

- **Validação de CEP**: Verificação precisa de códigos postais
- **Busca de Endereços**: Pesquisa flexível por componentes de endereço
- **Geocodificação**: Conversão de endereços para coordenadas geográficas
- **Cálculo de Distâncias**: Entre localidades brasileiras
- **Busca por Proximidade**: Encontre endereços próximos a um ponto

## 🛠 Tecnologias Utilizadas

- **Backend**: ASP.NET Core 8.0
- **Banco de Dados**: PostgreSQL com PostGIS
- **Cache**: Redis
- **Indexação**: Elasticsearch
- **Containerização**: Docker
- **Background Jobs**: Hangfire
- **Arquitetura**: CQRS com MediatR

## 📦 Pré-requisitos

- .NET 8.0 SDK
- Docker
- Docker Compose
- PostgreSQL 13+

## 🔧 Instalação e Configuração

### Clonar o Repositório

```bash
git clone https://github.com/seu-usuario/quero-places.git
cd quero-places
```

### Configuração com Docker

```bash
docker-compose up -d
```

### Configurações Adicionais

Copie `appsettings.example.json` para `appsettings.json` e ajuste as configurações conforme necessário.

## 🧪 Testes

```bash
dotnet test
```

## 📚 Documentação da API

Acesse a documentação Swagger em `/swagger` após iniciar a aplicação.

## 📋 Roadmap

- [ ] Suporte a mais idiomas
- [ ] Integração com APIs de mapas
- [ ] Melhorias de performance
- [ ] Mais testes de cobertura

## 🛡️ Avisos Legais

Dados baseados no Diretório Nacional de Endereços (DNE) dos Correios.
