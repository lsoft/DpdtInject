# DpDtInject

# Purpose

Dpdt is a DI container based on C# Source Generators. Its goal is to remove everything possible from runtime and make resolving process as faster as we can. This is achieved by transferring huge piece of resolving logic to the compilation stage into the source generator.

# Status

It's only a proof-of-concept. Nor alpha, neither beta.

# Features

0. Easy-to-read syntax `Bind<IA>().To<A>().WithTransientScope()`.
0. Generic `Get<T>` and non generic `Get(Type t)` resolution.
0. Single object `Get` or collection `GetAll` resolution.
0. `GetFunc` resolution (there `Func` objects are always singleton).
0. Custom constructor arguments `... Configure(new ConstructorArgument("message", Message))`.
0. Transient, singleton and constant (in progress) scopes.
0. [Additional compile-time safety](https://github.com/lsoft/DpdtInject/wiki#compile-time-safety)
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
|        Dryloc |     GenericSingleton |  96.19 ns | 0.913 ns | 0.854 ns |  96.31 ns |      - |     - |     - |         - |
|        Dryloc |     GenericTransient | 158.86 ns | 1.859 ns | 1.648 ns | 159.17 ns | 0.0186 |     - |     - |     144 B |
|        Dryloc |  NonGenericSingleton |  57.08 ns | 0.835 ns | 0.697 ns |  57.02 ns |      - |     - |     - |         - |
|        Dryloc |  NonGenericTransient | 116.67 ns | 1.981 ns | 1.654 ns | 116.48 ns | 0.0187 |     - |     - |     144 B |
| Microresolver |     GenericSingleton |  65.25 ns | 1.187 ns | 1.110 ns |  65.71 ns |      - |     - |     - |         - |
| Microresolver |     GenericTransient | 115.11 ns | 2.298 ns | 2.359 ns | 115.09 ns | 0.0186 |     - |     - |     144 B |
| Microresolver |  NonGenericSingleton |  30.19 ns | 0.407 ns | 0.381 ns |  30.22 ns |      - |     - |     - |         - |
| Microresolver |  NonGenericTransient |  93.30 ns | 1.281 ns | 1.198 ns |  93.43 ns | 0.0188 |     - |     - |     144 B |
