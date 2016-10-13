
namespace NiHaoCookie
{


    public class ExecutableExists 
    {

        private static bool? m_IpTablesExists;
        public static bool IPtables
        {
            get
            {
                if (m_IpTablesExists.HasValue)
                    return m_IpTablesExists.Value;

                m_IpTablesExists = ExistsOnPath("iptables");
                return m_IpTablesExists.Value;
            }
        }

        private static bool? m_ServiceExists;
        public static bool Service
        {
            get
            {
                if (m_ServiceExists.HasValue)
                    return m_ServiceExists.Value;

                m_ServiceExists = ExistsOnPath("service");
                return m_ServiceExists.Value;
            }
        }


        public static bool ExistsOnPath(string fileName)
        {
            return GetFullPath(fileName) != null;
        }

        public static string GetFullPath(string fileName)
        {
            if (System.IO.File.Exists(fileName))
                return System.IO.Path.GetFullPath(fileName);

            var values = System.Environment.GetEnvironmentVariable("PATH");
            foreach (var path in values.Split(';'))
            {
                var fullPath = System.IO.Path.Combine(path, fileName);
                if (System.IO.File.Exists(fullPath))
                    return fullPath;
            }
            return null;
        }

    }


}
