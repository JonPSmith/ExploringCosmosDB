// Copyright (c) 2025 Jon P Smith, GitHub: JonPSmith, web: http://www.thereformedprogrammer.net/
// Licensed under MIT license. See License.txt in the project root for license information.

namespace Test.Helpers;

public static class GetDataFromLogs
{
    //this takes the queries starting with "Executed DbCommand (31ms)" and returns the total time
    public static int FindTotalQueryTime(this List<string> logs)
    {
        int result = 0;
        var startPart = "Executed DbCommand (";
        foreach (var log in logs)
        {
            if (log.StartsWith(startPart))
            {
                var endCloseLength = log.IndexOf("ms)");
                var queryTime = log.Substring(startPart.Length, endCloseLength - startPart.Length);
                result += Int32.Parse(queryTime);
            }
        }

        return result;
    }
}