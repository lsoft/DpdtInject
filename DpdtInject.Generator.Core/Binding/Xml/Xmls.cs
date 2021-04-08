using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;

namespace DpdtInject.Generator.Core.Binding.Xml
{
    public interface ISolutionBindContainer
    {
        IReadOnlyList<IClusterBindContainer> ClusterBindContainers
        {
            get;
        }

        bool TryGetBinding(
            Guid bindingIdentifier,
            out IBindingStatement? resultBinding
            );
    }

    public class SolutionBindContainer : ISolutionBindContainer
    {
        private readonly List<ClusterBindContainerXml> _clusterBindContainers;

        public IReadOnlyList<IClusterBindContainer> ClusterBindContainers => _clusterBindContainers;



        public SolutionBindContainer()
        {
            _clusterBindContainers = new List<ClusterBindContainerXml>();
        }


        /// <inheritdoc />
        public bool TryGetBinding(
            Guid bindingIdentifier,
            out IBindingStatement? resultBinding
            )
        {
            foreach(var clusterBind in this.ClusterBindContainers)
            {
                foreach (var mpair in clusterBind.GetMethodBindContainerDict())
                {
                    var methodBind = mpair.Value;

                    foreach (var binding in methodBind.Bindings)
                    {
                        if (binding.Identifier == bindingIdentifier)
                        {
                            resultBinding = binding;

                            return true;
                        }
                    }
                }
            }


            resultBinding = null;
            return false;
        }

        public void Append(
            ProjectBindContainerXml other
            )
        {
            if (other?.ClusterBindContainers != null)
            {
                _clusterBindContainers.AddRange(other.ClusterBindContainers);
            }
        }
    }


    public interface IProjectBindContainer
    {

        IReadOnlyList<IClusterBindContainer> ClusterBindContainers
        {
            get;
        }

        IReadOnlyDictionary<string, IClusterBindContainer> GetDictByDisplayString();

    }

