using Neo4j.Driver;

namespace urele.Service.Helper
{
    public class Executor
    {

        static IDriver driver { get; } = GraphDatabase.Driver("neo4j+s://b92f89c5.databases.neo4j.io", AuthTokens.Basic("neo4j", "MPUHmKacL3lLKq34xQGBpyHMX-BBInxLiqTB0kJsZlw"));
        static IAsyncSession session { get; } = driver.AsyncSession(o => o.WithDatabase("neo4j"));
        public async static Task<IDictionary<string, List<object>>> execute(string query)
        {
            try
            {
                IResultCursor cursor = await session.RunAsync(query);
                List<IRecord> jsonsList1 = await cursor.ToListAsync();
                IDictionary<string, List<object>> dictionary = new Dictionary<string, List<object>>();
                foreach (IRecord item in jsonsList1)
                {
                    foreach (string key in item.Keys)
                    {
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, new List<object>());
                        }
                    }
                    foreach (var node in item.Values)
                    {
                        if (node.Value == null)
                        {
                            dictionary[node.Key].Add(null);
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.Internal.Types.Node")
                        {
                            dictionary[node.Key].Add(new NodeEntity((INode)node.Value));
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalDate")
                        {
                            int year = ((LocalDate)node.Value).Year;
                            int month = ((LocalDate)node.Value).Month;
                            int day = ((LocalDate)node.Value).Day;
                            dictionary[node.Key].Add(new DateTime(year, month, day));
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.ZonedDateTime")
                        {
                            int year = ((ZonedDateTime)node.Value).Year;
                            int month = ((ZonedDateTime)node.Value).Month;
                            int day = ((ZonedDateTime)node.Value).Day;
                            int hour = ((ZonedDateTime)node.Value).Hour;
                            int minute = ((ZonedDateTime)node.Value).Minute;
                            int second = ((ZonedDateTime)node.Value).Second;
                            dictionary[node.Key].Add(new DateTime(year, month, day, hour, minute, second));
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalTime")
                        {
                            int hour = ((LocalTime)node.Value).Hour;
                            int minute = ((LocalTime)node.Value).Minute;
                            int second = ((LocalTime)node.Value).Second;
                            dictionary[node.Key].Add(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second));
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalDateTime")
                        {
                            int year = ((LocalDateTime)node.Value).Year;
                            int month = ((LocalDateTime)node.Value).Month;
                            int day = ((LocalDateTime)node.Value).Day;
                            int hour = ((LocalDateTime)node.Value).Hour;
                            int minute = ((LocalDateTime)node.Value).Minute;
                            int second = ((LocalDateTime)node.Value).Second;
                            dictionary[node.Key].Add(new DateTime(year, month, day, hour, minute, second));
                        }
                        else
                        {
                            dictionary[node.Key].Add(node.Value);
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                throw new Exception("Hata: ", ex);
            }
        }

        public static DateTime NeoDateTimeDecrypt(object node)
        {
            if (node.GetType().ToString() == "Neo4j.Driver.LocalDate")
            {
                int year = ((LocalDate)node).Year;
                int month = ((LocalDate)node).Month;
                int day = ((LocalDate)node).Day;
                return new DateTime(year, month, day);
            }
            else if (node.GetType().ToString() == "Neo4j.Driver.ZonedDateTime")
            {
                int year = ((ZonedDateTime)node).Year;
                int month = ((ZonedDateTime)node).Month;
                int day = ((ZonedDateTime)node).Day;
                int hour = ((ZonedDateTime)node).Hour;
                int minute = ((ZonedDateTime)node).Minute;
                int second = ((ZonedDateTime)node).Second;
                return new DateTime(year, month, day, hour, minute, second);
            }
            else if (node.GetType().ToString() == "Neo4j.Driver.LocalTime")
            {
                int hour = ((LocalTime)node).Hour;
                int minute = ((LocalTime)node).Minute;
                int second = ((LocalTime)node).Second;
                return new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
            }
            else if (node.GetType().ToString() == "Neo4j.Driver.LocalDateTime")
            {
                int year = ((LocalDateTime)node).Year;
                int month = ((LocalDateTime)node).Month;
                int day = ((LocalDateTime)node).Day;
                int hour = ((LocalDateTime)node).Hour;
                int minute = ((LocalDateTime)node).Minute;
                int second = ((LocalDateTime)node).Second;
                return new DateTime(year, month, day, hour, minute, second);
            }
            else
            {
                return DateTime.MinValue;
            }
        }

        public async static Task<IDictionary<string, object>> executeOneNode(string query)
        {
            try
            {
                IResultCursor cursor = await session.RunAsync(query);
                List<IRecord> jsonsList1 = await cursor.ToListAsync();
                IDictionary<string, object> dictionary = new Dictionary<string, object>();
                foreach (IRecord item in jsonsList1)
                {
                    foreach (string key in item.Keys)
                    {
                        if (!dictionary.ContainsKey(key))
                        {
                            dictionary.Add(key, new List<object>());
                        }
                    }
                    foreach (var node in item.Values)
                    {
                        if (node.Value == null)
                        {
                            dictionary[node.Key] = null;
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.Internal.Types.Node")
                        {
                            dictionary[node.Key] = new NodeEntity((INode)node.Value);
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalDate")
                        {
                            int year = ((LocalDate)node.Value).Year;
                            int month = ((LocalDate)node.Value).Month;
                            int day = ((LocalDate)node.Value).Day;
                            dictionary[node.Key] = new DateTime(year, month, day);
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.ZonedDateTime")
                        {
                            int year = ((ZonedDateTime)node.Value).Year;
                            int month = ((ZonedDateTime)node.Value).Month;
                            int day = ((ZonedDateTime)node.Value).Day;
                            int hour = ((ZonedDateTime)node.Value).Hour;
                            int minute = ((ZonedDateTime)node.Value).Minute;
                            int second = ((ZonedDateTime)node.Value).Second;
                            dictionary[node.Key] = new DateTime(year, month, day, hour, minute, second);
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalTime")
                        {
                            int hour = ((LocalTime)node.Value).Hour;
                            int minute = ((LocalTime)node.Value).Minute;
                            int second = ((LocalTime)node.Value).Second;
                            dictionary[node.Key] = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, hour, minute, second);
                        }
                        else if (node.Value.GetType().ToString() == "Neo4j.Driver.LocalDateTime")
                        {
                            int year = ((LocalDateTime)node.Value).Year;
                            int month = ((LocalDateTime)node.Value).Month;
                            int day = ((LocalDateTime)node.Value).Day;
                            int hour = ((LocalDateTime)node.Value).Hour;
                            int minute = ((LocalDateTime)node.Value).Minute;
                            int second = ((LocalDateTime)node.Value).Second;
                            dictionary[node.Key] = new DateTime(year, month, day, hour, minute, second);
                        }

                        else
                        {
                            dictionary[node.Key] = node.Value;
                        }
                    }
                }
                return dictionary;
            }
            catch (Exception ex)
            {
                throw new Exception("Hata: ", ex);
            }
        }
        public async static Task executeReturnless(string query)
        {
            await (await session.RunAsync(query)).FetchAsync();
        }
    }
    public class NodeEntity
    {
        public NodeEntity(Neo4j.Driver.INode node)
        {
            this.Id = Convert.ToInt64(node.Id);
            this.ElementId = node.ElementId;
            Labels = new List<string>();
            foreach (var j in node.Labels)
            {
                Labels.Add(j);
            }
            Properties = new Dictionary<string, object>();
            foreach (var i in node.Properties)
            {
                Properties.Add(i.Key, i.Value);
            }
        }
        public NodeEntity() { }

        public string ElementId { get; set; }
        public long Id { get; set; }
        public List<string> Labels { get; set; }
        public IDictionary<string, object> Properties { get; set; }
    }
}
