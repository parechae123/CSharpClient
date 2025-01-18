using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf.Protocol;

namespace CSharpClient.Client
{
    internal class FirstWeekSummary
    {
        public void Start(NetworkStream stream)
        {
            while (true)
            {
                string inputMessage = Console.ReadLine();
                if (inputMessage != null)
                {
                    byte[] sendBuffer = Encoding.UTF8.GetBytes(inputMessage);
                    stream.Write(sendBuffer, 0, sendBuffer.Length);
                    if (sendBuffer.Length <= 0) continue;
                    //User tempUser = User.Parser.ParseDelimitedFrom(stream);
                    try 
                    {
                        stream.ReadTimeout = 2000;
                        User tempUser = User.Parser.ParseDelimitedFrom(stream);
                        if (tempUser == null)
                        {
                            Console.WriteLine("데이터가 없엉");
                            continue;
                        }
                        Console.WriteLine($"userID : {tempUser.UserID},ip : {tempUser.Ip} ,userToken : {tempUser.Token}");
                    } 
                    catch (TimeoutException ex)
                    {
                        Console.WriteLine("타임아웃 : " +ex);
                    }
                    catch(IOException ex)
                    { 
                        Console.WriteLine("객체를 읽어오는 중 오류발생 : " + ex);
                    }

                    //parsedelimitedfrom == 순차 읽기
                    //parser.parse == 읽기
                    //객체.WriteDelimitedTo == 순차쓰기
                    //객체.write == 쓰기


                }
            }
        }
    }
}
