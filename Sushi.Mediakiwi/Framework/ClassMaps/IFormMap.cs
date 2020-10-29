using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sushi.Mediakiwi.Framework
{
    public interface IFormMap
    {
        void Init(WimComponentListRoot wim);
        string UniqueId { get; set; }
        List<IFormMap> FormMaps { get; set; }
        void Evaluate();

        List<IContentInfo> Elements { get; set; }
        Object SenderInstance { get; set; }
        bool? IsHidden { get; set; }
        bool? IsCloacked { get; set; }
        bool? IsReadOnly { get; set; }
    }
}