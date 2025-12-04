# Архитектура GraphCalc API

## Обзор
API для вычисления и сохранения математических графиков с поддержкой пользователей, одиночных графиков и наборов графиков (GraphSet) на одном канвасе.

## Структура данных

```
User (пользователь)
├── Id
├── Username
├── Email
├── Description
└── PublishedGraphIds (список сохраненных графиков)

Graph (одиночный график)
├── Id
├── Expression (MathExpression - объект)
├── IndependentVariable
├── Range (NumericRange)
└── Points (List<MathPoint>)

PublishedGraph (связь пользователя с графиком)
├── Id
├── UserId
├── GraphId
├── Metadata (PublicationMetadata)
│   ├── Title
│   ├── Description
│   └── IsPublic
└── IsActive

GraphSet (набор графиков на одном канвасе)
├── Id
├── Graphs (List<Graph>)
└── Range (NumericRange - может быть общий для всех)
```

## Интеграция GraphSet

### 1. **Сохранение набора**
```
SaveGraphSetRequest (массив SaveGraphRequest)
  ↓
Создается GraphSet + добавляются все графики
  ↓
Каждый граф сохраняется в IGraphRepository
  ↓
Для каждого графика создается PublishedGraph
  ↓
Пользователю добавляются GraphIds
  ↓
GraphSet сохраняется в InMemoryGraphSetRepository
```

### 2. **Получение графиков пользователя**
```
GET /api/graphcalculation/user/{userId}/graphs
  ↓
Получаются все PublishedGraph пользователя
  ↓
Извлекаются соответствующие Graph'ы
  ↓
Ищутся GraphSet'ы, содержащие графики пользователя
  ↓
Возвращается UserGraphsListResponse с:
  - Индивидуальными графиками
  - GraphSet'ами с их графиками
```

### 3. **Масштабирование GraphSet**
- Можно добавлять новые графики: `graphSet.AddGraph(graph)`
- Можно применять общий Range: `graphSet.WithRange(range).ApplyRange()`
- Можно удалять графики: `graphSet.RemoveGraph(graphId)`

## DI Контейнер

```csharp
// Регистрация в Program.cs
builder.Services.AddScoped<GraphCalculationFacade>();
builder.Services.AddSingleton<IGraphRepository, InMemoryGraphRepository>();
builder.Services.AddSingleton<IUserRepository, InMemoryUserRepository>();
builder.Services.AddSingleton<InMemoryPublishedGraphRepository>();
builder.Services.AddSingleton<InMemoryGraphSetRepository>();
```

## Контроллеры

### GraphCalculationController
- `POST /calculate` - вычислить график
- `POST /save` - сохранить одиночный график
- `POST /saveset` - сохранить набор графиков
- `GET /user/{userId}/graphs` - получить все графики пользователя
- `GET /{id}` - получить график по ID
- `GET` - получить все графики
- `DELETE /{id}` - удалить график

### UserController
- `POST /register` - зарегистрировать пользователя
- `GET /{userId}` - профиль пользователя
- `PUT /{userId}/description` - обновить описание

## Поток сохранения GraphSet

```
POST /api/graphcalculation/saveset
{
  "graphs": [
    { expression, xMin, xMax, xStep, userId, title, description },
    { expression, xMin, xMax, xStep, userId, title, description }
  ],
  "userId": "guid",
  "title": "Set Title"
}
  ↓
Валидация (минимум 1 граф, title не пуст, userId существует)
  ↓
GraphSet graphSet = GraphSet.Create()
  ↓
Для каждого графика в request:
  ├─ Вычислить график (calculationFacade)
  ├─ Добавить в IGraphRepository
  ├─ Добавить в GraphSet
  ├─ Создать PublishedGraph с метаданными
  ├─ Добавить PublishedGraph в publishedGraphRepository
  └─ Добавить GraphId пользователю
  ↓
GraphSet сохранить в graphSetRepository
  ↓
Response: 201 Created с UserGraphSetDto
```

## Поток получения графиков пользователя

```
GET /api/graphcalculation/user/{userId}/graphs
  ↓
Получить User или 404
  ↓
publishedGraphs = publishedGraphRepository.GetByUserId(userId)
  ↓
Для каждого PublishedGraph:
  ├─ Получить Graph из IGraphRepository
  └─ Создать UserGraphDto с выражением и метаданными
  ↓
graphSets = graphSetRepository.GetAll()
            .Where(gs => gs.Graphs.Any(g => has userId's graphs))
  ↓
Для каждого GraphSet:
  ├─ Собрать только графики пользователя
  └─ Создать UserGraphSetDto
  ↓
Response: 200 OK с UserGraphsListResponse
```

## Преимущества текущей архитектуры

1. **Модульность**: GraphSet полностью независим, можно расширять
2. **Гибкость**: Пользователь может иметь как отдельные графики, так и наборы
3. **Масштабируемость**: In-Memory репозитории легко заменить на БД
4. **Чистая архитектура**: Domain entities не зависят от инфраструктуры
5. **Удобство**: PublishedGraph связывает график с метаданными без изменения Graph

## Возможные расширения

- Добавить сохранение в БД (SQL/MongoDB)
- Поддержка кооперативного редактирования GraphSet
- История версий графиков
- Общие доступ (sharing) графиков между пользователями
- Рекомендации похожих GraphSet'ов
