using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGraphQLClient.UnitTests.TestObjects
{
    public class ZipCode
    {
        public string ISO { get; set; }
        public string Format { get; set; }
        public string StreetLevelFormat { get; set; }
        public int CountryId { get; set; }
    }
}
