using System;
using System.Text;
using System.Threading.Tasks;
using Fiddler;
using Brotli;

namespace FiddlerCore_DecodeBrotli_NetFramework
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Custom cert provider can be used.
            BCCertMaker.BCCertMaker certProvider = new BCCertMaker.BCCertMaker();
            CertMaker.oCertProvider = certProvider;

            FiddlerApplication.AfterSessionComplete += FiddlerApplication_AfterSessionComplete;

            if (!CertMaker.createRootCert())
            {
                Console.WriteLine("Unable to create cert for FiddlerCore.");
                return;
            }

            if (!CertMaker.trustRootCert())
            {
                Console.WriteLine("Unable to install FiddlerCore's cert.");
                return;
            }

            FiddlerCoreStartupSettings startupSettings =
                                            new FiddlerCoreStartupSettingsBuilder()
                                                .ListenOnPort(8887)
                                                .DecryptSSL()
                                                .RegisterAsSystemProxy()
                                                .Build();


            FiddlerApplication.Startup(startupSettings);


            Console.WriteLine("\nPROXY IS NOW SET");
            Console.WriteLine("To remove the proxy and close the app, press Enter");

            Console.ReadLine();
            FiddlerApplication.Shutdown();
        }

        private static void FiddlerApplication_AfterSessionComplete(Session oSession)
        {
            if (oSession.ResponseHeaders.ExistsAndContains("content-encoding", "br") && oSession.responseBodyBytes.Length > 0)
            {
                var decodedBytes = oSession.responseBodyBytes.DecompressFromBrotli();
                var text = Encoding.Default.GetString(decodedBytes);

                Console.WriteLine("Decoded body is: " + 
                    ]);
            }
            else if (oSession.ResponseHeaders.ExistsAndContains("Report-To", "fastly-insights.com") && oSession.responseBodyBytes.Length > 0)
            {
                // TO test with https://fastly-insights.com/api/v1/config/a2560724-7682-4399-af18-96914684a88a
                var decodedBytes = oSession.responseBodyBytes;
                var text = Encoding.Default.GetString(decodedBytes);
                Console.WriteLine(">>>>>>>>>>>> Body is: " + text);
            }
        }
    }
}