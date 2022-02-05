﻿namespace Microsoft.DataTransfer.AzureTable.Utils
{
    using Azure.Storage;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Utility functions
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// Execute an operation with retries
        /// </summary>
        /// <param name="func">Function to execute</param>
        /// <param name="retries">Number of retries</param>
        public static async Task ExecuteWithRetryAsync(Func<Task> func, int retries=3)
        {
            while (retries > 0)
            {
                try
                {
                    await func();
                    break;
                }
                catch (StorageException ex)
                {
                    if (ex.RequestInformation.HttpStatusCode == 429)
                    {
                        retries--;
                        if(retries == 0)
                        {
                            throw new StorageException("The table's allocated RUs are too low " +
                                "to support the request rate generated by this process, " + 
                                " please either reduce batch size, " + 
                                "increase RUs or wait until total table load is reduced");
                        }
                        await Task.Delay(3000);
                    }
                    else
                    {
                        throw;
                    }
                }
            }
        }
    }
}
