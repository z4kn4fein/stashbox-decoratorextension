#stashbox-decoratorextension
[![Build status](https://ci.appveyor.com/api/projects/status/jkc3mbxaapufaobi/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-decoratorextension/branch/master) [![Coverage Status](https://coveralls.io/repos/github/z4kn4fein/stashbox-decoratorextension/badge.svg?branch=master)](https://coveralls.io/github/z4kn4fein/stashbox-decoratorextension?branch=master) [![Join the chat at https://gitter.im/z4kn4fein/stashbox-decoratorextension](https://img.shields.io/badge/gitter-join%20chat-green.svg)](https://gitter.im/z4kn4fein/stashbox-decoratorextension?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet Version](https://buildstats.info/nuget/Stashbox.DecoratorExtension)](https://www.nuget.org/packages/Stashbox.DecoratorExtension/)

This extension allows the usage of the decorator pattern on the services registered in the [Stashbox](https://github.com/z4kn4fein/stashbox) container.

##Supported platforms

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 
##Usage
Registering the decorator extension into the container.
```c#
var container = new StashboxContainer();
container.RegisterExtension(new DecoratorExtension());
```
Then you can register your decorator services globally, which means the container will decorate every related services at resolution time, or you can specify which services you want to decorate one by one.

###Global registration
```c#
container.RegisterDecorator<IFoo, FooDecorator>();
```
> With this type of registration, the container will decorate all `IFoo` implementations with the `FooDecorator`.

If you specify more than one decorators, the resolved services will be decorated in the exact same order as the decorators were registered into the container. 
```c#
container.RegisterDecorator<IFoo, FooDecorator1>();
container.RegisterDecorator<IFoo, FooDecorator2>();
```

###Explicit registration
You can bind decorators directly to a service registration.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator>().Register();
```
> In this way the container will decorate just the `Foo` implementation of the `IFoo` interface.

Just like with the global registration, you can register more than one decorators at the same time.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator1>().DecorateWith<FooDecorator2>().Register();
```

###Mixed registration
You can mix both ways of the registrations.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator1>().DecorateWith<FooDecorator2>().Register();
container.RegisterDecorator<IFoo, FooDecorator3>();
```
> The decoration order is depending on the order of the decorator registrations, so the order in this case will be the following: FooDecorator1 -> FooDecorator2 -> FooDecorator3.
