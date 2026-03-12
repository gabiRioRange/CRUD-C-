<div align="center">
  <h1>✅ TaskManagementApi</h1>
  <p>
    <em>API RESTful de gerenciamento de tarefas (CRUD) com .NET 10, EF Core 10 e SQLite.</em>
  </p>

  [![CI](https://github.com/gabiRioRange/CRUD-C-/actions/workflows/ci.yml/badge.svg)](https://github.com/gabiRioRange/CRUD-C-/actions/workflows/ci.yml)
  [![Coverage](https://codecov.io/gh/gabiRioRange/CRUD-C-/branch/main/graph/badge.svg)](https://codecov.io/gh/gabiRioRange/CRUD-C-)
</div>

---

A TaskManagementApi é uma API de nível iniciante com estrutura profissional, separando responsabilidades em Models, Data, Services e Controllers. O projeto foi pensado para estudo prático com boas práticas reais de mercado, incluindo migrations, camada de serviço e testes end-to-end via script.

## 🏷️ Badges (CI/Coverage)

Os badges acima já estão prontos como template. Para ativar de verdade:

1. O badge de CI já está apontando para `gabiRioRange/CRUD-C-`.
2. O workflow em `.github/workflows/ci.yml` executa restore, build e test em push/PR.
3. (Opcional) Integre com Codecov para publicar a cobertura no badge.

## ✨ Funcionalidades

- Listar todas as tarefas
- Buscar tarefa por ID
- Criar tarefa
- Atualizar tarefa completa
- Deletar tarefa
- Marcar tarefa como concluída com endpoint dedicado

## 🧱 Arquitetura

- Models: entidades de domínio
- Data: contexto do EF Core e mapeamento
- Services: regras de negócio e acesso ao banco
- Controllers: roteamento HTTP e respostas REST

## 🛠️ Stack Tecnológica

- C#
- ASP.NET Core 10 Web API
- Entity Framework Core 10
- SQLite
- .NET CLI (Linux-friendly)

## 📦 Endpoints

Base URL local padrão: http://localhost:5004

| Método | Rota | Descrição |
|---|---|---|
| GET | /api/tasks | Lista todas as tarefas |
| GET | /api/tasks/{id} | Busca uma tarefa específica |
| POST | /api/tasks | Cria uma nova tarefa |
| PUT | /api/tasks/{id} | Atualiza completamente uma tarefa |
| DELETE | /api/tasks/{id} | Remove uma tarefa |
| PATCH | /api/tasks/{id}/complete | Marca tarefa como concluída |

### Exemplo de payload

```json
{
  "id": 1,
  "titulo": "Balancear HP do Boss da Fase 2",
  "descricao": "Reduzir o dano de área do monstro e aumentar a vida em 15%",
  "concluida": false
}
```

## 🚀 Começando Rápido (Fedora/Linux)

1. Entrar na pasta do projeto

```bash
cd /home/gabriel/RiderProjects/CRUD-Project/TaskManagementApi
```

2. Restaurar dependências

```bash
dotnet restore
```

3. Criar migration inicial (se ainda não existir)

```bash
dotnet ef migrations add InitialCreate
```

4. Aplicar migration no SQLite

```bash
dotnet ef database update
```

5. Iniciar a API

```bash
dotnet run
```

## 🧪 Teste automático da API

O projeto inclui um script pronto para validar o fluxo completo de CRUD + PATCH:

```bash
./test-api.sh
```

Opcional com URL customizada:

```bash
./test-api.sh http://localhost:5004
```

## 🔍 OpenAPI

Com a API em execução no ambiente Development:

- Documento OpenAPI: http://localhost:5004/openapi/v1.json

## 🧯 Troubleshooting

### Erro: address already in use

A porta já está ocupada por outra instância da API.

```bash
lsof -i :5004
kill -9 <PID>
```

Ou rode com outra URL/porta:

```bash
ASPNETCORE_URLS=http://localhost:5090 dotnet run
```

### 404 em /

Isso é esperado. Esta API expõe rotas em /api/tasks (não há endpoint mapeado na raiz /).

## 📁 Estrutura de Pastas

```text
TaskManagementApi/
├── Controllers/
├── Data/
├── Migrations/
├── Models/
├── Services/
├── Program.cs
├── appsettings.json
└── test-api.sh
```

## 🤝 Contribuição

Sugestões e melhorias são bem-vindas. Mantendo o estilo atual do projeto:

- foco em mudanças pequenas e objetivas
- testes locais antes de abrir PR
- documentação atualizada quando necessário

## Licença

Este projeto pode ser adaptado para uso educacional e pessoal.
