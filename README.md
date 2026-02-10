# AI Foundry with Azure Key Vault: Secure Text Analyzer

A C# sample that calls **Azure AI Language Services** (Text Analytics) using credentials stored securely in **Azure Key Vault**. This approach keeps API keys and endpoints out of source code and config files, improving security and compliance.

---

## Security benefits

| Aspect | Benefit |
|--------|--------|
| **No secrets in code** | Keys and endpoints live only in Key Vault, not in repositories or config files. |
| **Centralized secret management** | Rotate or revoke credentials in one place without redeploying the app. |
| **Access control** | Only identities with Key Vault access policies (or RBAC) can read secrets. |
| **Auditability** | Key Vault logs access to secrets for compliance and forensics. |
| **Managed identity support** | Apps can use Azure AD identities (e.g. DefaultAzureCredential) instead of storing any credentials. |

---

## Prerequisites

- [Azure subscription](https://azure.microsoft.com/free/)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Visual Studio 2022 (or VS Code / Rider) with .NET workload
- Azure CLI (`az`) installed and used to sign in (optional but recommended)

---

## Setup steps

### 1. Sign in to Azure Portal

1. Go to [https://portal.azure.com](https://portal.azure.com).
2. Sign in with your Azure account.
3. Ensure you have permissions to create resources and assign access in the subscription or resource group you will use.

---

### 2. Create an Azure AI Services account

1. In the portal, search for **Azure AI services** (or **Cognitive Services**).
2. Click **Create** and choose **Azure AI services (multi-service)** or **Language**.
3. Select subscription, resource group, and region; give the resource a name.
4. Choose pricing tier and click **Review + create** → **Create**.
5. After deployment, go to the resource → **Keys and Endpoint**.
6. Copy one **Key** and the **Endpoint** URL; you will store these in Key Vault in a later step.

---

### 3. Create an Azure Key Vault

1. In the portal, search for **Key Vault** and click **Create**.
2. Choose subscription and resource group (same as the AI resource is fine).
3. Set a **Key vault name** (e.g. `my-ai-keyvault`). It must be globally unique.
4. Select region and leave other defaults or adjust as needed.
5. Click **Review + create** → **Create**.
6. Note the vault name; your app will use it (e.g. via `KEY_VAULT_NAME` or in code).

---

### 4. Add credentials to your Key Vault

1. Open your Key Vault resource.
2. Under **Objects**, go to **Secrets** → **+ Generate/Import**.
3. Create two secrets:

   | Secret name | Value |
   |-------------|--------|
   | `CognitiveServicesKey` | The **Key** from your AI services resource (Keys and Endpoint). |
   | `CognitiveServicesEndpoint` | The **Endpoint** URL from your AI services resource. |

4. Use **Secret value** only for the actual key/URL; do not put secrets in **Tags** or descriptions.

---

### 5. Create an Access Policy for your Key Vault

1. In your Key Vault, go to **Access policies** (under **Settings**).
2. Click **+ Create** (or **+ Add Access Policy**).
3. Under **Secret permissions**, select **Get** (and **List** if the app lists secrets).
4. Under **Select principal**, choose:
   - Your **user account** (for local development), and/or  
   - A **Managed Identity** or **service principal** (for deployed apps).
5. Click **Review + create** → **Create**, then **Save** to apply.

For production, prefer **Azure RBAC** (e.g. Key Vault Secrets User) instead of access policies, and grant only the identity that runs the app.

---

### 6. Create a new C# application in Visual Studio IDE

1. Open **Visual Studio** and create a new project.
2. Choose **Console App** (C#) and target **.NET 8**.
3. Add these NuGet packages:
   - `Azure.Core`
   - `Azure.Identity`
   - `Azure.Security.KeyVault.Secrets`
   - `Azure.AI.TextAnalytics`

   Or from the project directory in a terminal:

   ```bash
   dotnet add package Azure.Core
   dotnet add package Azure.Identity
   dotnet add package Azure.Security.KeyVault.Secrets
   dotnet add package Azure.AI.TextAnalytics
   ```

4. Replace the default `Program.cs` with the sample code that:
   - Uses `SecretClient` and `DefaultAzureCredential` to read from Key Vault.
   - Uses the retrieved key and endpoint to call the Text Analytics (Language) client.

   You can clone this repo and use its `Program.cs` as the sample.

5. Set the Key Vault name:
   - In code: set the variable (e.g. `keyVaultName`) to your vault name, or  
   - Via environment: set `KEY_VAULT_NAME` to your vault name so the app uses it at runtime.

---

### 7. Send a test Language Service call

1. For **local development**, sign in so `DefaultAzureCredential` can authenticate:
   - **Azure CLI**: run `az login`.
   - Or use Visual Studio **Azure sign-in** (signed-in user will be used by DefaultAzureCredential when no other credential is available).
2. Run the application (F5 in Visual Studio or `dotnet run` in the project folder).
3. The app will:
   - Retrieve `CognitiveServicesKey` and `CognitiveServicesEndpoint` from Key Vault.
   - Call the Language service (e.g. **Recognize Entities** on a sample sentence).
   - Print the detected entities (e.g. “Seattle” as Location) to the console.

If you see “Secrets retrieved successfully” and entity output, the end-to-end flow—Key Vault → app → AI Language—is working and credentials never appear in your source code.

---

## Sample output

When the application runs successfully, you should see output similar to the following (exact entities and confidence scores may vary slightly):

```
Retrieving secrets from Key Vault...
Secrets retrieved successfully.
Sending NER request...

Text: Seattle
Category: Location
Confidence: 0.99

Text: last week
Category: DateTime
Confidence: 0.98
```

- The first two lines confirm that the app read the API key and endpoint from Key Vault.
- **Sending NER request...** indicates the call to the Language service (Named Entity Recognition).
- Each block shows an **entity** detected in the sample text: the **Text** (span from the input), **Category** (e.g. Location, DateTime, Person, Organization), and **Confidence** score (0–1).

---

## Project structure

```
KeyVault_AIFoundry/
├── KeyVault_AIFoundry.csproj   # Project and package references
├── Program.cs                  # Key Vault + Text Analytics sample
└── README.md                   # This file
```

---

## Security checklist

- [ ] API key and endpoint stored only in Key Vault (not in code or config).
- [ ] Key Vault access limited to the app identity (and your dev account if needed).
- [ ] No secrets committed to Git; use env vars or Key Vault name only in config.
- [ ] For production, use Managed Identity or a service principal with minimal Key Vault permissions (e.g. Get/List on secrets only).
- [ ] Enable Key Vault logging/diagnostics for audit trails.

---

## References

- [Azure Key Vault](https://learn.microsoft.com/azure/key-vault/general/overview)
- [Azure AI Language Services](https://learn.microsoft.com/azure/ai-services/language-service/)
- [DefaultAzureCredential](https://learn.microsoft.com/dotnet/api/azure.identity.defaultazurecredential)
- [Best practices for Key Vault](https://learn.microsoft.com/azure/key-vault/general/best-practices)
