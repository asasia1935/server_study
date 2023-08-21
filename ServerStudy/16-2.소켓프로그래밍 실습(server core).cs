using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
{
    class Program
    {
        static void Main(string[] args)
        {
            // DNS (Domain Name System)
            // www.��¼����¼��.com -> 123.123.123.12
            string host = Dns.GetHostName(); // ���� ��ǻ���� ȣ��Ʈ �̸�
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0]; // ������ ȣ��Ʈ���� �迭 (�Ĵ� �ּ�)
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7777); // ���� �ּ� ���� (��Ʈ ��ȣ�� �ش� �Ĵ��� ���� �� �� �ϳ�)

            // �����Ⱑ ���� �޴���
            Socket listenSocket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // ������ ����
                listenSocket.Bind(endPoint);

                // ���� ����
                // backlog : �ִ� ���� -> live���� ����
                listenSocket.Listen(10);

                while (true)
                {
                    Console.WriteLine("Listening...");

                    // �մ��� ���� ��Ų��
                    Socket clientSocket = listenSocket.Accept(); // ������ ���� -> ������ �մ԰� �� �������� ��ȭ�� �ϸ� ��
                                                                 // �մ��� ������ �� �ٿ��� ��� ��� (Accept�� ������ ��� �Լ���)

                    // �޴´�
                    byte[] recvBuffer = new byte[1024]; // ���� ������ ����
                    int recvBytes = clientSocket.Receive(recvBuffer);
                    string recvData = Encoding.UTF8.GetString(recvBuffer, 0 /* ���� �ε��� */, recvBytes /* ���ڿ� ũ�� */); // ��� ���� �ָ� string���� ��ȯ
                    Console.WriteLine($"[From Client] {recvData}");

                    // ������
                    byte[] sendBuffer = Encoding.UTF8.GetBytes("Welcome to MMORPG Server !"); // ���� ���۴� �¥���� ������ �˰� ������ �ٷ� ������ָ� ��
                    clientSocket.Send(sendBuffer);

                    // �i�Ƴ���
                    clientSocket.Shutdown(SocketShutdown.Both); // ���̻� �������� �ʴ´ٴ� ���� ����
                    clientSocket.Close(); // ���� ����
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}