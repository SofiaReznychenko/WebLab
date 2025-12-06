#!/bin/bash

# Configuration
PORT=5122
PROJECT_DIR="ReznichenkoWeb"
TEST_DIR="ReznichenkoWeb.Tests"
BASE_URL="http://localhost:$PORT"

echo "--------------------------------------------------"
echo "   Automated Test Runner for ReznichenkoWeb"
echo "--------------------------------------------------"

# 1. Kill any existing process on the port
echo "[1/5] Cleaning up port $PORT..."
PID=$(lsof -t -i:$PORT)
if [ -n "$PID" ]; then
  echo "Killing existing process $PID..."
  kill -9 $PID
fi

# 2. Build the application
echo "[2/5] Building application..."
dotnet build $PROJECT_DIR/ReznichenkoWeb.csproj --nologo --verbosity quiet
if [ $? -ne 0 ]; then
    echo "Build failed!"
    exit 1
fi

# 3. Start the application in the background
echo "[3/5] Starting application..."
dotnet run --project $PROJECT_DIR/ReznichenkoWeb.csproj --urls "http://localhost:$PORT" > /dev/null 2>&1 &
APP_PID=$!
echo "App started with PID $APP_PID. Waiting for readiness..."

# Wait for the app to be ready (loop with curl)
MAX_RETRIES=30
COUNT=0
URL_READY=0

while [ $COUNT -lt $MAX_RETRIES ]; do
    HTTP_CODE=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$PORT")
    if [ "$HTTP_CODE" -eq "200" ]; then
        URL_READY=1
        echo "App is online (HTTP 200)!"
        break
    fi
    sleep 1
    COUNT=$((COUNT+1))
    echo -n "."
done

if [ $URL_READY -eq 0 ]; then
    echo "Timeout waiting for app to start."
    kill $APP_PID
    exit 1
fi

# 4. Run the tests
echo ""
echo "[4/5] Running Selenium tests..."
dotnet test $TEST_DIR/ReznichenkoWeb.Tests.csproj
TEST_EXIT_CODE=$?

# 5. Cleanup
echo "[5/5] Cleaning up..."
kill $APP_PID

echo "--------------------------------------------------"
if [ $TEST_EXIT_CODE -eq 0 ]; then
    echo "   SUCCESS: All tests passed!"
else
    echo "   FAILURE: Tests failed."
fi
echo "--------------------------------------------------"

exit $TEST_EXIT_CODE
