### **Commons** - A general purpose library
##### By [Aashish Koirala](https://aashishkoirala.github.io)

[GitHub Repository](https://github.com/aashishkoirala/commons) | [NuGet Package](https://www.nuget.org/packages/AK.Commons/)

This is a general purpose commons library that I've built mostly for my own use within applications that I build (but obviously you are welcome to use it should you choose). The goal is to have an easy-to-use and uniform yet modular and pluggable facility to handle common cross-cutting concerns in my applications, thereby freeing me up to focus on the actual application concerns and hopefully making it easier and faster to build them.

In general, the library consists of interfaces or SPIs along with ways to access them (using MEF). One can then build implementations for these interfaces (i.e. providers) and hook them up to an application through configuration. What this does for me as provide a uniform way to access these services in all my applications while allowing me to switch or extend providers as I see fit. You can get this as a NuGet package [here](https://www.nuget.org/packages/AK.Commons/).

This library only consists of interfaces and simple providers - the idea is to not need any extra libraries other than the framework assemblies. I have been building providers as I go. You can go through them in their own repository at [aashishkoirala.github.io/commons-providers](http://aashishkoirala.github.io/commons-providers).

#### Installation
The library is available in [Nuget](https://www.nuget.org/packages/AK.Commons/) as **AK.Commons**. You can use the Package Manager Console in Visual Studio and run `Install-Package AK.Commons`.

#### Release History
- Version 1.0.3
 - Added property `DomainRepositoryFactory` to `DomainDrivenUtility` for better testability.
 - Web: Downgraded to MVC 4 due to compatibility issues.
- Version 1.0.2
 - Added new logging provider that uses the default trace (i.e. `System.Diagnostics.Trace`) for logging.
 - Web: Added exception filters for both MVC and Web API that log the exception.
- Version 1.0.1
 - Made `LoginServiceHostFactory` more configurable.
 - Web: Changes to the `ConfigureSecureLogin` method - now accepts more parameters and is more configurable.
- Version 1.0.0 (Web)
 - This is the first major release of the dedicated Commons Web Library, which is a separate package.
 - Constructs to support web environment initialization, as well as various utilities.
 - Packaged libraries (such as Angular, Bootstrap, SignalR, etc.) in embedded form and a way to serve them (so that you don't have to keep including the scripts in every application).
 - Static content minification components.
 - REST-based resource mapping layer for Web API that lets you route resource requests directly to services without having to write controllers.
 - Security components targeted specifically at single page applications (SPAs) that use WIF.
- Version 1.0.0
 - Added new Aspects namespace that includes components to support aspect-oriented programming.
 - Breaking changes to the DataAccess namespace: a unit of work now spawns typed, scoped repositories that expose actual data access methods. This aligns more closely with my understanding of the unit-of-work/repository patterns.
 - Added new DomainDriven namespace that includes components to support domain-driven design (DDD).
 - Security components related to certificates and WIF.
 - Bunch of service-related components having to do with WCF as well as an OperationResult group of classes that represent values and status of results of service operations.
 - Added new Threading namespace for concurrency components- presently LockedObject and LockedValue classes that encapsulate reader/writer locking.
 - Added various constructs such as an EnumDescription attribute for enum mebmers, a Perhaps class to represent things that may not be there, an IProviderSource interface to standardize named provider interfaces, along with new Uri handling methods.
 - Removed Web components because: 1) all web-based components will be included in a new Commons Web Library, 2) I am deprecating bundling and OAuth-type security constructs that were originally defined in the Web namespace.
- Version 0.1.2
 - Introduced a logging provider base class that combines common properties such as Enabled and the new LogLevelFilter property. This supports the new feature of log-level filtering (i.e. you can now specify whether a logging provider is invoked only for certain types of log levels).
 - Added a new log level "Diagnostic" which is the most verbose.
 - Logging now never throws due to a silent catch that was necessary to keep from the logging thread crashing if a provider threw an exception.
 - Added a built-in e-mail logging provider.
 - Added a couple of interfaces to data access to support mapping entities to their keys.
- Version 0.1
 - An application environment initializer
 - Composition and dependency injection based on MEF
 - Configuration that uses variable types of configuration stores the content of which are based on the .NET System.Configuration structure. Configuration is presented to consumers using simple dictionary type methods.
 - Asynchronous logging that can use multiple providers
 - Data access interface based on Unit-of-Work and Repository patterns
 - Simple bundling and SSO-security interfaces for web applications
 - Simple built-in providers for logging (console and text file)
 - Simple built-in providers for configuration store (XML file and web URL)

#### Initialization
Initialization is handled by the `AK.Commons.AppEnvironment` class. You initialize the application environment by calling `AppEnvironment.Initialize()` in your application startup routine. This is the Main method for Windows applications and `Application_Start` for web applications. The call requires a unique name for the application that is used, among other things, to retrieve application-specific settings from the configuration store (see Configuration), and also to include in log entries.

The second parameter is an instance of `InitializationOptions` that is optional, but you're almost always better off providing it. It lets you specify whether logging is enabled (if you don't enable logging and use components that expect a logger, they will fail). It also lets you specify what Configuration Store you want to use for the application (see Configuration).

The following example initializes an application using configuration store settings from a mapped EXE config file.

 ```csharp
 var config = ConfigurationManager.OpenMappedExeConfiguration(...);

 AppEnvironment.Initialize("MyApplication",
 	new InitializationOptions
 	{
 		EnableLogging = true
 		ConfigStore = config.GetConfigStore(),
 	});
```

#### IoC/DI/Composition
The library uses MEF for IoC/DI and composition. You can build composable modules using regular MEF exports and imports. You can access the composition interface using `AppEnvironment.Composer`. If you're inside a composable class, you can also just put a MEF import for the `AK.Commons.Composition.IComposer` interface.

It consists of a number of wrapper methods around MEF's own resolve-type methods. The scope of DLLs that are loaded in by the composer are: the entry assembly, the executing assembly, and assemblies in the location defined by the configuration key `ak.commons.composition.modulesdirectories`. For more on configuration keys, see Configuration.

The composition container is built by the call to `AppEnvironment.Initialize()``.

An example of the simplest variant of dependency resolution is:

```csharp
var myInstance = AppEnvironment.Composer.Resolve<MyInterface>();
```

Or, if you're inside a composable class:

```csharp
public class MyClass
{
	[Import] private IComposer composer;

	public void MyMethod()
	{
		var myInstance = this.composer.Resolve<MyInterface>();
	}
}
```

For web applications, you can use `AK.Commons.Web.Composition.ComposableDependencyResolver` and `AK.Commons.Web.Composition.ComposableControllerFactory` to create composition-enabled MVC and Web API controllers.

#### Configuration

**Configuration Store**
A configuration store is a provider of configuration data. The library expects configuration data in .NET's configuration XML format (see more on the format below). You can implement the `AK.Commons.Configuration.IConfigStore` interface to implement your own configuration store. There are two built-in ones, `XmlFileConfigStore` and `WebUrlConfigStore` that use a local XML file or a web URL that serves up the configuration XML. The configuration store itself can be configured through a bootstrap configuration file. For example, the following `App.config` file tells the application to use the given XML file as the configuration store.

```xml
<configuration>
  <configSections>
    <section name="ak.commons.configuration.store"
     type="AK.Commons.Configuration.Sections.StoreConfigurationSection, AK.Commons" />    
  </configSections>
  <ak.commons.configuration.store>
    <store type="AK.Commons.Providers.Configuration.XmlFileConfigStore, AK.Commons">
      <properties>
        <property name="FilePath" value="~/AppConfig.xml" />
      </properties>
    </store>
  </ak.commons.configuration.store>
</configuration>
```

The `ak.commons.configuration.store` section in the above example could be changed to the following to use a web URL instead:

```xml
<ak.commons.configuration.store>
  <store type="AK.Commons.Providers.Configuration.WebUrlConfigStore, AK.Commons">
    <properties>
      <property name="Url" value="http://myconfig/AppConfig.xml" />
      <property name="Authenticate" value="true" />
      <property name="UseDefaultCredentials" value="false" />
      <property name="UserName" value="myuser" />
      <property name="Password" value="mypwd" />
      <property name="Domain" value="mydomain" />
    </properties>
  </store>
</ak.commons.configuration.store>
```

**Configuration Store Format**
The configuration store follows the .NET configuration XML format with a special configuration section holding all the configuration information. The section allows for configuration settings for multiple applications to be stored, as well as on the global level. There is also the concept of tokens (more on that later). Each configuration entry is defined as a property name (i.e. configuration key), the data type and the value. You can use data structures as property values as well.

Here is an example that tries to illustrate all these features:

```xml
<configuration>
 <configSections>
  <section name="ak.commons.configuration" type="AK.Commons.Configuration.Sections.ApplicationSettingsConfigurationSection, AK.Commons"/>
 </configSections>
 <ak.commons.configuration>
  <applicationSettings>
   <application>
    <settings>
	 <setting name="setting1" type="System.String" value="globalvalue" />
	 <setting name="tokenizedSetting" type="System.String" value="Abc{LocalPath}" />
    </settings>
    <tokens />
   </application>
   <application name="Application1">
	<settings>
	 <setting name="setting1" type="System.String" value="app1value" />
	 <setting name="setting2" type="System.String" value="Microsoft" />     
	 <setting name="setting3" type="System.Boolean" value="False" />
	 <setting name="setting4" type="MyType" value="">
	  <properties>
	   <property name="Prop1" value="20" />
	   <property name="Prop2" value="Sample" />
	   </properties>
	   <constructorParameters>
	   <param name="param1" Value="abc" />
	   </constructorParameters>
	  </setting>
	</settings>
    <tokens>
	 <token name="LocalPath" value="C:\Scratch" />
	</tokens>
   </application>
   <application name="Application2">
	  ...
   </application>
  </applicationSettings>
 </ak.commons.configuration>
</configuration>
```

The global node is the application node without a name. It is required, but can have an empty settings and tokens sub-nodes. When you initialize the application using an application name (see Initialization), the global configuration is combined with the application specific configuration for that application and loaded into memory. For entries with the same configuration keys, global ones are overridden by application specific ones. You can also define tokenized configuration values as shown above and then define different token replacement values for different applications. This comes in handy where a certain setting is pretty much the same across multiple applications, with only one identifier or something of the sort that is different.
Configuration Interface

Once you've "configured the configuration store" with the call to `AppEnvironment.Initialize()` (see Initialization), you can access the configuration interface using `AppEnvironment.Config` or with a MEF imported instance of `AK.Commons.Configuration.IAppConfig` (similar to how `IComposer` and `IAppLogger` work). The interface has dictionary-type `Get` and `TryGet` methods you can use to access the setting values.

Here are a few examples (assuming we have a `IAppConfig` instance).

```csharp
var mySetting1 = config.Get<string>("MySetting1"); // throws if setting is not present.
var mySetting2 = config.Get<MyType>("MySetting2"); // throws if setting is not present
var mySetting3 = config.Get("MySetting3", "DefaultValue");
var success = config.TryGet("MySetting4", out mySetting4); // you get the idea.
```

#### Logging
You access the logging interface much like the composition or configuration interface, i.e. through `AppEnvironment.Logger` or a MEF imported instance of `AK.Commons.Logging.IAppLogger`.

All logging is asynchronous and can use multiple providers. Everytime you log something, it goes into a queue which is processed by another thread that removes items from the queue and sends it to all configured, enabled logging providers. The logging interface has methods corresponding to each verbosity level (i.e. Verbose, Information, Warning and Error - in decreasing order of verbosity). You can set the verbosity level using the configuration key `ak.commons.logging.loglevel` and setting the values to "Verbose", "Information" and so on.

Each provider may have its own settings that need to be specified.

Simple examples:

```csharp
logger.Information("This thing happened.");
logger.Error(exception);
logger.Warning("Oops!");
```

There are two built-in providers: one that logs to the console and a very simple one that logs to text files.

#### Exceptions
The library provides a base class called `AK.Commons.Exceptions.ReasonedException` that has some formatting features for logging, and lets you specify error codes as an Enum - and also lets you override a method to provide friendly descriptions for each enumerated value.

Besides this, there are some convenience extension methods for `System.Exception` that let you wrap an exception as a given `ReasonedException`, log it using `IAppLogger` and/or re-throw it.

If you import the namespace `AK.Commons.Exceptions`, you should be able to use these methods: `Wrap, WrapAndLog, WrapAndThrow, WrapLogAndThrow`, etc.

#### Data Access
For data access, there are constructs to support the Unit-of-Work and Repository patterns. You get to the data access interface using `AppEnvironment.DataAccess` or a MEF imported instance of `AK.Commons.DataAccess.IAppDataAccess`. You can use it as a dictionary to look up a unit-of-work factory by name or get the default nameless one using the Default property. Once you have one, you can use the Create method on the factory to get an instance of `IUnitOfWork` - which is an `IDisposable`. You can therefore put your data access code inside a using block and let the library and the provider handle sessions, transactions, etc.

Once you have entities, you can build repositories by implementing `IRepository` or better yet extending `RepositoryBase`. Once inside a unit-of-work, you have to assign that unit to each repository you want to take part in the transaction using the `UnitOfWork` property on the repository. Here's an example (assuming we have an `IAppDataAccess` instance):

```csharp
using (var unit = appDataAccess["myDb"].Create())
{
	myRepo1.UnitOfWork = unit;
	myRepo2.UnitOfWork = unit;

	// Perform operations on myRepo1 and myRepo2.

	unit.Commit();
}
```

This can get a bit repetitive if you have multiple repositories - so there's an extension method that lets you write the above code as:

```csharp
appDataAccess["myDb"]
	.With(myRepo1, myRepo2, myRepo3)
	.Execute(unit =>
	{
		// Do stuff.
	});
```

The `IRepository` interface has query and update type methods that lend themselves well to LINQ and `IQueryable`. The idea is to be able to abstract different types of data access providers well.

You configure data access providers as follows:

```xml
<setting name="ak.commons.dataaccess.uowfactory.factoryName.provider" type="System.String" value="providerName" />
<setting name="ak.commons.dataaccess.uowfactory.factoryName.provider_specific_setting1" ... />
<setting name="ak.commons.dataaccess.uowfactory.factoryName.provider_specific_setting2" ... />
<setting name="ak.commons.dataaccess.uowfactory.factoryName.provider_specific_setting3" ... />
Here's an example of a factory called "myDb" (as shown in the code examples above) that uses the Fluent NHibernate provider (part of the Commons Providers library).
<setting name="ak.commons.dataaccess.uowfactory.myDb.provider"
 type="System.String" value="FluentNHibernate" />
<setting name="ak.commons.dataaccess.uowfactory.myDb.connection.provider"
 type="System.String" value="NHibernate.Connection.DriverConnectionProvider" />
<setting name="ak.commons.dataaccess.uowfactory.myDb.connection.driver_class"
 type="System.String" value="NHibernate.Driver.SqlClientDriver" />
<setting name="ak.commons.dataaccess.uowfactory.myDb.connection.connection_string"
 type="System.String" value="Server=(local);Database=MyDb;Integrated Security=SSPI;" />
<setting name="ak.commons.dataaccess.uowfactory.myDb.dialect"
 type="System.String" value="NHibernate.Dialect.MsSql2005Dialect" />
```

You can build your own factory provider by implementing `IUnitOfWorkFactory` and using it to provide your own implementation of `IUnitOfWork`. You need to decorate the class with `ProviderMetadataAttribute` to give it a unique provider name (such as "FluentNHibernate" in the above example).

Each repository gets the following methods:
- Get
- GetAll
- GetFor
- GetList
- Save
- Delete

Ideally, one should be able to get by with just these methods used within units of work as required.
