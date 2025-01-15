// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

using Microsoft.VisualStudio.TestPlatform.Utilities;
using SqlDataLayer.Classes;
using SqlDataLayer;
using Xunit.Abstractions;
using Xunit.Extensions.AssertExtensions;
using GenerateBooks;

namespace Test.UnitTests;

public class TestCreateBooksFromManningData(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void TestCreateOneBook()
    {
        //SETUP

        //ATTEMPT
        var manningBooks = CreateBooksFromManningData.CreateManningBooks().ToArray();

        //VERIFY
        _output.WriteLine($"{manningBooks.Count()}");

        for (int i = 0; i < 300; i++)
        {
            _output.WriteLine(manningBooks[i] + Environment.NewLine);
        }
        
    }
}