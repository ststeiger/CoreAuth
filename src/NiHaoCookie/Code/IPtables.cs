
namespace NiHaoCookie
{


    public class IPtables
    {


        public void Ban(string IP)
        {
            if (!ExecutableExists.IPtables)
                return;

            System.Net.IPAddress ipa;
            if (System.Net.IPAddress.TryParse(IP, out ipa))
            {
                // One port only:  iptables -A INPUT -s 65.55.44.100 -p tcp --destination-port 25 -j DROP
                // All ports: iptables -A INPUT -s 65.55.44.100 -j DROP
                using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start("iptables", "-A INPUT -s " + IP + " -j DROP"))
                {
                    proc.WaitForExit();
                } // End Using proc 

                SaveConfig();
            } // End if (System.Net.IPAddress.TryParse(IP, out ipa)) 

        } // End Sub Ban 


        public void Unban(string IP)
        {
            if (!ExecutableExists.IPtables)
                return;

            System.Net.IPAddress ipa;
            if (System.Net.IPAddress.TryParse(IP, out ipa))
            {
                // iptables -D INPUT -s 66.55.44.33 -j DROP
                using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start("iptables", "-D INPUT -s " + IP + " -j DROP"))
                {
                    proc.WaitForExit();
                } // End Using proc 

                SaveConfig();
            } // End if (System.Net.IPAddress.TryParse(IP, out ipa)) 

        } // End Sub Unban 


        private void SaveConfig()
        {
            if (!ExecutableExists.IPtables || !ExecutableExists.Service)
                return;

            using (System.Diagnostics.Process proc = System.Diagnostics.Process.Start("service", "iptables save"))
            {
                proc.WaitForExit();
            } // End Using proc 
        } // End Sub SaveConfig 


    } // End Class IPtables


} // End Namespace NiHaoCookie
