### Commons Library
##### By [Aashish Koirala](http://aashishkoirala.github.io)

This is a general purpose commons library that I've built mostly for my own use within applications that I build (but obviously you are welcome to use it should you choose). The goal is to have an easy-to-use and uniform yet modular and pluggable facility to handle common cross-cutting concerns in my applications, thereby freeing me up to focus on the actual application concerns and hopefully making it easier and faster to build them.

In general, the library consists of interfaces or SPIs along with ways to access them (using MEF). One can then build implementations for these interfaces (i.e. providers) and hook them up to an application through configuration. What this does for me as provide a uniform way to access these services in all my applications while allowing me to switch or extend providers as I see fit.

For more detailed information and documentation, please visit the GitHub page for this repository at [aashishkoirala.github.io/commons](http://aashishkoirala.github.io/commons). You can get this as a NuGet package [here](https://www.nuget.org/packages/AK.Commons/).

This library only consists of interfaces and simple providers - the idea is to not need any extra libraries other than the framework assemblies. I have been building providers as I go. You can go through them in their own repository at [aashishkoirala.github.io/commons-providers](http://aashishkoirala.github.io/commons-providers).

###### Updates (v1.0.2)

+ Added new logging provider that uses the default trace (i.e. System.Diagnostics.Trace) for logging.

###### Updates (Web v1.0.1)

+ Changes to the `ConfigureSecureLogin` method - now accepts more parameters and is more configurable.

###### Updates (v1.0.1)

+ Made `LoginServiceHostFactory` more configurable.

###### Updates (Web v1.0.0)
This is the first major release of the dedicated Commons Web Library, which is a separate package that includes the following features:

+ Constructs to support web environment initialization, as well as various utilities.
+ Packaged libraries (such as Angular, Bootstrap, SignalR, etc.) in embedded form and a way to serve them (so that you don't have to keep including the scripts in every application).
+ Static content minification components.
+ REST-based resource mapping layer for Web API that lets you route resource requests directly to services without having to write controllers.
+ Security components targeted specifically at single page applications (SPAs) that use WIF.

###### Updates (v1.0.0)
This major release consists of the following enhancements/changes:

+ Added new Aspects namespace that includes components to support aspect-oriented programming.
+ Breaking changes to the DataAccess namespace: a unit of work now spawns typed, scoped repositories that expose actual data access methods. This aligns more closely with my understanding of the unit-of-work/repository patterns.
+ Added new DomainDriven namespace that includes components to support domain-driven design (DDD).
+ Security components related to certificates and WIF.
+ Bunch of service-related components having to do with WCF as well as an OperationResult group of classes that represent values and status of results of service operations.
+ Added new Threading namespace for concurrency components- presently LockedObject and LockedValue classes that encapsulate reader/writer locking.
+ Added various constructs such as an EnumDescription attribute for enum mebmers, a Perhaps class to represent things that may not be there, an IProviderSource interface to standardize named provider interfaces, along with new Uri handling methods.
+ Removed Web components because: 1) all web-based components will be included in a new Commons Web Library, 2) I am deprecating bundling and OAuth-type security constructs that were originally defined in the Web namespace.

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
