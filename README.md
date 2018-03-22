# NBG Hackathon

NBG hackathon prototype implementing a customer onboarding process via using the following technologies:
- Microsoft Bot Framework
- Microsoft Azure Functions
- Microsoft Azure Storage (Tables, Queues, Blobs)
- Microsoft Cognitive Services (Face and Emotion APIs)
- Microsoft Logic Apps

### TODOs
- Deploy a public facing sample on Azure
- Document the intent, the architecture and the business flow of the scenario implemented

## Notes
- As the Azure Functions SDK v1.0.9 nuget has a strict dependency on NewtonSoft.Json (=v9.0.1), we had to fall back using WindowsAzure.Storage v8.7.0 as later versions depend on NewtonSoft.Json (>= v10.0.2).