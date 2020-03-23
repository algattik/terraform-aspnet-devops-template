// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.
// See LICENSE file in the project root for full license information.

namespace Contoso
{
    using System;

    /// <summary>
    /// Thrown when trying to sum number up to a negative number.
    /// </summary>
    public class SumComputationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SumComputationException"/> class.
        /// </summary>
        public SumComputationException()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SumComputationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public SumComputationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SumComputationException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference if no inner exception is specified.</param>
        public SumComputationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
