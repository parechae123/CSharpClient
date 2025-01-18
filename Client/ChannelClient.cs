using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Google.Protobuf.Protocol;
// 1. gRPC 채널 생성

namespace CSharpClient.Client
{
    internal class ChannelClient
    {
        GrpcChannel channel = GrpcChannel.ForAddress("http://127.0.0.1:5000");
        public async Task Start()
        {
            try
            {
                
                // 1. gRPC 채널 생성
                var client = new ServiceTest.ServiceTestClient(channel);
                // 2. 서버에 요청 보내기
                Console.WriteLine("서버 요청 중...");

                var request = new GetRequest { RequestWord = "안녕하세요, 서버!" };
                var response = await client.firstEventAsync(request);

                // 3. 응답 출력
                Console.WriteLine($"서버 응답: {response.RequestWord}");
            }
            catch (Grpc.Core.RpcException rpcEx)
            {
                Console.WriteLine($"gRPC 예외 발생: {rpcEx.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"일반 예외 발생: {ex.Message}");
            }
        }
    }
}
