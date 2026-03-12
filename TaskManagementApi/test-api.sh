#!/usr/bin/env bash
set -euo pipefail

BASE_URL="${1:-http://localhost:5004}"
TASKS_URL="$BASE_URL/api/tasks"

if ! command -v curl >/dev/null 2>&1; then
  echo "Erro: curl não encontrado no sistema."
  exit 1
fi

request() {
  local method="$1"
  local url="$2"
  local data="${3:-}"

  if [[ -n "$data" ]]; then
    curl -s -X "$method" "$url" -H 'Content-Type: application/json' -d "$data" -w '\n%{http_code}'
  else
    curl -s -X "$method" "$url" -w '\n%{http_code}'
  fi
}

print_step() {
  echo
  echo "============================================================"
  echo "$1"
  echo "============================================================"
}

assert_status() {
  local got="$1"
  local expected="$2"
  local step="$3"

  if [[ "$got" != "$expected" ]]; then
    echo "❌ Falha em: $step"
    echo "Esperado: $expected | Recebido: $got"
    exit 1
  fi

  echo "✅ $step ($got)"
}

print_step "0) Health check da API"
health_code=$(curl -s -o /dev/null -w '%{http_code}' "$TASKS_URL" || true)
if [[ "$health_code" == "000" ]]; then
  echo "❌ API indisponível em $BASE_URL"
  echo "Inicie com: dotnet run --project /home/gabriel/RiderProjects/CRUD-Project/TaskManagementApi/TaskManagementApi.csproj"
  exit 1
fi
assert_status "$health_code" "200" "GET /api/tasks"

print_step "1) POST /api/tasks"
create_payload='{"titulo":"Balancear HP do Boss da Fase 2","descricao":"Reduzir o dano de área do monstro e aumentar a vida em 15%","concluida":false}'
create_result=$(request "POST" "$TASKS_URL" "$create_payload")
create_status=$(echo "$create_result" | tail -n1)
create_body=$(echo "$create_result" | sed '$d')
assert_status "$create_status" "201" "POST /api/tasks"

task_id=$(echo "$create_body" | sed -n 's/.*"id":\([0-9][0-9]*\).*/\1/p' | head -n1)
if [[ -z "$task_id" ]]; then
  echo "❌ Não foi possível extrair o ID da tarefa criada."
  echo "Resposta: $create_body"
  exit 1
fi

echo "ID criado: $task_id"

after_create_expected="/api/tasks/$task_id"
if [[ "$create_body" != *"\"id\":$task_id"* ]]; then
  echo "❌ Corpo de criação sem id esperado."
  echo "Resposta: $create_body"
  exit 1
fi

print_step "2) GET /api/tasks/{id}"
get_one_result=$(request "GET" "$TASKS_URL/$task_id")
get_one_status=$(echo "$get_one_result" | tail -n1)
get_one_body=$(echo "$get_one_result" | sed '$d')
assert_status "$get_one_status" "200" "GET /api/tasks/$task_id"

if [[ "$get_one_body" != *"\"id\":$task_id"* ]]; then
  echo "❌ GET por ID não retornou a tarefa esperada."
  echo "Resposta: $get_one_body"
  exit 1
fi

print_step "3) PATCH /api/tasks/{id}/complete"
patch_result=$(request "PATCH" "$TASKS_URL/$task_id/complete")
patch_status=$(echo "$patch_result" | tail -n1)
assert_status "$patch_status" "204" "PATCH /api/tasks/$task_id/complete"

print_step "4) PUT /api/tasks/{id}"
put_payload="{\"id\":$task_id,\"titulo\":\"Boss Fase 2 ajustado\",\"descricao\":\"Dano AOE reduzido, HP +15%\",\"concluida\":true}"
put_result=$(request "PUT" "$TASKS_URL/$task_id" "$put_payload")
put_status=$(echo "$put_result" | tail -n1)
assert_status "$put_status" "204" "PUT /api/tasks/$task_id"

print_step "5) GET /api/tasks/{id} após update"
get_updated_result=$(request "GET" "$TASKS_URL/$task_id")
get_updated_status=$(echo "$get_updated_result" | tail -n1)
get_updated_body=$(echo "$get_updated_result" | sed '$d')
assert_status "$get_updated_status" "200" "GET pós-update"

if [[ "$get_updated_body" != *'"concluida":true'* ]]; then
  echo "❌ Tarefa não ficou concluída após PATCH/PUT."
  echo "Resposta: $get_updated_body"
  exit 1
fi

print_step "6) DELETE /api/tasks/{id}"
delete_result=$(request "DELETE" "$TASKS_URL/$task_id")
delete_status=$(echo "$delete_result" | tail -n1)
assert_status "$delete_status" "204" "DELETE /api/tasks/$task_id"

print_step "7) GET /api/tasks/{id} após delete"
get_deleted_result=$(request "GET" "$TASKS_URL/$task_id")
get_deleted_status=$(echo "$get_deleted_result" | tail -n1)
assert_status "$get_deleted_status" "404" "GET após delete"

echo
echo "🎉 Todos os testes do CRUD + PATCH complete passaram com sucesso."
