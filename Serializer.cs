using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Reflection
{
    /// <summary> Serializer </summary>
    public static class Serializer
    {
        /// <summary> Serialize from object to CSV </summary>
        /// <param name="obj">any object</param>
        /// <returns>CSV</returns>
        public static string SerializeFromObjectToCSV(object obj)
        {
            if(obj == null)
            {
                throw new NullReferenceException();
            }
            
            Type typeObj = obj.GetType();

            PropertyInfo[] props = typeObj.GetProperties();


            var i = 1;

            var sb = new StringBuilder();

            foreach (PropertyInfo prop in props)
            {
                sb.Append(prop.GetValue(obj).ToString());
                sb.Append(i < props.Count() ?  ";" : "");
                i++;

            }

            var ret = sb.ToString();
            
            return ret;
        }

        /// <summary> Deserialize from CSV to object</summary>
        /// <param name="csv">string in CSV format</param>
        /// <returns>object</returns>
        public static object DeserializeFromCSVToObject<T>(string csv)
        {

            string[] values = csv.Split(';');
            

            var instance  = Activator.CreateInstance(typeof(T));

            PropertyInfo[] props = typeof(T).GetProperties();
           

            var i = 0;

            foreach (PropertyInfo prop in props)
            {
                
                switch (prop.PropertyType.Name)
                {
                    case "Int32":
                        var toInt = Int32.Parse(values[i]);
                        prop.SetValue(instance, toInt);
                        break;
                    
                    default:
                        prop.SetValue(instance, values[i]);
                        break;
                }
                
                i++;

            }


            return instance;
        }
    }
}
