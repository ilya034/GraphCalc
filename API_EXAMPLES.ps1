# GraphCalc API Examples for PowerShell
# Примеры использования GraphCalc API с PowerShell

$BaseUrl = "http://localhost:5000/api/graphcalculation"

Write-Host "=== GraphCalc API Examples ===" -ForegroundColor Green
Write-Host ""

# Example 1: Calculate sin(x) without saving
Write-Host "1. Calculate sin(x) without saving" -ForegroundColor Cyan
Write-Host "POST /api/graphcalculation/calculate"
$body1 = @{
    expression = "sin(x)"
    xMin = -3.14159
    xMax = 3.14159
    xStep = 0.1
    autoYRange = $false
} | ConvertTo-Json

$response1 = Invoke-RestMethod -Uri "$BaseUrl/calculate" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body1

$response1 | ConvertTo-Json | Write-Host
Write-Host ""
Write-Host ""

# Example 2: Calculate x^2 with auto Y-range and save
Write-Host "2. Calculate x^2 with auto Y-range and save" -ForegroundColor Cyan
Write-Host "POST /api/graphcalculation/calculate-and-save"
$body2 = @{
    expression = "x*x"
    xMin = -5
    xMax = 5
    xStep = 0.1
    autoYRange = $true
} | ConvertTo-Json

$response2 = Invoke-RestMethod -Uri "$BaseUrl/calculate-and-save" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body2

# $response2 | ConvertTo-Json | Write-Host
$graphId = $response2.id
Write-Host "Saved graph ID: $graphId" -ForegroundColor Yellow
Write-Host ""
Write-Host ""

# Example 3: Calculate pow(sin(x),2) and save
Write-Host "3. Calculate pow(sin(x),2) and save" -ForegroundColor Cyan
Write-Host "POST /api/graphcalculation/calculate-and-save"
$body3 = @{
    expression = "pow(sin(x),2)"
    xMin = -3.14159
    xMax = 3.14159
    xStep = 0.1
    autoYRange = $true
} | ConvertTo-Json

$response3 = Invoke-RestMethod -Uri "$BaseUrl/calculate-and-save" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body3

# $response3 | ConvertTo-Json | Write-Host
$graphId2 = $response3.id
Write-Host "Saved graph ID: $graphId2" -ForegroundColor Yellow
Write-Host ""
Write-Host ""

# Example 4: Get all saved graphs
Write-Host "4. Get all saved graphs" -ForegroundColor Cyan
Write-Host "GET /api/graphcalculation"
$response4 = Invoke-RestMethod -Uri "$BaseUrl" -Method Get
$response4 | ConvertTo-Json | Write-Host
Write-Host ""
Write-Host ""

# Example 5: Get specific graph by ID
Write-Host "5. Get specific graph by ID" -ForegroundColor Cyan
Write-Host "GET /api/graphcalculation/$graphId"
$response5 = Invoke-RestMethod -Uri "$BaseUrl/$graphId" -Method Get
$response5 | ConvertTo-Json | Write-Host
Write-Host ""
Write-Host ""

# Example 6: Search graphs by expression text
Write-Host "6. Search graphs by expression text (sin)" -ForegroundColor Cyan
Write-Host "GET /api/graphcalculation/search?expressionText=sin"
$response6 = Invoke-RestMethod -Uri "$BaseUrl/search?expressionText=sin" -Method Get
$response6 | ConvertTo-Json | Write-Host
Write-Host ""
Write-Host ""

# Example 7: Calculate complex expression without saving
Write-Host "7. Calculate complex expression (x/2 + sin(x)) without saving" -ForegroundColor Cyan
Write-Host "POST /api/graphcalculation/calculate"
$body7 = @{
    expression = "x/2 + sin(x)"
    xMin = -6.28318
    xMax = 6.28318
    xStep = 0.1
    autoYRange = $true
} | ConvertTo-Json

$response7 = Invoke-RestMethod -Uri "$BaseUrl/calculate" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body7

$response7 | ConvertTo-Json | Write-Host
Write-Host ""
Write-Host ""

# Example 8: Calculate sqrt(x) and save
Write-Host "8. Calculate sqrt(x) and save" -ForegroundColor Cyan
Write-Host "POST /api/graphcalculation/calculate-and-save"
$body8 = @{
    expression = "sqrt(x)"
    xMin = 0
    xMax = 10
    xStep = 0.1
    autoYRange = $true
} | ConvertTo-Json

$response8 = Invoke-RestMethod -Uri "$BaseUrl/calculate-and-save" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body8

$response8 | ConvertTo-Json | Write-Host
$graphId3 = $response8.id
Write-Host "Saved graph ID: $graphId3" -ForegroundColor Yellow
Write-Host ""
Write-Host ""

# Example 9: Delete a graph
Write-Host "9. Delete a graph" -ForegroundColor Cyan
Write-Host "DELETE /api/graphcalculation/$graphId3"
try {
    $response9 = Invoke-RestMethod -Uri "$BaseUrl/$graphId3" `
        -Method Delete `
        -StatusCodeVariable statusCode
    Write-Host "Status: $statusCode" -ForegroundColor Green
} catch {
    Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Green
}
Write-Host ""
Write-Host ""

# Example 10: Verify deletion (should return 404)
Write-Host "10. Verify deletion (should return 404)" -ForegroundColor Cyan
Write-Host "GET /api/graphcalculation/$graphId3"
try {
    $response10 = Invoke-RestMethod -Uri "$BaseUrl/$graphId3" `
        -Method Get `
        -StatusCodeVariable statusCode
    Write-Host "Status: $statusCode" -ForegroundColor Green
} catch {
    Write-Host "Status: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    Write-Host "Error: Graph not found (expected)" -ForegroundColor Yellow
}
Write-Host ""
Write-Host ""

Write-Host "=== Examples completed ===" -ForegroundColor Green
