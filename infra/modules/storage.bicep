param location string
param storageAccountName string
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' = {
  name: storageAccountName
  location: location
  sku: {
    name: 'Standard_LRS'
  }
  kind: 'StorageV2'
  properties: {
    accessTier: 'Hot'
  }
  tags: tags
}

resource postgresFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: storageAccount
  name: 'default/postgres-data'
  properties: {
    shareQuota: 5
  }
}

resource seqFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: storageAccount
  name: 'default/seq-data'
  properties: {
    shareQuota: 2
  }
}

resource pgAdminFileShare 'Microsoft.Storage/storageAccounts/fileServices/shares@2023-01-01' = {
  parent: storageAccount
  name: 'default/pgadmin-data'
  properties: {
    shareQuota: 1
  }
}

output storageAccountName string = storageAccount.name
