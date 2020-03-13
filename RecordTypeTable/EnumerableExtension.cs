using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace RecordTypeTable
{
    public static class EnumerableExtension
    {
        public static void GetTypeTable<T>(this IEnumerable<T> sequence, StreamWriter sw)
        {
            if (sequence is null)
            {
                throw new ArgumentNullException($"{nameof(sequence)} is null.");
            }

            if (sw is null)
            {
                throw new ArgumentNullException($"{nameof(sw)} is null.");
            }

            StringBuilder result = new StringBuilder();
            List<PropertyInfo> properties = new List<PropertyInfo>();
            Queue<RowInfo> rowInfoQueue = new Queue<RowInfo>();
            Type type = typeof(T);
            string divider;
            properties = type.GetProperties().ToList();

            foreach (var property in properties)
            {
                Type propertyType = property.PropertyType;
                RowInfo rowInfo = new RowInfo(sequence.Select(arg => property.GetValue(arg) is DateTime ? ((DateTime)property.GetValue(arg)).ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) : property.GetValue(arg).ToString()).ToList(), propertyType, property.Name);
                rowInfoQueue.Enqueue(rowInfo);
            }

            divider = GetDivider(rowInfoQueue);

            result.Append(divider + "\n");

            foreach (var element in rowInfoQueue)
            {
                result.Append(" | " + element.PropertyName);
                for (int i = 0; i < element.MaxLength - element.PropertyName.Length; i++)
                {
                    result.Append(" ");
                }
            }

            result.Append(" |");
            result.Append("\n");
            result.Append(divider + "\n");

            for (int i = 0; i < sequence.Count(); i++)
            {
                foreach (var element in rowInfoQueue)
                {
                    if (element.Type.IsValueType && element.Type != typeof(char))
                    {
                        result.Append(" | ");
                        for (int j = 0; j < element.MaxLength - element[i].Length; j++)
                        {
                            result.Append(" ");
                        }

                        result.Append(element[i]);
                    }

                    if (element.Type == typeof(string) || element.Type == typeof(char))
                    {
                        result.Append(" | ");
                        result.Append(element[i]);
                        for (int j = 0; j < element.MaxLength - element[i].Length; j++)
                        {
                            result.Append(" ");
                        }
                    }
                }

                result.Append(" |");
                result.Append("\n");
                result.Append(divider + "\n");
            }

            sw.Write(result.ToString());
            sw.Close();
        }

        private static string GetDivider(Queue<RowInfo> queue)
        {
            StringBuilder divider = new StringBuilder();
            divider.Append(" +");
            foreach (var rowInfo in queue)
            {
                for (int i = 0; i < rowInfo.MaxLength + 2; i++)
                {
                    divider.Append("-");
                }

                divider.Append("+");
            }

            return divider.ToString();
        }
    }
}