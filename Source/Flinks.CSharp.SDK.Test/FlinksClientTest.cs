using Xunit;
using System;
using System.Collections.Generic;

namespace Flinks.CSharp.SDK.Test
{
    public class FlinksClientTest
    {
        public static class FlinksClientInstantiationValueTest
        {
            private static readonly List<object[]> Data = new List<object[]>
            {
                new object[] { string.Empty, string.Empty },
                new object[] { null, null},
                new object[] { string.Empty, null}
            };

            public static IEnumerable<object[]> TestData => Data;

        }

        [Theory]
        [MemberData(nameof(FlinksClientInstantiationValueTest.TestData), MemberType = typeof(FlinksClientInstantiationValueTest))]
        public void Should_throw_an_exception_if_customerId_or_endpoint_is_null_or_empty(string customerId, string endpoint)
        {
            Assert.Throws<NullReferenceException>(() => new FlinksClient(customerId, endpoint));
        }
    }
}
