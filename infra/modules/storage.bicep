param location string
param environmentName string
param tags object = {}

var storageAccountName = 'strraysoncv${environmentName}'

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
}

resource staticWebsite 'Microsoft.Storage/storageAccounts/staticSites@2023-01-01' = {
  name: 'staticWebsite'
  parent: storageAccount
  properties: {
    storageAccountName: storageAccount.name
  }
}

resource webContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '\$web'
  parent: storageAccount
  properties: {
    publicAccess: 'Blob'
  }
}

output storageAccountName string = storageAccount.name
output staticWebsiteUrl string = 'https://${storageAccount.name}.z13.web.core.windows.net'
