# dotnet-markdown-sample-code

[dotnet-markdown-sample-code](https://github.com/jamesivie/dotnet-markdown-sample-code) allows users to copy sample code that is actually compiled and optionally also tested from a source code file into specially-marked sections in a markdown file, ensuring that sample code will not have syntax errors, and if called from tests, also ensures that the code works as intended.

## Usage
`dotnet-markdown-sample-code [source [dest [language [timeout]]]]`

- `source` is the relative or full path to the source code from which samples should be extracted (default is Samples.cs)
- `dest` is the relative or full path to the destination markdown file into which the sample code should be placed (default is README.md)
- `language` is the markdown language specifier so that markdown can colorize the code sample properly (default is csharp)
- `timeout` is the maximum number of seconds to keep trying to update the destination file (uses expontential backoff with random offset starting at 250ms, default is 30)

Sections in the Readme file where sample code is to be places are marked as follows:

	[//]: # (MySampleSectionName)
	```language
	void MyFunction()
	{
		// my code here
	}
	```

Code to place into the Readme file is identified by `#region`/`#endregion` markers like this:

    #region MySampleSectionName
	void MyFunction()
	{
		// my code here
	}
    #endregion

For each region in the sample file, the corresponding Readme section is searched for, the current contents are removed (everything between and including the first two lines starting with triple-backtick), and everything between the `#region` and `#endregion` markers is put into that position in the Readme.
