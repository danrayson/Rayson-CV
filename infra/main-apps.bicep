param location string = resourceGroup().location
param environmentId string
param defaultDomain string
param acrLoginServer string
param acrName string
param imageTag string
param postgresDb string
param postgresUser string
@secure()
param postgresPassword string
param jwtIssuer string
param jwtAudience string
@secure()
param jwtSigningKey string
param corsOrigins array
@secure()
param seqAdminPassword string
param pgAdminEmail string
@secure()
param pgAdminPassword string

param environmentName string
param tags object = {
  Environment: environmentName
  Project: 'RaysonDev'
}

var postgresAppName = 'ca-postgres-${environmentName}'
var seqAppName = 'ca-seq-${environmentName}'
var apiAppName = 'ca-api-${environmentName}'
var uiAppName = 'ca-ui-${environmentName}'
var pgAdminAppName = 'ca-pgadmin-${environmentName}'

module postgres 'modules/postgres-container.bicep' = {
  name: 'postgres-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: postgresAppName
    postgresDb: postgresDb
    postgresUser: postgresUser
    postgresPassword: postgresPassword
    tags: tags
  }
}

module seq 'modules/seq-container.bicep' = {
  name: 'seq-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: seqAppName
    seqAdminPassword: seqAdminPassword
    tags: tags
  }
}

module api 'modules/api-container.bicep' = {
  name: 'api-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: apiAppName
    acrLoginServer: acrLoginServer
    acrName: acrName
    imageTag: imageTag
    jwtIssuer: jwtIssuer
    jwtAudience: jwtAudience
    jwtSigningKey: jwtSigningKey
    defaultConnection: 'Server=${postgresAppName}.internal.${defaultDomain};Port=5432;Database=${postgresDb};User Id=${postgresUser};Password=${postgresPassword};TrustServerCertificate=True'
    corsOrigins: corsOrigins
    seqUrl: 'http://${seqAppName}.internal.${defaultDomain}:5341'
    tags: tags
  }
  dependsOn: [
    postgres
    seq
  ]
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

module pgAdmin 'modules/pgadmin-container.bicep' = {
  name: 'pgadmin-container'
  params: {
    location: location
    environmentId: environmentId
    containerAppName: pgAdminAppName
    pgAdminEmail: pgAdminEmail
    pgAdminPassword: pgAdminPassword
    tags: tags
  }
  dependsOn: [
    postgres
  ]
}

output apiFqdn string = api.outputs.fqdn
output uiFqdn string = ui.outputs.fqdn
output seqFqdn string = seq.outputs.fqdn
output pgAdminFqdn string = pgAdmin.outputs.fqdn
