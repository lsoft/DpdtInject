# DpDtInject

![Dpdt logo](logo.png)

# Purpose

Dpdt is a DI container based on C# Source Generators. Its goal is to remove everything possible from runtime and make resolving process as faster as we can. This is achieved by transferring huge piece of resolving logic to the compilation stage into the source generator.

# Status

It's only a proof-of-concept. Nor alpha, neither beta.

# Features

0. Easy-to-read syntax `Bind<IA>().To<A>().WithTransientScope()`.
0. Generic `Get<T>` and non generic `Get(Type t)` resolution.
0. Single object `Get` or collection `GetAll` resolution.
0. `Func<T>` resolutions.
0. Custom constructor arguments `... Configure(new ConstructorArgument("message", Message))`.
0. Transient, singleton and constant scopes.
0. Child kernels (aka child clusters).
0. Custom scopes.
0. Additional compile-time safety
0. At last, it's very, very fast.

More to come!

# Performance

0. Very impressive Generic resolution performance.
0. Not best Non Generic resolution - Microresolver is fantastically fast; what's the magic? :)

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-preview.8.20417.9
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  Job-NLMRRB : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT

Runtime=.NET Core 3.1  Server=True  

```
|          Type |               Method |      Mean |    Error |   StdDev |    Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |--------------------- |----------:|---------:|---------:|----------:|-------:|------:|------:|----------:|
|          Dpdt |     GenericSingleton |  18.88 ns | 0.164 ns | 0.137 ns |  18.90 ns |      - |     - |     - |         - |
|          Dpdt |     GenericTransient |  66.61 ns | 1.347 ns | 1.888 ns |  66.42 ns | 0.0187 |     - |     - |     144 B |
|          Dpdt |  NonGenericSingleton |  56.20 ns | 1.949 ns | 5.717 ns |  54.29 ns |      - |     - |     - |         - |
|          Dpdt |  NonGenericTransient |  99.76 ns | 1.777 ns | 1.575 ns |  99.92 ns | 0.0187 |     - |     - |     144 B |
|        DryIoc |     GenericSingleton |  96.19 ns | 0.913 ns | 0.854 ns |  96.31 ns |      - |     - |     - |         - |
|        DryIoc |     GenericTransient | 158.86 ns | 1.859 ns | 1.648 ns | 159.17 ns | 0.0186 |     - |     - |     144 B |
|        DryIoc |  NonGenericSingleton |  57.08 ns | 0.835 ns | 0.697 ns |  57.02 ns |      - |     - |     - |         - |
|        DryIoc |  NonGenericTransient | 116.67 ns | 1.981 ns | 1.654 ns | 116.48 ns | 0.0187 |     - |     - |     144 B |
| Microresolver |     GenericSingleton |  65.25 ns | 1.187 ns | 1.110 ns |  65.71 ns |      - |     - |     - |         - |
| Microresolver |     GenericTransient | 115.11 ns | 2.298 ns | 2.359 ns | 115.09 ns | 0.0186 |     - |     - |     144 B |
| Microresolver |  NonGenericSingleton |  30.19 ns | 0.407 ns | 0.381 ns |  30.22 ns |      - |     - |     - |         - |
| Microresolver |  NonGenericTransient |  93.30 ns | 1.281 ns | 1.198 ns |  93.43 ns | 0.0188 |     - |     - |     144 B |



# Design

## Debugging your modules and conditional clauses

Because of source generators are generating new code based on your code, it's impossible to direclty debug your Module code, including its `When` predicates (because this code is not actually executed at runtime). It's a disadvantage of Dpdt design. For conditional clauses, you need to call another class to obtain an ability to catch a breakpoint:

```
    public partial class MyModule : DpdtModule
    {
        public override void Load()
        {
            Bind<IA, IA2>()
                .To<A>()
                .WithSingletonScope()
                .When(cc =>
                {
                    //here debugger is NOT working

                    return Debugger.Debug(cc);
                })
                ;
        }
    }

    public static class Debugger
    {
        public static bool Debug(IResolutionContext rc)
        {
            //here debugger is working
            return true;
        }
    }
```

## Syntax

Examples of allowed syntaxes are available in the test project. Please refer that code.

## Choosing constructor

Constructor is chosen at the compilation stage based on 2 principles:

1. Constructors are filtered by `ConstructorArgument` filter. If no `ConstructorArgument` has defined, all existing constructors will be taken.
2. The constructor with the minimum number of parameters is selected to make binding.

## Scope

Bind clause with no defined scope raises a question: an author did forgot set a scope or wanted a default scope? We make a decision not to have a default scope and force a user to define a scope.

### Singleton

The only one instance of defined type is created. If instance is `IDisposable` then `Dispose` method will be invoked at the moment the module are disposing.

### Transient

Each resolution call results with new instance. `Dispose` for targets will not be invoked.

### Constant

Constant scope is a scope when the module receive an outside-created object. Its `Dispose` will not be invoked, because the module was not a parent of the constant object.

## Conditional binding

Each bind clause may have an additional filter e.g.

```csharp
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .When(IResolutionContext rc =>
                {
                     condition to resolve
                })
                ;
