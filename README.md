# dotnet-markdown-sample-code

[dotnet-markdown-sample-code](https://github.com/jamesivie/dotnet-markdown-sample-code) inserts or updates specially marked-up sample code from a CSharp source file (usually Samples.cs) into specially-marked sections in a markdown file (usually README.md).

## Usage
dotnet-markdown-sample-code [source [dest [language [timeout]]]]
    source: the relative or full path to the source code from which samples should be extracted (default is Samples.cs)
      dest: the relative or full path to the destination markdown file into which the sample code should be placed (default is README.md)
  language: the markdown language specifier so that markdown can colorize the code sample properly (default is csharp)
   timeout: the maximum number of seconds to keep trying to update the destination file (uses expontential backoff with random offset starting at 250ms, default is 30)
