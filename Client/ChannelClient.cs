using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Google.Protobuf.Protocol;
using Grpc.Core;
// 1. gRPC 채널 생성

namespace CSharpClient.Client
{
    internal class ChannelClient
    {
        GrpcChannel channel;

        public async Task Start()
        {
            try
            {

                // 1. gRPC 채널 생성
                channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
                { 
                    HttpHandler = new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true,
                        
                    }
                });

                var client = new DBRequest.DBRequestClient(channel);
                //RequestCommands(client);
                await ChattingTest(client);

                // 2. 서버에 요청 보내기
                Console.WriteLine("서버 요청 중...");
                var request = new GetRequest { RequestWord = "wr" };
                var response = await client.GetUserInfoAsync(request);
                Console.WriteLine($"서버 응답: {response.Id}");
                Console.WriteLine($"서버 응답: {response.Index}");
                Console.WriteLine($"서버 응답: {response.Pw}");

            }
            catch (Grpc.Core.RpcException rpcEx)
            {
                Console.WriteLine($"gRPC 연결 실패: {rpcEx.Status.StatusCode}, {rpcEx.Status.Detail}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"예외 발생: {ex.Message}");
            }
            
            Console.ReadLine();
        }
        public async Task ConnectionOnly()
        {
            using var channel = GrpcChannel.ForAddress("localhost:5000");

            //var client = new ServiceTest.ServiceTestClient(channel);
            var client = new ServiceTest.ServiceTestClient(channel);
            var reply = await client.firstEventAsync(
                              new GetRequest { RequestWord = "GreeterClient" });
            Console.WriteLine("Greeting: " + reply.RequestWord);
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public async Task ChattingTest(DBRequest.DBRequestClient client)
        {

            var call = client.Chatting();

            // 2. 비동기적으로 서버에서 오는 메시지를 읽는 작업을 별도 태스크로 실행
            var readTask = Task.Run(async () =>
            {

                while (await call.ResponseStream.MoveNext())
                {
                    ChatMessage message = call.ResponseStream.Current;
                    Console.WriteLine($"[{message.RoomName}]{message.UserName} : {message.ChattingText}");
                }
            });
            Console.WriteLine("채팅방 이름을 입력하세요");
            string roomName = Console.ReadLine();
            Console.WriteLine("유저 이름을 입력하세요");
            string userName = Console.ReadLine();
            call.RequestStream.WriteAsync(new ChatMessage { UserName = userName, RoomName = roomName, ChattingText = "ㅎㅇㅎㅇ", Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() });
            
            while (true)
            {
                try
                {
                    string chatMessage = Console.ReadLine();
                    if (string.IsNullOrEmpty(chatMessage)) break;

                    await call.RequestStream.WriteAsync(new ChatMessage { UserName = userName, ChattingText = chatMessage, Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(), RoomName = roomName });
                    
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

            }

            // 클라이언트가 메시지 전송을 마치면 요청 스트림 완료 신호 보내기
            await call.RequestStream.CompleteAsync();
            Console.WriteLine("클라이언트 요청 스트림 만료");
            // 수신 작업이 종료될 때까지 대기
            await readTask;
        }

        public async Task RequestCommands(DBRequest.DBRequestClient client)
        {
            while (true)
            {
                Console.WriteLine("명령어를 입력하세요");
                string line;
                Commands inputCommand;
                try
                {
                    line = Console.ReadLine();
                    inputCommand = Enum.Parse<Commands>(line);
                }
                catch (ArgumentException ex)
                {
                    Console.WriteLine("잘못된 명령어 입니다.");
                    return;
                }

                switch (inputCommand)
                {
                    case Commands.regist:
                        Regist(client);

                        break;
                    case Commands.delete:
                        Delete(client);
                        break;
                    case Commands.login:
                        login(client);
                        break;
                    case Commands.chat:

                        Chatting(client);
                        break;
                }
            }
        }

        public async Task login(DBRequest.DBRequestClient client)
        {
            Console.WriteLine("아이디를 입력 해 주세요");
            string id = Console.ReadLine();
            var response = await client.GetUserInfoAsync(new GetRequest { RequestWord = id });
            Console.WriteLine(response.Index);
            Console.WriteLine(response.Id);
            Console.WriteLine(response.Pw);
        }
        public async Task Regist(DBRequest.DBRequestClient client)
        {
            Console.WriteLine("아이디를 입력 해 주세요");
            string id = Console.ReadLine();
            Console.WriteLine("비밀번호를 입력 해 주세요");
            string pw = Console.ReadLine();

            var request = await client.RegistUserAsync(new IDSet { Id = id, Password = pw });
            Console.WriteLine(request.ErrorText);
        }
        public async Task Delete(DBRequest.DBRequestClient client)
        {
            Console.WriteLine("아이디를 입력 해 주세요");
            string id = Console.ReadLine();
            Console.WriteLine("비밀번호를 입력 해 주세요");
            string pw = Console.ReadLine();

            var request = await client.DeleteUserAsync(new IDSet { Id = id, Password = pw });
            Console.WriteLine(request.ErrorText);
        }
        public async Task Chatting(DBRequest.DBRequestClient client)
        {
            // 1. 양방향 스트리밍 호출 시작
            var call = client.Chatting();


            // 2. 비동기적으로 서버에서 오는 메시지를 읽는 작업을 별도 태스크로 실행
            var readTask = Task.Run(async () =>
            {
                await foreach (var message in call.ResponseStream.ReadAllAsync())
                {
                    Console.WriteLine($"{message.UserName}: {message.ChattingText} ({message.Time})");
                }
            });

            string message = Console.ReadLine();

            call.RequestStream.WriteAsync(new ChatMessage { UserName = "ClientUser",ChattingText = message ,Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),RoomName = "dd" });

            // 5. 모든 작업이 끝난 후, 스트림 종료
            await readTask;
        }
    }
}

