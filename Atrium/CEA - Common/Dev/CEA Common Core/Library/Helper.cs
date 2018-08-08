using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AtriumWebApp.Web.Base.Library
{
    static public class Helper
    {
        static string ListToCVC(List<object> objects )
        {
            StringBuilder output = new StringBuilder();
            for(var i = 0; i < objects.Count(); i++)
            {
                var obj = objects[i];
                var type = obj.GetType();
                var properties = type.GetProperties();
                if(i == 0)
                {
                    for (var j = 0; j < properties.Count(); j++)
                    {
                        var property = properties[j];
                        output.Append(property.Name);
                        if(j < properties.Count() - 1)
                        {
                            output.Append(",");
                        }
                    }
                }
                for(var j = 0; j < properties.Count(); j++)
                {
                    var property = properties[j];
                    output.Append(property.GetValue(obj).ToString());
                    if (j < properties.Count() - 1)
                    {
                        output.Append(",");
                    }
                }
                
            }
            return output.ToString();
            
        }
    }
}
