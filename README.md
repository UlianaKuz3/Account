## Учебный проект микросервис «Банковские счета» на **ASP.NET Core**, демонстрирующий работу с:

- **CQRS и Mediator (MediatR)**
- **FluentValidation** для валидации запросов
- **Swagger/OpenAPI**
- Заглушки для внешних сервисов (проверка клиента и валют)

## Используемые технологии

- **ASP.NET Core 8+**
- **MediatR** – паттерн CQRS
- **FluentValidation** – валидация запросов
- **Swagger** – автогенерация документации
- **In-Memory заглушки** для:
  - Проверки существования клиента (`IClientVerificationService`)
  - Проверки поддерживаемой валюты (`ICurrencyService`)

## Функционал
### Accounts
- `GET /api/accounts` – получить список всех счетов
- `GET /api/accounts/{id}` – получить счёт по ID
- `POST /api/accounts` – создать новый счёт
- `PUT /api/accounts/{id}` – обновить данные счёта
- `DELETE /api/accounts/{id}` – удалить счёт
- `GET /api/accounts/check/{ownerId}` – проверить, есть ли счёт у владельца

### Transactions
- `POST /api/transactions` – зарегистрировать транзакцию (пополнение или списание)
- `POST /api/transactions/transfer` – перевести средства между счетами
