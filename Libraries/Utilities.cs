// Not used
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace GidIndustrial.Gideon.WebApi.Libraries {
    public static class Extensions {
        public static IQueryable<object> Set(this DbContext _context, Type t) {
            return (IQueryable<object>)_context.GetType().GetMethod("Set").MakeGenericMethod(t).Invoke(_context, null);
        }
    }
    public class Utilities {
        public Utilities() {

        }

        public static DateTime GetStartOfNextDay(DateTime dateTime) {
            var date = dateTime.AddDays(1);
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }
        public static DateTime GetStartOfDay(DateTime date) {
            return new DateTime(date.Year, date.Month, date.Day, 0, 0, 0, 0);
        }

        internal static string CSharpTypeToTypescriptType(Type underlyingSystemType) {
            var typeString = underlyingSystemType.ToString();
            var typeStringLower = typeString.ToLower();
            var ListRegex = new Regex(@"System.Collections.Generic.List`\d\[(.+)\]");
            var listMatch = ListRegex.Match(typeString);
            if (typeStringLower.Contains("system.int32") || typeStringLower.Contains("system.int64") || typeStringLower.Contains("system.int16") || typeStringLower.Contains("system.decimal")) {
                return "number";
            } else if (typeStringLower.Contains("system.boolean")) {
                return "boolean";
            } else if (typeStringLower.Contains("system.datetime")) {
                return "date";
            } else if (typeStringLower.Contains("system.string")) {
                return "string";
            } else if (listMatch.Success) {
                return listMatch.Groups.ToList()[1] + "[]";
            }
            return typeString;
        }
        private static HashSet<Type> NumericTypes = new HashSet<Type> {
            typeof(int),
            typeof(uint),
            typeof(double),
            typeof(decimal)
        };

        public static decimal RoundMoney(decimal? amount) {
            return Math.Round((amount ?? 0), 2);
        }

        internal static string GetQuotedSql(dynamic item) {
            if (item == null)
                return "NULL";
            var type = item.GetType();
            if (NumericTypes.Contains(type) ||
                   NumericTypes.Contains(Nullable.GetUnderlyingType(type))) {
                return item.ToString();
            }
            return $"'{item.ToString().Replace("'", "''")}'";
        }

        public static string CleanSqlColumnName(string colName) {
            var regex = new Regex(@"[^a-zA-Z_]");

            return regex.Replace(colName, "");
        }

        public static string PascalToHyphenated(string input) {
            string res = Regex.Replace(input, @"([a-z])([A-Z])", "$1-$2").ToLower();
            return res;
        }

        public static string FormatPhone(string number) {
            if (String.IsNullOrWhiteSpace(number)) {
                return "";
            }
            var finalString = "";
            if (number.Length > 10) {
                var countryCodeLength = number.Length - 10;
                var countryCode = number.Substring(0, countryCodeLength);
                finalString = "+" + countryCode + " ";
                number = number.Substring(countryCodeLength);
            }
            if (number.Length == 10) {
                finalString += "(" + number.Substring(0, 3) + ") " + number.Substring(3, 3) + "-" + number.Substring(6, 4);
            }
            return finalString;
        }


        // /// <summary>
        // /// 
        // /// </summary>
        // /// <param name="old">The item currently stored in the database. This SHOULD be tracking</param>
        // /// <param name="new">The new item. Make sure to set the entitystate of this one to detatched BEFORE selecting the old one</param>
        // /// <typeparam name="T"></typeparam>
        // /// <returns></returns>
        // public static Task UpdateChildEntities<T>(IEnumerable<dynamic> old, IEnumerable<dynamic> newItem){
        //     old.Filters.RemoveAll(item => !newItem.Filters.Any(f => f.Id == item.Id));
        //     old.Filters.AddRange(
        //         newItem.Filters.Where(item => !old.Filters.Any(f => f.Id == item.Id))
        //     );
        //     old.Filters.Where(item => newItem.Filters.Any(f => f.Id == item.Id)).ToList().ForEach(item => _context.Entry(item).State = EntityState.Modified);
        // }

        // public static DbSet<dynamic> GetDbSetByTypeName(AppDBContext _context, string typeName) {
        //     var type = Type.GetType(typeName);

        //     if (type != null)
        //         return _context.Set(type);
        //     // foreach(var property in _context.GetType().GetProperties()){
        //     //     if(property.PropertyType.GetGenericArguments().First().ToString() == typeName){
        //     //         return property.GetValue();
        //     //     }
        //     // }
        // }
    }
}