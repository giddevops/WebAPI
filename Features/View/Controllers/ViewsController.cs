/**
 * This is a tough file. Don't give up if it's confusing. You'll get it. Read some, take a break and work on it later if need to.
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GidIndustrial.Gideon.WebApi.Libraries;
using GidIndustrial.Gideon.WebApi.Models;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace WebApi.Features.Controllers {
    public class JoinInfo {
        public string ParentTableName { get; set; }
        public string ChildTableName { get; set; }
        public string ParentTableKey { get; set; }
        public string ChildTableKey { get; set; }
    }
    public class WhereConditionInfo {
        public string ChildrenAndOrOr { get; set; }
        public string Key { get; set; }
        public string Comparison { get; set; }
        public string Value { get; set; }
        public int? OrGroupNumber { get; set; }
        public List<WhereConditionInfo> Children { get; set; }

        public static WhereConditionInfo GetWhereConditionInfoForOrGroupNumber(List<WhereConditionInfo> conditions, int? OrGroupNumber) {
            if (conditions == null || conditions.Count == 0)
                return null;
            foreach (var condition in conditions) {
                if (condition.OrGroupNumber == OrGroupNumber) {
                    return condition;
                }
                var result = WhereConditionInfo.GetWhereConditionInfoForOrGroupNumber(condition.Children, OrGroupNumber);
                if (result != null)
                    return result;
            }
            return null;
        }
        public static string GetQuery(List<WhereConditionInfo> conditions, WhereConditionInfo parent = null) {
            var i = 0;
            var query = "";
            var conjunctionText = parent == null ? "AND" : parent.ChildrenAndOrOr;
            foreach (var condition in conditions) {
                if (i > 0)
                    query += $" {conjunctionText} ";
                if (condition.Children != null) {
                    query += "(" + WhereConditionInfo.GetQuery(condition.Children, condition) + ")";
                } else if (condition.Comparison == ViewFilterCondition.NULL_OR_EMPTY) {
                    query += $"({condition.Key} IS NULL OR {condition.Key} = '')";
                } else {
                    query += condition.Key + condition.Comparison + condition.Value;
                }
                ++i;
            }
            return query;
        }
    }

    [Produces("application/json")]
    [Route("Views")]
    public class ViewsController : Controller {
        private readonly AppDBContext _context;
        public ViewsController(AppDBContext context) {
            _context = context;
        }

        [HttpGet("GetObjectNames")]
        public IActionResult GetObjectNames([FromRoute] int id) {
            var properties = _context.GetType().GetProperties();
            var names = new List<string> { };
            foreach (var property in properties) {
                //check if it's a dbContext
                if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>)) {
                    if (!property.Name.Contains("Option") && !property.Name.Contains("Method") && !property.Name.Contains("Filter") && !property.Name.Contains("Type")) {
                        names.Add(property.PropertyType.GetGenericArguments()[0].ToString());
                    }
                }
            }
            return Ok(names.Select(name => new {
                Id = name,
                Value = name.Replace("GidIndustrial.Gideon.WebApi.Models.", "")
            }));
        }

        // Get the custom fields
        [HttpGet("GetObjectFieldsByObjectName")]
        public IActionResult GetObjectFieldsByObjectName([FromQuery] string ObjectName) {
            if (String.IsNullOrWhiteSpace(ObjectName)) {
                return BadRequest("ObjectName querystring parameter is required");
            }
            Type ObjectType = Type.GetType(ObjectName);
            var properties = ObjectType.GetProperties();
            return Ok(GetTypePropertiesList(ObjectType));
        }

        [HttpGet("GetNavigationPropertyDetails")]
        public IActionResult GetNavigationPropertyDetail([FromQuery] string ObjectName, [FromQuery] string propertyName) {
            Type ObjectType = Type.GetType(ObjectName);
            if (ObjectType == null) {
                return BadRequest($"ObjectName {ObjectName} was not found");
            }
            var navigationProp = _context.Model.FindEntityType(ObjectType).GetNavigations().FirstOrDefault(item => item.Name == propertyName);
            if (navigationProp == null) {
                return BadRequest($"The property {propertyName} was not found on object {ObjectName}");
            }

            // var properties = navigationProp.ClrType.GetProperties();
            return Ok(GetTypePropertiesList(navigationProp.ClrType));
        }

        private dynamic GetTypePropertiesList(Type ObjectType) {
            var navigations = _context.Model.FindEntityType(ObjectType).GetNavigations();
            return ObjectType.GetProperties().Select(item => new {
                Id = item.Name,
                Value = item.Name,
                Type = item.PropertyType.UnderlyingSystemType,
                TypePretty = Utilities.CSharpTypeToTypescriptType(item.PropertyType.UnderlyingSystemType),
                IsNavigationProperty = navigations.Any(nav => nav.Name == item.Name)
            }).OrderBy(item => item.Value);
        }


        /// <summary>
        /// This is the main function that gets results for a view.  It does MULTIPLE queries to get the data most efficiently.
        /// 
        /// The first query returns only columns from the main table. So if querying SalesOrders, even if there are joins, this query will only return stuff from the SalesOrders table
        /// The first query is generated by looping through all the filters.  It checks if there is a user supplied value or if we are going to use the value from the database
        /// When a filter is created, users can specify unlimited "drill-downs".  That is, they can say SalesOrder.SalesOrderLineItems[].Product.PartNumber='MyPartNumber'
        /// The program loops through each filter and determines what tables will need to be joined, and what the conditions are.
        /// Once it is done looping through filters, it then constructs the joins and queries.
        /// 
        /// The next queries are to get the results.  It loops through each column that should be displayed. If it's not a "drill-down" column, it just gets the result from the first query
        /// If it is a drill-down column, it performs another query to get the value(s) for that column for all the rows. It could be multiple values if it's an array drilldown. For example SalesOrder.SalesOrderLineItems[ALL].Product.PartNumber
        /// As you can see, for display fields, it allows users to specify [ALL] or [FIRST].  However, THIS IS CURRENTLY IGNORED ON THE SERVER I THINK
        /// The thing to realize is that it generates another query for every single drill-down column. So this seconds stage can do multiple queries. I designed it this way because it is simpler, and maybe faster because otherwise it could have a huge amount of joins, and large numbers of joins make things slow
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="timeZone"></param>
        /// <param name="Dictionary<int?"></param>
        /// <param name="UserSpecifiedFilterValues"></param>
        /// <param name="skip"></param>
        /// <param name="perPage"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        [HttpPost("{id}/GetDataRows")]
        public async Task<dynamic> GetDataRows(
           [FromRoute] int id,
           [FromQuery] string timeZone,
           [FromBody] Dictionary<int?, dynamic> UserSpecifiedFilterValues,
           [FromQuery] int skip = 0,
           [FromQuery] int perPage = 10,
           [FromQuery] int? sortBy = null,
           [FromQuery] bool sortAscending = true,
           bool idsOnly = false,
           List<int> itemIdList = null,
           bool forAlertsOnly = false
        ) {
            var view = await _context.Views
                .Include(item => item.Filters)
                .Include(item => item.DisplayFields)
                .Include(item => item.ConditionalFormatters)
                    .ThenInclude(item => item.Conditions)
                .FirstOrDefaultAsync(item => item.Id == id);
            if (view == null) {
                return NotFound("No view was found with that Id");
            }
            var conn = _context.Database.GetDbConnection();

            var tableName = (new Regex("[^a-zA-Z]").Replace(view.ObjectName.Split(".").Last(), ""));

            var baseQuery = $"FROM \"{tableName}\" ";
            var parameters = new List<dynamic> { };

            // The client will submit the user's current time zone. This is important because if they want to see leads from today only, it needs to be in their time zone
            var now = DateTime.UtcNow;
            var tzi = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneConverter.TZConvert.IanaToWindows(timeZone));
            var nowUserTime = TimeZoneInfo.ConvertTimeFromUtc(now, tzi);

            //used to remove [ALL]
            var removeArraySyntaxRegex = new Regex(@"\[(.*?)\]");

            //set filters
            var joins = new Dictionary<string, JoinInfo> { };
            var conditions = new List<WhereConditionInfo> { };

            //Get metedata about the current main object we are searching
            Type objectType = Type.GetType(view.ObjectName);
            var objectTableName = _context.Model.FindEntityType(objectType).SqlServer().TableName;
            var baseTypeName = view.ObjectName.Split(".").Last();
            if (objectType == null) {
                return BadRequest($"ObjectName {view.ObjectName} was not found as a type");
            }
            //Find properties that are "navigations", that is, properties that will need joins.
            var navigations = _context.Model.FindEntityType(objectType).GetNavigations();
            var mainTableName = _context.Model.FindEntityType(objectType).SqlServer().TableName;

            // Loop through each filter to generate conditions for each filter
            // If need be, will have to drill-down and figure out what joins will need to be added
            if (itemIdList != null && itemIdList.Count > 0) { //if just want to select by id
                conditions.Add(new WhereConditionInfo {
                    Key = $"\"{mainTableName}\".Id",
                    Comparison = " IN ",
                    Value = "(" + String.Join(',', itemIdList) + ")"
                });
            } else {
                var filters = view.Filters;
                if (!forAlertsOnly) {
                    filters = filters.Where(item => item.ForAlertsOnly == false).ToList();
                }
                foreach (var filter in filters) {
                    var value = filter.Value;
                    if (UserSpecifiedFilterValues.ContainsKey(filter.Id)) {
                        var valString = UserSpecifiedFilterValues[(int)filter.Id].ToString();
                        if (UserSpecifiedFilterValues[filter.Id] == null || valString == null) {
                            value = null;
                        } else {
                            value = valString;
                        }
                    }
                    string finalFilterFieldName = "";

                    //filter.FieldName is someting like this:
                    //Quote.QuoteLineItem[].Product.PartNumber
                    //This code splits that up and figures out what joins will be needed
                    //It stores joins in the joins Dictionary to make sure that multiple joins are not attempted for a single table which would cause an error
                    //this could happen if multiple filters are created for the same field (ex. Date < END_OF_TODAY AND Date > START_OF_MONTH)
                    var typeNames = filter.FieldName.Split('.');
                    INavigation currentNavigation = null;
                    var currentNavigationType = objectType;
                    Type previousNavigationType;
                    for (var j = 0; j < typeNames.Length; ++j) {
                        var typeName = typeNames[j];
                        var varTypeName = removeArraySyntaxRegex.Replace(typeName, ""); //replace array syntax in our notation to get the actual name of the underlying type
                        var isLast = j == typeNames.Length - 1;
                        if (isLast == false) {
                            //It's possible there are multiple conditions for a single navigation property. Make sure to only do the join once for proper SQL
                            //rummage through EF core metadata to figure out the parent table name, child table name, foreign key, and the associated properties on each respective object in order to generate the right query
                            if (currentNavigation == null) {
                                previousNavigationType = currentNavigationType;
                                currentNavigation = navigations.FirstOrDefault(item => item.Name == varTypeName);
                            } else {
                                previousNavigationType = currentNavigation.ClrType;
                                if (previousNavigationType.GetGenericArguments().FirstOrDefault() != null) {
                                    previousNavigationType = previousNavigationType.GetGenericArguments().FirstOrDefault();
                                }
                                currentNavigation = _context.Model.FindEntityType(previousNavigationType).GetNavigations().FirstOrDefault(item => item.Name == varTypeName);
                            }
                            if (currentNavigation == null) {
                                return BadRequest("Error getting navigation property " + varTypeName);
                            }
                            Type joinedType;
                            if (typeName != varTypeName) { //if it's an array type
                                joinedType = currentNavigation.PropertyInfo.PropertyType.GenericTypeArguments[0];
                                currentNavigationType = currentNavigation.ClrType.GetGenericArguments()[0];
                            } else {
                                joinedType = currentNavigation.PropertyInfo.PropertyType;
                                currentNavigationType = currentNavigation.ClrType;
                            }

                            var join = joins.GetValueOrDefault(varTypeName);
                            if (join == null) {
                                var childTableName = _context.Model.FindEntityType(joinedType).SqlServer().TableName;
                                //check if foreign key is on the currentNavigationProperty or on the next one
                                string childTableKey = "", parentTableKey = "";
                                if (currentNavigation.ForeignKey.PrincipalEntityType.ClrType == previousNavigationType) {
                                    childTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                                    parentTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                                } else {
                                    childTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                                    parentTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                                }

                                joins.Add(varTypeName, new JoinInfo {
                                    ParentTableName = _context.Model.FindEntityType(previousNavigationType).SqlServer().TableName,
                                    ChildTableName = childTableName,
                                    ChildTableKey = childTableKey,
                                    ParentTableKey = parentTableKey
                                });
                            }
                        } else {
                            finalFilterFieldName = $"\"{_context.Model.FindEntityType(currentNavigationType).SqlServer().TableName}\".\"{typeName}\"";
                        }
                    }

                    switch (value) {
                        // case ViewFilterConstants.TODAY:
                        //     value = now.ToString("Y-m-d");
                        //     break;
                        case ViewFilterConstants.START_OF_TODAY:
                            value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, nowUserTime.Day, 0, 0, 0, 0), tzi).ToString("o");
                            break;
                        case ViewFilterConstants.END_OF_TODAY:
                            value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, nowUserTime.Day, 23, 59, 59, 999), tzi).ToString("o");
                            break;
                        case ViewFilterConstants.START_OF_MONTH:
                            value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, 1), tzi).ToString("s");
                            break;
                        case ViewFilterConstants.START_OF_YEAR:
                            value = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, 1, 1), tzi).ToString("s");
                            break;
                    }
                    var parameterValue = "";
                    if (filter.ViewFilterCondition != ViewFilterCondition.NULL_OR_EMPTY) {
                        parameterValue = "@" + parameters.Count.ToString();
                        parameters.Add(value);
                    }
                    var newCondition = new WhereConditionInfo {
                        Key = finalFilterFieldName,
                        Comparison = filter.ViewFilterCondition,
                        Value = parameterValue
                    };
                    if (filter.OrGroupNumber != null) {
                        //first look to see if or group is there already
                        var existingGroup = WhereConditionInfo.GetWhereConditionInfoForOrGroupNumber(conditions, filter.OrGroupNumber);
                        if (existingGroup == null) {
                            existingGroup = new WhereConditionInfo {
                                OrGroupNumber = filter.OrGroupNumber,
                                ChildrenAndOrOr = "OR",
                                Children = new List<WhereConditionInfo> { }
                            };
                            conditions.Add(existingGroup);
                        }
                        existingGroup.Children.Add(newCondition);
                    } else {
                        conditions.Add(newCondition);
                    }
                }
            }

            //Now set up the sort.  Since the user could sort by a display field, not a filter, there may be another join that is needed
            //it takes quite a bit of code to add the joins... maybe will consolidate this at some point.
            var sortByFieldName = $"\"{objectTableName}\".\"CreatedAt\"";
            if (sortBy == null) {
                //find default sort by
                var defaultSortField = view.DisplayFields.FirstOrDefault(item => item.Sortable == true && item.SortOrder != null);
                if (defaultSortField != null) {
                    sortBy = defaultSortField.Id;
                    sortAscending = defaultSortField.SortOrder != SortOrder.Descending ? true : false;
                }
            }
            if (sortBy != null) {
                var displayField = view.DisplayFields.FirstOrDefault(item => item.Id == sortBy);
                var typeNames = displayField.FieldName.Split('.');
                INavigation currentNavigation = null;
                var currentNavigationType = objectType;
                Type previousNavigationType;

                //step through each item in the FieldName, one at a time and get joins.
                for (var j = 0; j < typeNames.Length; ++j) {
                    var typeName = typeNames[j];
                    var varTypeName = removeArraySyntaxRegex.Replace(typeName, ""); //replace array syntax in our notation to get the actual name of the underlying type
                    var isLast = j == typeNames.Length - 1;
                    if (isLast == false) {
                        //rummage through EF core metadata to figure out the parent table name, child table name, foreign key, and the associated properties on each respective object in order to generate the right query
                        if (currentNavigation == null) {
                            previousNavigationType = currentNavigationType;
                            currentNavigation = navigations.FirstOrDefault(item => item.Name == varTypeName);
                        } else {
                            previousNavigationType = currentNavigation.ClrType;
                            if (previousNavigationType.GetGenericArguments().FirstOrDefault() != null) {
                                previousNavigationType = previousNavigationType.GetGenericArguments().FirstOrDefault();
                            }
                            currentNavigation = _context.Model.FindEntityType(previousNavigationType).GetNavigations().FirstOrDefault(item => item.Name == varTypeName);
                        }
                        if (currentNavigation == null) {
                            return BadRequest("Error getting navigation property " + varTypeName);
                        }
                        Type joinedType;
                        if (typeName != varTypeName) { //if it's an array type
                            joinedType = currentNavigation.PropertyInfo.PropertyType.GenericTypeArguments[0];
                            currentNavigationType = currentNavigation.ClrType.GetGenericArguments()[0];
                        } else {
                            joinedType = currentNavigation.PropertyInfo.PropertyType;
                            currentNavigationType = currentNavigation.ClrType;
                        }

                        //It's possible there are multiple conditions for a single navigation property. Make sure to only do the join once for proper SQL
                        var join = joins.GetValueOrDefault(varTypeName);
                        if (join == null) {
                            var childTableName = _context.Model.FindEntityType(joinedType).SqlServer().TableName;
                            //check if foreign key is on the currentNavigationProperty or on the next one
                            string childTableKey = "", parentTableKey = "";
                            if (currentNavigation.ForeignKey.PrincipalEntityType.ClrType == previousNavigationType) {
                                childTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                                parentTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                            } else {
                                childTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                                parentTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                            }
                            joins.Add(varTypeName, new JoinInfo {
                                ParentTableName = _context.Model.FindEntityType(previousNavigationType).SqlServer().TableName,
                                ChildTableName = childTableName,
                                ChildTableKey = childTableKey,
                                ParentTableKey = parentTableKey
                            });
                        }
                    } else {
                        sortByFieldName = $"\"{_context.Model.FindEntityType(currentNavigationType).SqlServer().TableName}\".\"{typeName}\"";
                    }
                }
            }


            var joinsList = joins.ToList().Select(item => item.Value);
            foreach (var join in joinsList) {
                baseQuery += $"\n left join \"{join.ChildTableName}\" on \"{join.ParentTableName}\".\"{join.ParentTableKey}\" = \"{join.ChildTableName}\".\"{join.ChildTableKey}\" ";
            }
            baseQuery += "\n";

            if (conditions.Count > 0) {
                baseQuery += " WHERE ";
            }
            baseQuery += WhereConditionInfo.GetQuery(conditions);

            var sortDirectionText = sortAscending ? "ASC" : "DESC";

            var mainQuery = $"SELECT \"{baseTypeName}\".* {baseQuery} \n ORDER BY {sortByFieldName} {sortDirectionText} \n OFFSET @skip ROWS FETCH NEXT @limit ROWS ONLY";
            var countQuery = "SELECT COUNT(*) " + baseQuery;
            var mainQueryDebug = mainQuery;

            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>Query is >>>>>>>>>>>>>>>>>>>>\n" + mainQuery + "\n<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>Count Query is >>>>>>>>>>>>>>>>>>>>\n" + countQuery + "\n<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");
            var i = 0;
            foreach (var param in parameters) {
                mainQueryDebug = mainQueryDebug.Replace("@" + i.ToString(), Utilities.GetQuotedSql(parameters[i]));
                ++i;
            }
            mainQueryDebug = mainQueryDebug.Replace("@limit", perPage.ToString());
            mainQueryDebug = mainQueryDebug.Replace("@skip", skip.ToString());
            Console.WriteLine(">>>>>>>>>>>>>>>>>>>>Query [debug] is >>>>>>>>>>>>>>>>>>>>\n" + mainQueryDebug + "\n<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

            var telemetryClient = new TelemetryClient();
            telemetryClient.TrackTrace($"Query is \n {mainQuery}\n\nDebug query is \n\n{mainQueryDebug}\n", SeverityLevel.Information, new Dictionary<string, string> { { "url", "url" } });

            var table = new DataTable();
            int count;
            try {
                await conn.OpenAsync();
                //main query
                using (var command = conn.CreateCommand()) {
                    command.CommandText = mainQuery;

                    //paremeterize all inputs to avoid sql injection
                    DbParameter sender;
                    i = 0;
                    foreach (var param in parameters) {
                        sender = command.CreateParameter();
                        sender.ParameterName = i.ToString();
                        sender.Value = parameters[i];
                        command.Parameters.Add(sender);
                        ++i;
                    }
                    sender = command.CreateParameter();
                    sender.ParameterName = "skip";
                    sender.Value = skip;
                    command.Parameters.Add(sender);

                    sender = command.CreateParameter();
                    sender.ParameterName = "limit";
                    sender.Value = perPage;
                    command.Parameters.Add(sender);

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    table.Load(reader);
                    reader.Dispose();
                }
                //count query
                using (var command = conn.CreateCommand()) {
                    command.CommandText = countQuery;

                    DbParameter sender;
                    i = 0;
                    foreach (var param in parameters) {
                        sender = command.CreateParameter();
                        sender.ParameterName = i.ToString();
                        sender.Value = parameters[i];
                        command.Parameters.Add(sender);
                        ++i;
                    }

                    DbDataReader reader = await command.ExecuteReaderAsync();
                    await reader.ReadAsync();
                    count = reader.GetInt32(0);
                    reader.Dispose();
                }
            }
            finally { conn.Close(); }

            var rows = this.CopyRowsToObject(table, objectType);
            if (rows.Count == 0 || idsOnly) {
                return new {
                    Items = rows.Select(item => item.Id).ToList(),
                    Count = rows.Count
                };
            }

            var finalRows = new List<Dictionary<string, dynamic>> { };
            var ids = new List<int> { };

            foreach (var row in rows) {
                var itemId = objectType.GetProperty("Id").GetValue(row, null);
                ids.Add(itemId);
                var newDict = new Dictionary<string, dynamic> { };
                newDict["__MatchedConditionalFormats"] = new List<int> { };
                newDict[objectTableName + "Id"] = itemId;
                finalRows.Add(newDict);
            }

            var fieldNamesCleanedDictionary = new Dictionary<string, string> { };
            var fieldNamesAndConditionalFormattingColumnsCleanedDictionary = new Dictionary<string, string> { };
            foreach (var displayField in view.DisplayFields) {
                var cleanName = removeArraySyntaxRegex.Replace(displayField.FieldName, "[]");
                fieldNamesCleanedDictionary[cleanName] = displayField.FieldName;
                fieldNamesAndConditionalFormattingColumnsCleanedDictionary[cleanName] = displayField.FieldName;
            }
            foreach (var conditionalFormatter in view.ConditionalFormatters) {
                foreach (var condition in conditionalFormatter.Conditions) {
                    var cleanName = removeArraySyntaxRegex.Replace(condition.FieldName, "[]");
                    fieldNamesAndConditionalFormattingColumnsCleanedDictionary[cleanName] = condition.FieldName;
                }
            }

            //Need to show all display fields and any fields that are used for conditional formatting
            var fieldNamesForConditionalFormatting = view.ConditionalFormatters.SelectMany(item => item.Conditions.Select(item2 => item2.FieldName));
            var fieldNamesToDisplay = view.DisplayFields.Select(item => item.FieldName).Concat(fieldNamesForConditionalFormatting);
            foreach (var fieldName in fieldNamesToDisplay) {
                var typeNames = fieldName.Split(".");
                joins = new Dictionary<string, JoinInfo> { };
                //if this is not a navigation property, just use the value retrieved in the original query
                if (typeNames.Length == 1) {
                    i = 0;
                    foreach (var finalRow in finalRows) {
                        var property = objectType.GetProperty(typeNames[0]);
                        var value = property.GetValue(rows[i], null);
                        finalRow[typeNames[0]] = value;
                        this.CheckConditionalFormatting(view, finalRow, fieldName, fieldNamesAndConditionalFormattingColumnsCleanedDictionary, nowUserTime, tzi);
                        ++i;
                    }
                    continue;
                }

                //if this is a navigation property, need to do another query to get the final value of it.
                INavigation currentNavigation = null;
                var currentNavigationType = objectType;
                Type previousNavigationType;
                string finalDisplayFieldFieldName = "", finalDisplayFieldFieldNameSingleQuotes = "", finalDisplayFieldFieldNameNoQuotes = "", selectCountOption = "";
                var selectCountOptionRegex = new Regex(@"\[(.*?)\]");


                baseQuery = $"SELECT \"{objectTableName}\".Id as '{objectTableName}.Id', FINAL_DISPLAY_FIELD_FIELD_NAME as LAST_FIELD_SINGLE_QUOTES FROM \"{objectTableName}\"";

                //Loop through all the parts of the "name" to get all the joins needed.
                //Ex. if the name is SalesOrders.LineItems[].Product.PartNumber, do all joins needed.
                for (var j = 0; j < typeNames.Length; ++j) {
                    var typeName = typeNames[j];

                    var varTypeName = removeArraySyntaxRegex.Replace(typeName, ""); //For display field, it can have [ALL] or [FIRST] syntax
                    var isLast = j == typeNames.Length - 1;
                    if (isLast == false) {
                        if (currentNavigation == null) {
                            previousNavigationType = currentNavigationType;
                            currentNavigation = navigations.FirstOrDefault(item => item.Name == varTypeName);
                        } else {
                            previousNavigationType = currentNavigation.ClrType;
                            if (previousNavigationType.GetGenericArguments().FirstOrDefault() != null) {
                                previousNavigationType = previousNavigationType.GetGenericArguments().FirstOrDefault();
                            }
                            currentNavigation = _context.Model.FindEntityType(previousNavigationType).GetNavigations().FirstOrDefault(item => item.Name == varTypeName);
                        }
                        if (currentNavigation == null) {
                            return BadRequest("Error getting navigation property " + varTypeName);
                        }
                        Type joinedType;
                        if (typeName != varTypeName) { //if it's an array type
                            joinedType = currentNavigation.PropertyInfo.PropertyType.GenericTypeArguments[0];
                            selectCountOption = selectCountOptionRegex.Match(typeName).Groups[1].Value;
                            currentNavigationType = currentNavigation.ClrType.GetGenericArguments()[0];
                        } else {
                            joinedType = currentNavigation.PropertyInfo.PropertyType;
                            currentNavigationType = currentNavigation.ClrType;
                        }

                        var join = joins.GetValueOrDefault(varTypeName);
                        if (join == null) {
                            var childTableName = _context.Model.FindEntityType(joinedType).SqlServer().TableName;
                            //check if foreign key is on the currentNavigationProperty or on the next one
                            string childTableKey = "", parentTableKey = "";
                            if (currentNavigation.ForeignKey.PrincipalEntityType.ClrType == previousNavigationType) {
                                childTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                                parentTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                            } else {
                                childTableKey = currentNavigation.ForeignKey.PrincipalKey.Properties.First().Name;
                                parentTableKey = currentNavigation.ForeignKey.Properties.First().Name;
                            }

                            joins.Add(varTypeName, new JoinInfo {
                                ParentTableName = _context.Model.FindEntityType(previousNavigationType).SqlServer().TableName,
                                ChildTableName = childTableName,
                                ChildTableKey = childTableKey,
                                ParentTableKey = parentTableKey
                            });
                        }
                    } else {
                        var tblName = _context.Model.FindEntityType(currentNavigationType).SqlServer().TableName;
                        finalDisplayFieldFieldName = $"\"{tblName}\".\"{typeName}\"";
                        finalDisplayFieldFieldNameSingleQuotes = $"'{tblName}.{typeName}'";
                        finalDisplayFieldFieldNameNoQuotes = $"{tblName}.{typeName}";
                    }
                }
                baseQuery = baseQuery.Replace("FINAL_DISPLAY_FIELD_FIELD_NAME", finalDisplayFieldFieldName);
                baseQuery = baseQuery.Replace("LAST_FIELD_SINGLE_QUOTES", finalDisplayFieldFieldNameSingleQuotes);

                joinsList = joins.ToList().Select(item => item.Value);
                foreach (var join in joinsList) {
                    baseQuery += $"\n left join \"{join.ChildTableName}\" on \"{join.ParentTableName}\".\"{join.ParentTableKey}\" = \"{join.ChildTableName}\".\"{join.ChildTableKey}\" ";
                }
                baseQuery += "\n";

                baseQuery += $" WHERE \"{objectTableName}\".Id IN ({String.Join(",", ids)})";
                // if (selectCountOption == "FIRST") {
                //     baseQuery += $"\nORDER BY \"{objectTableName}\".Id";
                //     baseQuery += "\nOFFSET 0 ROWS \n FETCH NEXT 1 ROWS ONLY";
                // }
                Console.WriteLine($">>>>>>>>>>>>>>>>>>>>Query is For {fieldName} >>>>>>>>>>>>>>>>>>>>\n" + baseQuery + "\n<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<<");

                telemetryClient.TrackTrace($"Query is For display field: {fieldName} \n {baseQuery}\n\n", SeverityLevel.Information, new Dictionary<string, string> { { "url", "url" } });

                //Run the query to get the display field, and add the results to the output List
                try {
                    await conn.OpenAsync();
                    using (var command = conn.CreateCommand()) {
                        command.CommandText = baseQuery;
                        var reader = await command.ExecuteReaderAsync();
                        table = new DataTable();
                        table.Load(reader);
                        var tableRows = this.CopyRowsToDictionaryList(table);
                        reader.Dispose();
                        foreach (var resultRow in tableRows) {
                            var matchingRow = finalRows.FirstOrDefault(item => item[objectTableName + "Id"] == resultRow[objectTableName + ".Id"]);
                            // if (selectCountOption == "ALL") {
                            if (!matchingRow.ContainsKey(fieldName)) {
                                matchingRow[fieldName] = new List<dynamic> { };
                            }
                            matchingRow[fieldName].Add(resultRow[finalDisplayFieldFieldNameNoQuotes]);
                            // } else {
                            //     matchingRow[fieldName] = resultRow[finalDisplayFieldFieldNameNoQuotes];
                            // }

                        }
                    }
                }
                finally {
                    conn.Close();
                }
            }

            //check rows to see if conditional formatting applies
            foreach (var matchingRow in finalRows) {
                foreach (var formatter in view.ConditionalFormatters) {
                    foreach (var condition in formatter.Conditions) {
                        // var matchingRow = finalRows.FirstOrDefault(item => item[objectTableName + "Id"] == resultRow[objectTableName + ".Id"]);
                        this.CheckConditionalFormatting(view, matchingRow, condition.FieldName, fieldNamesAndConditionalFormattingColumnsCleanedDictionary, nowUserTime, tzi);
                    }
                }
            }

            return new ListResult {
                Items = finalRows,
                Count = count
            };
        }
        public void CheckConditionalFormatting(View view, dynamic matchingRow, string fieldName, Dictionary<string, string> fieldNamesAndConditionalFormattingColumnsCleanedDictionary, DateTime nowUserTime, TimeZoneInfo tzi) {
            var removeArraySyntaxRegex = new Regex(@"\[(.*?)\]");
            foreach (var formatter in view.ConditionalFormatters) {
                foreach (var condition in formatter.Conditions) {
                    //Check if this condition is for this field
                    //conditions don't have [ALL] or [FIRST] they just have []. So to match, need to get "cleaned" field names - i.e. remove ALL or FIRST
                    var cleanName = removeArraySyntaxRegex.Replace(condition.FieldName, "[]");
                    if (fieldName == fieldNamesAndConditionalFormattingColumnsCleanedDictionary[cleanName]) {
                        //check if the condition matches
                        var value = matchingRow[fieldName];
                        //the "value" may actually be an array of values. For example it could be LineItems[all].Product.PartNumber, in which there could be multiple.  So need to check all of them
                        bool satistfiesCondition = false;
                        if (value is IList && value.GetType().IsGenericType) {
                            foreach (var val in value) {
                                satistfiesCondition = this.CheckIfConditionMatches(val, condition, nowUserTime, tzi);
                                if (satistfiesCondition) {
                                    break;
                                }
                            }
                        } else {
                            satistfiesCondition = this.CheckIfConditionMatches(value, condition, nowUserTime, tzi);
                        }
                        if (satistfiesCondition) {
                            if (!(matchingRow["__MatchedConditionalFormats"] as List<int>).Contains(formatter.Id ?? 0)) {
                                (matchingRow["__MatchedConditionalFormats"] as List<int>).Add(formatter.Id ?? 0);
                            }
                        }
                    }
                }
            }
        }
        public bool CheckIfConditionMatches(dynamic value, ViewConditionalFormatterCondition condition, DateTime nowUserTime, TimeZoneInfo tzi) {
            //Check for null/empty
            if (String.IsNullOrWhiteSpace(condition.Value) || condition.Value == ViewFilterCondition.NULL_OR_EMPTY) {
                if (value == null) {
                    return true;
                }
                if (value is string && String.IsNullOrEmpty(value)) {
                    return true;
                }
                return false;
            }
            dynamic valueToMatch = condition.Value;
            switch (condition.FieldType) {
                case "date":
                    switch (condition.Value) {
                        case ViewFilterConstants.START_OF_TODAY:
                            valueToMatch = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, nowUserTime.Day, 0, 0, 0, 0), tzi).ToString("o");
                            break;
                        case ViewFilterConstants.END_OF_TODAY:
                            valueToMatch = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, nowUserTime.Day, 23, 59, 59, 999), tzi).ToString("o");
                            break;
                        case ViewFilterConstants.START_OF_MONTH:
                            valueToMatch = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, nowUserTime.Month, 1), tzi).ToString("s");
                            break;
                        case ViewFilterConstants.START_OF_YEAR:
                            valueToMatch = TimeZoneInfo.ConvertTimeToUtc(new DateTime(nowUserTime.Year, 1, 1), tzi).ToString("s");
                            break;
                    }
                    valueToMatch = DateTime.Parse(valueToMatch);
                    break;
                case "string":
                    break;
                case "number":
                    valueToMatch = decimal.Parse(valueToMatch);
                    value = value != null && (value as Object).GetType() != typeof(System.DBNull) ? (decimal?)value : null;
                    break;
                default:
                    break;
            }
            switch (condition.ViewFilterCondition) {
                case ViewFilterCondition.EQUALS:
                    if (value == valueToMatch)
                        return true;
                    break;
                case ViewFilterCondition.DOES_NOT_EQUAL:
                    if (value != valueToMatch)
                        return true;
                    break;
                case ViewFilterCondition.GREATER_THAN:
                    if (value > valueToMatch)
                        return true;
                    break;
                case ViewFilterCondition.GREATER_THAN_OR_EQUAL_TO:
                    if (value >= valueToMatch)
                        return true;
                    break;
                case ViewFilterCondition.LESS_THAN:
                    if (value < valueToMatch)
                        return true;
                    break;
                case ViewFilterCondition.LESS_THAN_OR_EQUAL_TO:
                    if (value <= valueToMatch)
                        return true;
                    break;
            }
            return false;
        }

        public void GetBaseQuery() { }

        public List<Dictionary<string, dynamic>> CopyRowsToDictionaryList(DataTable dataTable) {
            var columns = dataTable.Columns;
            var columnInfo = new Dictionary<string, int> { };
            foreach (DataColumn column in dataTable.Columns) {
                columnInfo[column.ColumnName] = column.Ordinal;
            }

            List<Dictionary<string, dynamic>> resultRows = new List<Dictionary<string, dynamic>> { };

            for (var i = 0; i < dataTable.Rows.Count; ++i) {
                var rowValues = new Dictionary<string, dynamic> { };
                foreach (var column in columnInfo) {
                    rowValues[column.Key] = dataTable.Rows[i][column.Value];
                }
                resultRows.Add(rowValues);
            }

            return resultRows;
        }

        public List<dynamic> CopyRowsToObject(DataTable dataTable, Type objectType) {
            var columns = dataTable.Columns;
            var columnInfo = new Dictionary<string, int> { };
            foreach (DataColumn column in dataTable.Columns) {
                columnInfo[column.ColumnName] = column.Ordinal;
            }
            var dbTypeInfo = _context.Model.FindEntityType(objectType);
            var typeProperties = dbTypeInfo.GetProperties();

            List<dynamic> resultRows = new List<dynamic> { };

            for (var i = 0; i < dataTable.Rows.Count; ++i) {
                dynamic newObj = Activator.CreateInstance(objectType);
                foreach (var property in typeProperties) {
                    if (property.IsShadowProperty)
                        continue;
                    dynamic value = dataTable.Rows[i][columnInfo[property.Name]];
                    if (value.GetType() == typeof(DBNull)) {
                        property.PropertyInfo.SetValue(newObj, null);
                    } else {
                        property.PropertyInfo.SetValue(newObj, value);
                    }
                }
                resultRows.Add(newObj);
            }

            return resultRows;
        }

        public async Task<SendGrid.Helpers.Mail.SendGridMessage> CreateAndSendNewItemInViewEmail(View view, List<Dictionary<string, dynamic>> newRows) {
            Console.WriteLine(JsonConvert.SerializeObject(newRows));
            var body = "<table border='1'><thead><tr>";

            Type objectType = Type.GetType(view.ObjectName);
            var objectTableName = _context.Model.FindEntityType(objectType).SqlServer().TableName;

            var i = 0;
            if (newRows.Count == 0)
                return null;
            // body += "<th>Id</th>"
            body += $"<th></th>";
            foreach (var displayField in view.DisplayFields) {
                body += $"<th>{displayField.Header}</th>";
            }
            body += "</tr></thead><tbody>";
            foreach (var row in newRows) {
                var url = "https://gideon.gidindustrial.com/";
                url += Utilities.PascalToHyphenated(objectTableName) + "s/" + row[objectTableName + "Id"];
                body += $"<tr>";

                foreach (var columnName in row.Keys) {
                    if (columnName == "__MatchedConditionalFormats")
                        continue;
                    var columnValue = row[columnName];
                    Type columnValueType = null;
                    bool columnValueIsIEnumerable = false;
                    bool columnValueIsGenericType = false;
                    if (columnValue != null)
                    {
                        columnValueType = row[columnName].GetType();
                        columnValueIsGenericType = columnValueType.IsGenericType;
                        columnValueIsIEnumerable = columnValue is IEnumerable;
                    }
                    if (columnName == objectTableName + "Id") {
                        body += $"<td><a href='{url}'>View</a></td>";
                    } else if (columnValueIsGenericType && columnValueIsIEnumerable) {
                        var values = new List<string> { };
                        foreach (var value in row[columnName]) {
                            values.Add(value.ToString());
                        }
                        body += "<td>" + String.Join(", ", values) + "</td>";
                    } else {
                        body += $@"<td>{row[columnName.ToString()]}</td>";
                    }
                }
                body += "</tr>";
                ++i;
            }
            body += "</tbody></table>";

            var client = EmailGenerator.GetNewSendGridClient();
            var msg = await EmailGenerator.GenerateEmail(_context, new EmailGeneratorParameters {
                To = view.AlertEmails,
                From = "views@gidindustrial.com",
                Subject = view.Name + " view has new data",
                HtmlContent = body
            });
            var response = await client.SendEmailAsync(msg);
            int responseStatusCodeNumber = (int)response.StatusCode;
            if (responseStatusCodeNumber < 200 || responseStatusCodeNumber >= 300) {
                Console.WriteLine("ERROR SENDING EMAIL");
            }
            return msg;
            // Console.WriteLine(body);
        }

        // [RequirePermission("EditViews")]
        [HttpGet("RunViewAlertsCron")]
        public async Task<ObjectResult> RunViewAlertsCron(bool returnEmailsSent = false) {
            var views = await _context.Views.ToListAsync();
            var emailsSent = new List<SendGrid.Helpers.Mail.SendGridMessage> { };

            foreach (var view in views) {
                if (String.IsNullOrWhiteSpace(view.AlertEmails))
                    continue;
                if (view.Deactivated) {
                    //if the view has been deactivated, we don't want alerts. If this is the first time that the view alerts cron is run after deactivation, go ahead and set the last view cache date to null, and delete all relevant items in the view cache. That way if it is deactivated the user won't get an alert for all new stuff since the last alert.
                    if (view.LastViewCacheDate != null) {
                        view.LastViewCacheDate = null;
                        _context.ViewCacheItems.RemoveRange(_context.ViewCacheItems.Where(item => item.ViewId == view.Id));
                        await _context.SaveChangesAsync();
                    }
                    continue;
                }
                //generate new view
                var result = await this.GetDataRows((int)view.Id, "America/Chicago", new Dictionary<int?, dynamic> { }, 0, 500, null, true, true, null, true);
                // return Ok(result);
                List<int> Ids = (result.Items as List<object>).Select(id => (int)id).ToList();

                if (view.LastViewCacheDate != null) {
                    //compare
                    var oldViewResults = await _context.ViewCacheItems.Where(item => item.ViewId == view.Id).ToListAsync();

                    //find new results
                    var newIds = Ids.Where(id => !oldViewResults.Any(item2 => item2.ItemId == id)).ToList();
                    //find removed results
                    var removedItems = oldViewResults.Where(item => !Ids.Contains((int)item.ItemId));

                    //mail out notifications for new Items
                    //first get the new items
                    if (newIds.Count > 0) {
                        var data = await this.GetDataRows((int)view.Id, "America/Chicago", new Dictionary<int?, dynamic> { }, 0, 500, null, true, false, newIds); ;
                        List<Dictionary<string, dynamic>> newItemFullObjects = data.Items;
                        //generate and send email
                        var email = await this.CreateAndSendNewItemInViewEmail(view, newItemFullObjects);
                        if (returnEmailsSent) {
                            emailsSent.Add(email);
                        }
                    }

                    // Console.WriteLine("New Items are " + JsonConvert.SerializeObject(newIds) + "  and remvoed " + JsonConvert.SerializeObject(removedItems));

                    //remove old items
                    _context.ViewCacheItems.RemoveRange(removedItems);
                    // //add new items
                    await _context.ViewCacheItems.AddRangeAsync(newIds.Select(id => new ViewCacheItem {
                        ViewId = (int)view.Id,
                        ItemId = (int)id
                    }));
                } else {
                    // //add all items
                    await _context.ViewCacheItems.AddRangeAsync(Ids.Select(item => new ViewCacheItem {
                        ViewId = (int)view.Id,
                        ItemId = (int)item
                    }));
                }
                view.LastViewCacheDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
            if (returnEmailsSent)
                return Ok(emailsSent);
            return Ok(null);
        }


        // GET: Views
        [HttpGet]
        public async Task<IActionResult> GetViews(
            [FromQuery] int skip = 0,
            [FromQuery] int perPage = 10,
            [FromQuery] string sortBy = null,
            [FromQuery] bool sortAscending = true
        ) {
            var query = from view in _context.Views select view;

            query = query
                .Include(item => item.Filters)
                .Include(item => item.DisplayFields)
                .Include(item => item.ConditionalFormatters)
                    .ThenInclude(item => item.Conditions)
                .OrderByDescending(q => q.CreatedAt);

            switch (sortBy) {
                case "Id":
                    query = sortAscending ? query.OrderBy(item => item.Id) : query.OrderByDescending(item => item.Id);
                    break;
                case "CreatedAt":
                    query = sortAscending ? query.OrderBy(item => item.CreatedAt) : query.OrderByDescending(item => item.CreatedAt);
                    break;
                default:
                    query = query.OrderByDescending(item => item.CreatedAt);
                    break;
            }
            return Ok(new ListResult {
                Items = query.Skip(skip).Take(perPage),
                Count = query.Count()
            });
        }

        // GET: Views/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetView([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            var view = await _context.Views
                .Include(item => item.Filters)
                .Include(item => item.DisplayFields)
                .Include(item => item.ConditionalFormatters)
                    .ThenInclude(item => item.Conditions)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (view == null) {
                return NotFound();
            }
            return Ok(view);
        }

        // PUT: Views/5
        [RequirePermission("EditViews")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutView([FromRoute] int id, [FromBody] View view) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            if (id != view.Id) {
                return BadRequest("Id in url and body don't match");
            }
            view.UpdatedAt = DateTime.UtcNow;
            _context.Entry(view).State = EntityState.Modified;

            try {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) {
                if (!ViewExists(id)) {
                    return NotFound();
                } else {
                    throw;
                }
            }
            _context.Entry(view).State = EntityState.Detached;

            this.CleanView(view);

            var oldView = await _context.Views
                .Include(item => item.DisplayFields)
                .Include(item => item.Filters)
                .Include(item => item.ConditionalFormatters)
                    .ThenInclude(item => item.Conditions)
                .FirstOrDefaultAsync(item => item.Id == id);

            //add/remove/update list items
            oldView.Filters.RemoveAll(item => !view.Filters.Any(f => f.Id == item.Id));
            view.Filters.Where(item => oldView.Filters.Any(f => f.Id == item.Id)).ToList().ForEach(item => {
                item.ViewId = (int)oldView.Id;
                _context.Entry(oldView.Filters.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                _context.Entry(item).State = EntityState.Modified;
            });
            oldView.Filters.AddRange(
                view.Filters.Where(item => item.Id == null)
            );

            oldView.DisplayFields.RemoveAll(item => !view.DisplayFields.Any(f => f.Id == item.Id));
            view.DisplayFields.Where(item => oldView.DisplayFields.Any(f => f.Id == item.Id)).ToList().ForEach(item => {
                item.ViewId = (int)oldView.Id;
                _context.Entry(oldView.DisplayFields.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                _context.Entry(item).State = EntityState.Modified;
            });
            oldView.DisplayFields.AddRange(
                view.DisplayFields.Where(item => item.Id == null)
            );

            oldView.ConditionalFormatters.RemoveAll(item => !view.ConditionalFormatters.Any(f => f.Id == item.Id));
            view.ConditionalFormatters.Where(item => oldView.ConditionalFormatters.Any(f => f.Id == item.Id)).ToList().ForEach(item => {
                item.ViewId = (int)oldView.Id;

                //each conditional formatter can have multple conditions. Need to check them.
                //For some reason there are 2!
                var oldConditionalFormatter = oldView.ConditionalFormatters.First(thing => thing.Id == item.Id);
                oldConditionalFormatter.Conditions.RemoveAll(thing => !item.Conditions.Any(f => f.Id == thing.Id));
                item.Conditions.Where(thing => oldConditionalFormatter.Conditions.Any(f => f.Id == thing.Id)).ToList().ForEach(thing => {
                    thing.ViewConditionalFormatterId = (int)oldConditionalFormatter.Id;
                    var oldThing = oldConditionalFormatter.Conditions.FirstOrDefault(old => old.Id == thing.Id);
                    _context.Entry(oldThing).State = EntityState.Detached;
                    _context.Entry(thing).State = EntityState.Modified;
                });
                oldConditionalFormatter.Conditions.AddRange(
                    item.Conditions.Where(thing => thing.Id == null)
                );
                _context.Entry(oldView.ConditionalFormatters.FirstOrDefault(oldItem => oldItem.Id == item.Id)).State = EntityState.Detached;
                _context.Entry(item).State = EntityState.Modified;
            });
            oldView.ConditionalFormatters.AddRange(
                view.ConditionalFormatters.Where(item => item.Id == null)
            );

            await _context.SaveChangesAsync();

            return Ok(view);
        }

        // POST: Views
        [RequirePermission("EditViews")]
        [HttpPost]
        public async Task<IActionResult> PostView([FromBody] View view) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }
            view.CreatedAt = DateTime.UtcNow;
            view.CreatedById = GidIndustrial.Gideon.WebApi.Models.User.GetId(HttpContext.User);
            this.CleanView(view);
            _context.Views.Add(view);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetView", new { id = view.Id }, view);
        }

        /// <summary>
        /// Prevent SQL injection
        /// </summary>
        public void CleanView(View view) {
            var nonSafeCharsRegex = new Regex(@"[^0-9a-zA-Z_\[\]=<>\!\.]");
            if (view.Filters != null)
                view.Filters.ForEach(item => {
                    item.FieldName = nonSafeCharsRegex.Replace(item.FieldName, "");
                    item.ViewFilterCondition = nonSafeCharsRegex.Replace(item.ViewFilterCondition, "");
                });
            if (view.DisplayFields != null)
                view.DisplayFields.ForEach(item => {
                    item.FieldName = nonSafeCharsRegex.Replace(item.FieldName, "");
                });
        }


        // DELETE: Views/5
        [RequirePermission("EditViews")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteView([FromRoute] int id) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var view = await _context.Views.SingleOrDefaultAsync(m => m.Id == id);
            if (view == null) {
                return NotFound();
            }

            _context.Views.Remove(view);
            await _context.SaveChangesAsync();

            return Ok(view);
        }

        private bool ViewExists(int id) {
            return _context.Views.Any(e => e.Id == id);
        }
    }
}