param location string = resourceGroup().location
param environmentId string
param containerAppName string
param acrLoginServer string
param acrName string
param imageTag string = 'latest'
param apiHealthUrl string
param appDownloadUrl string
param customDomainName string = ''
param tags object = {}
param environmentName string

resource acr 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: acrName
}

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' existing = {
  name: environmentName
}

resource rootCert 'Microsoft.App/managedEnvironments/managedCertificates@2023-05-01' = if (customDomainName != '') {
  name: 'cert-${customDomainName}'
  parent: containerAppEnvironment
  location: location
  properties: {
    subjectName: customDomainName
    domainControlValidation: 'CNAME'
  }
}

resource wwwCert 'Microsoft.App/managedEnvironments/managedCertificates@2023-05-01' = if (customDomainName != '') {
  name: 'cert-www-${customDomainName}'
  parent: containerAppEnvironment
  location: location
  properties: {
    subjectName: 'www.${customDomainName}'
    domainControlValidation: 'CNAME'
  }
}

resource uiContainer 'Microsoft.App/containerApps@2023-05-01' = {
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
      ]
      activeRevisionsMode: 'Single'
      ingress: {
        external: true
        targetPort: 3000
        transport: 'auto'
        customDomains: customDomainName != '' ? [
          {
            name: customDomainName
            bindingType: 'SniEnabled'
            certificateId: rootCert.id
          }
          {
            name: 'www.${customDomainName}'
            bindingType: 'SniEnabled'
            certificateId: wwwCert.id
          }
        ] : []
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
  tags: tags
}

output containerAppName string = uiContainer.name
output fqdn string = uiContainer.properties.configuration.ingress.fqdn
