﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DpdtInject.Generator.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resources {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("DpdtInject.Generator.Properties.Resources", typeof(Resources).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using DpdtInject.Generator.Producer.Blocks.Binding;
        ///using DpdtInject.Injector.Module.RContext;
        ///using System;
        ///using System.Runtime.CompilerServices;
        ///using System.Threading;
        ///
        ///namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
        ///{
        ///    public sealed class SingletonInstanceContainer
        ///    {
        ///#nullable enable
        ///        private volatile static Action? _currentDisposeAction = null;
        ///        private volatile static Action _realDisposeAction =
        ///            () =&gt;
        ///            {
        ///              [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string SingletonInstanceContainer {
            get {
                return ResourceManager.GetString("SingletonInstanceContainer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to using DpdtInject.Generator.Producer.Blocks.Binding;
        ///using DpdtInject.Injector.Module.RContext;
        ///using System;
        ///using System.Runtime.CompilerServices;
        ///using System.Threading;
        ///
        ///namespace DpdtInject.Generator.Producer.Blocks.Binding.InstanceContainer
        ///{
        ///    public static class TransientInstanceContainer
        ///    {
        ///#nullable enable
        ///
        ///        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ///        public static bool CheckPredicate(ResolutionContext resolutionContext)
        ///        {
        ///            return DateTim [rest of string was truncated]&quot;;.
        /// </summary>
        internal static string TransientInstanceContainer {
            get {
                return ResourceManager.GetString("TransientInstanceContainer", resourceCulture);
            }
        }
    }
}