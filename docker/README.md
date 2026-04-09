# MetaFlow Docker & Kubernetes Configuration

## 📁 Структура

```
docker/
├── docker-compose.yml          # Основная конфигурация для локальной разработки
└── .env.example                # Пример переменных окружения

k8s/
├── namespaces/
│   └── metaflow.yaml           # Namespace
├── configs/
│   ├── api-config.yaml         # ConfigMap для API
│   ├── email-config.yaml       # ConfigMap для Email Service
│   └── web-config.yaml         # ConfigMap для Web
├── secrets/
│   └── metaflow-secrets.yaml   # Secrets (в production используйте SealedSecrets)
├── deployments/
│   ├── api-deployment.yaml     # Deployment для API
│   ├── email-deployment.yaml   # Deployment для Email Service
│   └── web-deployment.yaml     # Deployment для Web UI
├── services/
│   ├── api-service.yaml        # Service для API
│   ├── email-service.yaml      # Service для Email Service
│   └── web-service.yaml        # Service для Web UI
└── ingress/
    └── ingress.yaml            # Ingress с TLS
```

## 🐳 Docker Compose (Локальная разработка)

### 1. Подготовка

```bash
# Скопируйте .env.example
cp docker/.env.example docker/.env

# Отредактируйте .env файл
nano docker/.env
```

### 2. Запуск

```bash
# Запустить весь стек
docker-compose -f docker/docker-compose.yml up -d

# Запустить с MailDev (для тестирования email)
docker-compose -f docker/docker-compose.yml --profile dev up -d

# Остановить
docker-compose -f docker/docker-compose.yml down

# Остановить с удалением volumes
docker-compose -f docker/docker-compose.yml down -v
```

### 3. Доступ к сервисам

| Сервис | URL | Описание |
|--------|-----|----------|
| **API** | http://localhost:7208 | MetaFlow API |
| **Web** | http://localhost:7209 | Blazor Web UI |
| **Email Service** | http://localhost:8081 | Email Service API |
| **MailDev Web** | http://localhost:1080 | Web UI для просмотра писем (dev profile) |
| **Seq** | http://localhost:5341 | Логирование |
| **PostgreSQL** | localhost:5432 | База данных |
| **Redis** | localhost:6379 | Кэш |

### 4. Логи

```bash
# Все логи
docker-compose -f docker/docker-compose.yml logs -f

# Логи конкретного сервиса
docker-compose -f docker/docker-compose.yml logs -f api
docker-compose -f docker/docker-compose.yml logs -f email-service
```

## ☸️ Kubernetes (Production)

### 1. Требования

- kubectl настроен на кластер
- Helm 3 (опционально)
- NGINX Ingress Controller
- cert-manager (для TLS)

### 2. Настройка

```bash
# Отредактируйте секреты
nano k8s/secrets/metaflow-secrets.yaml

# Отредактируйте ConfigMaps
nano k8s/configs/api-config.yaml
nano k8s/configs/email-config.yaml

# Отредактируйте image в deployments
nano k8s/deployments/*.yaml
```

### 3. Деплой

```bash
# Создать namespace
kubectl apply -f k8s/namespaces/metaflow.yaml

# Применить ConfigMaps и Secrets
kubectl apply -f k8s/configs/
kubectl apply -f k8s/secrets/

# Деплой приложений
kubectl apply -f k8s/deployments/
kubectl apply -f k8s/services/

# Деплой Ingress
kubectl apply -f k8s/ingress/ingress.yaml
```

### 4. Проверка

```bash
# Проверить статус подов
kubectl get pods -n metaflow

# Проверить сервисы
kubectl get svc -n metaflow

# Проверить Ingress
kubectl get ingress -n metaflow

# Логи
kubectl logs -n metaflow -l app=metaflow-api
kubectl logs -n metaflow -l app=email-service
```

### 5. Масштабирование

```bash
# Увеличить количество реплик API
kubectl scale deployment metaflow-api -n metaflow --replicas=5

# HPA (автоматическое масштабирование)
kubectl autoscale deployment metaflow-api -n metaflow --cpu-percent=70 --min=2 --max=10
```

### 6. Обновление

```bash
# Обновить image
kubectl set image deployment/metaflow-api api=ghcr.io/your-org/metaflow-api:v1.2.3 -n metaflow

# Проверить статус rollout
kubectl rollout status deployment/metaflow-api -n metaflow

# Откат при проблемах
kubectl rollout undo deployment/metaflow-api -n metaflow
```

## 🔧 Переменные окружения

### Критически важные

| Переменная | Где использовать | Описание |
|------------|------------------|----------|
| `SUPABASE_CONNECTION_STRING` | API | Строка подключения к PostgreSQL |
| `JWT_SECRET` | API | Секрет для подписи JWT токенов (мин. 32 символа) |
| `SMTP_USERNAME` | Email Service | Логин SMTP сервера |
| `SMTP_PASSWORD` | Email Service | Пароль SMTP сервера |
| `POSTGRES_PASSWORD` | PostgreSQL | Пароль базы данных |

### Email Service настройки

| Переменная | Значение | Описание |
|------------|----------|----------|
| `Smtp__Host` | smtp.gmail.com | SMTP сервер |
| `Smtp__Port` | 587 | SMTP порт |
| `Smtp__EnableSsl` | true | Использовать TLS |
| `Smtp__FromEmail` | noreply@metaflow.com | Email отправителя |

## 🛡️ Безопасность

### Production Checklist

- [ ] Использовать **SealedSecrets** вместо обычных Secrets
- [ ] Включить **RBAC** в namespace
- [ ] Настроить **NetworkPolicies**
- [ ] Использовать **Private Container Registry**
- [ ] Включить **Pod Security Policies**
- [ ] Настроить **Resource Quotas**
- [ ] Включить **Audit Logging**

### Sealed Secrets (для production)

```bash
# Установить kubeseal
brew install kubeseal

# Зашифровать secret
kubectl create secret generic my-secret \
  --from-literal=password=supersecret \
  --dry-run=client -o json | kubeseal --cert cert.pem -o yaml > sealed-secret.yaml
```

## 📊 Monitoring

### Health Checks

| Endpoint | Сервис | URL |
|----------|--------|-----|
| `/health` | API | http://api.metaflow.com/health |
| `/health` | Email Service | http://email-service:8080/health |
| `/health` | Web | http://app.metaflow.com/health |

### Prometheus Metrics (TODO)

- Добавить `prometheus-net` пакет
- Создать ServiceMonitor для Prometheus
- Настроить Grafana дашборды

## 🚀 CI/CD

### GitHub Actions (TODO)

```yaml
# .github/workflows/deploy.yml
name: Deploy to Kubernetes
on:
  push:
    branches: [main]

jobs:
  deploy:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Build and Push Docker images
        # ...
      - name: Deploy to K8s
        run: |
          kubectl apply -f k8s/
```

## 🐛 Troubleshooting

### Pods не запускаются

```bash
# Проверить события
kubectl describe pod <pod-name> -n metaflow

# Проверить логи
kubectl logs <pod-name> -n metaflow
```

### Email Service не отправляет письма

```bash
# Проверить подключение к SMTP
kubectl exec -it <email-pod> -n metaflow -- curl -v smtp://your-smtp-host:587

# Проверить логи
kubectl logs <email-pod> -n metaflow | grep -i error
```

### API не подключается к Email Service

```bash
# Проверить DNS
kubectl exec -it <api-pod> -n metaflow -- curl http://email-service:8080/health

# Проверить NetworkPolicies
kubectl get networkpolicy -n metaflow
```

## 📝 License

MIT
