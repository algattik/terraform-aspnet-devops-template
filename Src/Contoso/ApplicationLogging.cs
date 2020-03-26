// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Shared logger.
    /// </summary>
    internal static class ApplicationLogging
    {
        /// <summary>
        /// Gets or sets application logger factory. Value is set by the Startup class.
        /// </summary>
        internal static ILoggerFactory LoggerFactory { get; set; } = null!;

        /// <summary>
        /// Create a logger for a non-static class.
        /// </summary>
        /// <typeparam name="T">Class to create logger for.</typeparam>
        /// <returns>Logger for class.</returns>
        internal static Microsoft.Extensions.Logging.ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();

        /// <summary>
        /// Create a logger for a static class.
        /// </summary>
        /// <param name="categoryName">Class name to create logger for.</param>
        /// <returns>Logger for class.</returns>
        internal static Microsoft.Extensions.Logging.ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);
    }
}
