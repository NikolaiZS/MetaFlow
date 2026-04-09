# 🚀 MetaFlow — Микросервисная архитектура

Kanban/Task-Management система с микросервисной архитектурой, контейнеризацией и оркестрацией Kubernetes.

## 📐 Архитектура

```
┌─────────────────────────────────────────────────────────────┐
│                      Kubernetes Cluster                      │
├─────────────────────────────────────────────────────────────┤
│                                                               │
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐  │
│  │  MetaFlow    │    │  MetaFlow    │    │   Email      │  │
│  │    Web UI    │───▶│     API      │───▶│   Service    │  │
│  │  (Blazor)    │    │   (Carter)   │    │  (MailKit)   │  │
│  └──────────────┘    └──────┬───────┘    └──────┬───────┘  │
│                              │                   │           │
│                              ▼                   ▼           │
│                       ┌──────────┐         ┌──────────┐     │
│                       │PostgreSQL│         │   SMTP   │     │
│                       │(Supabase)│         │  Server  │     │
│                       └──────────┘         └──────────┘     │
│                              │                               │
│                              ▼                               │
│                       ┌──────────┐                          │
│                       │  Redis   │                          │
│                       │ (Upstash)│                          │
│                       └──────────┘                          │
│                                                               │
└─────────────────────────────────────────────────────────────┘
```

## 📁 Структура проекта

```
MetaFlow/
├── MetaFlow/                      # Основной монолит
│   ├── MetaFlow.Api/              # REST API (Carter + MediatR)
│   ├── MetaFlow.Web/              # Blazor Server UI
│   ├── MetaFlow.Domain/           # Domain entities
│   ├── MetaFlow.Infrastructure/   # Data access, services
│   └── MetaFlow.Contracts/        # DTOs, interfaces
│
├── services/
│   └── email-service/             # Email микросервис
│       ├── EmailService.Api/      # Email Service API
│       │   ├── Templates/         # HTML шаблоны писем
│       │   ├── Dockerfile
│       │   └── *.cs               # Сервисы, валидаторы
│       └── README.md
│
├── docker/
│   ├── docker-compose.yml         # Локальная разработка
│   ├── .env.example               # Переменные окружения
│   └── README.md                  # Документация Docker/K8s
│
└── k8s/                           # Kubernetes манифесты
    ├── namespaces/
    ├── configs/
    ├── secrets/
    ├── deployments/
    ├── services/
    └── ingress/
```

## 🛠️ Стек технологий

### Backend
- **.NET 9.0** — основная платформа
- **Carter** — минималистичный роутинг
- **MediatR** — CQRS паттерн
- **FluentValidation** — валидация
- **Entity Framework Core** — ORM
- **Dapper** — микро-ORM для сложных запросов

### Frontend
- **Blazor Server** — серверный рендеринг
- **Bootstrap CSS** — стилизация
- **HttpClient** — взаимодействие с API

### Инфраструктура
- **PostgreSQL (Supabase)** — основная БД
- **Redis (Upstash)** — кэширование
- **MailKit** — SMTP/IMAP для email
- **Serilog + Seq** — логирование

### Контейнеризация и оркестрация
- **Docker** — контейнеризация
- **Docker Compose** — локальная разработка
- **Kubernetes** — production оркестрация

## 🚀 Быстрый старт

### Локальная разработка (Docker Compose)

```bash
# 1. Клонировать репозиторий
git clone <your-repo>
cd MetaFlow

# 2. Настроить переменные окружения
cp docker/.env.example docker/.env
# Отредактируйте docker/.env

# 3. Запустить весь стек
docker-compose -f docker/docker-compose.yml --profile dev up -d

# 4. Открыть сервисы
# API: http://localhost:7208
# Web: http://localhost:7209
# Email Service: http://localhost:8081
# MailDev (тестовые письма): http://localhost:1080
# Seq (логи): http://localhost:5341
```

### Разработка без Docker

```bash
# 1. Запустить Email Service
cd services/email-service/EmailService.Api
dotnet run  # Порт: 8081

# 2. Запустить API
cd MetaFlow/MetaFlow/MetaFlow.Api
dotnet run  # Порт: 7208

# 3. Запустить Web
cd MetaFlow/MetaFlow/MetaFlow.Web
dotnet run  # Порт: 7209
```

## 📡 API Endpoints

### MetaFlow API

