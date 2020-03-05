using System;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace XMAS2019.Domain.Infrastructure
{
    public static class DocumentClientFactory
    {
        public static IDocumentClient Create()
        {
            // internal write-key (NOTE: never hard-code credentials nor store these in configuration - use Azure Key Vault)
            const string key = "...";

            return new DocumentClient(new Uri("https://xmas2019.documents.azure.com"), key);
        }

        public static string ReadOnlyAuthKey => "...";
    }
}