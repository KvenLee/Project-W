using System;
using System.IO;
using System.Net;
using System.Text;

/// <summary>
///  获取网络时间
/// </summary>
public class DateTimeHelper
{
    public static DateTime GetBeijingTime()
    {
        DateTime dt;

        // 返回国际标准时间
        // 只使用 timeServers 的 IP 地址，未使用域名
        try
        {
            string[,] timeServers = new string[14, 2];
            int[] searchOrder = { 3, 2, 4, 8, 9, 6, 11, 5, 10, 0, 1, 7, 12 };
            timeServers[0, 0] = "time-a.nist.gov";
            timeServers[0, 1] = "129.6.15.28";
            timeServers[1, 0] = "time-b.nist.gov";
            timeServers[1, 1] = "129.6.15.29";
            timeServers[2, 0] = "time-a.timefreq.bldrdoc.gov";
            timeServers[2, 1] = "132.163.4.101";
            timeServers[3, 0] = "time-b.timefreq.bldrdoc.gov";
            timeServers[3, 1] = "132.163.4.102";
            timeServers[4, 0] = "time-c.timefreq.bldrdoc.gov";
            timeServers[4, 1] = "132.163.4.103";
            timeServers[5, 0] = "utcnist.colorado.edu";
            timeServers[5, 1] = "128.138.140.44";
            timeServers[6, 0] = "time.nist.gov";
            timeServers[6, 1] = "192.43.244.18";
            timeServers[7, 0] = "time-nw.nist.gov";
            timeServers[7, 1] = "131.107.1.10";
            timeServers[8, 0] = "nist1.symmetricom.com";
            timeServers[8, 1] = "69.25.96.13";
            timeServers[9, 0] = "nist1-dc.glassey.com";
            timeServers[9, 1] = "216.200.93.8";
            timeServers[10, 0] = "nist1-ny.glassey.com";
            timeServers[10, 1] = "208.184.49.9";
            timeServers[11, 0] = "nist1-sj.glassey.com";
            timeServers[11, 1] = "207.126.98.204";
            timeServers[12, 0] = "nist1.aol-ca.truetime.com";
            timeServers[12, 1] = "207.200.81.113";
            timeServers[13, 0] = "nist1.aol-va.truetime.com";
            timeServers[13, 1] = "64.236.96.53";
            int portNum = 13;
            byte[] bytes = new byte[1024];
            int bytesRead = 0;
            System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
            for (int i = 0; i < 13; i++)
            {
                string hostName = timeServers[searchOrder[i], 1];
                try
                {
                    client.Connect(hostName, portNum);
                    System.Net.Sockets.NetworkStream ns = client.GetStream();
                    bytesRead = ns.Read(bytes, 0, bytes.Length);
                    client.Close();
                    break;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
            char[] sp = new char[1];
            sp[0] = ' ';
            dt = new DateTime();
            string str1 = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);

            string[] s = str1.Split(sp);
            if (s.Length >= 2)
            {
                dt = DateTime.Parse(s[1] + " " + s[2]); // 得到标准时间
                dt = dt.AddHours(8);                    // 得到北京时间
            }
            else
            {
                dt = DateTime.Parse("2016-1-1");
            }
        }
        catch (Exception)
        {
            dt = DateTime.Parse("2016-1-1");
        }
        return dt;
    }
}