| Метод | Путь | Описание |
|-------|------|----------|
| **Auth** | | |
| POST | `/api/auth/register` | Регистрация |
| POST | `/api/auth/login` | Вход |
| **Boards** | | |
| GET | `/api/boards` | Список досок |
| POST | `/api/boards` | Создать доску |
| GET | `/api/boards/{id}` | Получить доску |
| PUT | `/api/boards/{id}` | Обновить доску |
| DELETE | `/api/boards/{id}` | Удалить доску |
| **Columns** | | |
| GET | `/api/boards/{id}/columns` | Колонки доски |
| POST | `/api/boards/{id}/columns` | Создать колонку |
| PATCH | `/api/boards/{boardId}/columns/{colId}` | Обновить |
| DELETE | `/api/boards/{boardId}/columns/{colId}` | Удалить |
| PUT | `/api/boards/{id}/columns/reorder` | Переупорядочить |
| **Cards** | | |
| GET | `/api/boards/{id}/cards` | Карточки доски |
| POST | `/api/boards/{id}/cards` | Создать карточку |
| GET | `/api/boards/cards/{cardId}` | Получить карточку |
| PATCH | `/api/boards/{boardId}/cards/{cardId}` | Обновить |
| DELETE | `/api/boards/{boardId}/cards/{cardId}` | Удалить |
| PUT | `/api/boards/{boardId}/cards/{cardId}/move` | Переместить |
| **Comments** | | |
| GET | `/api/cards/{id}/comments` | Комментарии |
| POST | `/api/cards/{id}/comments` | Добавить комментарий |
| PATCH | `/api/cards/{cardId}/comments/{id}` | Обновить |
| DELETE | `/api/cards/{cardId}/comments/{id}` | Удалить |
| **Email Service** | | |
| POST | `/api/email/send` | Отправить письмо |
| POST | `/api/email/send-batch` | Массовая отправка |
| GET | `/api/email/templates` | Доступные шаблоны |
| POST | `/api/email/verify` | Проверить SMTP |

## 📧 Email Service

### Шаблоны писем

| Шаблон | Описание | Переменные |
|--------|----------|------------|
| `email-verification` | Подтверждение email | `userName`, `verificationLink` |
| `password-reset` | Сброс пароля | `userName`, `resetLink` |
| `board-invitation` | Приглашение на доску | `userName`, `inviterName`, `boardName`, `boardDescription`, `memberRole`, `boardLink` |
| `card-assigned` | Назначение карточки | `userName`, `assignedByName`, `cardTitle`, `cardDescription`, `priority`, `dueDate`, `cardLink` |
| `comment-mention` | Упоминание в комментарии | `userName`, `commentAuthor`, `commentContent`, `cardTitle`, `boardName`, `cardLink` |

### Пример использования

```bash
curl -X POST http://localhost:8081/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "user@example.com",
    "subject": "Welcome!",
    "template": "email-verification",
    "variables": {
      "userName": "John Doe",
      "verificationLink": "https://metaflow.com/verify?token=abc123"
    }
  }'
```

## ☸️ Kubernetes Деплой

### 1. Подготовка

```bash
# Отредактировать секреты и конфиги
nano k8s/secrets/metaflow-secrets.yaml
nano k8s/configs/api-config.yaml
nano k8s/configs/email-config.yaml
```

### 2. Деплой

```bash
# Применить все манифесты
kubectl apply -f k8s/

# Проверить статус
kubectl get pods -n metaflow
kubectl get svc -n metaflow
kubectl get ingress -n metaflow
```

### 3. Доступ к сервисам

```bash
# API
https://api.metaflow.com

# Web UI
https://app.metaflow.com
```

## 🔧 Переменные окружения

### Критически важные

| Переменная | Сервис | Описание |
|------------|--------|----------|
| `SUPABASE_CONNECTION_STRING` | API | Строка подключения к БД |
| `JWT_SECRET` | API | Секрет JWT (мин. 32 символа) |
| `SMTP_USERNAME` | Email Service | SMTP логин |
| `SMTP_PASSWORD` | Email Service | SMTP пароль |
| `EmailService__BaseUrl` | API | URL Email Service |

## 📊 Мониторинг

### Health Checks

| Сервис | URL |
|--------|-----|
| API | `/health` |
| Email Service | `/health` |
| Web | `/health` |

### Логирование

- **Serilog** → Console + Seq
- **Seq UI**: http://localhost:5341 (локально)

## 🧪 Тестирование

```bash
# Запустить тесты (когда будут созданы)
dotnet test

# Integration тесты с Testcontainers
dotnet test --filter "Integration"
```

## 📚 Документация

- [Docker & Kubernetes Guide](docker/README.md)
- [Email Service Docs](services/email-service/README.md)
- [API Docs (Swagger)](http://localhost:7208/swagger)

## 🤝 Contributing

1. Fork репозиторий
2. Создать ветку (`git checkout -b feature/amazing-feature`)
3. Commit изменения (`git commit -m 'Add amazing feature'`)
4. Push в ветку (`git push origin feature/amazing-feature`)
5. Открыть Pull Request

## 📄 License

MIT

## 👥 Авторы

- Your Name Here

---

**Статус проекта:** В активной разработке ✅

**Последнее обновление:** 7 апреля 2026
