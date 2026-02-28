param location string = resourceGroup().location
param containerAppName string
param acrLoginServer string
param acrName string
param imageTag string = 'latest'
param apiHealthUrl string
param appDownloadUrl string
param customDomainName string = ''
param tags object = {}
param caeName string

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: caeName
}

resource uiContainer 'Microsoft.App/containerApps@2025-07-01' = {
  name: containerAppName
  location: location
  tags: tags

  properties: {
    managedEnvironmentId: containerAppEnvironment.id

    configuration: {
      activeRevisionsMode: 'Single'

      secrets: [
        {
          name: 'acr-password'
          value: acr.listCredentials().passwords[0].value
        }
      ]

      registries: [
        {
          server: acrLoginServer
          username: acrName
          passwordSecretRef: 'acr-password'
        }
      ]

      ingress: {
        external: true
        targetPort: 3000
        transport: 'auto'

        customDomains: customDomainName != '' ? [
          {
            name: customDomainName
            bindingType: 'Auto'
          }
          {
            name: 'www.${customDomainName}'
            bindingType: 'Auto'
          }
        ] : []
      }
    }

    template: {
      containers: [
        {
          name: 'ui'
          image: '${acrLoginServer}/raysoncv-ui:${imageTag}'

          env: [
            {
              name: 'API_HEALTH_URL'
              value: apiHealthUrl
            }
            {
              name: 'VITE_APP_DOWNLOAD_URL'
              value: appDownloadUrl
            }
            {
              name: 'LOG_LEVEL'
              value: 'info'
            }
          ]

          resources: {
            cpu: json('0.25')
            memory: '0.5Gi'
          }
        }
      ]

      scale: {
        minReplicas: 1
        maxReplicas: 1
      }
    }
  }
}

output containerAppName string = uiContainer.name
output fqdn string = uiContainer.properties.configuration.ingress.fqdn
