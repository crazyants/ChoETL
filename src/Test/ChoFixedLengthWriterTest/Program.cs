﻿using ChoETL;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChoFixedLengthWriterTest
{
    class Program
    {
        static void Main(string[] args)
        {
            ToTextTest();
        }

        static void ToTextTest()
        {
            ChoTypeConverterFormatSpec.Instance.DateTimeFormat = "G";
            ChoTypeConverterFormatSpec.Instance.BooleanFormat = ChoBooleanFormatSpec.YesOrNo;
            //ChoTypeConverterFormatSpec.Instance.EnumFormat = ChoEnumFormatSpec.Name;

            List<EmployeeRecSimple> objs = new List<EmployeeRecSimple>();
            EmployeeRecSimple rec1 = new EmployeeRecSimple();
            rec1.Id = 10;
            rec1.Name = "Mark";
            objs.Add(rec1);

            EmployeeRecSimple rec2 = new EmployeeRecSimple();
            rec2.Id = 200;
            rec2.Name = "Lou";
            objs.Add(rec2);

            Console.WriteLine(ChoFixedLengthWriter.ToText< EmployeeRecSimple>(objs));
        }

        static void POCOTest()
        {
            List<EmployeeRecSimple> objs = new List<EmployeeRecSimple>();

            EmployeeRecSimple rec1 = new EmployeeRecSimple();
            rec1.Id = 1;
            rec1.Name = "Mark";
            objs.Add(rec1);

            EmployeeRecSimple rec2 = new EmployeeRecSimple();
            rec2.Id = 2;
            rec2.Name = "Jason";
            objs.Add(rec2);

            using (var parser = new ChoFixedLengthWriter<EmployeeRecSimple>("Emp.txt").
                WithFirstLineHeader().
                WithField("Id", 0, 8).
                WithField("Name", 5, 10))
            {
                parser.Write(objs);
            }
        }

        static void QuickWriteTest2()
        {
            using (var parser = new ChoFixedLengthWriter("Emp.txt").
                WithFirstLineHeader().
                WithField("Id", 0, 8).
                WithField("Name", 5, 10))
            {
                dynamic rec1 = new ExpandoObject();
                rec1.Id = 1;
                rec1.Name = "Mark";

                parser.Write(rec1);

                dynamic rec2 = new ExpandoObject();
                rec2.Id = 2;
                rec2.Name = "Jason";

                parser.Write(rec2);
            }
        }

        static void QuickWriteTest()
        {
            List<ExpandoObject> objs = new List<ExpandoObject>();
            dynamic rec1 = new ExpandoObject();
            rec1.Id = 1;
            rec1.Name = "Mark";
            objs.Add(rec1);

            dynamic rec2 = new ExpandoObject();
            rec2.Id = 2;
            rec2.Name = "Jason";
            objs.Add(rec2);

            using (var parser = new ChoFixedLengthWriter("Emp.txt").
                WithFirstLineHeader().
                WithField("Id", 0, 8).
                WithField("Name", 5, 10))
            {
                parser.Write(objs);
            }
        }

        static void QuickDynamicTest()
        {
            ChoTypeConverterFormatSpec.Instance.DateTimeFormat = "MM/dd/yyyy";
            ChoTypeConverterFormatSpec.Instance.BooleanFormat = ChoBooleanFormatSpec.YOrN;

            List<ExpandoObject> objs = new List<ExpandoObject>();
            dynamic rec1 = new ExpandoObject();
            rec1.Id = 10;
            rec1.Name = "Mark";
            rec1.JoinedDate = new DateTime(2001, 2, 2);
            rec1.IsActive = true;
            rec1.Salary = new ChoCurrency(100000);
            objs.Add(rec1);

            dynamic rec2 = new ExpandoObject();
            rec2.Id = 2000;
            rec2.Name = "Lou";
            rec2.JoinedDate = null; // new DateTime(1990, 10, 23);
            rec2.IsActive = false;
            rec2.Salary = null; // new ChoCurrency(150000);
            objs.Add(rec2);

            using (var stream = new MemoryStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(stream))
            using (var parser = new ChoFixedLengthWriter(writer).WithFirstLineHeader().WithField("Id", 0, 3, null, '0', ChoFieldValueJustification.Right, true).WithField("Name", 3, 10).
                WithField("JoinedDate", 13, 10, fieldType: typeof(DateTime), fillChar: '0').WithField("IsActive", 23, 1, fieldName: "A").
                WithField("Salary", 24, 20))
            {
                parser.Configuration["Name"].AddConverter(new ChoUpperCaseConverter());
                parser.Write(objs);

                writer.Flush();
                stream.Position = 0;

                Console.WriteLine(reader.ReadToEnd());
            }
        }
    }

    public partial class EmployeeRecSimple
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
