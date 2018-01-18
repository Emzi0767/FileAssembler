using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.Extensions.CommandLineUtils;

namespace Emzi0767.FileAssembler
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var cli = new CommandLineApplication();

            var md = cli.Argument("mode", "Operation mode; either split or assemble.");
            var fn = cli.Option("-$ | -f <filename> | --file=<filename>", "File to split, or metafile containing information about the file to assemble.", CommandOptionType.SingleValue);
            var cs = cli.Option("-s <size> | --chunk-size=<size>", "Size of the output chunk when splitting.", CommandOptionType.SingleValue);
            var od = cli.Option("-o <directory> | --output-directory=<directory>", "Directory in which output file(s) will be placed. If unspecified, will be placed next to input file.", CommandOptionType.SingleValue);

            cli.HelpOption("-? | -h | --help");
            cli.OnExecute(async () =>
            {
                if (!fn.HasValue())
                {
                    cli.ShowHelp();
                    return 0.StopIfDebugger();
                }

                var xfn = fn.Value();
                var fi = new FileInfo(xfn);
                if (!fi.Exists || (fi.Attributes & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    Console.WriteLine("File '{0}' does not exist. Specify a valid file and try again.");
                    return 1.StopIfDebugger();
                }

                var di = fi.Directory;
                if (od.HasValue())
                    di = new DirectoryInfo(od.Value());

                if (!di.Exists)
                    di.Create();

                if (md.Value.ToLowerInvariant() == "assemble")
                {
                    if (cs.HasValue())
                    {
                        Console.WriteLine("Chunk size can only be specified when splitting a file.");
                        return 2.StopIfDebugger();
                    }

                    var op = new Unsplitter();
                    await op.UnsplitFileAsync(fi, di);
                }
                else if (md.Value.ToLowerInvariant() == "split")
                {
                    if (!cs.HasValue())
                    {
                        Console.WriteLine("You need to specify chunk size when splitting.");
                        return 2.StopIfDebugger();
                    }

                    var reg = new Regex(@"^(?<digits>\d+)(?<suffix>K|M|G|T|k|m|g|t)?$", RegexOptions.ECMAScript | RegexOptions.Compiled);
                    var m = reg.Match(cs.Value());
                    if (!m.Success)
                    {
                        Console.WriteLine("Invalid chunk size specified.");
                        return 4.StopIfDebugger();
                    }

                    if (!int.TryParse(m.Groups["digits"].Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var digits))
                    {
                        Console.WriteLine("Something went seriously wrong when parsing chunk size.");
                        return 8.StopIfDebugger();
                    }

                    if (m.Groups["suffix"].Success)
                        switch (m.Groups["suffix"].Value)
                        {
                            case "k":
                                digits *= 1000;
                                break;

                            case "K":
                                digits *= 1024;
                                break;

                            case "m":
                                digits *= 1000 * 1000;
                                break;

                            case "M":
                                digits *= 1024 * 1024;
                                break;

                            case "g":
                                digits *= 1000 * 1000 * 1024;
                                break;

                            case "G":
                                digits *= 1024 * 1024 * 1024;
                                break;
                        }

                    if (digits <= 0)
                    {
                        Console.WriteLine("Chunk size overflowed or negative.");
                        return 16.StopIfDebugger();
                    }

                    var op = new Splitter();
                    await op.SplitFileAsync(fi, digits, di);
                }
                
                return 0.StopIfDebugger();
            });

            cli.Execute(args);
        }

        private static int StopIfDebugger(this int value)
        {
            if (Debugger.IsAttached)
                Console.ReadKey();

            return value;
        }
    }
}
