using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CSharpClient.Client
{
    public class ProtocTest
    {
        public async Task Process(TcpClient client, NetworkStream stream)
        {

            Login(stream,client);
        }
        public async void Login(NetworkStream stream,TcpClient client)
        {
            byte[] buff = new byte[1024];
            byte[] data;
            string message;
            int buffRead;
            
            while (true)
            {
                buffRead = stream.Read(buff, 0, buff.Length);
                Console.WriteLine(Encoding.UTF8.GetString(buff,0,buffRead));
                message = Console.ReadLine() ?? ""; //readLine으로 string 받아와서
                data = Encoding.UTF8.GetBytes(message);//byte 배열로 바꿈

                //닉네임 객체 보냄
                await stream.WriteAsync(data, 0, data.Length);//바이트 배열을 전송
                User tempUser = ProtoBuf.Serializer.Deserialize<User>(stream);
                Console.WriteLine(tempUser.UserID??string.Empty);
                
            }
            Console.WriteLine("클라에서 탈출");
        }

    }
}
