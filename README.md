FancyMapper
===========

Data annotation based object mapper for C#

WARNING: This code is extremely experimental. I am in no way liable for any damage it may cause.

FancyMapper maps scalars in a classy, sophisticated way. Instead of doing something like 

objA.a = objB.a
objA.b = objB.b
obcA.c = objB.c
...
repeat until bored

This relationship may be repeated in many places in the application.

FancyMapper lets you define these relationships as data annotations which are used to map these properties with a single
method call. This means that these relationships are defined in one place: the relevant model class you use.

Anything can be a model class. There are no special interfaces, any C# object you have sources for will due.

And then all you have to do is use Reflect() or Mirror() to transfer the values on any instance of that model class into or out of any object you've defined a route for. Isn't that great?

So instead of doing mapping line by line, you can define reusable routes with Data Annotations like this:

(in a model class SimpleModel)
[Mirror("SimpleObject.TestString")]
public string PoorlyNamedString { get; set; }

And then do this

FancyUtil.mirror(object, model) //transfer routed properties from object -> model
FancyUtil.reflect(model, object) //transfer routed properties from model -> object

True to its name, FancyMapper is Fancy. It will do stuff like automatically instantiate null target properties in order to
write to their children, and it supports recursive routing. It can also substitute null values as long as those values are
known at compile time (read: constants).

You can define pretty deep relationships thanks to recursion and automatic property initialization on reflect(). The map
used is simple too, a route is just

Class Name.Property1.Property2....LastProperty

FancyMapper detects what route to use based on the class of the object you're feeding it. You can put as may routes on
a property as you want, as long as they don't compete with each other. So feel free to write stuff like this.

[Mirror("SimpleObject.TestString")]
[Mirror("OverlyComplexObject.NestedComplexObject.NestedSimpleObject.TestString")]
public string ReallyWantThisString { get; set; }

Still don't get it? You can read the unit tests. I think this is pretty cool.

But it's also brand new, incomplete, and written by someone with a strong Java background. You have been warned!

As of October 2014, FancyMapper now provides more support for Entity framework. If you are working with proxied EF
classes, you can use the new proxy parameter to indicate you are comparing a proxied EF type like this:

FancyUtil.mirror(object, model, proxy: true)

Which will allow proper mapping onto EF classes that represent objects you retrieved from your database.
