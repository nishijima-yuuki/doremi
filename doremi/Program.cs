using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace doremi
{
    class Program
    {
        /// <summary>
        /// 終了コード。
        /// </summary>
        enum ExitCode
        {
            Succeeded = 0,
            InvalidArguments = 1,
            NotFound = 2,
            RuntimeError = 10,
        }

        /// <summary>
        /// エントリポイント。
        /// </summary>
        /// <param name="args">実行時引数。</param>
        /// <returns>終了コード。</returns>
        static int Main(string[] args)
        {
            // 引数チェック
            if (args.Length != 3)
            {
                return (int)ExitCode.InvalidArguments;
            }

            var iconText = args[0];
            int x = 0, y = 0;
            if (!int.TryParse(args[1], out x) || !int.TryParse(args[2], out y))
            {
                return (int)ExitCode.InvalidArguments; ;
            }

            try
            {
                var targetIcons = DesktopIconCollector.Collect().Where(icon => icon.Text.Equals(iconText));
                if (targetIcons.Any())
                {
                    targetIcons.First().SetLocation(x, y);
                }
                else
                {
                    return (int)ExitCode.NotFound;
                }

                return (int)ExitCode.Succeeded;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Source);
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine(e.StackTrace);

                return (int)ExitCode.RuntimeError;
            }
        }
    }
}
