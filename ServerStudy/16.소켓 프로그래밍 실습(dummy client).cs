using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        string host = Dns.GetHostName(); // ���� ��ǻ���� ȣ��Ʈ �̸�
        IPHostEntry ipHost = Dns.GetHostEntry(host);
        IPAddress ipAddr = ipHost.AddressList[0]; // ������ ȣ��Ʈ���� �迭 (�Ĵ� �ּ�)
        IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // ���� �ּ� ���� (��Ʈ ��ȣ�� �ش� �Ĵ��� ���� �� �� �ϳ�)

        // �޴��� ����
        Socket socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

        try
        {
            // �����⿡�� ���� ����
            socket.Connect(endPoint); // ������ �ϰڴ� -> ������ �ּҷ� ����
            Console.WriteLine($"Connected to {socket.RemoteEndPoint.ToString()}"); // ������ �ݴ��� ����� ���

            // ������
            byte[] sendBuffer = Encoding.UTF8.GetBytes("Hello World!");
            int sendBytes = socket.Send(sendBuffer);

            // �޴´�
            byte[] recvBuffer = new byte[1024];
            int recvBytes = socket.Receive(recvBuffer);
            string recvData = Encoding.UTF8.GetString(recvBuffer, 0, recvBytes);
            Console.WriteLine($"[From Server] {recvData}");

            // ������
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}