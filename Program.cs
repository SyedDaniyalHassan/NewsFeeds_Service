using System;
using System.Runtime.InteropServices;
using Topshelf;
using Topshelf.Runtime.DotNetCore;

namespace K180239_Q2
{
    class Program
    {
        static void Main(string[] args)
        {
            var is_windows = false;
            var exitcode = HostFactory.Run(x =>
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    x.UseEnvironmentBuilder(
                      target => new DotNetCoreEnvironmentBuilder(target)
                    );
                    is_windows = false;
                }
                else
                {
                    is_windows = true;
                }
                x.Service<NewsFeedService>(s =>
                {
                    s.ConstructUsing(feeds => new NewsFeedService());
                    s.WhenStarted(feeds => feeds.Start());
                    s.WhenStopped(feeds => feeds.Stop());
                });

                x.RunAsLocalSystem();
                x.SetServiceName("News_Feeds_Service");
                x.SetDisplayName("News Feeds Service ");
                x.SetDescription("This is the Rss News Feeds Cross-platform Service");
            });
            if (is_windows == true)
            {
                int exitCodeValue = (int)Convert.ChangeType(exitcode, exitcode.GetTypeCode());
                Environment.ExitCode = exitCodeValue;
            }

        }
    }
}
