using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lithnet.MetadirectoryServices
{
    /// <summary>
    /// A list of attribute types that extended from the standard Microsoft MetadirectoryServices Attribute Type enum
    /// </summary>
    public enum ExtendedAttributeType
    {
        /// <summary>
        /// A string data type
        /// </summary>
        String = 0,

        /// <summary>
        /// An integer data type
        /// </summary>
        Integer = 1,

        /// <summary>
        /// A reference data type
        /// </summary>
        Reference = 2,

        /// <summary>
        /// A binary data type
        /// </summary>
        Binary = 3,

        /// <summary>
        /// A boolean data type
        /// </summary>
        Boolean = 4,

        /// <summary>
        /// An undefined data type
        /// </summary>
        Undefined = 5,

        /// <summary>
        /// A DateTime data type
        /// </summary>
        DateTime = 99
    }
}
