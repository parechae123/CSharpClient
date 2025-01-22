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
        public async Task Start()
        {
            try
            {

                // 1. gRPC 채널 생성
                using var channel = GrpcChannel.ForAddress("http://localhost:5000", new GrpcChannelOptions
                { 
                    HttpHandler = new SocketsHttpHandler
                    {
                        EnableMultipleHttp2Connections = true,
                        
                    }
                });
                var client = new ServiceTest.ServiceTestClient(channel);
                // 2. 서버에 요청 보내기
                Console.WriteLine("서버 요청 중...");

                var request = new GetRequest { RequestWord = "hellow server" };
                var response = await client.firstEventAsync(new GetRequest { RequestWord = "테스트 요청" });
                Console.WriteLine($"서버 응답: {response.RequestWord}");

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
