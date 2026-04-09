# MetaFlow Email Service

Микросервис для отправки email уведомлений в проекте MetaFlow.

## 🚀 Возможности

- ✅ Отправка email через SMTP (MailKit)
- ✅ Шаблоны писем (HTML)
- ✅ Rate limiting (лимиты на отправку)
- ✅ Retry policy при ошибках
- ✅ Поддержка вложений
- ✅ Health checks
- ✅ Логирование через Serilog

## 📋 Шаблоны писем

| Шаблон | Описание | Переменные |
|--------|----------|------------|
| `email-verification` | Подтверждение email | `userName`, `verificationLink` |
| `password-reset` | Сброс пароля | `userName`, `resetLink` |
| `board-invitation` | Приглашение на доску | `userName`, `inviterName`, `boardName`, `boardDescription`, `memberRole`, `boardLink` |
| `card-assigned` | Назначение карточки | `userName`, `assignedByName`, `cardTitle`, `cardDescription`, `priority`, `dueDate`, `cardLink` |
| `comment-mention` | Упоминание в комментарии | `userName`, `commentAuthor`, `commentContent`, `cardTitle`, `boardName`, `cardLink` |

## 🔧 Конфигурация

### Переменные окружения

| Переменная | Описание | По умолчанию |
|------------|----------|--------------|
| `Smtp__Host` | SMTP сервер | `localhost` |
| `Smtp__Port` | SMTP порт | `1025` |
| `Smtp__EnableSsl` | Использовать SSL | `false` |
| `Smtp__Username` | SMTP логин | - |
| `Smtp__Password` | SMTP пароль | - |
| `Smtp__FromEmail` | Email отправителя | `noreply@metaflow.local` |
| `Smtp__FromName` | Имя отправителя | `MetaFlow (Dev)` |
| `RateLimiting__MaxEmailsPerHour` | Лимит писем/час | `100` |
| `RateLimiting__MaxEmailsPerDay` | Лимит писем/день | `1000` |

## 📡 API Endpoints

### POST /api/email/send
Отправить одно письмо.

**Request:**
```json
{
  "to": "user@example.com",
  "subject": "Test Email",
  "template": "email-verification",
  "variables": {
    "userName": "John Doe",
    "verificationLink": "https://..."
  }
}
```

**Response:**
```json
{
  "success": true,
  "sentAt": "2026-04-07T12:00:00Z"
}
```

### POST /api/email/send-batch
Массовая отправка писем.

**Request:**
```json
[
  {
    "to": "user1@example.com",
    "subject": "Hello",
    "template": "...",
    "variables": {...}
  },
  {
    "to": "user2@example.com",
    "subject": "Hello",
    "template": "...",
    "variables": {...}
  }
]
```

### GET /api/email/templates
Получить список доступных шаблонов.

**Response:**
```json
[
  { "name": "email-verification" },
  { "name": "password-reset" },
  { "name": "board-invitation" },
  { "name": "card-assigned" },
  { "name": "comment-mention" }
]
```

### POST /api/email/verify
Проверить подключение к SMTP.

### GET /health
Health check endpoint.

## 🐳 Docker

### Сборка
```bash
docker build -t metaflow-email-service .
```

### Запуск
```bash
docker run -d \
  -p 8080:8080 \
  -e Smtp__Host=smtp.gmail.com \
  -e Smtp__Port=587 \
  -e Smtp__Username=your@email.com \
  -e Smtp__Password=your-password \
  metaflow-email-service
```

### Docker Compose (с MailDev для dev)
```yaml
version: '3.8'
services:
  email-service:
    build: .
    ports:
      - "8080:8080"
    environment:
      - Smtp__Host=maildev
      - Smtp__Port=1025
    depends_on:
      - maildev

  maildev:
    image: maildev/maildev
    ports:
      - "1080:1080" # Web UI
      - "1025:1025" # SMTP
```

## 🧪 Локальная разработка

### Требования
- .NET 9 SDK
- (Опционально) Docker для MailDev

### Запуск
```bash
dotnet run
```

### Тестирование
```bash
# Проверить health
curl http://localhost:8080/health

# Отправить тестовое письмо
curl -X POST http://localhost:8080/api/email/send \
  -H "Content-Type: application/json" \
  -d '{
    "to": "test@example.com",
    "subject": "Test",
    "template": "email-verification",
    "variables": {
      "userName": "Test User",
      "verificationLink": "https://example.com/verify"
    }
  }'
```

## 📊 Rate Limiting

- **Максимум писем в час:** 100 (настраивается)
- **Максимум писем в день:** 1000 (настраивается)
- При превышении лимита возвращается HTTP 429

## 🔄 Retry Policy

При ошибках отправки:
- 3 попытки с экспоненциальной задержкой (2s, 4s, 8s)
- Логирование каждой попытки

## 📝 Логирование

Используется Serilog с выводом в:
- Console (stdout)
- Debug (при development)

В production рекомендуется добавить:
- Seq
- Elasticsearch
- Cloud logging (CloudWatch, Stackdriver, etc.)

## 🛡️ Безопасность

- ✅ Не хранит email credentials в коде
- ✅ Использует SSL/TLS
- ✅ Rate limiting защита
- ✅ Валидация входных данных
- ✅ Non-root user в Docker

## 📦 Зависимости

- **MailKit** - SMTP/IMAP клиент
- **MimeKit** - Создание MIME писем
- **FluentValidation** - Валидация
- **Serilog** - Логирование
- **Polly** - Retry policy

## 📄 License

MIT
