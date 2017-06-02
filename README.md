# ExamineFileIndexer

[![Build status](https://ci.appveyor.com/api/projects/status/wp5cgxe89sywvjed/branch/master?svg=true)](https://ci.appveyor.com/project/Cogworks/examinefileindexer/branch/master)
[![NuGet release](https://img.shields.io/nuget/v/Cogworks.ExamineFileIndexer.svg)](https://www.nuget.org/packages/Cogworks.ExamineFileIndexer)
[![Our Umbraco project page](https://img.shields.io/badge/our-umbraco-orange.svg)](https://our.umbraco.org/projects/developer-tools/examinefileindexer/)

Custom Examine indexer to index any umbraco media nodes. 
Under the hood it makes use of [Apache Tika](http://tika.apache.org/) to extract content and meta data from umbraco media files. 
Tika can handle the [following formats](http://tika.apache.org/1.2/formats.html).  The package also supports VPP (Virtual path provider) so if you media files are in azure etc it will also index those.

## Getting started

This package is supported on Umbraco 7.6.1+.

### Installation

ExamineFileIndexer is available from Our Umbraco, NuGet, or as a manual download directly from GitHub.

#### Our Umbraco repository
You can find a downloadable package, along with a discussion forum for this package, on the [Our Umbraco](https://our.umbraco.org/projects/developer-tools/examinefileindexer/) site.

#### NuGet package repository
To [install from NuGet](https://www.nuget.org/packages/Cogworks.ExamineFileIndexer/), run the following command in your instance of Visual Studio.

    PM> Install-Package Cogworks.ExamineFileIndexer

## Usage

After installation your *ExamineIndex.config* and *ExamineSettings.config* file will updated. The following entries will be added.

#### ExamineIndex.config ###

```xml

  <IndexSet SetName="MediaIndexSet" IndexPath="~/App_Data/TEMP/ExamineIndexes/MediaIndexSet">
    <IndexAttributeFields>
      <add Name="id" />
      <add Name="nodeName" />
      <add Name="updateDate" />
      <add Name="writerName" />
      <add Name="path" />
      <add Name="nodeTypeAlias" />
      <add Name="parentID" />
    </IndexAttributeFields>
    <IncludeNodeTypes>
      <add Name="File" />
    </IncludeNodeTypes>
  </IndexSet>

```
  
#### ExamineSettings.config ###
Under *ExamineIndexProviders/providers*:

```xml

<add name="MediaIndexer" type="Cogworks.ExamineFileIndexer.UmbracoMediaFileIndexer, Cogworks.ExamineFileIndexer" 
extensions=".pdf,.docx" 
umbracoFileProperty="umbracoFile" />

```

Under *ExamineSearchProviders/providers*:

```xml

<add name="MediaSearcher" type="UmbracoExamine.UmbracoExamineSearcher, UmbracoExamine" indexSet="MediaIndexSet" 
analyzer="Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net" />

```

By default the following file types will be indexed: **pdf**, **docx**. To add other file types to index you need to update *ExamineSettings.config*:


```xml

<add name="MediaIndexer" type="Cogworks.ExamineFileIndexer.UmbracoMediaFileIndexer, Cogworks.ExamineFileIndexer" 
extensions=".pdf,.docx" 
umbracoFileProperty="umbracoFile" />

```


Update the **extensions** attribute and add any other file types. They need to be separated by colons (,).

You can also add the image file types eg. **.jpg**. **PLEASE NOTE INDEXING IMAGES WILL ONLY ADD EXIF META DATA.**

### Contribution guidelines

To raise a new bug, create an issue on the GitHub repository. To fix a bug or add new features, fork the repository and send a pull request with your changes. Feel free to add ideas to the repository's issues list if you would to discuss anything related to the package.

### Who do I talk to?

This project is maintained by [Cogworks](http://www.thecogworks.com/) and contributors. If you have any questions about the project please contact us through the forum on Our Umbraco, on [Twitter](https://twitter.com/cogworks), or by raising an issue on GitHub.

## License

Copyright &copy; 2017 [The Cogworks Ltd](http://www.thecogworks.com/), and other contributors

Licensed under the MIT License.
