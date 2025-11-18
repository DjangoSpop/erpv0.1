#!/bin/bash

# Motel Management System - Professional Startup Script
# ========================================================

echo "========================================="
echo "  Motel Management System"
echo "  موتيل دهب - نظام الإدارة"
echo "========================================="
echo ""

# Colors for output
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
RED='\033[0;31m'
NC='\033[0m' # No Color

# Change to Web project directory
cd src/Motel.Web

echo -e "${YELLOW}[1/4]${NC} Checking .NET SDK..."
if ! command -v dotnet &> /dev/null; then
    echo -e "${RED}ERROR: .NET SDK not found!${NC}"
    echo "Please install .NET 8 SDK from: https://dotnet.microsoft.com/download"
    exit 1
fi

DOTNET_VERSION=$(dotnet --version)
echo -e "${GREEN}✓${NC} .NET SDK ${DOTNET_VERSION} found"
echo ""

echo -e "${YELLOW}[2/4]${NC} Restoring NuGet packages..."
dotnet restore
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Package restore failed!${NC}"
    exit 1
fi
echo -e "${GREEN}✓${NC} Packages restored successfully"
echo ""

echo -e "${YELLOW}[3/4]${NC} Building application..."
dotnet build --no-restore
if [ $? -ne 0 ]; then
    echo -e "${RED}ERROR: Build failed!${NC}"
    exit 1
fi
echo -e "${GREEN}✓${NC} Build completed successfully"
echo ""

echo -e "${YELLOW}[4/4]${NC} Starting application..."
echo ""
echo -e "${GREEN}==========================================${NC}"
echo -e "${GREEN}  Application starting...${NC}"
echo -e "${GREEN}  Open: http://localhost:5000${NC}"
echo -e "${GREEN}  or:   https://localhost:5001${NC}"
echo -e "${GREEN}==========================================${NC}"
echo ""

dotnet run --no-build
