// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso.UnitTests
{
    using Contoso;
    using Xunit;

    public class CosmosDBServiceTests
    {
        [Fact]
        public void ParseCosmosDBContainerResourceId()
        {
            var cosmosDBContainerResourceId = "/subscriptions/a4ed7b9a-0000-49b4-a6ee-fd07ff6e296d/resourceGroups/aspnetmplt/providers/Microsoft.DocumentDB/databaseAccounts/cosmos-aspnetmplt-cd/apis/sql/databases/ComputedSums/containers/ComputedSumsCtr";

            CosmosDBService.ParseCosmosDBContainerResourceId(
                cosmosDBContainerResourceId,
                out string cosmosDBResourceId,
                out string cosmosDBDatabase,
                out string cosmosDBContainer);
            Assert.Equal("/subscriptions/a4ed7b9a-0000-49b4-a6ee-fd07ff6e296d/resourceGroups/aspnetmplt/providers/Microsoft.DocumentDB/databaseAccounts/cosmos-aspnetmplt-cd", cosmosDBResourceId);
            Assert.Equal("ComputedSums", cosmosDBDatabase);
            Assert.Equal("ComputedSumsCtr", cosmosDBContainer);
        }
    }
}
