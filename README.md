# Exceptional for ReSharper

Exceptional is an extension for ReSharper which analyzes thrown and documented C# exceptions and suggests improvements. 

## Motivation

When working with a code base - whether it is a small one or a big one - developers constantly encounter issues caused by wrong exception handling. There may be an excellent exception handling policy, but it is the developer who must execute this policy on its own code. Even with no policy defined, there are good practices on how to properly handle exceptions. This extension allows you to seamlessly apply these good practices with a couple of key strokes. 

Generally, the public API should be documented and thrown exceptions should be part of this documentation. But even if documenting thrown exceptions is pretty easy, the maintenance of the code that is using a particular method or property is not. This is where this extension comes into play: The extension analyzes call sites and provides hints on exceptions thrown from that invocations. If an exception is either not caught or not documented then you will be proposed to fix this problem. The extension also checks other good practices, for example that an inner exception is provided when rethrowing a new exception. 

## Installation

Requires ReSharper v8.2 or v9 

- Open the ReSharper menu in Visual Studio and select Extension Manager... 
- Search for Exceptional and install the extension

Open the menu ReSharper / Options... / Exceptional to configure the extension. 

Check out the extension in the ReSharper plugin gallery: 
 
- Exceptional for ReSharper 8 in the plugin gallery 
- Exceptional for ReSharper 9 in the plugin gallery 
- Exceptional for ReSharper 10 in the plugin gallery
