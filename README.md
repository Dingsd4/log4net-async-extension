# log4net-async-extension

log4net async extension is an async execution extension for ILog and ILogger implementations.

## Package

A package is available at [**nuget.org**](https://www.nuget.org/packages/log4net.async.extension)

## Master

The primary repo for the project is on [GitHub](https://github.com/Dingsd4/log4net-async-extension) and this is where the [wiki](https://github.com/Dingsd4/log4net-async-extension/wiki) and [issues](https://github.com/Dingsd4/log4net-async-extension/issues) are managed from.

## Licence

All original software is licensed under the [LGPL-3 Licence](https://github.com/Dingsd4/log4net-async-extension/blob/master/LICENSE). This does not apply to any other 3rd party tools, utilities or code which may be used to develop this application.

If anyone is aware of any licence violations that this code may be making please inform the developers so that the issue can be investigated and rectified.

## Building

You will need:

1. Visual Studio VS2017 (Community Edition) or later
2. Official log4net nuget package
3. Target Framework packs:
    * netstandard1.0
    * netstandard1.3
    * netstandard2.0
    * netcoreapp1.0
    * netcoreapp2.0
    * net20
    * net35
    * net40
    * net45
    * net46
    * net47

## Notes

Using an async logger is only a good idea if you know exactly that you need it.

We use async loggers in high priority threads needing to wait for long periods of time and then generating thousands of log events in a very short working time. Since the short working time period shall not wait until the backend has time for it an async logger is used.

Please keep in mind that the log backend may no longer receive all events in the correct order (async log events will be late, but will be in order when only viewing the async events).

A common log result may look like:

```log
00:00:01 sync event 1
00:00:01 async event 1
00:00:02 async event 2
00:00:02 sync event 2
00:00:03 sync event 3
00:00:04 sync event 4
00:00:03 async event 3
00:00:05 sync event 5
00:00:04 async event 4
00:00:05 async event 5
```

## First use

You can simply convert any ILog and ILogger implementation using the ``AsAsync()`` extension method.
With compilers not supporting extension methods, please use ``LoggerAsync.AsAsync(ILogger)`` and ``LoggerAsync.AsAsync(ILog)``.

This is how you get an async logger:

```csharp
using log4net;

static ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType).AsAsync();
```

