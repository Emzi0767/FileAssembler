# FileAssembler
A simple tool which lets you split and reassmeble files, for e.g. transport purposes.

This tool came to be when I needed to split large files for uploading them to my server, as my internet connection is 
unreliable. So I decided to split the files for transport, then reassemble them on the server.

## Requirements
This tool is written in C#, and requires an implementation of .NET Framework to be available on your system. It has 2 
targets:

- .NET Core 2.0
- .NET Framework 4.5.2

This means that at least one of these must be available on the system.

### .NET Core
If using .NET Core version of the tool, you need to have .NET Core 2.0 installed on your system. It is available for 
download from [.NET's official website](https://www.microsoft.com/net/download/windows). Make sure to download the 2.0 
version of .NET Core runtime for your operating system. You don't need the SDK, which is a much larger download.

### .NET Framework
To use the .NET Framework (.NETFX) version of the tool, you need to make sure that an implementation of .NET Framework 
4.5.2 is available on your system. This means that you need to be running Windows 7 or newer. The required .NET version 
is available for download from [.NET's official website](https://www.microsoft.com/net/download/thank-you/net471).

#### Non-Windows Operating Systems
To run the .NET Framework version of the tool on non-Windows Operating Systems, you will need to find a suitable 
implementation of .NET Framework 4.5.2 for your operating system. For GNU/Linux, BSD, and OS X (and possibly other 
\*NIX systems), you can use Mono (at least version 3.2).

However, due to poor implementations and numerous bugs, I will not be providing support for the tool when using Mono, 
nor will I make any effort to fix the issues on said runtime. If you aren't running on Windows, I recommend using .NET 
Core, as it's significantly more stable and bug-free, and has much lower resource requirements.

## Usage
Using the tool is relatively straightforward. First, download the latest version of the tool from 
[the Releases page](https://github.com/Emzi0767/FileAssembler/releases). Please note that the tool has 2 versions 
available:

- `fileassembler-x.y.z-netfx.zip` - .NET Framework 4.5.2 version. It is recommended for Windows Operating Systems.
- `fileassembler-x.y.z-netcore.zip` - .NET Core 2.0 version. It is recommended for non-Windows Operating Systems.

Depending on whether you pick the .NETFX or .NET Core version, basic usage is slightly different:

- .NET Framework: `Emzi0767.FileAssembler.exe arguments...`
- .NET Core: `dotnet Emzi0767.FileAssembler.dll arguments...`

Beyond that, the tool has 2 operation modes: `split` and `assemble`.

## Splitting files
Splitting files is very easy. You need to specify the file to split, chunk size, and optionally output directory. 
Arguments are as follows:

```
program split arguments...

arguments accepted:
  -f | --filename           File to split.
  -s | --chunk-size         Size of the output chunk, in bytes.
  -o | --output-directory   Directory in which the files will be placed.
```

Chunk size can be specified with one of the following prefixes:

- `K`: kibbi; the value will be multiplied by 1024, e.g. `64K` will result in a chunk size of 64 \* 1024, or 65,536 
  bytes (64 KiB).
- `k`: kilo; the value will be multiplied by 1000, e.g. `64k` will result in a chunk size of 64 \* 1000, or 64,000 
  bytes (64 KB).
- `M`: mebbi; the value will be multiplied by 1024 \* 1024, e.g. `64M` will result in a chunk size of 64 \* 1024 \* 
  1024, or 67,108,864 bytes (64 MiB).
- `m`: mega; the value will be multiplied by 1000 \* 1000, e.g. `64m` will result in a chunk size of 64 \* 1000 \* 
  1000, or 64,000,000 bytes (64 MB).
- `G`: gibbi; the value will be multiplied by 1024 \* 1024 \* 1024, e.g. `64G` will result in a chunk size of 64 \* 
  1024 \* 1024 \* 1024, or 68,719,476,736 bytes (64 GiB).
- `g`: giga; the value will be multiplied by 1000 \* 1000 \* 1000, e.g. `64g` will result in a chunk size of 64 \* 1000 
  \* 1000 \* 1000, or 64,000,000,000 bytes (64 GB).

If these units are confusing to you, remember that KiB, MiB, GiB, etc are the units that Windows inaccurately reports 
to you as KB, MB, GB, etc.

When output directory is not specified, output files will be placed alongside the input file.

The switches follow POSIX syntax (with the exception of grouping and non-spaced arguments), meaning that when 
specifying e.g. filename can be done as `-f "file name"`, `-f file\ name`, `-f filename`, `--filename="file name"`, 
etc.

### Example 1
To split a file called `test.xyz` into chunks of 128 MiB, you would do something like this:

`Emzi0767.FileAssembler.exe split -f "text.xyz" -s 128M`

### Example 2
To split a file called `foo.bar` into chunks of 64 MiB, and place them in `F:\oo\bar`, you would do something 
like this:

`Emzi0767.FileAssembler.exe split -f "foo.bar" -s 64M -o "F:\oo\bar"`

## Assembling files
Assembling files requires that you specify the metadata file, and optionally an output directory. Note that all the 
chunk files must be in the same directory as the metadata file.

The metadata file is usually named `filename.ext.pmeta`, and the chunk files follow a naming scheme like this: 
`filename.ext.000.pdat`.

Arguments are as follows:

```
program assemble arguments...

arguments accepted:
  -f | --filename           Metadata of the file to assemble.
  -o | --output-directory   Directory in which the files will be placed.
```

When output directory is not specified, output files will be placed alongside the metadata file.

The switches follow POSIX syntax (with the exception of grouping and non-spaced arguments), meaning that when 
specifying e.g. filename can be done as `-f "file name"`, `-f file\ name`, `-f filename`, `--filename="file name"`, 
etc.

### Example 1
To assemble a file called `test.xyz`, you will need to specify `test.xyz.pdat` as input file, as follows:

`Emzi0767.FileAssembler.exe -f "test.xyz.pdat"`

### Example 2
To assemble a file called `foo.bar` and place the output in `F:\oo\bar`, you will need to specify `foo.bar.pdat` as 
input file, and `F:\oo\bar` as output directory, as follows:

`Emzi0767.FileAssembler.exe -f "foo.bar.pdat" -o "F:\oo\bar"`

## Issues and suggestions
If you find a problem, have an idea, or a suggestion, open an issue, or issue a pull request.
