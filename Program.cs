using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
//using System.Text.Json;

namespace Reflection
{
    class Sample
    {
        private int field = 42;

        public int Field { get => field; }
    }

    class F
    {
       
        public int i1 { get; set; }
        public int i2 { get; set; }
        public int i3 { get; set; }
        public int i4 { get; set; }
        public int i5 { get; set; }

    }

    

    

    class Program
    {
        static void Main(string[] args)
        {
            // FieldMagic();
            //PropertiesMagic();
            //DynamicAssembly();
            //DynamicSample();

            //SerializeStandartFunc(100000);


            // сериализация / десериализация
            var csv = SerializeFunc(1);

            var dateStart = DateTime.Now;
            int count = 1000;
            Console.WriteLine();
            for (int i = 0; i < count; i++)
            {
                F f1 = (F)Serializer.DeserializeFromCSVToObject<F>(csv);
            }
            Console.WriteLine($"Время начала десериализации {count} раз: {dateStart}");
            Console.WriteLine($"Время конца десериализации {count} раз: {DateTime.Now}");
            Console.WriteLine($"Разница: {DateTime.Now - dateStart}");
            //F f = (F)Serializer.DeserializeFromCSVToObject(csv);
            //Console.WriteLine($"f.i1: {f.i1}, f.i2: {f.i2}, f.i3: {f.i3}, f.i4: {f.i4}, f.i5: {f.i5}");


        }


        static string SerializeFunc(int count)
        {
            var dateStart = DateTime.Now;

            string ret = null;

            for (int i = 0; i < count; i++)
            {

                var f = new F() { i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5 };

                ret = Serializer.SerializeFromObjectToCSV(f);

                
            }

            Console.WriteLine($"Время начала сериализации {count} раз: {dateStart}");
            Console.WriteLine($"Время конца сериализации {count} раз: {DateTime.Now}");
            Console.WriteLine($"Разница: {DateTime.Now - dateStart}");

            
            
            return ret;
        }

        static void SerializeStandartFunc(int count)
        {
            var dateStart = DateTime.Now;


            for (int i = 0; i < count; i++)
            {

                var f = new F() { i1 = 1, i2 = 2, i3 = 3, i4 = 4, i5 = 5 };

                var ret = JsonConvert.SerializeObject(f);

                //Console.WriteLine(ret);
            }

            Console.WriteLine($"Время начала сериализации {count} раз: {dateStart}");
            Console.WriteLine($"Время конца сериализации {count} раз: {DateTime.Now}");
            Console.WriteLine($"Разница: {DateTime.Now - dateStart}");
        }


        static void FieldMagic()
        {
            Sample sample = new Sample();

            Type sampleType = sample.GetType();
            FieldInfo fieldInfo = sampleType
                .GetField(name: "field", bindingAttr: BindingFlags.Instance | BindingFlags.NonPublic);

            //get value
            object fieldValue = fieldInfo.GetValue(obj: sample);
            Console.WriteLine(value: fieldValue);

            //set value
            fieldInfo.SetValue(obj: sample, value: 1);
            int sampleField = sample.Field;
            Console.WriteLine(value: sampleField);
        }

        static void PropertiesMagic()
        {
            Type type = typeof(string);
            PropertyInfo[] props = type.GetProperties();
            Console.WriteLine(value: "Properties:");
            foreach (PropertyInfo prop in props)
            {
                Console.WriteLine(value: prop.Name);
            }
            Console.WriteLine();

            FieldInfo[] fieldInfos = type.GetFields();
            Console.WriteLine(value: "Fields:");
            foreach (FieldInfo fieldInfo in fieldInfos)
            {
                Console.WriteLine(value: fieldInfo.Name);
            }
            Console.WriteLine();

            MethodInfo[] methodInfos = type.GetMethods();
            Console.WriteLine(value: "Methods:");
            foreach (MethodInfo methodInfo in methodInfos)
            {
                Console.WriteLine(value: methodInfo.Name);
            }

            //https://stackoverflow.com/questions/41468722/loop-reflect-through-all-properties-in-all-ef-models-to-set-column-type

            //https://stackoverflow.com/questions/19792295/mapping-composite-keys-using-ef-code-first
        }

        static void DynamicAssembly()
        {
            string solutionRoot = Directory.GetParent(path: Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName;
            string assemblyFile = Path.Combine(solutionRoot, @"MyLibrary\bin\Debug\netcoreapp3.1\MyLibrary.dll");
            if (!File.Exists(assemblyFile))
            {
                throw new FileNotFoundException("please build MyLibrary project");
            }

            Assembly assembly = Assembly.LoadFrom(assemblyFile: assemblyFile);
            Console.WriteLine(value: assembly.FullName);
            // получаем все типы из сборки MyLibrary.dll
            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                Console.WriteLine(value: type.Name);
            }

            // Позднее связывание
            Type myLibraryClassType = assembly.GetType(name: "MyLibrary.MyLibraryClass", throwOnError: true, ignoreCase: true);
            
            // создаем экземпляр класса
            object myLibraryClass = Activator.CreateInstance(type: myLibraryClassType);

            MethodInfo sumMethodInfo = myLibraryClassType.GetMethod(name: "Sum");

            object result = sumMethodInfo.Invoke(obj: myLibraryClass, parameters: new object[] { 2, 6 });

            Console.WriteLine(value: result);
        }

        static void DynamicSample()
        {
            dynamic sample1 = new Sample();
            var sample2 = new Sample();
            object sample3 = new Sample();
            Sample sample4 = new Sample();

            Console.WriteLine(sample1.Field);
            //Console.WriteLine(sample1.Method());
            Console.WriteLine();

            Console.WriteLine(value: "dynamic: " + sample1.GetType().Name); 
            Console.WriteLine(value: "var: " + sample2.GetType().Name);
            Console.WriteLine(value: "object: " + sample3.GetType().Name); 
            Console.WriteLine(value: "Sample: " + sample4.GetType().Name);
            Console.WriteLine();

            dynamic expando = new ExpandoObject();
            Console.WriteLine(value: "expando: " + expando.GetType().Name);

            expando.Name = "Brian";
            expando.Country = "USA";
            expando.City = new object();

            expando.IsValid = (Func<int, bool>)((number) =>
            {
                // Check that they supplied a name
                return !string.IsNullOrWhiteSpace(value: expando.Name);
            });

            expando.Print = (Action)(() =>
            {
                Console.WriteLine(value: $"{expando.Name} {expando.Country} {expando.IsValid(456456)}");
            });

            expando.Print();
            expando.Name = "Jack";
            expando.Print();
        }
    }
}