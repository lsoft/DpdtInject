# DpDtInject

![Dpdt logo](logo.png)

# Purpose

Dpdt is a DI container based on C# Source Generators. Its goal is to remove everything possible from runtime and make resolving process as faster as we can. This is achieved by transferring huge piece of resolving logic to the compilation stage into the source generator.

# Status

It's only a proof-of-concept. Nor alpha, neither beta.

# Features

0. Easy-to-read syntax `Bind<IA>().To<A>().WithTransientScope()`.
0. Custom constructor arguments `... Configure(new ConstructorArgument("message", Message))`.
0. Generic `Get<T>` and non generic `Get(Type t)` resolutions.
0. Constained `GetFast` fast resolutions.
0. Single object `Get` or collection `GetAll` resolutions.
0. `Func<T>` resolutions.
0. Transient, singleton and constant scopes.
0. Custom scopes.
0. Child kernels (aka child clusters).
0. Additional compile-time safety.
0. Same performance on the platforms with no compilation at runtine.
0. At last, it's very, very fast.

More to come!

# Performance

0. Very impressive Fast resolutions.
0. Good Generic resolution performance.
0. Not best Non Generic resolution - Microresolver is fantastically fast; what's the magic? :)

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-rc.1.20452.10
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  Job-DOJUWL : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT

Runtime=.NET Core 3.1  Server=True  

|                                         Namespace |          Type |              Method |       Mean |     Error |    StdDev |     Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------------------------------------------- |-------------- |-------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|       DpdtInject.Tests.Performance.Fast.Singleton |          Dpdt |       FastSingleton |   9.390 ns | 0.1064 ns | 0.0889 ns |   9.398 ns |      - |     - |     - |         - |
|       DpdtInject.Tests.Performance.Fast.Transient |          Dpdt |       FastTransient |  47.154 ns | 0.4834 ns | 0.4522 ns |  47.110 ns | 0.0156 |     - |     - |     120 B |
|    DpdtInject.Tests.Performance.Generic.Singleton |          Dpdt |    GenericSingleton |  15.656 ns | 0.2481 ns | 0.2199 ns |  15.701 ns |      - |     - |     - |         - |
|    DpdtInject.Tests.Performance.Generic.Transient |          Dpdt |    GenericTransient |  61.170 ns | 1.1193 ns | 0.8739 ns |  61.359 ns | 0.0187 |     - |     - |     144 B |
| DpdtInject.Tests.Performance.NonGeneric.Singleton |          Dpdt | NonGenericSingleton |  47.878 ns | 0.6966 ns | 0.6516 ns |  47.760 ns |      - |     - |     - |         - |
| DpdtInject.Tests.Performance.NonGeneric.Transient |          Dpdt | NonGenericTransient |  96.832 ns | 1.3139 ns | 1.0971 ns |  96.587 ns | 0.0187 |     - |     - |     144 B |
|-------------------------------------------------- |-------------- |-------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|    DpdtInject.Tests.Performance.Generic.Singleton |        DryIoc |    GenericSingleton |  96.620 ns | 1.0785 ns | 1.0088 ns |  96.710 ns |      - |     - |     - |         - |
|    DpdtInject.Tests.Performance.Generic.Transient |        DryIoc |    GenericTransient | 140.565 ns | 2.8101 ns | 2.7598 ns | 141.088 ns | 0.0186 |     - |     - |     144 B |
| DpdtInject.Tests.Performance.NonGeneric.Singleton |        DryIoc | NonGenericSingleton |  56.725 ns | 0.6225 ns | 0.5198 ns |  56.826 ns |      - |     - |     - |         - |
| DpdtInject.Tests.Performance.NonGeneric.Transient |        DryIoc | NonGenericTransient | 104.145 ns | 2.1310 ns | 2.0929 ns | 103.582 ns | 0.0188 |     - |     - |     144 B |
|-------------------------------------------------- |-------------- |-------------------- |-----------:|----------:|----------:|-----------:|-------:|------:|------:|----------:|
|    DpdtInject.Tests.Performance.Generic.Singleton | Microresolver |    GenericSingleton |  59.431 ns | 1.0300 ns | 0.9634 ns |  59.484 ns |      - |     - |     - |         - |
|    DpdtInject.Tests.Performance.Generic.Transient | Microresolver |    GenericTransient | 119.079 ns | 2.3919 ns | 2.9375 ns | 118.693 ns | 0.0186 |     - |     - |     144 B |
| DpdtInject.Tests.Performance.NonGeneric.Singleton | Microresolver | NonGenericSingleton |  31.869 ns | 1.1528 ns | 3.3991 ns |  30.088 ns |      - |     - |     - |         - |
| DpdtInject.Tests.Performance.NonGeneric.Transient | Microresolver | NonGenericTransient |  81.541 ns | 1.7322 ns | 3.4991 ns |  80.248 ns | 0.0188 |     - |     - |     144 B |
```

Few more numbers for complex tree total of 1000 bindings (make note NonGeneric now is faster than Generic for DryIoc and Microresolver):

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-rc.1.20452.10
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  Job-ACPELA : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT

Runtime=.NET Core 3.1  Server=True  

|          Type |                  Method |      Mean |    Error |   StdDev | Gen 0 | Gen 1 | Gen 2 | Allocated |
|---------------|------------------------ |----------:|---------:|---------:|------:|------:|------:|----------:|
|          Dpdt |    GenericSingleton1000 |  51.83 us | 0.441 us | 0.412 us |     - |     - |     - |         - |
|          Dpdt | NonGenericSingleton1000 | 100.29 us | 0.747 us | 0.662 us |     - |     - |     - |         - |
|          Dpdt |       FastSingleton1000 |  22.71 us | 0.238 us | 0.211 us |     - |     - |     - |         - |
|---------------|------------------------ |----------:|---------:|---------:|------:|------:|------:|----------:|
|        DryIoc |    GenericSingleton1000 | 136.04 us | 2.497 us | 2.336 us |     - |     - |     - |         - |
|        DryIoc | NonGenericSingleton1000 |  93.73 us | 1.393 us | 1.235 us |     - |     - |     - |         - |
|---------------|------------------------ |----------:|---------:|---------:|------:|------:|------:|----------:|
| Microresolver |    GenericSingleton1000 |  75.04 us | 1.461 us | 2.095 us |     - |     - |     - |         - |
| Microresolver | NonGenericSingleton1000 |  29.61 us | 0.411 us | 0.384 us |     - |     - |     - |         - |

```


