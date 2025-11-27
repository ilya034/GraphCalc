#!/bin/bash


BASE_URL="http://localhost:5000/api/graphcalculation"

echo "=== GraphCalc API Examples ==="
echo ""

# Example 1: Calculate sin(x) without saving
echo "1. Calculate sin(x) without saving"
echo "POST /api/graphcalculation/calculate"
curl -X POST "$BASE_URL/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "expression": "sin(x)",
    "xMin": -3.14159,
    "xMax": 3.14159,
    "xStep": 0.1,
    "autoYRange": false
  }' | jq .
echo ""
echo ""

# Example 2: Calculate x^2 with auto Y-range and save
echo "2. Calculate x^2 with auto Y-range and save"
echo "POST /api/graphcalculation/calculate-and-save"
RESPONSE=$(curl -s -X POST "$BASE_URL/calculate-and-save" \
  -H "Content-Type: application/json" \
  -d '{
    "expression": "x*x",
    "xMin": -5,
    "xMax": 5,
    "xStep": 0.1,
    "autoYRange": true
  }')
echo "$RESPONSE" | jq .
GRAPH_ID=$(echo "$RESPONSE" | jq -r '.id')
echo "Saved graph ID: $GRAPH_ID"
echo ""
echo ""

# Example 3: Calculate pow(sin(x),2) and save
echo "3. Calculate pow(sin(x),2) and save"
echo "POST /api/graphcalculation/calculate-and-save"
RESPONSE2=$(curl -s -X POST "$BASE_URL/calculate-and-save" \
  -H "Content-Type: application/json" \
  -d '{
    "expression": "pow(sin(x),2)",
    "xMin": -3.14159,
    "xMax": 3.14159,
    "xStep": 0.1,
    "autoYRange": true
  }')
echo "$RESPONSE2" | jq .
GRAPH_ID2=$(echo "$RESPONSE2" | jq -r '.id')
echo "Saved graph ID: $GRAPH_ID2"
echo ""
echo ""

# Example 4: Get all saved graphs
echo "4. Get all saved graphs"
echo "GET /api/graphcalculation"
curl -s -X GET "$BASE_URL" | jq .
echo ""
echo ""

# Example 5: Get specific graph by ID
echo "5. Get specific graph by ID"
echo "GET /api/graphcalculation/$GRAPH_ID"
curl -s -X GET "$BASE_URL/$GRAPH_ID" | jq .
echo ""
echo ""

# Example 6: Search graphs by expression text
echo "6. Search graphs by expression text (sin)"
echo "GET /api/graphcalculation/search?expressionText=sin"
curl -s -X GET "$BASE_URL/search?expressionText=sin" | jq .
echo ""
echo ""

# Example 7: Calculate complex expression without saving
echo "7. Calculate complex expression (x/2 + sin(x)) without saving"
echo "POST /api/graphcalculation/calculate"
curl -s -X POST "$BASE_URL/calculate" \
  -H "Content-Type: application/json" \
  -d '{
    "expression": "x/2 + sin(x)",
    "xMin": -6.28318,
    "xMax": 6.28318,
    "xStep": 0.1,
    "autoYRange": true
  }' | jq .
echo ""
echo ""

# Example 8: Calculate sqrt(x) and save
echo "8. Calculate sqrt(x) and save"
echo "POST /api/graphcalculation/calculate-and-save"
RESPONSE3=$(curl -s -X POST "$BASE_URL/calculate-and-save" \
  -H "Content-Type: application/json" \
  -d '{
    "expression": "sqrt(x)",
    "xMin": 0,
    "xMax": 10,
    "xStep": 0.1,
    "autoYRange": true
  }')
echo "$RESPONSE3" | jq .
GRAPH_ID3=$(echo "$RESPONSE3" | jq -r '.id')
echo "Saved graph ID: $GRAPH_ID3"
echo ""
echo ""

# Example 9: Delete a graph
echo "9. Delete a graph"
echo "DELETE /api/graphcalculation/$GRAPH_ID3"
curl -s -X DELETE "$BASE_URL/$GRAPH_ID3" -w "\nStatus: %{http_code}\n"
echo ""
echo ""

# Example 10: Verify deletion
echo "10. Verify deletion (should return 404)"
echo "GET /api/graphcalculation/$GRAPH_ID3"
curl -s -X GET "$BASE_URL/$GRAPH_ID3" -w "\nStatus: %{http_code}\n"
echo ""
echo ""
