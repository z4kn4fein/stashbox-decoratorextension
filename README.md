##stashbox-decoratorextension [![Build status](https://ci.appveyor.com/api/projects/status/smb4da1ftxc18ddo/branch/master?svg=true)](https://ci.appveyor.com/project/pcsajtai/stashbox-extensions/branch/master) [![Coverage Status](https://coveralls.io/repos/z4kn4fein/stashbox-extensions/badge.svg?branch=master&service=github)](https://coveralls.io/github/z4kn4fein/stashbox-extensions?branch=master) [![Join the chat at https://gitter.im/z4kn4fein/stashbox-extensions](https://img.shields.io/badge/gitter-join%20chat-green.svg)](https://gitter.im/z4kn4fein/stashbox-extensions?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) [![NuGet Version](http://img.shields.io/nuget/v/Stashbox.Extensions.svg?style=flat)](https://www.nuget.org/packages/Stashbox.Extensions/) [![NuGet Downloads](http://img.shields.io/nuget/dt/Stashbox.Extensions.svg?style=flat)](https://www.nuget.org/packages/Stashbox.Extensions/)
This extension supposed to allow decorator registration for services registered in the [Stashbox](https://github.com/z4kn4fein/stashbox) container.

**Supported platforms**:

 - .NET 4.5 and above
 - Windows 8/8.1/10
 - Windows Phone Silverlight 8/8.1
 - Windows Phone 8.1
 - Xamarin (Android/iOS/iOS Classic)
 
###Usage
Register the decorator extension into the container.
```c#
var container = new StashboxContainer();
container.RegisterExtension(new DecoratorExtension());
```
Then you can register your decorators globally, which means the container will decorate every related service at resolution time, or you can specify which services you want to decorate one by one.

#####Global registration
```c#
container.RegisterDecorator<IFoo, FooDecorator>();
```
> With this type of decorator registration the container will decorate all *IFoo* implementations with the *FooDecorator*.

If you specify more than one decorators, the resolved services will be decorated in the exact same order as the decorators were registered into the container. 
```c#
container.RegisterDecorator<IFoo, FooDecorator1>();
container.RegisterDecorator<IFoo, FooDecorator2>();
```

#####Explicit registration
You can register your decorators directly to a service registration with the fluent form.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator>().Register();
```
> With this type of decorator registration the container will decorate just the *Foo* implementation of the *IFoo* interface.

Just like with the global registration, you can register more than one decorators at the same time.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator1>().DecorateWith<FooDecorator2>().Register();
```

#####Mixed decoration
You can use both ways of decorator registration at the same time.
```c#
container.PrepareType<IFoo, Foo>().DecorateWith<FooDecorator1>().DecorateWith<FooDecorator2>().Register();
container.RegisterDecorator<IFoo, FooDecorator3>();
```
> The decoration order depending on the registration order of the decorators, so the order in this case is: FooDecorator1 -> FooDecorator2 -> FooDecorator3.