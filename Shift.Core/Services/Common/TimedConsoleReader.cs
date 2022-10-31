// -----------------------------------------------------------------------
// <copyright file="TimedConsoleReader.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System;
using System.Threading;

namespace Shift.Core.Services.Common
{
    /// <summary>
    /// Console reader with a timeout period.
    /// Reference: https://stackoverflow.com/a/18342182
    /// </summary>
    public class TimedConsoleReader
    {
        private static AutoResetEvent getInput, gotInput;
        private static string input;
        private static Thread inputThread;

        static TimedConsoleReader()
        {
            getInput = new AutoResetEvent(false);
            gotInput = new AutoResetEvent(false);
            inputThread = new Thread(reader);
            inputThread.IsBackground = true;
            inputThread.Start();
        }

        // omit the parameter to read a line without a timeout
        public static string ReadLine(int timeOutMillisecs = Timeout.Infinite)
        {
            getInput.Set();
            bool success = gotInput.WaitOne(timeOutMillisecs);
            return success ? input : throw new TimeoutException(
                message: $"User did not provide input within {timeOutMillisecs / 1000f} seconds.");
        }

        private static void reader()
        {
            while (true)
            {
                getInput.WaitOne();
                input = Console.ReadLine();
                gotInput.Set();
            }
        }
    }
}