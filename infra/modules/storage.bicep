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
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: tags

  resource lokiBlobContainer 'containers@2023-01-01' = {
    name: 'loki'
    properties: {
      publicAccess: 'None'
    }
  }
}

output storageAccountName string = storageAccount.name
output lokiBlobEndpoint string = '${storageAccount.properties.primaryEndpoints.blob}loki'
