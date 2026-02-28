param environmentName string
param location string = resourceGroup().location
param postgresAdminUsername string
@secure()
param postgresAdminPassword string
param tags object = {}

var serverName = 'pg-${environmentName}-${uniqueString(resourceGroup().id)}'
var databaseName = 'raysoncv'

resource postgresServer 'Microsoft.DBforPostgreSQL/flexibleServers@2023-06-01' = {
  name: serverName
  location: location
  tags: tags
  sku: {
    name: 'Standard_B1ms'
    tier: 'Burstable'
  }
  properties: {
    version: '16'
    storage: {
      storageSizeGB: 32
    }
    administratorLogin: postgresAdminUsername
    administratorLoginPassword: postgresAdminPassword
    highAvailability: {
      mode: 'Disabled'
    }
    azure: {
      extensions: ['vector']
    }
  }
}

resource database 'Microsoft.DBforPostgreSQL/flexibleServers/databases@2023-06-01' = {
  name: '${serverName}/${databaseName}'
  properties: {
    charset: 'utf8'
    collation: 'en_US.utf8'
  }
}

resource firewallRule 'Microsoft.DBforPostgreSQL/flexibleServers/firewallRules@2023-06-01' = {
  name: '${serverName}/allow-container-apps'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '255.255.255.255'
  }
}

output postgresFqdn string = postgresServer.properties.fullyQualifiedDomainName
output postgresHost string = serverName
output databaseName string = databaseName
