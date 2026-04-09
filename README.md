# 🚀 MetaFlow — Микросервисная архитектура

Kanban/Task-Management система с микросервисной архитектурой, контейнеризацией и оркестрацией Kubernetes.

## 📐 Архитектура

```
┌─────────────────────────────────────────────────────────────┐
│                      Kubernetes Cluster                     |
├─────────────────────────────────────────────────────────────┤
│                                                             |
│  ┌──────────────┐    ┌──────────────┐    ┌──────────────┐   |
│  │  MetaFlow    │    │  MetaFlow    │    │   Email      │   |
│  │    Web UI    │───▶│     API      │───▶│   Service   │   │
│  │  (Blazor)    │    │   (Carter)   │    │  (MailKit)   │   │
│  └──────────────┘    └──────┬───────┘    └──────┬───────┘   │
│                              │                   │          │
│                              ▼                   ▼          │
│                       ┌──────────┐         ┌──────────┐     │
│                       │PostgreSQL│         │   SMTP   │     │
│                       │(Supabase)│         │  Server  │     │
│                       └──────────┘         └──────────┘     │
│                              │                              │
│                              ▼                              │
│                       ┌──────────┐                          │
│                       │  Redis   │                          │
│                       │ (Upstash)│                          │
│                       └──────────┘                          │
│                                                             │
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
---

**Статус проекта:** Заброшен ✔️

**Последнее обновление:** 7 апреля 2026
