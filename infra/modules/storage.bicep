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
    staticWebsite: {
      enabled: true
      indexDocument: 'index.html'
    }
  }
  tags: tags
}

output storageAccountName string = storageAccount.name
output staticWebsiteUrl string = 'https://${storageAccount.name}.z13.web.${environment().suffixes.storage}'
