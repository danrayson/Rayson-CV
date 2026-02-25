param location string = resourceGroup().location
param containerGroupName string
param storageAccountName string
@secure()
param seqAdminPassword string
param tags object = {}

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-01-01' existing = {
  name: storageAccountName
}

resource seqContainer 'Microsoft.ContainerInstance/containerGroups@2023-05-01' = {
  name: containerGroupName
  location: location
  properties: {
    osType: 'Linux'
    ipAddress: {
      type: 'Public'
      ports: [
        {
          protocol: 'TCP'
          port: 80
        }
        {
          protocol: 'TCP'
          port: 5341
        }
      ]
      dnsNameLabel: containerGroupName
    }
    containers: [
      {
        name: 'seq'
        properties: {
          image: 'datalust/seq:latest'
          ports: [
            {
              protocol: 'TCP'
              port: 80
            }
            {
              protocol: 'TCP'
              port: 5341
            }
          ]
          environmentVariables: [
            {
              name: 'ACCEPT_EULA'
              value: 'Y'
            }
            {
              name: 'SEQ_FIRSTRUN_ADMINPASSWORD'
              secureValue: seqAdminPassword
            }
          ]
          resources: {
            requests: {
              cpu: 1
              memoryInGB: 1
            }
          }
          volumeMounts: [
            {
              name: 'seq-data'
              mountPath: '/data'
            }
          ]
        }
      }
    ]
    volumes: [
      {
        name: 'seq-data'
        azureFile: {
          shareName: 'seq-data'
          storageAccountName: storageAccountName
          storageAccountKey: storageAccount.listKeys().keys[0].value
        }
      }
    ]
  }
  tags: tags
}

output containerGroupName string = seqContainer.name
output fqdn string = seqContainer.properties.ipAddress.fqdn
output ipAddress string = seqContainer.properties.ipAddress.ip
