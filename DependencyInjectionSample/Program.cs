// See https://aka.ms/new-console-template for more information

using Microsoft.Extensions.DependencyInjection;

IServiceCollection serviceCollection = new ServiceCollection();
serviceCollection.AddTransient<IFoo, Foo>();
serviceCollection.AddSingleton<IBar, Bar>();
serviceCollection.AddScoped<IDog, Dog>();


var buildServiceProvider = serviceCollection.BuildServiceProvider();
var foo = buildServiceProvider.GetService(typeof(IFoo));
var foo1 = buildServiceProvider.GetService(typeof(IFoo));
((IFoo)foo).Shout();
((IFoo)foo1).Shout();
Console.WriteLine(foo == foo1);

var bar = buildServiceProvider.GetService(typeof(IBar));
var bar1 = buildServiceProvider.GetService(typeof(IBar));
((IBar)bar).Say();
((IBar)bar1).Say();
Console.WriteLine(bar == bar1);

using (var scope = buildServiceProvider.CreateScope())
{
    var bar2 = scope.ServiceProvider.GetService(typeof(IBar));
    Console.WriteLine(bar1 == bar2);
}

var buildServiceProvider1 = serviceCollection.BuildServiceProvider();
var bar3 = buildServiceProvider1.GetService(typeof(IBar)); 
var bar4 = buildServiceProvider1.GetService(typeof(IBar));
Console.WriteLine(bar1 == bar3);
Console.WriteLine(bar3 == bar4);



var dog = buildServiceProvider.GetService(typeof(IDog));
var dog1 = buildServiceProvider.GetService(typeof(IDog));
((IDog)dog).Eat();
((IDog)dog1).Eat(); 
Console.WriteLine(dog == dog1);
using (var serviceScope= buildServiceProvider.CreateScope())
{
    var dog2 = serviceScope.ServiceProvider.GetService(typeof(IDog));
    var dog3 = serviceScope.ServiceProvider.GetService(typeof(IDog));
    Console.WriteLine(dog == dog2);
    Console.WriteLine(dog2 == dog3);
}
buildServiceProvider.Dispose();


class BaseClass : IDisposable
{
    public void Dispose()
    {
        Console.WriteLine($"{GetType().Name} Disposed");
    }
}


interface IFoo
{
    public void Shout();
}

class Foo : BaseClass,IFoo
{
    public void Shout()
    {
        Console.WriteLine("Oh My God!!!");
    }
}

interface IBar
{
    public void Say();
}

class Bar : BaseClass, IBar
{

    public void Say()
    {
        Console.WriteLine("Oh My LadyGaGa");
    }
}

interface IDog
{
    public void Eat();
}

class Dog : BaseClass, IDog
{
    public void Eat()
    {
        Console.WriteLine("好吃的不得了");
    }
}