    public class ProjectBindContainerXml : IProjectBindContainer
    {
        [XmlElement(ElementName = "ClusterBindContainer")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ClusterBindContainerXml[] ClusterBindContainers
        {
            get;
            set;
        }

        public int TotalBindingCount
        {
            get
            {
                if (ClusterBindContainers == null)
                {
                    return 0;
                }

                return ClusterBindContainers.Sum(
                    c => c.MethodBindContainers?.Sum(
                        m => m.Bindings?.Length ?? 0
                        ) ?? 0
                    );
            }
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IClusterBindContainer> GetDictByDisplayString()
        {
            if (this.ClusterBindContainers == null)
            {
                return new Dictionary<string, IClusterBindContainer>();
            }

            return this.ClusterBindContainers.ToDictionary(
                cbc => cbc.ClusterTypeInfo.FullDisplayName,
                cbc => (IClusterBindContainer)cbc
                );
        }

        IReadOnlyList<IClusterBindContainer> IProjectBindContainer.ClusterBindContainers => ClusterBindContainers;

        public ProjectBindContainerXml()
        {
            ClusterBindContainers = new ClusterBindContainerXml[0];
        }

        public ProjectBindContainerXml(
            ClusterBindContainerXml[] clusterBindContainers
            )
        {
            ClusterBindContainers = clusterBindContainers;
        }
    }


    public interface IClusterBindContainer
    {
        IClassTypeInfo ClusterTypeInfo
        {
            get;
        }

        IReadOnlyList<IMethodBindContainer> MethodBindContainers
        {
            get;
        }

        IReadOnlyDictionary<string, IMethodBindContainer> GetMethodBindContainerDict();
    }

    public class ClusterBindContainerXml : IClusterBindContainer
    {
        [XmlElement(ElementName = "ClusterTypeInfo")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ClassTypeInfoXml ClusterTypeInfo
        {
            get;
            set;
        }

        IClassTypeInfo IClusterBindContainer.ClusterTypeInfo => ClusterTypeInfo;

        [XmlElement(ElementName = "MethodBindContainer")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MethodBindContainerXml[] MethodBindContainers
        {
            get;
            set;
        }

        /// <inheritdoc />
        public IReadOnlyDictionary<string, IMethodBindContainer> GetMethodBindContainerDict()
        {
            return MethodBindContainers.ToDictionary(
                mbc => mbc.MethodDeclaration.MethodName,
                mbc => (IMethodBindContainer)mbc
                );
        }

        IReadOnlyList<IMethodBindContainer> IClusterBindContainer.MethodBindContainers => MethodBindContainers;

        public ClusterBindContainerXml()
        {
            ClusterTypeInfo = null!;
            MethodBindContainers = null!;
        }

        public ClusterBindContainerXml(
            ClassTypeInfoXml clusterTypeInfo,
            MethodBindContainerXml[] methodBindContainers
            )
        {
            ClusterTypeInfo = clusterTypeInfo;
            MethodBindContainers = methodBindContainers;
        }
    }

    public interface IMethodBindContainer
    {
        IClassTypeInfo ClusterTypeInfo
        {
            get;
        }

        IMethodDeclarationInfo MethodDeclaration
        {
            get;
        }

        IReadOnlyList<IBindingStatement> Bindings
        {
            get;
        }
    }

    public class MethodBindContainerXml : IMethodBindContainer
    {
        [XmlElement(ElementName = "ClusterTypeInfo")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ClassTypeInfoXml ClusterTypeInfo
        {
            get;
            set;
        }

        IClassTypeInfo IMethodBindContainer.ClusterTypeInfo => ClusterTypeInfo;

        [XmlElement(ElementName = "MethodDeclaration")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public MethodDeclarationInfoXml MethodDeclaration
        {
            get;
            set;
        }

        IMethodDeclarationInfo IMethodBindContainer.MethodDeclaration => MethodDeclaration;

        [XmlElement(ElementName = "Binding")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public BindingXml[] Bindings
        {
            get;
            set;
        }

        IReadOnlyList<IBindingStatement> IMethodBindContainer.Bindings => Bindings;

        public MethodBindContainerXml()
        {
            ClusterTypeInfo = null!;
            MethodDeclaration = null!;
            Bindings = null!;
        }

        public MethodBindContainerXml(
            ClassTypeInfoXml clusterTypeInfo,
            MethodDeclarationInfoXml methodDeclaration,
            BindingXml[] bindings
            )
        {
            if (clusterTypeInfo is null)
            {
                throw new ArgumentNullException(nameof(clusterTypeInfo));
            }

            if (methodDeclaration is null)
            {
                throw new ArgumentNullException(nameof(methodDeclaration));
            }

            if (bindings is null)
            {
                throw new ArgumentNullException(nameof(bindings));
            }

            ClusterTypeInfo = clusterTypeInfo;
            MethodDeclaration = methodDeclaration;
            Bindings = bindings;
        }
    }

    public class BindingXml : IBindingStatement
    {
        [XmlElement(ElementName = "Identifier")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Identifier
        {
            get;
            set;
        }

        Guid IBindingStatement.Identifier => new Guid(this.Identifier);

        [XmlElement(ElementName = "TargetRepresentation")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string TargetRepresentation
        {
            get;
            set;
        }


        [XmlElement(ElementName = "FromType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ClassTypeInfoXml[] FromTypes
        {
            get;
            set;
        }

        IReadOnlyList<IClassTypeInfo> IBindingStatement.FromTypes => FromTypes;


        [XmlElement(ElementName = "ToType")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public ClassTypeInfoXml ToType
        {
            get;
            set;
        }

        IClassTypeInfo IBindingStatement.BindToType => ToType;

        [XmlElement(ElementName = "ScopeString")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string ScopeString
        {
            get;
            set;
        }

        [XmlElement(ElementName = "ScopeEnum")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int ScopeEnumValue
        {
            get;
            set;
        }

        [XmlElement(ElementName = "IsConditional")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsConditional
        {
            get;
            set;
        }

        [XmlElement(ElementName = "IsConventional")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsConventional
        {
            get;
            set;
        }


        [XmlElement(ElementName = "Position")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PositionXml Position
        {
            get;
            set;
        }

        IPosition IBindingStatement.Position => Position;


        public BindingXml()
        {
            Identifier = null!;
            TargetRepresentation = null!;
            FromTypes = null!;
            ToType = null!;
            ScopeString = null!;
            Position = null!;
        }

        public BindingXml(
            string identifier,
            string targetRepresentation,
            ClassTypeInfoXml[] fromTypes,
            ClassTypeInfoXml toType,
            string scopeString,
            int scopeEnumValue,
            bool isConditional,
            bool isConventional,
            PositionXml position
            )
        {
            Identifier = identifier;
            TargetRepresentation = targetRepresentation;
            FromTypes = fromTypes;
            ToType = toType;
            ScopeString = scopeString;
            ScopeEnumValue = scopeEnumValue;
            IsConditional = isConditional;
            IsConventional = isConventional;
            Position = position;
        }
    }

    public class ClassTypeInfoXml : IClassTypeInfo
    {
        [XmlElement(ElementName = "FullDisplayName")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FullDisplayName
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FullyQualifiedName")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FullyQualifiedName
        {
            get;
            set;
        }

        [XmlElement(ElementName = "Name")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string Name
        {
            get;
            set;
        }

        [XmlElement(ElementName = "FullNamespaceDisplayName")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FullNamespaceDisplayName
        {
            get;
            set;
        }

        public ClassTypeInfoXml()
        {
            FullDisplayName = null!;
            FullyQualifiedName = null!;
            Name = null!;
            FullNamespaceDisplayName = null!;
        }

        public ClassTypeInfoXml(
            string fullDisplayName,
            string fullyQualifiedName,
            string name,
            string fullNamespaceDisplayName
            )
        {
            FullDisplayName = fullDisplayName;
            FullyQualifiedName = fullyQualifiedName;
            Name = name;
            FullNamespaceDisplayName = fullNamespaceDisplayName;
        }
    }

    public interface IMethodDeclarationInfo
    {
        IPosition Position
        {
            get;
        }

        string MethodName
        {
            get;
        }
    }

    public class MethodDeclarationInfoXml : IMethodDeclarationInfo
    {
        [XmlElement(ElementName = "Position")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public PositionXml Position
        {
            get;
            set;
        }

        IPosition IMethodDeclarationInfo.Position
        {
            get
            {
                return this.Position;
            }
        }


        [XmlElement(ElementName = "MethodName")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string MethodName
        {
            get;
            set;
        }

        public MethodDeclarationInfoXml(
            )
        {
            Position = null!;
            MethodName = null!;
        }

        public MethodDeclarationInfoXml(
            PositionXml position,
            string methodName
            )
        {
            if (position is null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            if (methodName is null)
            {
                throw new ArgumentNullException(nameof(methodName));
            }

            Position = position;
            MethodName = methodName;
        }
    }



    public class PositionXml : IPosition
    {
        [XmlElement(ElementName = "FilePath")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string FilePath
        {
            get;
            set;
        }

        [XmlElement(ElementName = "StartLine")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int StartLine
        {
            get;
            set;
        }


        [XmlElement(ElementName = "StartColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int StartColumn
        {
            get;
            set;
        }

        [XmlElement(ElementName = "EndLine")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int EndLine
        {
            get;
            set;
        }


        [XmlElement(ElementName = "EndColumn")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int EndColumn
        {
            get;
            set;
        }


        [XmlElement(ElementName = "SpanStart")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SpanStart
        {
            get;
            set;
        }

        [XmlElement(ElementName = "SpanLength")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public int SpanLength
        {
            get;
            set;
        }


        public PositionXml()
        {
            FilePath = null!;
        }

        public PositionXml(
            string filePath,
            int startLine,
            int startColumn,
            int endLine,
            int endColumn,
            int spanStart,
            int spanLength
            )
        {
            FilePath = filePath;
            StartLine = startLine;
            StartColumn = startColumn;
            EndLine = endLine;
            EndColumn = endColumn;
            SpanStart = spanStart;
            SpanLength = spanLength;
        }
    }


    public interface IBindingStatement
    {
        Guid Identifier
        {
            get;
        }

        string TargetRepresentation
        {
            get;
        }

        IReadOnlyList<IClassTypeInfo> FromTypes
        {
            get;
        }

        IClassTypeInfo BindToType
        {
            get;
        }

        string ScopeString
        {
            get;
        }

        int ScopeEnumValue
        {
            get;
        }

        bool IsConditional
        {
            get;
        }

        bool IsConventional
        {
            get;
        }

        IPosition Position
        {
            get;
        }
    }

    public interface IPosition
    {
        string FilePath
        {
            get;
        }

        int StartLine
        {
            get;
        }

        int StartColumn
        {
            get;
        }

        int EndLine
        {
            get;
        }

        int EndColumn
        {
            get;
        }

        int SpanStart
        {
            get;
        }

        int SpanLength
        {
            get;
        }

    }



    public interface IClassTypeInfo
    {
        string FullDisplayName
        {
            get;
        }

        string FullyQualifiedName
        {
            get;
        }

        string Name
        {
            get;
        }

        string FullNamespaceDisplayName
        {
            get;
        }
    }


}