Also I recommend disable tiered compilation for composition root assembly if you want to obtain full performance at the start.


# How to try

Please refer to Dpdt.Injector nuget package at nuget.org. Keep in mind you need to set 'net5' target framework and 'preview' language version. For example:

```
<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net5</TargetFramework>
    <LangVersion>preview</LangVersion>

    <!-- disable tiered compilation for composition root assembly -->
    <TieredCompilation>false</TieredCompilation>
    <TieredCompilationQuickJit>false</TieredCompilationQuickJit>
    <TieredCompilationQuickJitForLoops>false</TieredCompilationQuickJitForLoops>

    <Dpdt_Generator_GeneratedSourceFolder>C:\Temp\ConsoleApp1\ConsoleApp1\Dpdt.Pregenerated</Dpdt_Generator_GeneratedSourceFolder>
  </PropertyGroup>

  <ItemGroup>
    <CompilerVisibleProperty Include="Dpdt_Generator_GeneratedSourceFolder" />
  </ItemGroup>

  <ItemGroup>
    <Compile Remove="Dpdt.Pregenerated\**" />
    <EmbeddedResource Remove="Dpdt.Pregenerated\**" />
    <None Remove="Dpdt.Pregenerated\**" />
  </ItemGroup>

  <ItemGroup>
    <PackageReference Include="Dpdt.Injector" Version="0.2.0-alpha" />
  </ItemGroup>

</Project>
```


# Design

## Design drawbacks

0. Because of design, it's impossible to `Unbind`.
0. Because of source generators, it's impossible to direclty debug your bind code, including its `When` predicates.
0. Because of massive rewriting the body of the cluster, it's impossible to use a local variables (local methods and other local stuff) in `ConstructorArgument` and `When` predicates. To make bind works use instance based fields, properties and methods instead. To make bind debuggable use fields, properties and methods of the other, helper class.
0. No deferred bindings by design with exception of cluster hierarchy.
0. Because of performance reasons if binding does not exists, Dpdt throw an invalid cast exception, but no DpdtException. I'm trying to fix that without performance lost.
0. Slower source-to-IL compilation, slower JIT compilation.

## Syntax

Examples of allowed syntaxes are available in the test project. Please refer that code.

## Choosing constructor

Constructor is chosen at the compilation stage based on 2 principles:

1. Constructors are filtered by `ConstructorArgument` filter. If no `ConstructorArgument` has defined, all existing constructors will be taken.
2. The constructor with the minimum number of parameters is selected to make binding.

## Scope

Bind clause with no defined scope raises a question: an author did forgot set a scope or wanted a default scope? We make a decision not to have a default scope and force a user to define a scope.

### Singleton

