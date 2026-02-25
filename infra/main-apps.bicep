param location string = resourceGroup().location
param environmentId string
param defaultDomain string
param acrLoginServer string
param acrName string
param imageTag string
param jwtIssuer string
param jwtAudience string
@secure()
param jwtSigningKey string
@secure()
param seqAdminPassword string
param storageAccountName string
param environmentName string
param tags object = {
  Environment: environmentName
  Project: 'RaysonDev'
}

var postgresServiceName = 'ca-postgres-${environmentName}'
var seqContainerName = 'ci-seq-${environmentName}'
var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var uiFqdn = '${uiAppName}.${defaultDomain}'

module postgresService 'modules/postgres-service.bicep' = {
  name: 'postgres-service'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: postgresServiceName
    tags: tags
  }
}

module seq 'modules/seq-container.bicep' = {
  name: 'seq-container'
  params: {
    location: location
    containerGroupName: seqContainerName
    storageAccountName: storageAccountName
    seqAdminPassword: seqAdminPassword
    tags: tags
  }
}

module api 'modules/api-container.bicep' = {
  name: 'api-container'
  params: {
    location: location
    environmentId: environmentId
    environmentName: environmentName
    containerAppName: apiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience
    jwtSigningKey: jwtSigningKey
    uiFqdn: uiFqdn
    seqUrl: 'http://${seq.outputs.fqdn}:5341'
    postgresServiceId: postgresService.outputs.serviceId
    tags: tags
  }
}

module ui 'modules/ui-container.bicep' = {
  name: 'ui-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: uiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    apiHealthUrl: 'http://${apiAppName}.internal.${defaultDomain}:8080/health'
    tags: tags
  }
  dependsOn: [
    api
  ]
}

output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
