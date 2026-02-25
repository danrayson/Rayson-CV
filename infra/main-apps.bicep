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
param environmentName string
param tags object = {
  Environment: environmentName
  Project: 'RaysonDev'
}

var postgresServiceName = 'ca-postgres-${environmentName}'
var lokiAppName = 'ca-loki-${environmentName}'
var grafanaAppName = 'ca-grafana-${environmentName}'
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

module loki 'modules/loki-container.bicep' = {
  name: 'loki-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: lokiAppName
    tags: tags
  }
}

module grafana 'modules/grafana-container.bicep' = {
  name: 'grafana-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: grafanaAppName
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
    lokiUrl: loki.outputs.url
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
output lokiFqdn string = loki.outputs.fqdn
output grafanaFqdn string = grafana.outputs.fqdn
