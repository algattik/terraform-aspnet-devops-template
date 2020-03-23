// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System.Threading.Tasks;

    /// <summary>
    /// Sample service.
    /// </summary>
    public interface ISampleService
    {
        /// <summary>
        /// Add two numbers and return their sum.
        /// </summary>
        /// <param name="value">Number to add values up to.</param>
        /// <returns>Sum of integer numbers from 0 to value.</returns>
        Task<int> SumNumbersUpToAsync(int value);
    }
}
