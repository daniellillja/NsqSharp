using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace NsqSharp.TestMethods.TestMethodHelpers
{
    public class TestData<TInput, TOutput> : Dictionary<TInput, Result<TOutput>>
    {
    }
}
