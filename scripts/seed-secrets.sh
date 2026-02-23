#!/bin/bash

set -e

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_ROOT="$(dirname "$SCRIPT_DIR")"

RESOURCE_GROUP="${RESOURCE_GROUP:-rg-raysondev-staging}"
LOCATION="${LOCATION:-uksouth}"
ENVIRONMENT="${ENVIRONMENT:-staging}"

API_APP="ca-api-${ENVIRONMENT}"
POSTGRES_APP="ca-postgres-${ENVIRONMENT}"
SEQ_APP="ca-seq-${ENVIRONMENT}"
PGADMIN_APP="ca-pgadmin-${ENVIRONMENT}"

echo "============================================"
echo "RaysonDev Secret Seeding Script"
echo "============================================"
echo ""
echo "Resource Group: $RESOURCE_GROUP"
echo "Environment: $ENVIRONMENT"
echo ""

read -s -p "Enter JWT Signing Key: " JWT_SIGNING_KEY
echo ""
read -s -p "Enter PostgreSQL Password: " POSTGRES_PASSWORD
echo ""
read -s -p "Enter Seq Admin Password: " SEQ_ADMIN_PASSWORD
echo ""
read -p "Enter pgAdmin Email: " PGADMIN_EMAIL
echo ""
read -s -p "Enter pgAdmin Password: " PGADMIN_PASSWORD
echo ""
echo ""

echo "Setting GitHub repository secrets..."
echo ""
echo "Note: This script outputs the values you need to add as GitHub secrets."
echo "Run the following commands to set them:"
echo ""
echo "  gh secret set POSTGRES_PASSWORD --body \"$POSTGRES_PASSWORD\""
echo "  gh secret set JWT_SIGNING_KEY --body \"$JWT_SIGNING_KEY\""
echo "  gh secret set SEQ_ADMIN_PASSWORD --body \"$SEQ_ADMIN_PASSWORD\""
echo "  gh secret set PGADMIN_EMAIL --body \"$PGADMIN_EMAIL\""
echo "  gh secret set PGADMIN_PASSWORD --body \"$PGADMIN_PASSWORD\""
echo ""

echo "============================================"
echo "Next Steps"
echo "============================================"
echo ""
echo "1. Create a Service Principal for GitHub Actions:"
echo "   az ad sp create-for-rbac \\"
echo "     --name \"raysondev-github-actions\" \\"
echo "     --role \"Contributor\" \\"
echo "     --scopes /subscriptions/\$(az account show --query id -o tsv)/resourceGroups/$RESOURCE_GROUP \\"
echo "     --sdk-auth"
echo ""
echo "2. Add the JSON output to GitHub secret: AZURE_CREDENTIALS"
echo ""
echo "3. Add the secrets above to your GitHub repository"
echo ""
echo "4. Push to main branch to trigger deployment"
echo ""
