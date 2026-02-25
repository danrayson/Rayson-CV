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
    allowBlobPublicAccess: true
    networkAcls: {
      defaultAction: 'Allow'
    }
  }
  tags: tags
}

resource webContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-01-01' = {
  name: '${storageAccountName}/default/$web'
  properties: {
    publicAccess: 'Blob'
  }
}

output storageAccountName string = storageAccount.name
output blobBaseUrl string = 'https://${storageAccount.name}.blob.${environment().suffixes.storage}'
