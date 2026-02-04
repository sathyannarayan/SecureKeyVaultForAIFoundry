using Azure;
using Azure.AI.TextAnalytics;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

// Set your Key Vault name here or via environment variable
string keyVaultName = "whizlab21";

const string keySecretName = "CognitiveServicesKey";
const string endpointSecretName = "CognitiveServicesEndpoint";

var kvUri = new Uri($"https://{keyVaultName}.vault.azure.net/");
var secretClient = new SecretClient(kvUri, new DefaultAzureCredential());

Console.WriteLine("Retrieving secrets from Key Vault...");

try
{
    KeyVaultSecret keySecret = secretClient.GetSecret(keySecretName).Value;
    KeyVaultSecret endpointSecret = secretClient.GetSecret(endpointSecretName).Value;
    string key = keySecret.Value;
    string endpoint = endpointSecret.Value;

    Console.WriteLine("Secrets retrieved successfully.");

    EntityRecognitionExample(key, endpoint);
}
catch (RequestFailedException ex)
{
    Console.Error.WriteLine($"Key Vault or Text Analytics error: {ex.Message}");
    throw;
}

static void EntityRecognitionExample(string key, string endpoint)
{
    var text = "I wish to visit USA California and New York for education and business this year";

    var credential = new AzureKeyCredential(key);
    var client = new TextAnalyticsClient(new Uri(endpoint), credential);

    Console.WriteLine("Sending NER request...\n");

    Response<CategorizedEntityCollection> response = client.RecognizeEntities(text);

    foreach (CategorizedEntity entity in response.Value)
    {
        Console.WriteLine($"Text: {entity.Text}");
        Console.WriteLine($"Category: {entity.Category}");
        Console.WriteLine($"Confidence: {entity.ConfidenceScore:F2}\n");
    }
}
