# API Documentation

## Архитектура

### Основные компоненты:
- **GraphCalculationController** - вычисление и сохранение графиков
- **UserController** - управление пользователями и их графиками
- **InMemoryRepositories** - репозитории для данных (User, Graph, PublishedGraph, GraphSet)

### Ключевые сущности:
- **User** - пользователь с email и username
- **Graph** - одиночный график (уравнение + точки)
- **GraphSet** - набор нескольких графиков на одном канвасе
- **PublishedGraph** - связь пользователя с графиком (с метаданными)

---

## API Endpoints

### User Management

#### Регистрация пользователя
```
POST /api/user/register
Content-Type: application/json

{
  "username": "john_doe",
  "email": "john@example.com",
  "description": "My profile"
}

Response: 201 Created
{
  "userId": "guid",
  "graphs": [],
  "graphSets": []
}
```

#### Получение профиля пользователя
```
GET /api/user/{userId}

Response: 200 OK
{
  "id": "guid",
  "username": "john_doe",
  "email": "john@example.com",
  "description": "My profile",
  "publishedGraphCount": 0
}
```

#### Получение графиков пользователя
```
GET /api/graphcalculation/user/{userId}/graphs

Response: 200 OK
{
  "userId": "guid",
  "graphs": [
    {
      "id": "guid",
      "expression": "sin(x)",
      "title": "Sine wave",
      "description": "Basic sine function"
    }
  ],
  "graphSets": [
    {
      "id": "guid",
      "title": "GraphSet 1",
      "description": null,
      "graphs": [...]
    }
  ]
}
```

### Graph Operations

#### Вычисление графика (без сохранения)
```
POST /api/graphcalculation/calculate
Content-Type: application/json

{
  "expression": "sin(x)",
  "xMin": -10,
  "xMax": 10,
  "xStep": 0.1,
  "autoYRange": true
}

Response: 200 OK
{
  "id": "guid",
  "expression": "sin(x)",
  "independentVariable": "x",
  "points": [{"x": -10, "y": ...}, ...],
  "range": {"min": -10, "max": 10, "step": 0.1}
}
```

#### Сохранение графика
```
POST /api/graphcalculation/save
Content-Type: application/json

{
  "expression": "sin(x)",
  "xMin": -10,
  "xMax": 10,
  "xStep": 0.1,
  "autoYRange": true,
  "userId": "guid",
  "title": "Sine Wave",
  "description": "Beautiful sine function"
}

Response: 201 Created
{
  "id": "graph-guid",
  "expression": "sin(x)",
  "points": [...],
  "range": {...}
}
```

#### Получение графика по ID
```
GET /api/graphcalculation/{id}

Response: 200 OK
{...}
```

---

### GraphSet Operations

#### Сохранение набора графиков
```
POST /api/graphcalculation/saveset
Content-Type: application/json

{
  "graphs": [
    {
      "expression": "sin(x)",
      "xMin": -10,
      "xMax": 10,
      "xStep": 0.1,
      "autoYRange": true,
      "userId": "guid",
      "title": "Sine",
      "description": "Sin function"
    },
    {
      "expression": "cos(x)",
      "xMin": -10,
      "xMax": 10,
      "xStep": 0.1,
      "autoYRange": true,
      "userId": "guid",
      "title": "Cosine",
      "description": "Cos function"
    }
  ],
  "userId": "guid",
  "title": "Trigonometric Functions",
  "description": "Set of trig functions"
}

Response: 201 Created
{
  "id": "graphset-guid",
  "title": "Trigonometric Functions",
  "description": "Set of trig functions",
  "graphs": [
    {"id": "guid1", "expression": "sin(x)", "title": "Sine", ...},
    {"id": "guid2", "expression": "cos(x)", "title": "Cosine", ...}
  ]
}
```

---

## Интеграция GraphSet в архитектуру

GraphSet интегрирован через:

1. **Сохранение**: Все графики в наборе автоматически добавляются в репозиторий
2. **Публикация**: Каждый график связывается с пользователем через PublishedGraph
3. **Получение**: При запросе графиков пользователя автоматически собираются GraphSet'ы, в которых есть его графики
4. **Масштабирование**: GraphSet может иметь общий NumericRange для всех графиков

---

## Примеры использования

### 1. Создание пользователя и сохранение графика
```powershell
$user = Invoke-RestMethod -Uri "http://localhost:5000/api/user/register" `
  -Method POST `
  -Headers @{"Content-Type"="application/json"} `
  -Body '{"username":"test","email":"test@example.com"}'

$graph = Invoke-RestMethod -Uri "http://localhost:5000/api/graphcalculation/save" `
  -Method POST `
  -Headers @{"Content-Type"="application/json"} `
  -Body "{`"expression`":`"x^2`",`"xMin`":-5,`"xMax`":5,`"xStep`":0.1,`"userId`":`"$($user.userId)`",`"title`":`"Parabola`"}"
```

### 2. Получение всех графиков пользователя
```powershell
Invoke-RestMethod -Uri "http://localhost:5000/api/graphcalculation/user/$userId/graphs" `
  -Method GET
```

### 3. Создание GraphSet с несколькими функциями
```powershell
$graphSet = Invoke-RestMethod -Uri "http://localhost:5000/api/graphcalculation/saveset" `
  -Method POST `
  -Headers @{"Content-Type"="application/json"} `
  -Body @{
    graphs = @(
      @{expression="x";xMin=-5;xMax=5;xStep=0.1;userId=$userId;title="Linear"},
      @{expression="x^2";xMin=-5;xMax=5;xStep=0.1;userId=$userId;title="Quadratic"},
      @{expression="x^3";xMin=-5;xMax=5;xStep=0.1;userId=$userId;title="Cubic"}
    )
    userId = $userId
    title = "Power Functions"
    description = "Linear, quadratic, cubic"
  } | ConvertTo-Json
```
