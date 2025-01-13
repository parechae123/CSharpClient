//Client
using System.Net.Sockets;
using System.Text;
class Program
{

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

}