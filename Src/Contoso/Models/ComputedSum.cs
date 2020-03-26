// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Microsoft.Azure.Cosmos.Fluent;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Computed sum of numbers.
    /// </summary>
    public class ComputedSum
    {
        /// <summary>
        /// Gets or sets the value up to which the sum is computed.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Format required by Cosmos DB.")]
        public string? id { get; set; }

        /// <summary>
        /// Gets or sets the sum of numbers from 0 to Id.
        /// </summary>
        public long? Sum { get; set; }
    }
}
