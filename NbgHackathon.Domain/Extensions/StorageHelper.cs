using Microsoft.WindowsAzure.Storage.Table;
using System;

namespace NbgHackathon.Domain
{
    internal static class StorageHelper
    {
        private const string TokenPartDelimiter = "||";

        public static TableContinuationToken ConvertToContinuationToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return null;

            var tokenParts = GetTokenParts(token);
            return new TableContinuationToken()
            {
                NextPartitionKey = tokenParts.nextPartitionKey,
                NextRowKey = tokenParts.nextRowKey,
                NextTableName = tokenParts.nextTableName,
                TargetLocation = Microsoft.WindowsAzure.Storage.StorageLocation.Primary
            };
        }

        public static string ConvertToString(this TableContinuationToken token)
        {
            if (token == null)
                return null;

            var tokenParts = new[] { token.NextPartitionKey, token.NextRowKey, token.NextTableName };
            return string.Join(TokenPartDelimiter, tokenParts);
        }

        private static (string nextPartitionKey, string nextRowKey, string nextTableName) GetTokenParts(string token)
        {
            var tokenParts = token.Split(new[] { TokenPartDelimiter }, StringSplitOptions.None);
            return (tokenParts[0], tokenParts[1], tokenParts[2]);
        }
    }
}
