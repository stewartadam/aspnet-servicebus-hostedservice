# aspnet-servicebus-hostedservice
## Overview
This project provides a Web API (ASP.NET Core 2) that submits messages to Service Bus, and then has background worker service created as a IHostedService to read from the queue and handle the long-running tasks.

For more details, see [Background tasks with hosted services in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/host/hosted-services?view=aspnetcore-2.2).

This is a code sample and not indended for production use as-is. Some components are missing, for example:
1. Connection clean-up on shutdown
2. Interface to support Event Hub or other enterprise messaging bus implementations
3. Exception and error handling
4. Aritrary message types

## License
This code sample is released under the MIT license.