```

Please refer unit tests to see the examples. Please note, than any filter makes a resolution process slower (a much slower! 10x slower in compare of unconditional binding!), so use this feature responsibly. Resolution slowdown with conditional bindings has an effect even on those bindings that do not have conditions, but they directly or indirectly takes conditional binding as its dependency. Therefore, it is advisable to place conditions as close to the dependency tree root as possible.

## Compile-time safety

### Did source generators are finished their job?

Dpdt adds a warning to compilation log with the information about how many modules being processed. It's an useful bit of information for debugging purposes.

### Unknown constructor argument

Dpdt will break ongoing compilation if binding has useless `ConstructorArgument` clause (no constructor with this parameter exists).

### Singleton takes transient

Dpdt can detect cases of singleton takes a transient as its dependency, and make signals to the programmer. It's not always a bug, but warning might be useful.

### Circular dependencies

Dpdt is available to determine circular dependencies in your dependency tree. In that cases it raise a compilation error. One additional point: if that circle contains a conditional binding, Dpdt can't determine if circular dependency will exists at runtime, so Dpdt raises a compile-time warning instead of error.

## Module life cycle

The life cycle of a module begins by creating it with `new`. The module instance should only be 1 for the entire domain, since the Dpdt logic relies heavily on the generated `private static readonly` fields. Attempting to create a second instance of the module will throw an error.

After that, the module is available for dependency resolutions.

The end of the life cycle of a module occurs after the call to its `Dispose` method. At this point, all of its disposable singleton bindings are also being disposed. It is prohibited to dispose of the module and use it for resolving in parallel . It is forbidden to resolve after a `Dispose`.

## Design drawbacks

0. Because of design, it's impossible to `Unbind`.
0. Because of source generators, it's impossible to direclty debug your bind code, including its `When` predicates.
0. Because of massive rewriting of `DpdtModule.Load` method, it's impossible to use a local variables (local methods and other local stuff) in `ConstructorArgument` and `When` predicates. To make bind works use instance based fields, properties and methods instead. To make bind debuggable use fields, properties and methods of the other, helper class.
0. No deferred bindings by design.

## Beautifier

To keep Dpdt core as fast as we can, we choose move all facultative code from the core to the beautifier. For example the following things were moved:

0. `IEnumerable<object>` in `GetAll(Type requestedType)` method; Beautifier can return `List` and its main interfaces.
0. `Get<NotBindedType>()` results in unclear `InvalidCastException`; Beautifier catch it and rethrow a appropriate `NoBindingAvailable` signal.

We recommend use beautifier for resolutions unless you need ABSOLUTELY EVERY CPU tact.

## Artifact folder

Dpdt's source generator is able to store pregenerated C# code at the disk. The only thing you need is correctly setup your csproj. For example:

```
  <PropertyGroup>
    <Dpdt_Generator_GeneratedSourceFolder>c:\temp\Dpdt.Pregenerated</Dpdt_Generator_GeneratedSourceFolder>
  </PropertyGroup>

  <ItemGroup>
    <CompilerVisibleProperty Include="Dpdt_Generator_GeneratedSourceFolder" />
  </ItemGroup>

  <ItemGroup>
    <Compile Remove="Dpdt.Pregenerated\**" />
    <EmbeddedResource Remove="Dpdt.Pregenerated\**" />
    <None Remove="Dpdt.Pregenerated\**" />
  </ItemGroup>

```

`Dpdt_Generator_GeneratedSourceFolder` is a builtin variable name; `c:\temp\Dpdt.Pregenerated` is an **absolute** folder name for Dpdt artifacts and allowed to be changed.

## Child clusters

```
        Bind<IA>()
            .To<A>()
            .WithSingletonScope()
            .InCluster<DefaultCluster>()
            ;

        Bind<IB>()
            .To<B>()
            .WithSingletonScope()
            .InCluster<ChildCluster>()
           ;

...

        public partial class DefaultCluster
        {
        }

        public partial class ChildCluster : DefaultCluster
        {
        }
```

Clusters are organized into a tree. This tree cannot have a circular dependency, since it is based on class inheritance. Each cluster must be a partial class declared inside a nested class. Dependencies consumed by the binding in the child cluster are resolved from the native cluster and **all parent clusters**. This behavior can be changed in the future if practice shows the need for it.
Like with a scope, we make a decision to force a user to define a cluster.

## Custom scopes

```csharp
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                .InCluster<DefaultCluster>()
                ;

...

            using(var scope1 = module.CreateCustomScope())
            {
                var a1 = module.Get<IA>(scope1);

                using(var scope2 = module.CreateCustomScope())
                {
                    var a2 = module.Get<IA>(scope2);
                }
            }
```