The only one instance of defined type is created. If instance is `IDisposable` then `Dispose` method will be invoked at the moment the cluster is disposing.

### Transient

Each resolution call results with new instance. `Dispose` for targets will not be invoked.

### Constant

Constant scope is a scope when the cluster receive an outside-created object. Its `Dispose` will not be invoked, because the cluster was not a parent of the constant object.

## Conditional binding

Each bind clause may have an additional filter e.g.

```csharp
            Bind<IA>()
                .To<A>()
                .WithSingletonScope()
                .When(IResolutionTarget rt =>
                {
                     condition to resolve
                })
                ;
```

Please refer unit tests to see the examples. Please note, than any filter makes a resolution process slower (a much slower! 10x slower in compare of unconditional binding!), so use this feature responsibly. Resolution slowdown with conditional bindings has an effect even on those bindings that do not have conditions, but they directly or indirectly takes conditional binding as its dependency. Therefore, it is advisable to place conditions as close to the dependency tree root as possible.

## Fast resolutions

Dpdt contains a special resolution type named 'fast'. Its syntax is `cluster.GetFast(default(IMyInterface));`. In general this syntax is faster that generic resolutions, but it has one additional constraint: you need to resolve directly from cluster type, it is impossible to cast cluster to the one of its interface (like `ICluster` or `IResolution`) and do fast resolutions.

## Compile-time safety

Each safety checks are processed in the scope of concrete cluster. Dpdt cannot check for cross-cluster issues because clusters tree is built at runtime.

### Did source generators are finished their job?

Dpdt adds a warning to compilation log with the information about how many clusters being processed. It's an useful bit of information for debugging purposes.

### Unknown constructor argument

Dpdt will break ongoing compilation if binding has useless `ConstructorArgument` clause (no constructor with this parameter exists).

### Singleton takes transient or custom

Dpdt can detect cases of singleton binding takes a transient/custom binding as its dependency, and make signals to the programmer. It's not always a bug, but warning might be useful.

### Circular dependencies

Dpdt is available to determine circular dependencies in your dependency tree. In that cases it raise a compilation error. One additional point: if that circle contains a conditional binding, Dpdt can't determine if circular dependency will exists at runtime, so Dpdt raises a compile-time warning instead of error.

### More than 1 unconditional child

If for some binding more than 1 unconditional child exists it renders parent unresolvable, so Dpdt will break the compilation is that case.

## Cluster life cycle

The life cycle of the cluster begins by creating it with `new`. The cluster can take other cluster as its parent, so each unknown dependency will be resolved from the parent (if parent exists, otherwise exception would be thrown).

The end of the life cycle of a cluster occurs after the call to its `Dispose` method. At this point, all of its disposable singleton bindings are also being disposed. It is prohibited to dispose of the cluster and use it for resolving in parallel . It is forbidden to resolve after a `Dispose`.

## Custom scopes

```csharp
            Bind<IA>()
                .To<A>()
                .WithCustomScope()
                ;

...

            using(var scope1 = cluster.CreateCustomScope())
            {
                var a1 = cluster.Get<IA>(scope1);

                using(var scope2 = cluster.CreateCustomScope())
                {
                    var a2 = cluster.Get<IA>(scope2);
                }
            }
```

`IDisposable` custom-binded objects will be disposed at the moment of the scope object dispose. Keep in mind, custom-scoped bindings are resolved much slower than singleton/transient/constant bindings.

## Child clusters

```csharp
    public partial class RootCluster : DefaultCluster
    { ... }

    public partial class ChildCluster : DefaultCluster
    { ... }

...

            var rootCluster = new RootCluster(
                null
                );
            var childCluster = new ChildCluster(
                rootCluster
                );
```

Clusters are organized into a tree. This tree cannot have a circular dependency, since it is based on constructor argument. Dependencies, consumed by the binding in the child cluster, are resolved from the native cluster if exists, if not - from **parent cluster**.

## Debugging your clusters and conditional clauses

Because of source generators are generating new code based on your code, it's impossible to direclty debug your cluster code, including its `When` predicates (because this code is not actually executed at runtime). It's a disadvantage of Dpdt design. For conditional clauses, you need to call another class to obtain an ability to catch a breakpoint:

```csharp
    public partial class MyCluster : DefaultCluster
    {
        public override void Load()
        {
            Bind<IA, IA2>()
                .To<A>()
                .WithSingletonScope()
                .When(rt =>
                {
                    //here debugger is NOT working

                    return Debugger.Debug(rt);
                })
                ;
        }
    }

    public static class Debugger
    {
        public static bool Debug(IResolutionTarget rt)
        {
            //here debugger is working
            return true;
        }
    }
```

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
