using SimpleGraphQLClient.UnitTests.TestObjects;
using System.Collections.Generic;

namespace Tests.TestObjects
{
    public class User
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
    }
    public class UserFull
    {
        public Address Address { get; set; }
        public UserStatus Status { get; set; }
        public string Username { get; set; }
        public double Debt { get; set; }
        public int Id { get; set; }
    }
    public class UserFullWithGenericType
    {
        public List<Address> Addresses { get; set; }
        public UserStatus Status { get; set; }
        public string Username { get; set; }
        public double Debt { get; set; }
        public int Id { get; set; }
    }
    public class UserWithEmptyClass
    {
        public int Id { get; set; }
        public EmptyClass EmptyClass { get; set; }
    }
}