using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleGraphQLClient.UnitTests.TestObjects
{
    public class Address
    {
        public ZipCode ZipCode { get; set; }
        public int HouseNumber { get; set; }
        public string Street { get; set; }
    }
}
