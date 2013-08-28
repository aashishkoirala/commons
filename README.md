### Commons Library
##### By [Aashish Koirala](http://aashishkoirala.github.io)

This is a general purpose commons library that I've built mostly for my own use within applications that I build (but obviously you are welcome to use it should you choose). The goal is to have an easy-to-use and uniform yet modular and pluggable facility to handle common cross-cutting concerns in my applications, thereby freeing me up to focus on the actual application concerns and hopefully making it easier and faster to build them.

In general, the library consists of interfaces or SPIs along with ways to access them (using MEF). One can then build implementations for these interfaces (i.e. providers) and hook them up to an application through configuration. What this does for me as provide a uniform way to access these services in all my applications while allowing me to switch or extend providers as I see fit.

For more detailed information and documentation, please visit the GitHub page for this repository at [aashishkoirala.github.io/commons](http://aashishkoirala.github.io/commons). You can get this as a NuGet package [here](https://www.nuget.org/packages/AK.Commons/).

This library only consists of interfaces and simple providers - the idea is to not need any extra libraries other than the framework assemblies. The web components need System.Web.Mvc and System.Net.Http - but I believe you can do without them if you don't use the web components. I have been building providers as I go. You can go through them in their own repository at [aashishkoirala.github.io/commons-providers](http://aashishkoirala.github.io/commons-providers).

###### Updates (v0.1.2)
This release consists of the following enhancements:

+ Introduced a logging provider base class that combines common properties such as Enabled and the new LogLevelFilter property. This supports the new feature of log-level filtering (i.e. you can now specify whether a logging provider is invoked only for certain types of log levels).
+ Added a new log level "Diagnostic" which is the most verbose. 
+ Logging now never throws due to a silent catch that was necessary to keep from the logging thread crashing if a provider threw an exception.
+ Added a built-in e-mail logging provider. 
+ Added a couple of interfaces to data access to support mapping entities to their keys.

###### Initial Release (v0.1.0)
This version consists of the following "facilities":

+ An application environment initializer
+ Composition and dependency injection based on MEF
+ Configuration that uses variable types of configuration stores the content of which are based on the .NET System.Configuration structure. Configuration is presented to consumers using simple dictionary type methods.
+ Asynchronous logging that can use multiple providers
+ Data access interface based on Unit-of-Work and Repository patterns
+ Simple bundling and SSO-security interfaces for web applications
+ Simple built-in providers for logging (console and text file)
+ Simple built-in providers for configuration store (XML file and web URL)
