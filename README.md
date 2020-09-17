# DpDtInject

# Purpose

Dpdt is a DI container based on C# Source Generators. Its goal is to remove everything possible from runtime and make resolving process as faster as we can. This is achieved by transferring huge piece of resolving logic to the compilation stage into the source generator.

# Status

It's only a proof-of-concept. Nor alpha, neither beta.

# Features

0. Easy-to-read syntax `Bind<IA>().To<A>().WithTransientScope()`.
0. Generic `Get<T>` and non generic `Get(Type t)` resolution.
0. Single object `Get` or collection `GetAll` resolution.
0. Custom constructor arguments `... Configure(new ConstructorArgument("message", Message))`.
0. Transient, singleton and constant (in progress) scopes.
0. [Additional compile-time safety](https://github.com/lsoft/DpdtInject/wiki#compile-time-safety)
0. At last, it's very, very fast.

More to come!

# Performance

+ Very impressed Generic resolution performance
- Not best Non Generic resolution (Microresolver is fantastically fast)

``` ini

BenchmarkDotNet=v0.12.0, OS=Windows 10.0.19041
Intel Core i5-4200U CPU 1.60GHz (Haswell), 1 CPU, 4 logical and 2 physical cores
.NET Core SDK=5.0.100-preview.8.20417.9
  [Host]     : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT
  Job-NRDMFW : .NET Core 3.1.7 (CoreCLR 4.700.20.36602, CoreFX 4.700.20.37001), X64 RyuJIT

Runtime=.NET Core 3.1  Server=True  

```
|          Type |               Method |      Mean |    Error |    StdDev |    Median |  Gen 0 | Gen 1 | Gen 2 | Allocated |
|-------------- |--------------------- |----------:|---------:|----------:|----------:|-------:|------:|------:|----------:|
|          Dpdt |     GenericSingleton |  43.02 ns | 0.843 ns |  0.788 ns |  42.94 ns |      - |     - |     - |         - |
|          Dpdt |     GenericTransient |  65.93 ns | 1.155 ns |  1.080 ns |  65.65 ns | 0.0187 |     - |     - |     144 B |
|          Dpdt |  NonGenericSingleton |  79.71 ns | 3.464 ns | 10.105 ns |  75.87 ns |      - |     - |     - |         - |
|          Dpdt |  NonGenericTransient | 100.22 ns | 1.975 ns |  2.028 ns |  99.92 ns | 0.0187 |     - |     - |     144 B |
|        Dryloc |     GenericSingleton |  96.37 ns | 0.989 ns |  0.877 ns |  96.52 ns |      - |     - |     - |         - |
|        Dryloc |     GenericTransient | 156.20 ns | 1.830 ns |  1.712 ns | 156.72 ns | 0.0188 |     - |     - |     144 B |
|        Dryloc |  NonGenericSingleton |  56.82 ns | 0.998 ns |  0.933 ns |  56.50 ns |      - |     - |     - |         - |
|        Dryloc |  NonGenericTransient | 117.67 ns | 2.046 ns |  1.709 ns | 117.69 ns | 0.0188 |     - |     - |     144 B |
| Microresolver |     GenericSingleton |  57.18 ns | 0.985 ns |  0.921 ns |  57.06 ns |      - |     - |     - |         - |
| Microresolver |     GenericTransient | 124.55 ns | 1.937 ns |  1.812 ns | 124.48 ns | 0.0188 |     - |     - |     144 B |
| Microresolver |  NonGenericSingleton |  30.84 ns | 0.490 ns |  0.458 ns |  30.83 ns |      - |     - |     - |         - |
| Microresolver |  NonGenericTransient |  91.18 ns | 1.445 ns |  1.281 ns |  91.36 ns | 0.0187 |     - |     - |     144 B |
