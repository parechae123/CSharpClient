//Client
using CSharpClient.Client;
using ProtoBuf.Meta;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Text;
class Program
{
    static void Main()
    {
        try
        {
            var server = new ChannelClient();
            //server.Start();
            var temp = server.Start();
        } 
        catch(Exception ex)
        { 
            Console.WriteLine(ex.ToString());
        }

        Console.ReadLine();
    }

/*    static async Task Main()
    {
        try
        {
            FirstWeekSummary sequence = new FirstWeekSummary();
            TcpClient tcpClient = new TcpClient("127.0.0.1", 7777);
            NetworkStream stream = tcpClient.GetStream();
            sequence.Start(stream);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류 발생: {ex.Message}");
        }
    }*/
    /*
    static async Task Main()
    {
        try
        {
            ProtocTest sequence = new ProtocTest();
            TcpClient tcpClient = new TcpClient("127.0.0.1", 7777);
            NetworkStream stream = tcpClient.GetStream();
            await sequence.Process(tcpClient,stream);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류 발생: {ex.Message}");
        }
    }*/
    /*
    static async Task Main()
    {
        try
        {
            using var client = new TcpClient("127.0.0.1", 7777);
            using var stream = client.GetStream();
            Sequence sequence = new Sequence();
            await sequence.Login(stream);
            _= Task.Run(()=>sequence.ReceiveMessage(stream));
            await sequence.Chatting(stream);

        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류 발생: {ex.Message}");
        }
    }
*/
}