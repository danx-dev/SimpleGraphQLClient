using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGraphQLClient.UnitTests.TestObjects
{
    public class Country
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ISO { get; set; }
        public List<ZipCode> ZipCodes { get; set; }
    }
}
