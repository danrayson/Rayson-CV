param environmentName string
param location string = resourceGroup().location
param environmentId string
param containerAppName string
param acrLoginServer string
param acrName string
param imageTag string = 'latest'
param jwtIssuer string
param jwtAudience string
@secure()
param jwtSigningKey string
param uiFqdn string
param postgresServiceId string
param tags object = {}

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource apiContainer 'Microsoft.App/containerApps@2023-05-01' = {
  name: containerAppName
  location: location
  properties: {
    managedEnvironmentId: environmentId
    configuration: {
      secrets: [
        {
          name: 'acr-password'
          value: acr.listCredentials().passwords[0].value
        }
        {
          name: 'jwt-signing-key'
          value: jwtSigningKey
        }
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 8080
        transport: 'auto'
      }
      registries: [
        {
          server: acrLoginServer
          username: acrName
          passwordSecretRef: 'acr-password'
        }
      ]
    }
    template: {
      serviceBinds: [
        {
          serviceId: postgresServiceId
          name: 'postgres'
        }
      ]
      containers: [
        {
          name: 'api'
          image: '${acrLoginServer}/raysondev-api:${imageTag}'
          env: [
            {
              name: 'ASPNETCORE_ENVIRONMENT'
              value: environmentName
            }
            {
              name: 'ASPNETCORE_URLS'
              value: 'http://+:8080'
            }
            {
              name: 'AuthOptions__Issuer'
              value: jwtIssuer
            }
            {
              name: 'AuthOptions__Audience'
              value: jwtAudience
            }
            {
              name: 'AuthOptions__IssuerSigningKey'
              secretRef: 'jwt-signing-key'
            }
            {
              name: 'Cors__AllowedOrigins__0'
              value: 'https://${uiFqdn}'
            }
            {
              name: 'LOG_LEVEL'
              value: 'Debug'
            }
          ]
          resources: {
            cpu: json('0.5')
            memory: '1.0Gi'
          }
        }
      ]
      scale: {
        minReplicas: 1
        maxReplicas: 3
      }
    }
  }
  tags: tags
}

output containerAppName string = apiContainer.name
output fqdn string = apiContainer.properties.configuration.ingress.fqdn
