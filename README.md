# Exceptional for ReSharper

Exceptional is an extension for ReSharper which analyzes thrown and documented C# exceptions and suggests improvements.

### Motivation

When working with a code base - whether it is a small one or a big one - developers constantly encounter issues caused by wrong exception handling. There may be an excellent exception handling policy, but it is the developer who must execute this policy on its own code. Even with no policy defined, there are good practices on how to properly handle exceptions. This extension allows you to seamlessly apply these good practices with a couple of key strokes. 

Generally, the public API should be documented and thrown exceptions should be part of this documentation. But even if documenting thrown exceptions is pretty easy, the maintenance of the code that is using a particular method or property is not. This is where this extension comes into play: The extension analyzes call sites and provides hints on exceptions thrown from that invocations. If an exception is either not caught or not documented then you will be proposed to fix this problem. The extension also checks other good practices, for example that an inner exception is provided when rethrowing a new exception. 

## Installation

Requires ReSharper v8.2, v9 or v10

- Open the ReSharper menu in Visual Studio and select Extension Manager... 
- Search for Exceptional and install the extension

Open the menu ReSharper / Options... / Exceptional to configure the extension. 

Check out the extension in the ReSharper plugin gallery: 
 
- [Exceptional for ReSharper 8 in the plugin gallery](https://resharper-plugins.jetbrains.com/packages/Exceptional)
- [Exceptional for ReSharper 9 in the plugin gallery](https://resharper-plugins.jetbrains.com/packages/Exceptional.R9)
- [Exceptional for ReSharper 10 in the plugin gallery](https://resharper-plugins.jetbrains.com/packages/Exceptional.R10)
- [Exceptional for ReSharper 2016.1 in the plugin gallery](https://resharper-plugins.jetbrains.com/packages/Exceptional.2016.1)

## Features

### Thrown exception not documented or caught

**Warning:** Exceptions thrown outside the scope of method\property that are not documented in methods xml documentation (thrown with use of throw keyword).

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922440)

**Fix:** Document or catch thrown exception.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922441)

**Warning:** Exception thrown outside the scope of method\property that are not documented in methods xml documentation (thrown from another invocation).

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922442)

**Fix:** Document or catch thrown exception.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922443)

### Documented exception is not thrown

**Warning:** Exceptions documented in XML documentation that are not thrown from method/property.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922448)

**Fix:** Remove documentation of not thrown exception.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922445)

### Catch-all clauses

**Warning:** General catch-all clauses should be avoided.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922446)

### Not passing inner exception

**Warning:** Throwing new exception from catch clause should include message and inner exception.

![](http://download-codeplex.sec.s-msft.com/Download?ProjectName=exceptional&DownloadId=922447)

### Do not throw System.Exception

**Warning:** Throwing System.Exception should be avoided.

More features to come...



(This project has originally been [hosted on CodePlex](https://exceptional.codeplex.com/))
