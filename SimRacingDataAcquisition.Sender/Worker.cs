using Microsoft.Extensions.Hosting;
using System.Net.Sockets;
using System.Net;
using System.Text;

namespace SimRacingDataAcquisition.Sender
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            long lastSendTimeFromEposch = 0;
            UdpClient udpClient = new UdpClient();
            IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddress, 11004);

            while (!stoppingToken.IsCancellationRequested)
            {
                long startOperationTime = GetUnixTimeMiliseconds();
                byte[] sendBytes = Encoding.ASCII.GetBytes("Is anybody there?");
                try
                {
                    udpClient.Send(sendBytes, sendBytes.Length, ipEndPoint);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation($"{startOperationTime} - Message send");
                }
                long operationCostMiliseconds = GetUnixTimeMiliseconds() - startOperationTime;
                if (operationCostMiliseconds < 20)
                {
                    Thread.Sleep((int)(20 - operationCostMiliseconds));
                }
            }
        }

        private long GetUnixTimeMiliseconds()
        {
            return DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }
    }
}
