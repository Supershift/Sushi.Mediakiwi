using System;
namespace Sushi.Mediakiwi.Framework
{
    public interface IMultiFieldParser
    {
        //MultiField[] MultiFields { get; set; }
        ///// <summary>
        ///// Write the HTML output
        ///// </summary>
        ///// <returns></returns>
        //string WriteHTML(Sushi.Mediakiwi.Data.Page page);
        string WriteHTML(MultiField[] MultiFields, Sushi.Mediakiwi.Data.Page page = null);
        string WriteHTML(string serialized, Sushi.Mediakiwi.Data.Page page = null);
    }
}
