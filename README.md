# aspnet-servicebus-hostedservice
## Overview
This project provides a Web API (ASP.NET Core 2) that submits messages to Service Bus, and then has background worker service created as a IHostedService to read from the queue and handle the long-running tasks.

For more details, see [Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2).

This is a code sample and not indended for production use as-is. Some components are missing, for example:
1. Connection clean-up on shutdown
2. Interface to support Event Hub or other enterprise messaging bus implementations
3. Exception and error handling
4. Aritrary message types

## Known issues
Due to the way the .NET Azure SDK is coded, the message auto-renew timer continues in the background even if messages are completed. This can result in harmless `MessageLockLostException` exceptions, especially for some long-running message handlers:
```
Microsoft.Azure.ServiceBus.MessageLockLostException: The lock supplied is invalid. Either the lock expired, or the message has already been removed from the queue.
```
The SDK authors have determined this behavior is as designed. See [Azure/azure-sdk-for-net#6723](https://github.com/Azure/azure-sdk-for-net/issues/6723) for details.

## License
This code sample is released under the MIT license.
