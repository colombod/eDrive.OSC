#region

using System;

#endregion

namespace eDrive.Osc
{
    /// <summary>
    ///     Hold session and debug setting
    /// </summary>
    public class OscSessionSettings
    {
        /// <summary>
        ///     Gets or sets a value indicating whether [debug dump].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [debug dump]; otherwise, <c>false</c>.
        /// </value>
        public static bool DebugDump { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether [raise parsing exceptions].
        /// </summary>
        /// <value>
        ///     <c>true</c> if [raise parsing exceptions]; otherwise, <c>false</c>.
        /// </value>
        public static bool RaiseParsingExceptions { get; set; }
    }
}