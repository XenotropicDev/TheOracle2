using TheOracle2.Data;

namespace Server.Data
{
    public interface IOracleRepository
    {
        IEnumerable<Oracle> GetOracles();
        IEnumerable<OracleRoot> GetOracleRoots();

        Oracle? GetOracle(string id);

        //void CreateOracle(int id);
        //void UpdateOracle(int id);
        //void DeleteOracle(int id);
    }

    public class JsonOracleRepository : IOracleRepository
    {
        private List<OracleRoot>? _oracles;

        public Oracle? GetOracle(string id)
        {
            return GetOracles().FirstOrDefault(o => o.Id == id);
        }

        public IEnumerable<OracleRoot> GetOracleRoots()
        {
            if (_oracles == null)
            {
                _oracles = new List<OracleRoot>();
                var baseDir = new DirectoryInfo(Path.Combine("Data", "ironsworn"));
                var files = baseDir.GetFiles("*oracle*.json");

                foreach (var file in files)
                {
                    using var fileStream = file.OpenText();
                    string text = fileStream.ReadToEnd();

                    var root = JsonConvert.DeserializeObject<List<OracleRoot>>(text);
                    
                    if (root != null) _oracles.AddRange(root);
                }

                foreach (var node in _oracles)
                {
                    foreach (var oracle in node.Oracles)
                    {
                        oracle.Parent = node;
                    }
                }
            }

            return _oracles;
        }

        public IEnumerable<Oracle> GetOracles()
        {
            return GetOracleRoots().SelectMany(root => root.Oracles);
        }
    }
}
