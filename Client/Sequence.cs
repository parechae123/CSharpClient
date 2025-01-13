using System;
using System.IO;
using System.Text;
using System.Net.Sockets;
using MessagePack.Formatters;


public class Sequence
{
    public string myName = string.Empty;
    public async Task Login(NetworkStream stream)
    {
        byte[] buffer = new byte[1];
        while (true)
        {
            //메세지받기

            Console.Write("닉네임을 입력 해 주세요 : ");
            var message = Console.ReadLine() ?? "";
            var data = Encoding.UTF8.GetBytes(message);

            //닉네임 객체 보냄
            await stream.WriteAsync(data, 0, data.Length);

            //1 true, 0 false
            int isSuccess = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (isSuccess == 0)
            {
                Console.WriteLine("로그인 실패 동일한 이름이 있습니다.");
                continue;
            }
            else
            {
                Console.WriteLine($"로그인 성공,귀하의 닉네임 {message}");
                myName = message;
                break;
            }
        }
    }

    public async Task ReceiveMessage(NetworkStream stream)
    {
        try
        {
            var buffer = new byte[1024];
            while (true)
            { 
                int byteRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                string message = Encoding.UTF8.GetString(buffer,0,byteRead);
                Console.WriteLine(message);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("에러발생 : "+ex);
            throw;
        }
    }
    public async Task Chatting(NetworkStream stream)
    {
        byte[] buffer = new byte[1024];
        while (true)
        {

            var message = Console.ReadLine() ?? "";
            Console.SetCursorPosition(0, Console.CursorTop - 1);

            // 해당 줄을 공백으로 덮어씌움
            Console.Write(new string(' ', Console.WindowWidth));

            // 다시 커서를 해당 줄의 시작 위치로 이동
            Console.SetCursorPosition(0, Console.CursorTop);
            var data = Encoding.UTF8.GetBytes($"{myName} : {message}");

            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}