public enum Commands
{
    empty,regist,delete,login,chat
}

public class DBCunnectService : DBRequest.DBRequestBase
{
    public override Task<GetUserDB> GetUserInfo(GetRequest request, ServerCallContext context)
    {
        Console.WriteLine($"클라이언트가 접속: {context.Peer}");
        Console.WriteLine($"클라이언트 요청: {request.RequestWord}");

        // 응답 데이터 설정
        return Task.FromResult(new GetUserDB());
    }
}

public class CustomService : ServiceTest.ServiceTestBase
{
    public override Task<GetRequest> firstEvent(GetRequest request, ServerCallContext context)
    {
        // 클라이언트의 IP 정보 (RequestHeaders나 Peer에서 추출)
        var clientIp = context.Peer;
        Console.WriteLine($"클라이언트가 접속: {clientIp}");

        // 요청 받은 데이터를 출력 (예: 클라이언트 요청 내용)
        Console.WriteLine($"클라이언트 요청: {request.RequestWord}");

        // 응답 데이터 설정
        return Task.FromResult(new GetRequest { RequestWord = "서버 응답: " + request.RequestWord });
    }

    public override Task<KeyNumberReq> secondEvent(KeyNumberReq request, ServerCallContext context)
    {
        // 클라이언트의 IP 정보 (RequestHeaders나 Peer에서 추출)
        var clientIp = context.Peer;
        Console.WriteLine($"클라이언트가 접속: {clientIp}");

        // 요청 받은 데이터를 출력 (예: 클라이언트 요청 내용)
        Console.WriteLine($"클라이언트 요청: {request.RequestWord}{request.RequestNumb}");

        // 응답 데이터 설정
        return Task.FromResult(new KeyNumberReq { RequestWord = "서버 응답: " + request.RequestWord, RequestNumb = request.RequestNumb });
    }
}
