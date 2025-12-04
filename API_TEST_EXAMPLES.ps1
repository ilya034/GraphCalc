# API Test Examples - PowerShell

# BASE URL - change if needed
$baseUrl = "http://localhost:5000"

# ========== USER MANAGEMENT ==========

# 1. Register user
$user = Invoke-RestMethod -Uri "$baseUrl/api/user/register" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{
    "username": "alice",
    "email": "alice@example.com",
    "description": "Graph enthusiast"
  }'

$userId = $user.userId
Write-Host "Created user: $userId"

# 2. Get user profile
$profile = Invoke-RestMethod -Uri "$baseUrl/api/user/$userId" `
  -Method GET

Write-Host "User profile: $($profile | ConvertTo-Json)"

# ========== SINGLE GRAPH OPERATIONS ==========

# 3. Calculate graph (no save)
$calculated = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/calculate" `
  -Method POST `
  -ContentType "application/json" `
  -Body '{
    "expression": "sin(x)",
    "xMin": -10,
    "xMax": 10,
    "xStep": 0.5,
    "autoYRange": true
  }'

Write-Host "Calculated graph has $(($calculated.points).Count) points"

# 4. Save single graph
$savedGraph = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/save" `
  -Method POST `
  -ContentType "application/json" `
  -Body @{
    expression = "sin(x)"
    xMin = -10
    xMax = 10
    xStep = 0.5
    autoYRange = $true
    userId = $userId
    title = "Sine Wave"
    description = "Basic sine function"
  } | ConvertTo-Json

$graphId1 = $savedGraph.id
Write-Host "Saved graph: $graphId1"

# 5. Save another graph
$savedGraph2 = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/save" `
  -Method POST `
  -ContentType "application/json" `
  -Body @{
    expression = "cos(x)"
    xMin = -10
    xMax = 10
    xStep = 0.5
    autoYRange = $true
    userId = $userId
    title = "Cosine Wave"
    description = "Basic cosine function"
  } | ConvertTo-Json

$graphId2 = $savedGraph2.id
Write-Host "Saved second graph: $graphId2"

# 6. Get single graph by ID
$graph = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/$graphId1" `
  -Method GET

Write-Host "Retrieved graph: $($graph.title)"

# ========== GRAPHSET OPERATIONS ==========

# 7. Save GraphSet (multiple graphs)
$graphSet = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/saveset" `
  -Method POST `
  -ContentType "application/json" `
  -Body @{
    graphs = @(
      @{
        expression = "x"
        xMin = -10
        xMax = 10
        xStep = 0.5
        autoYRange = $false
        userId = $userId
        title = "Linear"
        description = "y = x"
      },
      @{
        expression = "x^2"
        xMin = -10
        xMax = 10
        xStep = 0.5
        autoYRange = $true
        userId = $userId
        title = "Quadratic"
        description = "y = x^2"
      },
      @{
        expression = "x^3"
        xMin = -5
        xMax = 5
        xStep = 0.5
        autoYRange = $true
        userId = $userId
        title = "Cubic"
        description = "y = x^3"
      }
    )
    userId = $userId
    title = "Power Functions"
    description = "Linear, quadratic, and cubic functions"
  } | ConvertTo-Json -Depth 10

Write-Host "Created GraphSet with $($graphSet.graphs.Count) graphs"

# ========== USER GRAPHS RETRIEVAL ==========

# 8. Get all user's graphs (individual + in sets)
$userGraphs = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/user/$userId/graphs" `
  -Method GET

Write-Host "User graphs:"
Write-Host "  Individual graphs: $($userGraphs.graphs.Count)"
Write-Host "  GraphSets: $($userGraphs.graphSets.Count)"

foreach ($graph in $userGraphs.graphs) {
  Write-Host "    - $($graph.title): $($graph.expression)"
}

foreach ($set in $userGraphs.graphSets) {
  Write-Host "  Set: $($set.title) ($($set.graphs.Count) graphs)"
  foreach ($g in $set.graphs) {
    Write-Host "      - $($g.title): $($g.expression)"
  }
}

# ========== ADDITIONAL OPERATIONS ==========

# 9. Get all graphs
$allGraphs = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation" `
  -Method GET

Write-Host "Total graphs in system: $($allGraphs.Count)"

# 10. Update user description
$updated = Invoke-RestMethod -Uri "$baseUrl/api/user/$userId/description" `
  -Method PUT `
  -ContentType "application/json" `
  -Body '{
    "description": "Updated profile - love graphs!"
  }'

Write-Host "Updated user description"

# 11. Delete graph
$deleted = Invoke-RestMethod -Uri "$baseUrl/api/graphcalculation/$graphId1" `
  -Method DELETE -ErrorAction SilentlyContinue

Write-Host "Deleted graph"

Write-Host "`nAll tests completed!"
